using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

public class AttackController : MonoBehaviour
{
    #region define

    [SerializeField] Image buttonMask;
    private ObservableEventTrigger attackControllerEventTrigger;
    private bool isCooldowning;
    private bool isAttackInput;

    private IDisposable cooldowner;

    #endregion define


    void Start()
    {
        attackControllerEventTrigger = this.GetComponent<ObservableEventTrigger>();
        isCooldowning = false;
        initButtonMask();

        //ドラッグ等無いためマルチタップ問題なし
        this.attackControllerEventTrigger.OnPointerDownAsObservable()
            .Where(_ => !isCooldowning)
            .Subscribe(_ => attackInput());
    }

    #region public method

    public bool GetAttackable() 
    {
        return !isCooldowning && isAttackInput;
    }

    public void CoolDownToController() {
        isCooldowning = true;
        isAttackInput = false;
        buttonMask.enabled = true;
    }

    public void RecoveryCoolDown()
    {
        isCooldowning = false;
        isAttackInput = false;
        if(cooldowner != null)
        {
            cooldowner.Dispose();
        }
        initButtonMask();
    }

    #endregion public method

    #region private method

    private void initButtonMask()
    {
        buttonMask.enabled = false;
        buttonMask.fillAmount = 1;
    }

    private void attackInput() {
        isAttackInput = true;
        //ToDo:攻撃クールタイムのマジックナンバー対応
        cooldowner = Observable.Interval(TimeSpan.FromMilliseconds(50))
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
