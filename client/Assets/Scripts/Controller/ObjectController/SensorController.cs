using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

public class SensorController : MonoBehaviour
{

    #region define

    [SerializeField] Image buttonMask;
    [SerializeField] Text stockText;
    private ObservableEventTrigger sensorControllerEventTrigger;
    private bool isUseInput;
    private bool hasItem;

    #endregion define


    void Start()
    {
        sensorControllerEventTrigger = this.GetComponent<ObservableEventTrigger>();

        //ドラッグ等無いためマルチタップ問題なし
        this.sensorControllerEventTrigger.OnPointerDownAsObservable()
            .Where(_ => hasItem)
            .Subscribe(_ => useInput());
    }

    public bool GetUsable()
    {
        return hasItem && isUseInput;
    }

    /// <summary>
    /// 防犯センサーをセット
    /// </summary>
    public void addSensor()
    {
        setEnable(true);
        stockText.text = "1/1";
    }

    public void disableSensor()
    {
        setEnable(false);
        isUseInput = false;
        stockText.text = "0/1";
    }

    private void setEnable(bool enabled)
    {
        buttonMask.enabled = !enabled;
        hasItem = enabled;
    }

    private void useInput()
    {
        isUseInput = true;
    }
}
