using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

public class JumpController : MonoBehaviour
{
    #region define

    [SerializeField] Image buttonMask;
    private ObservableEventTrigger jumpControllerEventTrigger;
    private bool isCooldowning;
    private bool isJumpInput;

    #endregion define


    void Start()
    {
        jumpControllerEventTrigger = this.GetComponent<ObservableEventTrigger>();
        isCooldowning = false;
        initButtonMask();

        //ドラッグ等無いためマルチタップ問題なし
        this.jumpControllerEventTrigger.OnPointerDownAsObservable()
            .Where(_ => !isCooldowning)
            .Subscribe(_ => attackInput());
    }

    #region public method

    public bool GetJumpable()
    {
        return !isCooldowning && isJumpInput;
    }

    public void CoolDownToController()
    {
        isCooldowning = true;
        isJumpInput = false;
        buttonMask.enabled = true;
    }

    #endregion public method

    #region private method

    private void initButtonMask()
    {
        buttonMask.enabled = false;
        buttonMask.fillAmount = 1;
    }

    private void attackInput()
    {
        isJumpInput = true;
        //ToDo:ジャンプクールタイムのマジックナンバー対応
        Observable.Interval(TimeSpan.FromMilliseconds(25))
            .Take(100)
            .Select(_ => 0.01f)
            .Subscribe(i => {
                buttonMask.fillAmount -= i;
            }, () => {
                isCooldowning = false;
                initButtonMask();
            });
    }
    #endregion private method
}
