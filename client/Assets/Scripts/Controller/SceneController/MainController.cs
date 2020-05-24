using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class MainController : MonoBehaviour
{
    #region SerializeField Define

    [SerializeField]
    private Button button;

    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private GameObject readyScreen;

    [SerializeField]
    private CountDownText countDownText;

    [SerializeField]
    private GameObject jumpButton;

    [SerializeField]
    private GameObject attackButton;

    #endregion

    #region private Define

    private PlayerType playerType;

    private MainState mainState;

    #endregion

    private void Start()
    {
        mainState = new MainState();
        playerType = PhotonManager.Instance.NowPlayerType;
        AudioManager.Instance.ChangeBGM(0, 0.3f);
        //AudioManager.Instance.StopBGM();
        //AudioManager.Instance.PlayBGMClipFromIndex(0, 0.35f);
        initButton();
        button.OnPointerDownAsObservable()
            .Subscribe(_ => {
                ScreenStateManager.Instance.GoToNextScene(0);
            });
        Observable.Timer(TimeSpan.FromSeconds(5)).Subscribe(_ =>
        {
            createPlayerObject();
        }).AddTo(this);
    }

    private void initButton()
    {
        //TODO: 通信検証まだ
        if (playerType == PlayerType.Cat)
        {
            jumpButton.SetActive(true);
            attackButton.SetActive(false);
        }
        else
        {
            jumpButton.SetActive(false);
            attackButton.SetActive(true);
        }
    }

    private void createPlayerObject()
    {
        countDownText.InitTimer();
        readyScreen.SetActive(false);
        if (PhotonManager.Instance.IsConnect)
        {
            PhotonManager.Instance.CreatePlayerObject();
        }
        else
        {
            playerPrefab = (GameObject)Resources.Load(PathMaster.PLAYER_PREFAB_ROOT + playerType.ToString());
            int distance = playerType == PlayerType.Cat ? 29 : 5;
            Vector3 tempPos = distance * ArgUtility.GetVector3ByPlayerId(0);
            tempPos.y += 4f;
            Vector3 playerPos = tempPos;
            //TODO: 生成位置変更
            GameObject.Instantiate(playerPrefab, playerPos, Quaternion.Euler(Vector3.zero));
            Camera.main.transform.position = playerType == PlayerType.Cat ? playerPos + new Vector3(0f, 2f, -5f) : playerPos + new Vector3(0f, 1.5f, -4f);
            Camera.main.fieldOfView = playerType == PlayerType.Cat ? 60f : 40f; ;
        }
    }
}
