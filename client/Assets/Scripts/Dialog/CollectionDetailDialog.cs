using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class CollectionDetailDialog : DetailDialog
{
    [SerializeField]
    private Text descriptionText;

    [SerializeField]
    private Image collectionImage;

    private int collectionId;

    // Start is called before the first frame update
    void Start()
    {
        setup();
    }

    public override void setup(){
        base.setup();

        setupEvent();
    }

    public override void setupEvent(){
        base.setupEvent();


    }

    public IObservable<Unit> SetupAsObservable(int id){
        collectionId = id;
        // ダイアログ詳細の設定
        title.text = CollectionMenu.collectionModelDic[id].itemName;
        descriptionText.text = CollectionMenu.collectionModelDic[id].description;
        if(CollectionMenu.collectionModelDic[id].icon == null){
            collectionImage.sprite = Resources.Load(PathMaster.ITEM_ROOT + CollectionMenu.collectionModelDic[id].iconPath) as Sprite;
        }else
        {
            collectionImage.sprite = CollectionMenu.collectionModelDic[id].icon;
        }


        return Observable.Return(Unit.Default);
    }

    public IObservable<Unit> CloseAsObservable(){
        OnDeactive();
        CollectionMenu.collectionModelDic[collectionId].icon = null;
        return Observable.Return(Unit.Default);
    }
}
