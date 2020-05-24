using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class TreasureBox : MonoBehaviour
{
    private PhotonView m_photonView = null;
    private PhotonView playerPhotonView = null;

    private new Collider collider;
    private Animator treasreAnimator;
    private bool isOpened;
    private const int OPEN_WAIT_SEC = 2;
    private bool isDogOpening; //false:猫
    private bool isOpening;

    GameObject progressUI;
    GameObject treasureMessageUI;

    void Awake()
    {
        m_photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        collider = GetComponent<Collider>();
        treasreAnimator = GetComponent<Animator>();
        progressUI = GameObject.Find("Canvas/MainController/TreasureProgress").gameObject;

        // Playerに衝突した時
        var OnTriggerEnterPlayer = collider.OnTriggerEnterAsObservable()
            .Where(collider => (collider.CompareTag("Cat") || collider.CompareTag("Dog")) && !isOpened && !isOpening)
            .Subscribe(collider => inTreasure(collider));

        // Playerが離れた時
        var OnTriggerExitPlayer = collider.OnTriggerExitAsObservable()
            .Where(collider => (collider.CompareTag("Cat") || collider.CompareTag("Dog")) && !isOpened && isOpening)
            .Subscribe(collider => outTreasure(collider));
    }


    /// <summary>
    /// 宝箱エリアに入る
    /// </summary>
    /// <param name="playerCollider"></param>
    private void inTreasure(Collider playerCollider)
    {
        if (!isMyCharactor(playerCollider))
        {
            return;
        }
        isOpening = true;
        AudioManager.Instance.PlaySEClipFromIndex(16, 0.6f);
        isDogOpening = playerCollider.CompareTag("Dog");
        StartCoroutine("open");
        progressUI.SetActive(true);
    }

    /// <summary>
    /// 宝箱から離れる
    /// </summary>
    /// <param name="playerCollider"></param>
    private void outTreasure(Collider playerCollider)
    {
        if (!isMyCharactor(playerCollider))
        {
            return;
        }
        isOpening = false;
        isDogOpening = playerCollider.CompareTag("Dog");
        StopCoroutine("open");
        progressUI.SetActive(false);
    }

    /// <summary>
    /// 操作してるキャラか確認
    /// </summary>
    private bool isMyCharactor(Collider playerCollider)
    {
        //ソロなら自分
        if (!PhotonManager.Instance.IsConnect)
        {
            return true;
        }
        else
        {
            playerPhotonView = playerCollider.gameObject.GetComponent<PhotonView>();
            //接触キャラが自分
            if (playerPhotonView.isMine)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    IEnumerator open()
    {//2秒経過後開く
        yield return new WaitForSeconds(OPEN_WAIT_SEC);
        AudioManager.Instance.PlaySEClipFromIndex(4, 0.6f);
        itemLooting();
        progressUI.SetActive(false);
        if (PhotonManager.Instance.IsConnect)
        {
            m_photonView.RPC("openAnim", PhotonTargets.All);
        }
        else
        {
            openAnim();
        }
    }

    private void itemLooting()
    {
        Debug.Log("isDog:" + isDogOpening);
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (isDogOpening)
        {
            gameManager.SetObtainedObject(drawDogItem(Random.value));
        }
        else
        {
            gameManager.SetObtainedObject(GameItemType.jewel);
        }
    }

    [PunRPC]
    private void openAnim()
    {
        isOpened = true;
        treasreAnimator.SetTrigger("open");
    }

    GameItemType drawDogItem(float rval)
    {
        //センサー40%
        if(rval < 0.4f)
        {
            return GameItemType.securityCamera;
        }
        //ミルク60%
        else
        {
            return GameItemType.bread;
        }

        //以下旧仕様
        // 10%
        if (rval < 0.1)
        {
            return GameItemType.securityCamera;
        }
        // 30%
        else if (rval < 0.4)
        {
            return GameItemType.bread;
        }
        // 50%
        else if (rval < 0.9)
        {
            return GameItemType.treasureCoin;
        }
        // 10%
        else
        {
            return GameItemType.empty;
        }
    }
}
