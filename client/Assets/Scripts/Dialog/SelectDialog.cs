using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
public abstract class SelectDialog : DialogBase
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
    // キャンセルボタン
    [SerializeField] 
    protected Button cancelButton;

    [SerializeField]
    private DialogAnimation dialogAnimation;
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
        closeButton.OnSafeClickAsObservable()
            .Subscribe(_ => {
                Debug.Log("close btn clicked");
                dialogAnimation.CloseAnimation(() => {
                    OnSelectIndex.OnNext(SelectIndex.Cancel);
                    OnDialogState.OnNext(DialogState.Close);
                });
                
            })
            .AddTo(this);

        cancelButton.OnSafeClickAsObservable()
            .Subscribe(_ => {
                Debug.Log("close btn clicked");
                dialogAnimation.CloseAnimation(() => {
                    OnSelectIndex.OnNext(SelectIndex.Cancel);
                    OnDialogState.OnNext(DialogState.Close);
                });
            })
            .AddTo(this);

        okButton.OnSafeClickAsObservable()
            .Subscribe(_ => {
                Debug.Log("ok btn clicked");
                dialogAnimation.CloseAnimation(() => {
                    OnSelectIndex.OnNext(SelectIndex.OK);
                    OnDialogState.OnNext(DialogState.Close);
                });
            })
            .AddTo(this);
    }

    protected override void OnActive()
    {
        dialogAnimation.InitAnimation();
    }

    #endregion
}
