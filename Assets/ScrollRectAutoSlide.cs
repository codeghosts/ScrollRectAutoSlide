using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollRectAutoSlide : MonoBehaviour
{
    public ScrollRect worldMap;
    public Image[] CurrentPoint;
    public Button[] button;

    Vector2 _newPosition;

    bool _isSlide = false;

    float _speed = 2.0f;
    float _startTime = 2.0f;

    Vector2 _startPosition;

    float _ratio = 0.0f;

    float _limitTime = 2.0f;

    float _currentTime = 0.0f;
    void Start()
    {
        _speed = 1.0f;
        _startTime = 2.0f;
        System.Action<RectTransform> callbackfunc = SlideToSpecificPoint;
        //给每个按钮和目标点关联起来
        for (int i = 0; i < CurrentPoint.Length; ++i)
        {
            var targetImage = CurrentPoint[i].GetComponent<RectTransform>();
            button[i].onClick.AddListener(delegate {
            callbackfunc?.Invoke(targetImage);
            });
        }
        
        worldMap.verticalNormalizedPosition = 0.5f;
        worldMap.horizontalNormalizedPosition = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {         
        if (_isSlide)
        {
            _currentTime += Time.deltaTime;
            _ratio = _currentTime / _limitTime;
            if (_currentTime >= _limitTime)
            {
                _isSlide = false;
                _ratio = _limitTime;
                Debug.Log("Arrived");
                _currentTime = 0.0f;
                foreach (var btn in button)
                {
                    btn.interactable = true;
                }
            }
            var tempPosition = Vector2.Lerp(_startPosition, _newPosition, _ratio);
            worldMap.normalizedPosition = tempPosition;           
        }
    }

    private void SlideToSpecificPoint(RectTransform target)
    {
        _newPosition = CenterPoint(target);
        _startPosition = worldMap.normalizedPosition;
        foreach (var btn in button)
        {
            btn.interactable = false; //滑动至当前目标位置之前不允许更改
        }
        _isSlide = true;
        Debug.Log("oldNormalizedPosition: "+_startPosition);
    }

    private Vector2 CenterPoint(RectTransform target)
    {
        var content = worldMap.content.GetChild(0).GetComponent<RectTransform>();
        var viewport = worldMap.viewport;
        Vector3 targetPosition = worldMap.GetComponent<RectTransform>().InverseTransformPoint(Clear_Pivot_Offset(target));
        Vector3 viewportPosition = worldMap.GetComponent<RectTransform>().InverseTransformPoint(Clear_Pivot_Offset(viewport));
        Vector3 distance_vec = viewportPosition - targetPosition;  //向目标滑动的位移向量  
        var height_Delta = content.rect.height - viewport.rect.height;
        var width_Delta = content.rect.width - viewport.rect.width;      
        var ratio_x = distance_vec.x / width_Delta;
        var ratio_y = distance_vec.y / height_Delta; 
        var ratioDistance = new Vector2(ratio_x, ratio_y);
        var newPosition = worldMap.normalizedPosition - ratioDistance;
        return new Vector2(Mathf.Clamp01(newPosition.x), Mathf.Clamp01(newPosition.y));        
    }

    //消除Pivot不在中心时的影响
    private Vector3 Clear_Pivot_Offset(RectTransform rec)
    {
        var offset = new Vector3((0.5f - rec.pivot.x) * rec.rect.width,
        (0.5f - rec.pivot.y) * rec.rect.height, 0.0f);
        var newPosition = rec.localPosition + offset;
        return rec.parent.TransformPoint(newPosition);
    }
 }
