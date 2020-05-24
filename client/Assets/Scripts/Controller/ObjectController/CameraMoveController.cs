using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

public class CameraMoveController : MonoBehaviour
{
    #region define

    private bool isDraging = false;
    public bool IsDraging
    {
        get{ return isDraging; }
    }
    private float deltaX = 0f;
    public float DeltaX
    {
        get{ return deltaX; }
    }

    private ObservableEventTrigger cameraControllerEventTrigger;
    private int touchFingerId = -1;
    #endregion

    #region public method

    /// <summary>
    /// ドラッグしたら呼ばれる
    /// </summary>
    /// <param name="e"></param>
    private void onDrag(PointerEventData data)
    {
        Vector2 delta = data.delta;
        if (Mathf.Abs(delta.x / 10) < 1)
        {
            isDraging = false;
            return;
        }

    #if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN

        isDraging = true;
        deltaX = delta.x / 10;

        #elif UNITY_IOS || UNITY_ANDROID

        int touchCount = Input.touchCount;
        if (touchCount <= 0)
            return;

        for (var i = 0; i < touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (touch.fingerId == touchFingerId)
            {
                isDraging = true;
                deltaX = delta.x / 10;
            }
        }

        #endif
    }

    public void InitCameraMoveController()
    {
        isDraging = false;
        deltaX = 0f;
        touchFingerId = -1;
    }

    #endregion

    private void Start()
    {
        cameraControllerEventTrigger = this.GetComponent<ObservableEventTrigger>();

    #if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN

        this.UpdateAsObservable()
            .Where(x => Input.GetMouseButtonUp(0))
            .Subscribe(x => { isDraging = false; });
        cameraControllerEventTrigger.OnDragAsObservable()
            .Subscribe(pointerEventData => onDrag(pointerEventData));

        #elif UNITY_IOS || UNITY_ANDROID

        cameraControllerEventTrigger.OnPointerDownAsObservable()
            .Subscribe(pointerEventData => OnPointerDownValidation(pointerEventData));
        cameraControllerEventTrigger.OnDragAsObservable()
            .Subscribe(pointerEventData => onDrag(pointerEventData));
        cameraControllerEventTrigger.OnPointerUpAsObservable()
            .Where(_ => isPointerUp())
            .Subscribe(_ => {
                InitCameraMoveController();
            });
        this.UpdateAsObservable()
            .Where(x => isNoPointer())
            .Subscribe(x => { isDraging = false; });

        #endif
    }

    private void OnPointerDownValidation(PointerEventData data)
    {
        int touchCount = Input.touchCount;

        switch (touchCount)
        {
            case 1:
                {
                    Touch touch = Input.GetTouch(0);

                    touchFingerId = touch.fingerId;
                    break;
                }
            case 2:
                {
                    Touch touch0 = Input.GetTouch(0);
                    Touch touch1 = Input.GetTouch(1);

                    // タップしている2本の指のうち、適する方を選ぶ
                    float delta0 = (touch0.position - data.position).magnitude;
                    float delta1 = (touch1.position - data.position).magnitude;
                    Touch touch = delta0 < delta1 ? touch0 : touch1;

                    touchFingerId = touch.fingerId;
                    break;
                }
        }
    }

    private bool isPointerUp()
    {
        int touchCount = Input.touchCount;
        if (touchCount <= 0)
            return false;

        for (var i = 0; i < touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (touch.fingerId == touchFingerId)
            {
                return touch.phase == TouchPhase.Ended;
            }
            
        }
        return false;
    }

    private bool isNoPointer()
    {
        int touchCount = Input.touchCount;
        return isDraging && touchCount == 0;
    }
}
