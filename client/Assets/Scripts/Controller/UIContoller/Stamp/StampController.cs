using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class StampController : MonoBehaviour
{
    [SerializeField] StampersController stampers;
    [SerializeField] StampButtonsController stamps;

    private PhotonView photonView;

    private void Start()
    {
        if (PhotonManager.Instance.IsConnect)
        {
            setEvent();
        }
    }

    private void setEvent()
    {
        photonView = GetComponent<PhotonView>();
        stamps.StampEventTrigger[0].OnPointerClickAsObservable()
            .Subscribe(_ =>
            {
                Debug.Log("stampが押された");
                if (photonView.isMine)
                {
                    // RPCの処理
                    photonView.RPC("stampSync", PhotonTargets.All, new object[] { PhotonManager.Instance.PlayerId, 0 });
                }
            });

        stamps.StampEventTrigger[1].OnPointerClickAsObservable()
            .Subscribe(_ =>
            {
                Debug.Log("stampが押された");
                if (photonView.isMine)
                {
                    // RPCの処理
                    photonView.RPC("stampSync", PhotonTargets.All, new object[] { PhotonManager.Instance.PlayerId, 1 });
                }
            });

        stamps.StampEventTrigger[2].OnPointerClickAsObservable()
            .Subscribe(_ =>
            {
                Debug.Log("stampが押された");
                if (photonView.isMine)
                {
                    // RPCの処理
                    photonView.RPC("stampSync", PhotonTargets.All, new object[] { PhotonManager.Instance.PlayerId, 2 });
                }
            });
    }

    [PunRPC]
    private void stampSync(int playerId, int stampId)
    {
        // Catだったら処理を行わない
        if (PhotonManager.Instance.NowPlayerType == PlayerType.Cat)
        {
            return;
        }
        else
        {
            stampers.Stampers
                .Where(stamper => stamper.StamperId == playerId)
                .First()
                .ShowStamp(stampId);
        }
    }

}
