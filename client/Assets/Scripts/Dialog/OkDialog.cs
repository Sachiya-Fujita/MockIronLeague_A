using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
public abstract class OkDialog : DialogBase
{
    #region define

    #endregion

    #region SerializeField Define
    // ダイアログ閉じるボタン
    [SerializeField] 
    protected Button closeButton;
    // OKボタン
    [SerializeField] 
    protected Button okButton;
    // 説明
    [SerializeField] 
    private Text description;
    public Subject<SelectIndex> OnSelectIndex = new Subject<SelectIndex>();

    #endregion

    #region method
    void Start()
    {
        setup();
    }

    public virtual void setup()
    {
        setupEvent();
    }

    public virtual void setupEvent()
    {
        Debug.Log("base setup Event");
        closeButton.OnSafeClickAsObservable()
            .Subscribe(_ => {
                Debug.Log("close btn clicked");
                OnSelectIndex.OnNext(SelectIndex.Cancel);
                OnDialogState.OnNext(DialogState.Close);
            })
            .AddTo(this);

        okButton.OnSafeClickAsObservable()
            .Subscribe(_ => {
                Debug.Log("ok btn clicked");
                OnSelectIndex.OnNext(SelectIndex.OK);
                OnDialogState.OnNext(DialogState.Close);
            })
            .AddTo(this);
    }

    #endregion
}
