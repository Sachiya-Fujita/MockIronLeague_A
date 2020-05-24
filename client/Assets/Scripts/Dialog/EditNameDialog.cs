using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class EditNameDialog : SelectDialog
{
    #region define

    #endregion

    #region variable
    // 入力欄
    [SerializeField]
    private InputField inputField;

    private String userName = "オリバー";
    public string UserName { get => userName; set => userName = value; }

    private const int MAX_USER_NAME_CHARACTER = 8;

    #endregion

    #region method
    void Start()
    {
        userName = PlayerDataManager.Instance.UserName;
        setup();
        setupEvent();
    }

    // Update is called once per frame
    public override void setup()
    {
        // 元々の名前をInputFieldへ
        if(String.IsNullOrEmpty(PlayerDataManager.Instance.UserName)){
            inputField.text = userName;
            return;
        }
        inputField.text = PlayerDataManager.Instance.UserName;
    }

    public override void setupEvent()
    {
        base.setupEvent();
        inputField.OnEndEditAsObservable()
            .Subscribe(result => {
                // 1行以上入力があればカット
                var name = result.GetFirstLine();
                // 文字数制限
                if (name.Length > MAX_USER_NAME_CHARACTER) {
                    name = name.Substring(0, MAX_USER_NAME_CHARACTER);
                }
                // 空白やspaceのチェック
                if(!String.IsNullOrWhiteSpace(name)){
                    userName = name;
                }
            })
            .AddTo(this);
    }

    protected override void open()
    {
        base.open();
        setup();
    }

    private void RefreshName(){
        userName = PlayerDataManager.Instance.UserName;
    }

    #endregion
}
