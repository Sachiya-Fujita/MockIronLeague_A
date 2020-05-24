using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using System.Collections.Generic;

public class CharacterMenu : MenuBase
{
    #region define

    // TODO: インゲームとかに支障きたさないならOK、でも基本的には直したい
    public static Dictionary<int, CharacterModel> characterModelDic = new Dictionary<int, CharacterModel>
    {
        { 0,    new CharacterModel{ characterName = "オリバー",     iconPath = "CHR_DOG",   homeIconPath = "CHARACTER_MAIN",       description = WordMaster.OLIVER_DESCRIPTION,    personarity = WordMaster.OLIVER_PERSONARITY,    hobby = WordMaster.OLIVER_HOBBY,    icon = null, homeIcon = null, isSercret= false } },
        { 1,    new CharacterModel{ characterName = "ジョージ",     iconPath = "CHR_DOG",   homeIconPath = "CHARACTER_MAIN", description = WordMaster.GEORGE_DESCRIPTION,    personarity = WordMaster.GEORGE_PERSONARITY,    hobby = WordMaster.GEORGE_HOBBY,    icon = null, homeIcon = null, isSercret= false } },
        { 2,    new CharacterModel{ characterName = "ハリー",       iconPath = "CHR_DOG",   homeIconPath = "CHARACTER_MAIN",  description = WordMaster.HARRY_DESCRIPTION,     personarity = WordMaster.HARRY_PERSONARITY,     hobby = WordMaster.HARRY_HOBBY,     icon = null, homeIcon = null, isSercret= false } },
        { 3,    new CharacterModel{ characterName = "ジャック",     iconPath = "CHR_DOG",   homeIconPath = "CHARACTER_MAIN", description = WordMaster.JACK_DESCRIPTION,      personarity = WordMaster.JACK_PERSONARITY,      hobby = WordMaster.JACK_HOBBY,      icon = null, homeIcon = null, isSercret= false } },
        { 4,    new CharacterModel{ characterName = "チャーリー",   iconPath = "CHR_DOG",   homeIconPath = "CHARACTER_MAIN",  description = WordMaster.CHARILE_DESCRIPTION,   personarity = WordMaster.CHARILE_PERSONARITY,   hobby = WordMaster.CHARILE_HOBBY,   icon = null, homeIcon = null, isSercret= false } },
        { 5,    new CharacterModel{ characterName = "アーサー",     iconPath = "CHR_DOG",   homeIconPath = "CHARACTER_MAIN", description = WordMaster.ARTHUR_DESCRIPTION,    personarity = WordMaster.ARTHUR_HOBBY,          hobby = WordMaster.ARTHUR_HOBBY,    icon = null, homeIcon = null, isSercret= false } },
        { 6,    new CharacterModel{ characterName = "陰の功労者",   iconPath = "CHR_DOG",   homeIconPath = "CHARACTER_MAIN", description = WordMaster.SERCRET_DESCRIPTION,   personarity = WordMaster.SERCRET_PERSONARITY,   hobby = WordMaster.SERCRET_HOBBY,   icon = null, homeIcon = null, isSercret= true } },
        { 7,    new CharacterModel{ characterName = "サイバーマン", iconPath = "CHR_DOG",   homeIconPath = "CHARACTER_MAIN", description = WordMaster.SERCRET_DESCRIPTION,   personarity = WordMaster.SERCRET_PERSONARITY,   hobby = WordMaster.SERCRET_HOBBY,   icon = null, homeIcon = null, isSercret= true } },
        { 8,    new CharacterModel{ characterName = "ミア",         iconPath = "MIA_DUMMY_IMAGE",   homeIconPath = "MIA_DUMMY_IMAGE", description = WordMaster.SERCRET_DESCRIPTION,   personarity = WordMaster.SERCRET_PERSONARITY,   hobby = WordMaster.SERCRET_HOBBY,   icon = null, homeIcon = null,isSercret= true } },
        { 99,   new CharacterModel{ characterName = "???",          iconPath = "HATENA",   homeIconPath = "HATENA", description = WordMaster.SERCRET_DESCRIPTION,   personarity = WordMaster.SERCRET_PERSONARITY,   hobby = WordMaster.SERCRET_HOBBY,   icon = null, homeIcon = null, isSercret= true } },
    };


    #endregion

    #region variable

    [SerializeField]
    private CharacterDetailDialog dialog;

    [SerializeField]
    private GameObject cellObject;

    [SerializeField]
    private Transform parent;

    [SerializeField]
    private Image characterIcon;

    [SerializeField]
    private Image rankIcon;

    [SerializeField]
    private Button selectButton;

    [SerializeField]
    private Button detailButton;

    private int currentCharacterId = 0;
    public int CurrentCharacterId { get => currentCharacterId; }

    private const int DISPLAY_CHARACTER_CELL_COUNT = 8;

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

        selectButton.OnSafeClickAsObservable()
            .Where(_ => characterModelDic[currentCharacterId].isSercret == false)
            .Subscribe(_ => {
                PlayerDataManager.Instance.NowPlayerType = (PlayerType)Enum.ToObject(typeof(PlayerType), currentCharacterId);
                onMenuStateType.OnNext(MenuStateType.Home);
            })
            .AddTo(this);

        detailButton.OnSafeClickAsObservable()
            .SelectMany(_ => dialog.SetupAsObservable(currentCharacterId))
            .Subscribe(_ => charcterDetail())
            .AddTo(this);
    }

    public void setup()
    {
        characterIcon.sprite = CharacterMenu.characterModelDic[(int)PlayerDataManager.Instance.NowPlayerType].icon;

        for (int i=0; i < DISPLAY_CHARACTER_CELL_COUNT; i++){
            GameObject cell = Instantiate(cellObject, parent);
            cell.GetComponent<CharacterCell>().OnCreateAsObservable(i)
                .Subscribe()
                .AddTo(this);

            cell.GetComponent<CharacterCell>().OnIconClickAsObservable
                .Select(id => currentCharacterId = id)
                .Subscribe(_ => {
                    changeCharacter();
                    })
                .AddTo(this);
        }
    }

    private void charcterDetail(){
        dialog.OpenAsObservable()
            .Subscribe()
            .AddTo(this);
    }

    private void changeCharacter(){
        if(CharacterMenu.characterModelDic[currentCharacterId].isSercret == true){
            characterIcon.sprite = CharacterMenu.characterModelDic[99].icon;
        }else{
            characterIcon.sprite = characterModelDic[currentCharacterId].icon;
        }
    }

    #endregion
}
