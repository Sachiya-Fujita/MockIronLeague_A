using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using System.Collections.Generic;
public class CharacterCell : MonoBehaviour
{
    #region define

    #endregion

    [SerializeField]
    private Image bg;

    [SerializeField]
    private Image icon;

    [SerializeField]
    private Button button;

    [SerializeField]
    private Text characterName;

    private Subject<int> onIconClick = new Subject<int>();
    public IObservable<int> OnIconClickAsObservable { get { return onIconClick; }}

    void Start()
    {
        Debug.Log("Start");
    }

    private void setup(int id){
        Debug.Log("Setup");

        var value = CharacterMenu.characterModelDic[id];

        if(CharacterMenu.characterModelDic[id].isSercret == true){
            characterName.text = CharacterMenu.characterModelDic[99].characterName;
            icon.sprite = CharacterMenu.characterModelDic[99].icon;
        }else{
            characterName.text = value.characterName;
            icon.sprite = value.icon;
        }

        setupEvent(id);
    }

    private void setupEvent(int id){
        button.OnSafeClickAsObservable()
            .Subscribe(_ => {
                onIconClick.OnNext(id);
            })
            .AddTo(this);
    }

    public IObservable<int> OnCreateAsObservable(int id){
        return Observable.Create<int>(observer =>
        {
            setup(id);

            var	disposables	= new CompositeDisposable();

            onIconClick
                .Subscribe(_ => {
                    observer.OnNext(id);
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

    private void resourceDownLoad(int id)
    {
        icon.sprite = Resources.Load<Sprite>(PathMaster.CHARACTER_ICON_ROOT + CharacterMenu.characterModelDic[id].iconPath) as Sprite;
        CharacterMenu.characterModelDic[id].icon = icon.sprite;
    }
}
