using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

public class CatSkillController : MonoBehaviour
{
    #region define

    [SerializeField] Image buttonMask;
    [SerializeField] float coolDownSec;
    private ObservableEventTrigger skillControllerEventTrigger;
    private bool isCooldowning;
    private bool isSkillInput;

    #endregion define


    void Start()
    {
        skillControllerEventTrigger = this.GetComponent<ObservableEventTrigger>();
        initButtonMask();

        this.skillControllerEventTrigger.OnPointerDownAsObservable()
            .Where(_ => !isCooldowning)
            .Subscribe(_ => skillInput());
    }

    #region public method

    public bool GetSkillUsable()
    {
        return !isCooldowning && isSkillInput;
    }

    public void CoolDownToController()
    {
        isCooldowning = true;
        isSkillInput = false;
        buttonMask.enabled = true;
    }

    #endregion public method

    #region private method

    private void initButtonMask()
    {
        buttonMask.enabled = false;
        buttonMask.fillAmount = 1;
    }

    private void skillInput()
    {
        Debug.Log("インプット");
        isSkillInput = true;
        resetSkill();
    }

    private void resetSkill()
    {
        //クールタイム
        Observable.Interval(TimeSpan.FromMilliseconds(coolDownSec * 10))
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
