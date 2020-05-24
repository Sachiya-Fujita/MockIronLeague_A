using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class RoomMatchDialog : MonoBehaviour
{
    #region SerializeField Define
    // ダイアログ閉じるボタン
    [SerializeField]
    private Button dialogCloseButton;

    // ダイアログ外をタップした時閉じるためのボタン
    [SerializeField]
    private Button backGroundButton;

    // ルームを作るボタン
    [SerializeField]
    private Button roomCreateButton;

    // ルームに入るボタン
    [SerializeField]
    private Button roomJoinButton;

    // ルーム作るボタンとルーム入るボタンが統合されたview
    [SerializeField]
    private GameObject roomMatchIntegration;

    // ルームに入るボタンを押した後のview
    [SerializeField]
    private GameObject roomMatchJoin;

    // ルームIDを入力するInputField
    [SerializeField]
    private InputField roomNameInputField;

    // okボタン
    [SerializeField]
    private Button roomJoinOkButton;

    // Canvas
    [SerializeField]
    private GameObject canvas;

    #endregion

    private GameObject loadingDisplay = null;
    private Subject<MenuStateType> onMenuStateType = new Subject<MenuStateType>();
    public IObservable<MenuStateType> OnMenuStateTypeAsObservable { get { return onMenuStateType; } }

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.transform.SetParent(canvas.transform, false);
        InitDialog();
        setupEvent();
    }

    public void InitDialog()
    {
        roomMatchIntegration.SetActive(true);
        roomMatchJoin.SetActive(false);
    }

    private void setupEvent()
    {
        // ダイアログ閉じる時のイベント処理
        dialogCloseButton.OnSafeClickAsObservable()
            .Subscribe(_ => {
                SetupCloseDialogEvent();
             })
            .AddTo(this);
        // ダイアログ閉じる時のイベント処理
        backGroundButton.OnSafeClickAsObservable()
            .Subscribe(_ => {
                SetupCloseDialogEvent();
             })
            .AddTo(this);

        roomCreateButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                connectPhoton(ConnectType.CreateRoom);
            });

        roomJoinButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                roomMatchIntegration.SetActive(false);
                roomMatchJoin.SetActive(true);
            });
        roomJoinOkButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                PhotonManager.Instance.RoomName = roomNameInputField.text;
                connectPhoton(ConnectType.Join);
            });
    }

    private void SetupCloseDialogEvent()
    {
        onMenuStateType.OnNext(MenuStateType.Home);
    }

    private void connectPhoton(ConnectType connectType)
    {
        PhotonManager.Instance.Connect(connectType);
    }

}
