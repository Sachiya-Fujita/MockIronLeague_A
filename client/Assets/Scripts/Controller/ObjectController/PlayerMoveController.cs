using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

public class PlayerMoveController : MonoBehaviour
{
    #region define

    [SerializeField] private GameObject pointerPivot;
    [SerializeField] private GameObject fingerPointer;

    private float controllerArg = 0f;
    public float ControllerArg
    {
        get { return controllerArg; }
    }

    // コントローラの入力中かどうかを判定するメソッド
    private bool isMoveControllerInputing = false;

    // ニュートラルと判定する閾値
    private const int LEVEL_OF_NEUTRAL = 40;

    // 指ポインターの半径
    private const int RADIUS_OF_FINGER_POINTER = 150;

    private ObservableEventTrigger playerControllerEventTrigger;
    private int touchFingerId = -1;

    #endregion

    private void Start()
    {
        playerControllerEventTrigger = this.GetComponent<ObservableEventTrigger>();

    #if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN

        this.playerControllerEventTrigger.OnPointerDownAsObservable()
            .Subscribe(_ => pushInputController());
        this.UpdateAsObservable()
            .Where(x => Input.GetMouseButtonUp(0))
            .Subscribe(x => InitPlayerMoveController());
        this.UpdateAsObservable()
            .Where(x => isMoveControllerInputing)
            .Subscribe(x => moveFingerPointer(Input.mousePosition));

        #elif UNITY_IOS || UNITY_ANDROID

        this.playerControllerEventTrigger.OnPointerDownAsObservable()
            .Subscribe(pointerEventData => OnPointerDownValidation(pointerEventData));
        this.UpdateAsObservable()
            .Where(x => isMoveControllerInputing)
            .Subscribe(x => moveFingerPointer());
        this.UpdateAsObservable()
            .Where(x => isPointerUp())
            .Subscribe(x => InitPlayerMoveController());
        #endif

    }


    #region public method

    /// <summary>
    /// コントローラの初期化
    /// </summary>
    public void InitPlayerMoveController()
    {
        fingerPointer.SetActive(false);
        isMoveControllerInputing = false;
        touchFingerId = -1;

    }

    /// <summary>
    /// pointerがニュートラルかどうかを返すメソッド
    /// </summary>
    /// <returns></returns>
    public bool IsNeutral()
    {
        Vector3 diffPos = pointerPivot.transform.position - fingerPointer.transform.position;
        return diffPos.magnitude < LEVEL_OF_NEUTRAL;
    }

    /// <summary>
    /// 移動可能かどうかを返すメソッド
    /// </summary>
    /// <returns></returns>
    public bool GetMoveable()
    {
        return isMoveControllerInputing && !IsNeutral();
    }

    #endregion

    #region private method

    /// <summary>
    /// コントローラエリアを押したらpivotとfingerポインターが出る
    /// </summary>
    private void pushInputController()
    {
        fingerPointer.SetActive(true);
        // pointerPivot.transform.position = Input.mousePosition;
        isMoveControllerInputing = true;
    }

    private void pushInputController(Vector3 fingerPos)
    {
        fingerPointer.SetActive(true);
        // pointerPivot.transform.position = fingerPos;
        isMoveControllerInputing = true;
    }

    /// <summary>
    /// ポインタの角度によって進行方向変える
    /// 半径を超えていたら角度のみ保持し、その半径に設定する
    /// </summary>
    private void moveFingerPointer(Vector3 pointerPos)
    {
        Vector3 pointerPivotPos = pointerPivot.transform.position;
        Vector3 nowFingerPos = pointerPos;
        controllerArg = PointsVectorUtility.GetArg(pointerPivotPos, nowFingerPos);
        // 半径を超えてない時、指の位置がポインターの位置
        if (PointsVectorUtility.GetDistance(pointerPivotPos, nowFingerPos) <= RADIUS_OF_FINGER_POINTER)
        {
            fingerPointer.transform.position = nowFingerPos;
        }
        else
        {
            // ポインターの位置は角度のみ保持し、半径はRADIUS_OF_FINGER_POINTERとなる
            Vector3 expectationFingerPointerPos = new Vector3(Mathf.Sin(controllerArg * Mathf.Deg2Rad), Mathf.Cos(controllerArg * Mathf.Deg2Rad), 0f);
            fingerPointer.transform.position = pointerPivotPos + RADIUS_OF_FINGER_POINTER * expectationFingerPointerPos;
        }
    }

    private void moveFingerPointer()
    {
        int touchCount = Input.touchCount;
        if (touchCount <= 0)
            return;

        for (var i = 0; i < touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (touch.fingerId == touchFingerId)
            {
                moveFingerPointer(touch.position);
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

    private bool isPointerMoving()
    {
        int touchCount = Input.touchCount;
        if (touchCount <= 0)
            return false;

        for (var i = 0; i < touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (touch.fingerId == touchFingerId)
            {
                return touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary;
            }

        }
        return false;
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
                    pushInputController(touch.position);
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
                    pushInputController(touch.position);
                    break;
                }
        }
    }

    #endregion
}
