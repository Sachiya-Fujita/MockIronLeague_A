using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class CatDecoy : MonoBehaviour
{
    private new Collider collider;
    private PhotonView m_photonView = null;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider>();
        m_photonView = GetComponent<PhotonView>();

        // Player攻撃コライダーに衝突した時
        var OnTriggerEnterAttack = collider.OnTriggerEnterAsObservable()
            .Select(collision => collision.tag)
            .Where(tag => tag == "PlayerAttack");
        // 自身をDestroy
        OnTriggerEnterAttack
            .Subscribe(_ => attaked());
    }

    private void attaked()
    {
        GameObject.FindObjectOfType<GamePopupMessage>().SetMessage("偽物だ！", 1.5f, GamePopUpColor.yellow);
        if (PhotonManager.Instance.IsConnect)
        {
            m_photonView.RPC("syncDestroy", PhotonTargets.All);
        }
        else
        {
            syncDestroy();
        }
    }

    [PunRPC]
    private void syncDestroy()
    {
        Destroy(gameObject);
    }
}
