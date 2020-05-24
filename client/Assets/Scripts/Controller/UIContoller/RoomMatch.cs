using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class RoomMatch : MenuBase
{
    #region define

    #endregion

    #region variable

    [SerializeField]
    private RoomMatchConfirmDialog dialog;

    [SerializeField]
    private Button createRoomButton;

    [SerializeField]
    private Button enterRoomButton;

    #endregion

    #region method

    // Start is called before the first frame update
    void Start()
    {
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

        createRoomButton.OnSafeClickAsObservable()
            .Subscribe(_ => {
                connectPhoton(ConnectType.CreateRoom);
            })
            .AddTo(this);

        enterRoomButton.OnSafeClickAsObservable()
            .Subscribe(_ => {
                Debug.Log("enter room");
                confirmRoomId();
            })
            .AddTo(this);

        dialog.OnSelectIndex
            .Where(index => SelectIndex.OK == index)
            .Subscribe(_ => {
                Debug.Log("roomID id" + dialog.RoomId);
                if(!String.IsNullOrEmpty(dialog.RoomId)){
                    Debug.Log("Join room");
                    PhotonManager.Instance.RoomName = dialog.RoomId;
                    connectPhoton(ConnectType.Join);
                }
            })
            .AddTo(this);
    }

    private void setup(){

    }

    private void confirmRoomId(){
        dialog.OpenAsObservable()
            .Subscribe()
            .AddTo(this);
    }

    private void connectPhoton(ConnectType connectType)
    {
        PhotonManager.Instance.Connect(connectType);
    }

    #endregion
}
