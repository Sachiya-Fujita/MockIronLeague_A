using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class RoomMatchConfirmDialog : SelectDialog
{
    #region define


    #endregion

    #region variable

    [SerializeField]
    private InputField idInputField;

    private const int MAX_ROOM_ID_CHARACTER = 5;

    private string roomId = "";
    public string RoomId { get => roomId; }

    #endregion

    #region method

    // Start is called before the first frame update
    void Start()
    {
        setup();
    }

    protected override void OnActive(){
        idInputField.text = "";
        base.OnActive();
    }

    public override void setupEvent(){
        base.setupEvent();
        // okButton.OnSafeClickAsObservable()
        //     .Subscribe(_ =>{
        //         PhotonManager.Instance.RoomName = idInputField.text;
        //         connectPhoton(ConnectType.Join);
        //     })
        //     .AddTo(this);

        // cancelButton.OnSafeClickAsObservable()
        //     .Subscribe()
        //     .AddTo(this);

        idInputField.OnEndEditAsObservable()
            .Subscribe(result => {
                // 1行以上入力があればカット
                var id = result.GetFirstLine();
                // 文字数制限
                if (id.Length > MAX_ROOM_ID_CHARACTER) {
                    id = id.Substring(0, MAX_ROOM_ID_CHARACTER);
                }
                // 空白やspaceのチェック
                if(!String.IsNullOrWhiteSpace(id)){
                    roomId = id;
                }
            })
            .AddTo(this);
    }

    public IObservable<Unit> CloseAsObservable(){
        OnDeactive();
        return Observable.Return(Unit.Default);
    }

    private void connectPhoton(ConnectType connectType)
    {
        PhotonManager.Instance.Connect(connectType);
    }

    #endregion
}
