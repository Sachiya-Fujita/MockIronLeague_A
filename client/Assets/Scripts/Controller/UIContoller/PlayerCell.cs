using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;

public class PlayerCell : MonoBehaviour
{
    [SerializeField]
    private Text userNameText;

    [SerializeField]
    private Image playerTypeImage;

    [SerializeField]
    private Image spotLight;

    [SerializeField]
    private GameObject smoke;

    private string userName;
    public string UserName
    {
        get{ return userName; }
    }

    //TODO: 作ったはいいものの使わなそう
    private int cellPlayerId;
    public int CellPlayerId
    {
        get{ return cellPlayerId; }
        set{ cellPlayerId = value; }
    }

    private PlayerType cellPlayerType;
    public PlayerType CellPlayerType
    {
        get{ return cellPlayerType; }
    }

    private PhotonView photonView;
    private Sequence seq;
    private bool isPerformanceNow;

    private void Start()
    {
        isPerformanceNow = false;
        photonView = GetComponent<PhotonView>();
    }

    public void InitPlayerCell(string userName, PlayerType playerType)
    {
        if(this.gameObject != null)
        {
            this.gameObject.SetActive(true);
            setUserName(userName);
            setPlayerType(playerType);
        }
    }

    public void ChangeType(PlayerType playerType)
    {
        setPlayerType(playerType);
    }

    public void CloseCell()
    {
        refreshCell();
        this.gameObject.SetActive(false);
    }

    private void refreshCell()
    {
        setUserName("");
        setPlayerType(PlayerType.Oliver);
    }

    private void setUserName(string userName)
    {
        this.userName = userName;
        userNameText.text = userName;
    }

    private void setPlayerType(PlayerType playerType)
    {
        this.cellPlayerType = playerType;
        if (this.cellPlayerType == PlayerType.Cat)
        {
            // 演出
            if (!isPerformanceNow)
            {
                Observable.Timer(TimeSpan.FromMilliseconds(100))
                    .Subscribe(_ => {
                        doAnimation(playerType);
                     });
            }
        }
        else
        {
            // TODO: 猫以外の画像
            playerTypeImage.sprite = CharacterMenu.characterModelDic[(int)playerType].homeIcon;
        }
    }

    private void doAnimation(PlayerType playerType)
    {
        seq = DOTween.Sequence()
            .OnStart(() => {
                isPerformanceNow = true;
            })
            .Append(
                spotLight.DOFade(1f, 0.2f).SetEase(Ease.OutElastic)
            )
            .AppendInterval(1f)
            .AppendCallback(() =>
            {
                smoke.SetActive(true);
            })
            .AppendInterval(1f)
            .AppendCallback(() => {
                //TODO: 画像が置き換わる演出,　猫
                playerTypeImage.sprite = CharacterMenu.characterModelDic[(int)playerType].homeIcon;
            })//
            .AppendInterval(1f)
            .AppendCallback(() =>
            {
                smoke.SetActive(false);
            })
            .AppendInterval(5f)
            .AppendCallback(() =>
            {
                if (photonView.isMine)
                {
                    photonView.RPC("GotoMainScene", PhotonTargets.All);
                }
            });
    }

    [PunRPC]
    public void GotoMainScene()
    {
        ScreenStateManager.Instance.GoToNextScene(ScreenStateType.Main);
    }
}
