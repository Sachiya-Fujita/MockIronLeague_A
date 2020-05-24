using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using Photon.Realtime;


public class MatchingController : MonoBehaviour
{
    [SerializeField]
    private PlayerCell[] playerCells; 

    [SerializeField]
    private Text RoomNameText;
    [SerializeField]
    private GameObject RoomNameBg;

    [SerializeField]
    private Button backButton;

    [SerializeField]
    private GameObject openButtonObj;
    [SerializeField]
    private Button openButton;

    private PhotonPlayer[] photonPlayers;

    private string userName;
    private int playerId;

    private void Start()
    {
        if (PhotonManager.Instance.IsVisiableOpenButton())
        {
            openButtonObj.SetActive(true);
        }
        else
        {
            openButtonObj.SetActive(false);
        }
        userName = PlayerDataManager.Instance.UserName;
        playerId = PhotonManager.Instance.PlayerId;
        MatchingState matchingState = new MatchingState();
        if (PhotonManager.Instance.NowConnectType != ConnectType.RandomJoin)
        {
            RoomNameBg.SetActive(true);
            RoomNameText.text = PhotonManager.Instance.RoomName;
        }
        playerCells[0].InitPlayerCell(PhotonNetwork.player.NickName, (PlayerType)PhotonNetwork.player.CustomProperties["PlayerType"]);
        setupEvent();
    }

    private void setupEvent()
    {
        backButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                PhotonManager.Instance.DisConnectedPhotonManager();
                ScreenStateManager.Instance.GoToNextScene(ScreenStateType.Menu);
            });
        openButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                PhotonManager.Instance.SetIsVisiable(true);
                openButtonObj.SetActive(false);
            });
        this.UpdateAsObservable()
            .Where(_ => PhotonManager.Instance.IsMaxPlayerInThisRoom())
            .First()
            .Subscribe(_ =>
            {
                backButton.interactable = false;
                startPerformance();
            });
        Observable.Interval(TimeSpan.FromMilliseconds(500))
            .Subscribe(_ =>
            {
                photonPlayers = PhotonManager.Instance.PhotonPlayers;
                setPlayerCell();
            });
    }

    public void setPlayerCell()
    {
        string[] photonPlayersName = photonPlayers.Where(player => player.ID != playerId)
                                        .Select(player => player.NickName).ToArray();
        int[] photonPlayersId = photonPlayers.Where(player => player.ID != playerId)
                                .Select(player => player.ID).ToArray();
        for (int i = 1; i < playerCells.Length; i++)
        {
            if (i < photonPlayersName.Length + 1)
            {
                playerCells[i].InitPlayerCell(photonPlayersName[i - 1],
                (PlayerType)PhotonPlayer.Find(photonPlayersId[i - 1]).CustomProperties["PlayerType"]);
            }
            else
            {
                playerCells[i].CloseCell();
            }
        }
    }

    /// <summary>
    /// 猫になる演出についてのメソッド
    /// </summary>
    private void performance()
    {
        // ルームに保存した猫になる人のindexからPlayerIdを取得
        if (PhotonManager.Instance.GetPlayerIdByIndex((int)PhotonNetwork.room.CustomProperties["PlayerIndex"]) == PhotonNetwork.player.ID)
        {
            // 猫にする
            PhotonManager.Instance.ChangeCat();
            playerCells[0].ChangeType(PlayerType.Cat);
            setPlayerCell();
        }
        else
        {
            setPlayerCell();
        }
    }

    private void startPerformance()
    {
        Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            performance();
        }).AddTo(this);
    }
}