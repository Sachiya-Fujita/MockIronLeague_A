using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public abstract class DetailDialog : DialogBase
{
    #region define

    #endregion

    #region SerializeField Define
    // ダイアログ閉じるボタン
    [SerializeField] 
    protected Button closeButton;
    public Subject<SelectIndex> OnSelectIndex = new Subject<SelectIndex>();

    [SerializeField]
    private NonButtonDialogAnimation nonButtonDialogAnimation;

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
        closeButton.OnSafeClickAsObservable()
            .Subscribe(_ => {
                Debug.Log("close btn clicked");
                nonButtonDialogAnimation.CloseAnimation(() =>
                {
                    OnSelectIndex.OnNext(SelectIndex.Cancel);
                    OnDialogState.OnNext(DialogState.Close);
                });
            })
            .AddTo(this);
    }

    protected override void OnActive()
    {
        nonButtonDialogAnimation.InitAnimation();
    }


    #endregion
}
