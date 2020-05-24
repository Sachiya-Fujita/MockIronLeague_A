using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class CharacterDetailDialog : DetailDialog
{
    [SerializeField]
    private Text description;

    [SerializeField]
    private  Text hobby;

    [SerializeField]
    private  Text sex;

    [SerializeField]
    private  Text personarity;

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
        // ダイアログ詳細の設定
        if(CharacterMenu.characterModelDic[id].isSercret == true){
            title.text = "???";
            description.text = CharacterMenu.characterModelDic[99].description;
            hobby.text = CharacterMenu.characterModelDic[99].hobby;
            personarity.text = CharacterMenu.characterModelDic[99].personarity;
            sex.text = "不明";

            return Observable.Return(Unit.Default);
        }
        title.text = CharacterMenu.characterModelDic[id].characterName;
        description.text = CharacterMenu.characterModelDic[id].description;
        hobby.text = CharacterMenu.characterModelDic[id].hobby;
        personarity.text = CharacterMenu.characterModelDic[id].personarity;
        sex.text = "オス";

        return Observable.Return(Unit.Default);
    }

    public IObservable<Unit> CloseAsObservable(){
        OnDeactive();
        return Observable.Return(Unit.Default);
    }
}
