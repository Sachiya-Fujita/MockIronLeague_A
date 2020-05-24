using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

public class SweetController : MonoBehaviour
{

    #region define

    [SerializeField] Image buttonMask;
    [SerializeField] Text stockText;
    private ObservableEventTrigger sweetControllerEventTrigger;
    private bool isUseInput;
    private int itemNum;

    #endregion define


    void Start()
    {
        sweetControllerEventTrigger = this.GetComponent<ObservableEventTrigger>();
        itemNum = 0;
        
        //ドラッグ等無いためマルチタップ問題なし
        this.sweetControllerEventTrigger.OnPointerDownAsObservable()
            .Where(_ => itemNum > 0)
            .Subscribe(_ => useInput());
    }

    public bool GetUsable()
    {
        return itemNum > 0 && isUseInput;
    }

    /// <summary>
    /// ボタンの有効無効切り替え(マスクで実現)
    /// </summary>
    public void setButtonEnable(bool enable)
    {
        if(itemNum == 0)
        {
            buttonMask.enabled = true;
            return;
        }
        buttonMask.enabled = !enable;
    }

    /// <summary>
    /// 獲得した菓子パンを使えるようにUIにセット
    /// </summary>
    public void addSweet()
    {
        setEnable(true);
    }

    public void disableSweet()
    {
        setEnable(false);
        isUseInput = false;
    }

    private void setEnable(bool enabled)
    {
        if (enabled && itemNum < 2)
        {
            itemNum++;
        }
        else if(!enabled)
        {
            itemNum--;
        }
        stockText.text = itemNum.ToString() + "/2";

        if (itemNum == 0)
        {
            setButtonEnable(false);
        }
    }

    private void useInput()
    {
        Debug.Log("いんぷっと");
        isUseInput = true;
    }
}
