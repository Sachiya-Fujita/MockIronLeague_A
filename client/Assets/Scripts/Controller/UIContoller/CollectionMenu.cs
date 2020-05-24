using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using System.Collections.Generic;

public class CollectionMenu : MenuBase
{
    #region define

    // TODO: インゲームとかに支障きたさないならOK、でも基本的には直したい
    public static Dictionary<int, CollectionModel> collectionModelDic = new Dictionary<int, CollectionModel>
    {
        { 0,    new CollectionModel{ id = 0,    itemName = "テスト",            iconPath = "ORANGE_JEWELRY",        description = WordMaster.DIAMOND_DETAIL,        icon = null } },
        { 1,    new CollectionModel{ id = 1,    itemName = "アメジスト",        iconPath = "ORANGE_JEWELRY",        description = WordMaster.AMETHYST_DETAIL,       icon = null } },
        { 2,    new CollectionModel{ id = 2,    itemName = "アクアマリン",      iconPath = "PINK_BIG_JEWELRY",      description = WordMaster.AQUAMARINE_DETAIL,     icon = null } },
        { 3,    new CollectionModel{ id = 3,    itemName = "シトリンクォーツ",  iconPath = "PINK_JEWELRY",          description = WordMaster.CITRINE_QUARTZ_DETAIL, icon = null } },
        { 4,    new CollectionModel{ id = 4,    itemName = "ローズクォーツ",    iconPath = "RED_JEWELRY",           description = WordMaster.ROSE_QUARTZ_DETAIL,    icon = null } },
        { 5,    new CollectionModel{ id = 5,    itemName = "ラピスラズリ",      iconPath = "TRIANGLE",              description = WordMaster.LAPIS_LAZULI_DETAIL,   icon = null } },
        { 6,    new CollectionModel{ id = 6,    itemName = "パール",            iconPath = "DIAMOND",               description = WordMaster.PEARL_DETAIL,          icon = null } },
        { 7,    new CollectionModel{ id = 7,    itemName = "サファイア",        iconPath = "YELLOW_JEWELRY",        description = WordMaster.SAPPHIRE_DETAIL,       icon = null } },
        { 8,    new CollectionModel{ id = 8,    itemName = "ルビー",            iconPath = "RUBY",                  description = WordMaster.RUBY_DETAIL,           icon = null } },
        { 9,    new CollectionModel{ id = 9,    itemName = "エメラルド",        iconPath = "YELLOW_JEWELRY",        description = WordMaster.EMERALD_DETAIL,        icon = null } },
        { 10,   new CollectionModel{ id = 10,   itemName = "ダイアモンド",      iconPath = "DIAMOND",               description = WordMaster.DIAMOND_DETAIL,        icon = null } },
        { 11,   new CollectionModel{ id = 11,   itemName = "ダイアモンド",      iconPath = "DIAMOND",               description = WordMaster.EMERALD_DETAIL,        icon = null } },
    };

    #endregion

    #region variable

    [SerializeField]
    private CollectionDetailDialog dialog;

    [SerializeField]
    private GameObject cellObject;

    [SerializeField]
    private Transform parent;

    #endregion

    #region method

    private void Start(){
        setup();
        setupEvent();
    }

    public override void setupEvent(){
        backButton.OnSafeClickAsObservable()
            .SelectMany(_ => dialog.CloseAsObservable())
            .Subscribe(_ => {
                onMenuStateType.OnNext(MenuStateType.Home);
                })
            .AddTo(this);
    }

    private void setup(){
        foreach (var key in collectionModelDic.Keys)
        {
            GameObject cell = Instantiate(cellObject, parent);
                cell.GetComponent<CollectionDetailCell>().OnCreateAsObservable(key)
                    .Subscribe()
                    .AddTo(this);

                cell.GetComponent<CollectionDetailCell>().OnIconClickAsObservable
                    .SelectMany(id => dialog.SetupAsObservable(id))
                    .Subscribe(_ => {
                        collectionDetail();
                        })
                    .AddTo(this);
        }
    }

    private void collectionDetail(){
        dialog.OpenAsObservable()
            .Subscribe()
            .AddTo(this);
    }

    // private  void resetIcons(){
    //     foreach (var id in collectionModelDic.Keys)
    //     {
    //         collectionModelDic[id].icon = null;
    //     }
    // }

    #endregion
}
