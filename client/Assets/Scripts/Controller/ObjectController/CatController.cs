using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class CatController : MonoBehaviour
{
    #region define
    private PhotonView m_photonView = null;

    private GameManager gameManager;
    private Camera mainCamera;
    private Animator cameraAnimator;
    private Vector3 forward = new Vector3(0f, 0f, 1f);
    // TODO: 動的に変更可にする
    private float moveSpeed = 0.125f;

    private PlayerMoveController playerMoveController;
    private JumpController jumpController;
    private CatSkillController dashSkillController;
    private CatSkillController decoySkillController;
    private CatSkillController smokeSkillController;

    [SerializeField] private GameObject catDecoyPrefab;
    [SerializeField] private GameObject catSmokePrefab;

    //接地判定
    private bool isGround = false;
    public bool IsGround
    {
        get {
            return isGround;
        }
        set {
            isGround = value;
        }
    }

    // コントローラーの猶予
    private const float ARGS_BUFFER = 10f;
    [SerializeField]
    GameObject FootPrints;

    private PhotonView photonView;

    private CatAnimation catAnimation;

    private new Rigidbody rigidbody;
    private float upspeed = 300;

    private int playerId = -1;
    public int PlayerId
    {
        get { return playerId; }
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        m_photonView = GetComponent<PhotonView>();
        if (PhotonManager.Instance.IsConnect)
        {
            gameManager = GameObject.FindObjectOfType<GameManager>();
            photonView = this.GetComponent<PhotonView>();
            if (photonView.isMine)
            {
                playerId = PhotonManager.Instance.PlayerId;
                initCatController();
            }

            gameManager.ObserveEveryValueChanged(x => x.IsCatWin)
                .Where(x => x)
                .Subscribe(_ => EndGame());
        }
        else
        {
            initCatController();
        }

        catAnimation = GetComponent<CatAnimation>();
        rigidbody = GetComponent<Rigidbody>();
        putFootPrints();
    }

    public void EndGame()
    {
        if (photonView.isMine)
        {
            photonView.RPC("finish", PhotonTargets.All, playerId);
        }
    }

    /// <summary>
    /// コントローラの初期化
    /// </summary>
    private void initCatController()
    {
        playerMoveController = GameObject.Find(PathMaster.CAT_MOVE_CONTROLLER).GetComponent<PlayerMoveController>();
        jumpController = GameObject.Find(PathMaster.JUMP_CONTROLLER).GetComponent<JumpController>();
        dashSkillController = GameObject.Find(PathMaster.DASH_SKILL_CONTROLLER).GetComponent<CatSkillController>();
        decoySkillController = GameObject.Find(PathMaster.DECOY_SKILL_CONTROLLER).GetComponent<CatSkillController>();
        smokeSkillController = GameObject.Find(PathMaster.SMOKE_SKILL_CONTROLLER).GetComponent<CatSkillController>();
        mainCamera = Camera.main;

        cameraAnimator = mainCamera.GetComponent<Animator>();
        CameraController cameraController = mainCamera.GetComponent<CameraController>();
        cameraController.Player = this.gameObject;
        cameraController.InitCameraController();

        this.FixedUpdateAsObservable()
            .Subscribe(x => playerBehavior());

        this.FixedUpdateAsObservable()
            .Where(x => jumpController.GetJumpable())
            .Subscribe(_ => catJump());

        this.FixedUpdateAsObservable()
            .Where(x => dashSkillController.GetSkillUsable())
            .Subscribe(_ => useDashSkill());

        this.FixedUpdateAsObservable()
            .Where(x => decoySkillController.GetSkillUsable())
            .Subscribe(_ => useDecoySkill());

        this.FixedUpdateAsObservable()
            .Where(x => smokeSkillController.GetSkillUsable())
            .Subscribe(_ => useSmokeSkill());
    }
    #region private method

    /// <summary>
    /// プレイヤーの挙動を変更するメソッド
    /// </summary>
    private void playerBehavior()
    {
        if (!playerMoveController.GetMoveable())
        {
            catAnimation.stateProcessor.state.Value = catAnimation.stateIdle;
            return;
        }

        float CatArgs = transform.rotation.eulerAngles.y;
        float targetArgs = ArgUtility.CastNomalArgByOverArg(ArgUtility.CastPlusArgByMinusArg(playerMoveController.ControllerArg) + mainCamera.transform.rotation.eulerAngles.y);
        if (NumberUtility.BetweenAAndB(CatArgs, targetArgs - ARGS_BUFFER, targetArgs + ARGS_BUFFER))
        {
            moveForwardCat();
        }
        else
        {
            rotateCat(targetArgs);
        }
    }

    /// <summary>
    /// 親オブジェクトごとplayerを移動する
    /// </summary>
    private void moveForwardCat()
    {
        this.transform.Translate(forward * moveSpeed);
        catAnimation.stateProcessor.state.Value = catAnimation.stateRun;
    }

    /// <summary>
    /// ねこだけ回転する
    /// </summary>
    /// <param name="targetArgs"></param>
    /// <param name="gorillaArgs"></param>
    private void rotateCat(float targetArgs)
    {
        transform.rotation = Quaternion.Euler(0f, targetArgs, 0f);
    }

    /// <summary>
    /// 猫ジャンプ
    /// </summary>
    private void catJump()
    {
        Debug.Log("jump");
        rigidbody.AddForce(transform.up * upspeed);
        jumpController.CoolDownToController();
    }

    /// <summary>
    /// 足跡スキル作動
    /// </summary>
    private void useDashSkill()
    {
        GameObject.FindObjectOfType<GamePopupMessage>().SetMessage("スキル「猫の瞬足」", 1.5f, GamePopUpColor.white);
        AudioManager.Instance.PlaySEClipFromIndex(17, 0.6f);
        dashSkillController.CoolDownToController();
        cameraAnimator.SetBool("usingSkill", true);

        moveSpeed = 0.15f
;
        //終了演出は前から
        Observable.Timer(System.TimeSpan.FromMilliseconds(1200)).
            Subscribe(_ => skillAnimEnd());
        //スキル有効時間：1.5sec
        Observable.Timer(System.TimeSpan.FromMilliseconds(1500)).
            Subscribe(_ => dashSkillEnd());
    }

    private void skillAnimEnd()
    {
        cameraAnimator.SetBool("usingSkill", false);
    }

    private void dashSkillEnd()
    {
        moveSpeed = 0.125f;
    }

    /// <summary>
    /// デコイ設置
    /// </summary>
    private void useDecoySkill()
    {
        GameObject.FindObjectOfType<GamePopupMessage>().SetMessage("スキル「猫の分身」", 1.5f, GamePopUpColor.white);
        AudioManager.Instance.PlaySEClipFromIndex(8, 1f);
        if (PhotonManager.Instance.IsConnect)
        {
            photonView.RPC("syncDecoy", PhotonTargets.All);
        }
        else
        {
            syncDecoy();
        }
        decoySkillController.CoolDownToController();
    }
    [PunRPC]
    private void syncDecoy()
    {
        //一体のみ存在
        CatDecoy oldDecoy = GameObject.FindObjectOfType<CatDecoy>();
        if (oldDecoy != null)
        {
            Destroy(oldDecoy.gameObject);
        }
        
        Vector3 t = transform.position;
        Vector3 pos = new Vector3(t.x, t.y + 2.5f, t.z);
        Instantiate(catDecoyPrefab, pos, transform.rotation);
    }


    /// <summary>
    /// 煙幕設置
    /// </summary>
    private void useSmokeSkill()
    {
        GameObject.FindObjectOfType<GamePopupMessage>().SetMessage("スキル「煙幕」", 1.5f, GamePopUpColor.white);
        AudioManager.Instance.PlaySEClipFromIndex(5, 1);
        if (PhotonManager.Instance.IsConnect)
        {
            photonView.RPC("syncSmoke", PhotonTargets.All);
        }
        else
        {
            syncSmoke();
        }
        smokeSkillController.CoolDownToController();
    }
    [PunRPC]
    private void syncSmoke()
    {
        GameObject s = Instantiate(catSmokePrefab, transform.position, transform.rotation);
        Observable.Timer(System.TimeSpan.FromMilliseconds(8000)).
            Subscribe(_ => Destroy(s));
    }

    private void putFootPrints()
    {
        //接地中のみ足跡つける
        if (isGround)
        {
            if (PhotonManager.Instance.IsConnect)
            {
                m_photonView.RPC("syncFootPrints", PhotonTargets.All);
            }
            else
            {
                syncFootPrints();
            }
        }
        Observable.Timer(TimeSpan.FromMilliseconds(200))
            .Subscribe(_ => putFootPrints());
    }

    [PunRPC]
    private void syncFootPrints()
    {
        Quaternion rot = Quaternion.AngleAxis(180, Vector3.up);
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z);

        Instantiate(FootPrints, pos, transform.rotation * rot);
    }

    [PunRPC]
    private void finish(int playerId)
    {
        PlayerDataManager.Instance.WinnerPlayer = PhotonManager.Instance.GetPlayerByPlayerId(playerId);
        Debug.Log("猫のPlayerId: " + PlayerDataManager.Instance.WinnerPlayer.ID);
        gameManager.EndBattle();
    }

    #endregion
}
