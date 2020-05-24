using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using System.Collections.Generic;

public class CollectionDetailCell : MonoBehaviour
{
    #region define

    #endregion

    [SerializeField]
    private Image icon;

    [SerializeField]
    private Button button;

    private Subject<int> onIconClick = new Subject<int>();
    public IObservable<int> OnIconClickAsObservable { get { return onIconClick; }}

    void Start()
    {
        Debug.Log("Start");
    }

    private void setup(int id){
        Debug.Log("Setup");

        resourceDownLoad(id);
        Debug.Log("icon is : " + CollectionMenu.collectionModelDic[id].icon);
        var value = CollectionMenu.collectionModelDic[id];

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
        icon.sprite = Resources.Load<Sprite>(PathMaster.ITEM_ROOT + CollectionMenu.collectionModelDic[id].iconPath) as Sprite;
        CollectionMenu.collectionModelDic[id].icon = icon.sprite;
    }
}
