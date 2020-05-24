using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using UniRx.Triggers;
using System.Linq;
using Photon.Realtime;

public class PhotonManager : Photon.MonoBehaviour
{
    [SerializeField]
    private GameObject loadingDisplayPrefab;

    #region Setter or Getter Define

    private string roomName;
    public string RoomName
    {
        set{ roomName = value; }
        get{ return roomName; }
    }

    private ConnectType nowConnectType = ConnectType.Join;
    public ConnectType NowConnectType
    {
        get{ return nowConnectType; }
    }

    private PhotonPlayer[] photonPlayers;
    public PhotonPlayer[] PhotonPlayers
    {
        get{ return photonPlayers; }
    }

    //TODO: 仕様考える
    private int playerId;
    public int PlayerId
    {
        get { return playerId; }
    }

    private bool isConnect = false;
    public bool IsConnect
    {
        get{ return isConnect; }
    }

    private PlayerType nowPlayerType;
    public PlayerType NowPlayerType
    {
        get{ return nowPlayerType; }
        set{ nowPlayerType = value; }
    }

    private GameObject playerObj;
    public GameObject PlayerObj
    {
        get{ return playerObj; }
    }

    #endregion

    #region private Define

    private const int ROOM_MAX_PLAYER = 2;

    private RoomInfo[] roomInfos;

    private bool isCalledRoomInfoList = false;

    private GameObject loadingDisplay;

    #endregion

    #region Singleton

    /// <summary>
    /// シングルトンを継承できなかったため、PhotonManagerそのものをシングルトンにした
    /// </summary>
    private static PhotonManager instance;
    public static PhotonManager Instance
    {
        get
        {
            if (instance == null)
            {
                Type t = typeof(PhotonManager);

                instance = (PhotonManager)FindObjectOfType(t);
            }

            return instance;
        }
    }

    /// <summary>
    /// シングルトンのAwake処理
    /// </summary>
    virtual protected void Awake()
    {
        // 他のGameObjectにアタッチされているか調べる.
        // アタッチされている場合は破棄する.
        if (this != Instance)
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    #endregion

    #region public method

    public bool IsMaxPlayerInThisRoom()
    {
        initPlayerList();
        return PhotonNetwork.playerList.Length == ROOM_MAX_PLAYER;
    }

    /// <summary>
    /// 同期可能なplayerを作るメソッド
    /// </summary>
    public void CreatePlayerObject()
    {
        int distance = nowPlayerType == PlayerType.Cat ? 29 : 5;
        Vector3 tempPos = distance * ArgUtility.GetVector3ByPlayerId(playerId);
        tempPos.y += 4f;
        Vector3 playerPos = tempPos;
        playerObj = PhotonNetwork.Instantiate(PathMaster.PLAYER_PREFAB_ROOT + nowPlayerType.ToString(), playerPos, Quaternion.Euler(Vector3.zero), 0) as GameObject;
        Camera.main.transform.position = nowPlayerType == PlayerType.Cat ? playerPos + new Vector3(0f, 2f, -5f) : playerPos + new Vector3(0f, 1.5f, -4f);
        Camera.main.fieldOfView = nowPlayerType == PlayerType.Cat ? 60f : 40f; ;
    }

    /// <summary>
    /// 接続するメソッド
    /// </summary>
    public void Connect(ConnectType connectType)
    {
        displayLoadingView();
        nowConnectType = connectType;
        PhotonNetwork.ConnectUsingSettings("1.0.0");
        PhotonNetwork.playerName = PlayerDataManager.Instance.UserName;
    }

    /// <summary>
    /// 接続を切るメソッド
    /// </summary>
    public void DisConnectedPhotonManager()
    {
        isConnect = false;
        PhotonNetwork.Disconnect();
    }


    public bool IsGameStartable()
    {
        int thisRoomMemberCount = PhotonNetwork.playerList.Length;
        Debug.Log("ルーム人数：" + thisRoomMemberCount);
        return thisRoomMemberCount == ROOM_MAX_PLAYER;
    }

    public bool IsConnected()
    {
        return PhotonNetwork.connected;
    }

    public bool IsConnecting()
    {
        return PhotonNetwork.connecting;
    }

    public int GetPlayerIdByIndex(int Index)
    {
        int PlayerId = photonPlayers[Index].ID;
        return PlayerId;
    }

    public void ChangeCat()
    {
        nowPlayerType = PlayerType.Cat;
        PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "PlayerType", PlayerType.Cat } });
    }

    public PhotonPlayer GetPlayerByPlayerId(int playerId)
    {
        return PhotonPlayer.Find(playerId);
    }

    public void SetIsVisiable(bool isVisiable)
    {
        PhotonNetwork.room.IsVisible = isVisiable;
    }

    public bool IsVisiableOpenButton()
    {
        return PhotonNetwork.isMasterClient && nowConnectType == ConnectType.CreateRoom;
    }

    #endregion

    #region private method

    /// <summary>
    /// PlayerTypeをphotonPlayerに紐付けるメソッド
    /// </summary>
    private void setPlayerTypeForPhoton()
    {
        PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "PlayerType",  nowPlayerType} });
    }

    /// <summary>
    /// ロビーに入室するメソッド
    /// </summary>
    private void joinLobby()
    {
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// ルーム作成
    /// </summary>
    /// <param name="roomName">ルーム名</param>
    /// <param name="isVisible">表示するかどうか</param>
    private void createRoom(string roomName, bool isVisible)
    {
        RoomOptions roomOptions = new RoomOptions();
        // 部屋の最大人数
        roomOptions.MaxPlayers = ROOM_MAX_PLAYER;
        // 入室を許可する
        roomOptions.IsOpen = true;
        //ロビーから見えるようにする
        roomOptions.IsVisible = isVisible;
        PhotonNetwork.CreateRoom(roomName, roomOptions, null);
    }

    /// <summary>
    /// ルーム作成
    /// </summary>
    /// <param name="isVisible"></param>
    private void createRoom(bool isVisible)
    {
        Observable.Interval(TimeSpan.FromMilliseconds(50))
            .TakeUntil(this.UpdateAsObservable().Where(_ => {
                roomName = NumberUtility.RandomAToB(10000, 100000).ToString();
                return !roomInfos.Select(room => room.Name).Any(name => name.Equals("" + roomName));
            }).Take(1))
            .Subscribe(_ => {
                Debug.Log("roomName : " + roomName + "はすでに存在します");
            }, () => {
                createRoom("" + roomName, isVisible);
            });
        Debug.Log("");
    }

    private void joinRoomByRoomName()
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    /// <summary>
    /// ランダムにルームに入室します
    /// </summary>
    private void joinRandomRoom()
    {
        // roomCountが0だったらルームを作る
        if(!existsJoinableRoom())
        {
            createRoom(true);
            return;
        }
        PhotonNetwork.JoinRandomRoom();

    }

    // 入ることのできるルームが存在するか
    private bool existsJoinableRoom()
    {
        //ルーム一覧を取る
        if (roomInfos.Length == 0)
        {
            return false;
        }
        return roomInfos.Select(room => room.PlayerCount).Any(count => count < ROOM_MAX_PLAYER);
    }

    private void initPlayerList()
    {
        photonPlayers = PhotonNetwork.playerList
            .OrderBy(player => player.ID)
            .Select(player => player)
            .ToArray();

        // 自分のPlayerTypeが変更する可能性があるためPlayerTypeを取得する
        nowPlayerType = photonPlayers
            .Where(player => player.ID == playerId)
            .Select(player => (PlayerType)player.CustomProperties["PlayerType"])
            .First();
            
    }

    private void displayLoadingView()
    {
        loadingDisplay = GameObject.Instantiate(loadingDisplayPrefab) as GameObject;
        loadingDisplay.transform.SetParent(GameObject.Find("Canvas").transform, false);
        loadingDisplay.GetComponent<RectTransform>().SetAsLastSibling();
    }

    #endregion

    #region success callback

    /// <summary>
    /// photonに接続した時に呼ばれるコールバック
    /// </summary>
    void OnConnectedToMaster()
    {
        isConnect = true;
        nowPlayerType = PlayerDataManager.Instance.NowPlayerType;
        PlayerDataManager.Instance.JewelCount = 0;
        Debug.Log("isConnect : " + isConnect);
        joinLobby();
    }

    /// <summary>
    /// ロビーに入室したときに呼ばれるコールバック
    /// </summary>
    void OnJoinedLobby()
    {
        Debug.Log("PhotonManager OnJoinedLobby");
        PhotonNetwork.player.NickName = PlayerDataManager.Instance.UserName;
        setPlayerTypeForPhoton();
        // ルーム作成
        this.UpdateAsObservable()
            .Where(_ => isCalledRoomInfoList)
            .First()
            .Subscribe(_ => {
                switch (nowConnectType)
                {
                    case ConnectType.CreateRoom:
                        createRoom(false);
                        break;
                    case ConnectType.Join:
                        joinRoomByRoomName();
                        break;
                    case ConnectType.RandomJoin:
                        joinRandomRoom();
                        break;
                }
            });
    }

    /// <summary>
    /// ルームに入室しとときに呼ばれるコールバック
    /// </summary>
    void OnJoinedRoom()
    {
        Debug.Log("PhotonManager OnJoinedRoom");
        DestroyImmediate(loadingDisplay);
        ScreenStateManager.Instance.GoToNextScene(ScreenStateType.Matching);
        // PlayerId設定
        playerId = PhotonNetwork.player.ID;
        initPlayerList();
        if (PhotonNetwork.player.IsMasterClient)
        {
            int randomIndex = NumberUtility.RandomLendth(ROOM_MAX_PLAYER);
            Debug.Log("RandomIndex : " + randomIndex);
            PhotonNetwork.room.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "PlayerIndex", randomIndex } });
        }
    }

    /// <summary>
    /// photonの通信切った時に呼ばれるコールバック
    /// </summary>
    void OnDisconnectedFromPhoton()
    {
        Debug.Log("isConnect : " + isConnect);
        Debug.Log("DisConnected from photon");
        //TODO: 退出した時のロジック考える
        if (ScreenStateManager.Instance.NowScreenStateType == ScreenStateType.Main)
        {
            //TODO: 切断ダイアログ出す
            ScreenStateManager.Instance.GoToNextScene(ScreenStateType.Menu);
        }
        if (loadingDisplay != null)
        {
            DestroyImmediate(loadingDisplay);
        }
    }

    /// <summary>
    /// ルームを作った時の呼ばれるコールバック
    /// </summary>
    void OnCreatedRoom()
    {
        Debug.Log("OnCreated from photon");
    }

    //ルーム一覧が取れると
    void OnReceivedRoomListUpdate()
    {
        //ルーム一覧を取る
        roomInfos = PhotonNetwork.GetRoomList();
        if (roomInfos.Length == 0)
        {
            Debug.Log("ルームが一つもありません");
        }
        isCalledRoomInfoList = true;
    }

    #endregion

    #region failure callback

    /// <summary>
    /// ネットワークにつながらなかった時に呼ばれるコールバック
    /// </summary>
    void OnFailedToConnectToPhoton()
    {
        Debug.Log("OnFailedToConnectToPhoton from photon");
        DisConnectedPhotonManager();
    }

    /// <summary>
    /// ルーム作成ができなかった時に呼ばれるコールバック
    /// </summary>
    void OnPhotonCreateRoomFailed()
    {
        Debug.Log("OnPhotonCreateRoomFailed from photon");
        DisConnectedPhotonManager();
    }

    /// <summary>
    /// RoomにJoinできなかった時に呼ばれるコールバック
    /// </summary>
    void OnPhotonJoinRoomFailed()
    {
        Debug.Log("OnPhotonJoinRoomFailed from photon");
        DisConnectedPhotonManager();
    }

    /// <summary>
    /// ランダム入室できなかった時に呼ばれるコールバック
    /// </summary>
    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("OnPhotonRandomJoinFailed from photon");
        DisConnectedPhotonManager();
    }


    #endregion

    #region callback in Room

    /// <summary>
    /// 誰かがルームに入室した時に呼ばれるコールバック
    /// </summary>
    /// <param name="newPlayer">新しく入ってきたPlayer情報</param>
    void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        //TODO: 新しく入ってきたPlayerの名前を表示する
        initPlayerList();
    }

    /// <summary>
    /// 誰かがルームを退出した時に呼ばれるコールバック
    /// </summary>
    /// <param name="otherPlayer">残ってるユーザ</param>
    void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        //TODO: 退出した時のロジック考える
        if (ScreenStateManager.Instance.NowScreenStateType == ScreenStateType.Main)
        {
            //TODO: 誰かが切ったダイアログ出す
            DisConnectedPhotonManager();
            // ScreenStateManager.Instance.GoToNextScene(ScreenStateType.Menu);
        }
        else
        {
            initPlayerList();
        }
    }

    #endregion
}
