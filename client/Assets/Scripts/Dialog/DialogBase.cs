using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public abstract class DialogBase : MonoBehaviour
{
    #region define

    #endregion

    #region SerializeField Define
    // ダイアログ外をタップした時閉じるためのボタン
    [SerializeField]
    private Button backGroundButton;
    // ブラー
    [SerializeField]
    private Image blur;
    // タイトル
    [SerializeField]
    protected Text title;
    /// 黒背景を付けるか
    [SerializeField]
    private bool addBackground = true;
    /// 背景を透明化するか否か
    [SerializeField]
    private bool isTransParent = false;
    /// 背景画像
    [SerializeField]
    private Image bg;

    public Subject<DialogState> OnDialogState = new Subject<DialogState>();

    #endregion

    #region method
    void Start()
    {

    }

    protected virtual void OnActive(){
        if (!addBackground){
            Debug.Log("背景なし");
            blur.gameObject.SetActive(false);
            return;
        }
        if (isTransParent){
            Debug.Log("透明化");
            blur.color = new Color (0, 0, 0, 0);
        }
    }

    protected virtual void OnDeactive(){
        this.gameObject.SetActive(false);
    }

    public virtual IObservable<DialogState> OpenAsObservable()
    {
        return Observable.Create<DialogState>(observer =>
        {
            open();

            var	disposables	= new CompositeDisposable();

            OnDialogState
                .Where(state => DialogState.Close == state)
                .Subscribe(x => {
                    observer.OnNext(x);
                    close();
                }, () => {})
                .AddTo(this)
                .AddTo(disposables);

            observer.OnCompleted();
            return Disposable.Create(() =>
            {
                //終了時の処理
                Debug.Log("Dispose");
            });
        });
    }

    protected virtual void open()
    {
        Debug.Log("open");
        AudioManager.Instance.PlaySEClipFromIndex(15, 0.6f);
        this.gameObject.SetActive(true);
        OnActive();
    }

    protected virtual void close(){
        Debug.Log("close");
        // AudioManager.Instance.PlaySEClipFromIndex(15, 0.6f);
        this.gameObject.SetActive(false);
    }
    #endregion
}
