using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class GorillaController : MonoBehaviour
{
    #region define

    private GameManager gameManager;
    private Camera mainCamera;
    private Vector3 forward = new Vector3(0f, 0f, 1f);
    // TODO: 動的に変更可にする
    private float moveSpeed = 0.1f; 

    //コントローラー等
    private PlayerMoveController playerMoveController;
    private AttackController attackController;
    private SweetController sweetController;
    private SensorController sensorController;
    private SkillController skillController;

    //アイテム，スキル効果等で使用
    [SerializeField] private AttackCollider attackCollider;
    private IDisposable attacker;
    private bool isAttacking;
    [SerializeField] private GameObject sensorPrefab;
    private Animator cameraAnimator;
    [SerializeField] private GameObject sweetEffect;

    // コントローラーの猶予
    private const float ARGS_BUFFER = 10f;
    private PhotonView photonView;

    private DogAnimation dogAnimation;

    private GamePopupMessage popupMessage;
    
    private int playerId = -1;
    public int PlayerId
    {
        get{ return playerId; }
    }

    #endregion


    // Start is called before the first frame update
    void Start()
    {
        if (PhotonManager.Instance.IsConnect)
        {
            gameManager = GameObject.FindObjectOfType<GameManager>();
            photonView = this.GetComponent<PhotonView>();
            if (photonView.isMine)
            {
                playerId = PhotonManager.Instance.PlayerId;
                initGorillaController();
            }

            gameManager.ObserveEveryValueChanged(x => x.IsDogWin)
                .Where(x => x)
                .Subscribe(_ => EndGame());
        }
        else
        {
            initGorillaController();
        }

        dogAnimation = GetComponent<DogAnimation>();
        popupMessage = GameObject.FindObjectOfType<GamePopupMessage>();
    }

    public void EndGame()
    {
        if (photonView.isMine)
        {
            photonView.RPC("catchCat", PhotonTargets.All, playerId);
        }
    }

    /// <summary>
    /// コントローラの初期化
    /// </summary>
    private void initGorillaController()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        playerMoveController = GameObject.Find(PathMaster.DOG_MOVE_CONTROLLER).GetComponent<PlayerMoveController>();
        attackController = GameObject.Find(PathMaster.ATTACK_CONTROLLER).GetComponent<AttackController>();
        sweetController = GameObject.Find(PathMaster.SWEET_CONTROLLER).GetComponent<SweetController>();
        sensorController = GameObject.Find(PathMaster.SENSOR_CONTROLLER).GetComponent<SensorController>();
        skillController = GameObject.Find(PathMaster.SKILL_CONTROLLER).GetComponent<SkillController>();
        mainCamera = Camera.main;
        CameraController cameraController = mainCamera.GetComponent<CameraController>();
        cameraController.Player = this.gameObject;
        cameraController.InitCameraController();
        cameraAnimator = mainCamera.GetComponent<Animator>();

        this.FixedUpdateAsObservable()
            .Where(_ => isInBattleState())
            .Subscribe(x => playerBehavior());
        this.FixedUpdateAsObservable()
            .Where(x => attackController.GetAttackable() && isInBattleState())
            .Subscribe(_ => gorillaAttack());
        this.FixedUpdateAsObservable()
            .Where(x => sweetController.GetUsable() && isInBattleState())
            .Subscribe(_ => useSweet());
        this.FixedUpdateAsObservable()
            .Where(x => sensorController.GetUsable() && isInBattleState())
            .Subscribe(_ => useSensor());
        this.FixedUpdateAsObservable()
            .Where(x => skillController.GetSkillUsable() && isInBattleState())
            .Subscribe(_ => useSkill());
    }

    #region private method

    /// <summary>
    /// プレイヤーの挙動を変更するメソッド
    /// </summary>
    private void playerBehavior()
    {
        if (!playerMoveController.GetMoveable())
        {
            if (!isAttacking)
            {
                dogAnimation.stateProcessor.state.Value = dogAnimation.stateIdle;
            }
            return;
        }

        float gorillaArgs = transform.rotation.eulerAngles.y;
        float targetArgs = ArgUtility.CastNomalArgByOverArg(ArgUtility.CastPlusArgByMinusArg(playerMoveController.ControllerArg) + mainCamera.transform.rotation.eulerAngles.y);
        if (NumberUtility.BetweenAAndB(gorillaArgs, targetArgs - ARGS_BUFFER, targetArgs + ARGS_BUFFER))
        {
            moveForwardGorilla();
        }
        else
        {
            rotateGorilla(targetArgs);
        }
    }

    /// <summary>
    /// 親オブジェクトごとplayerを移動する
    /// </summary>
    private void moveForwardGorilla()
    {
        this.transform.Translate(forward * moveSpeed);
        if (!isAttacking)
        {
            dogAnimation.stateProcessor.state.Value = dogAnimation.stateRun;
        }
    }

    /// <summary>
    /// ゴリラだけ回転する
    /// </summary>
    /// <param name="targetArgs"></param>
    /// <param name="gorillaArgs"></param>
    private void rotateGorilla(float targetArgs)
    {
        transform.rotation = Quaternion.Euler(0f, targetArgs, 0f);
    }

    /// <summary>
    /// ゴリラの攻撃
    /// とりあえずで1秒間の攻撃コライダー有効化という処理
    /// </summary>
    private void gorillaAttack()
    {
        attackController.CoolDownToController();
        sweetController.setButtonEnable(true);
        AudioManager.Instance.PlaySEClipFromIndex(7, 1f);
        //攻撃有効中に再度攻撃したとき
        if (attackCollider.IsActive())
        {
            againAttack();
        }
        attackCollider.ActivateCollider();
        isAttacking = true;
        dogAnimation.stateProcessor.state.Value = dogAnimation.stateAttack;
        //ToDo:ゴリラ攻撃モーション時間のマジックナンバー対応
        attacker = Observable.Timer(System.TimeSpan.FromMilliseconds(1500))
            .Subscribe(_ => endAttack());
    }

    private void endAttack()
    {
        sweetController.setButtonEnable(false);
        isAttacking = false;
        attackCollider.DisableCollider();
    }

    /// <summary>
    /// 菓子パン効果付与
    /// </summary>
    private void useSweet()
    {
        popupMessage.SetMessage("ミルクを飲んだ!", 1.5f, GamePopUpColor.white);
        sweetController.disableSweet();
        attackController.RecoveryCoolDown();
        AudioManager.Instance.PlaySEClipFromIndex(3, 0.8f);

        Vector3 t = transform.position;
        Vector3 pos = new Vector3(t.x, t.y + 1f, t.z);
        GameObject s = Instantiate(sweetEffect, pos, transform.rotation);
        Observable.Timer(System.TimeSpan.FromMilliseconds(5000)).
            Subscribe(_ => Destroy(s));
    }

    /// <summary>
    /// 攻撃無効化時間の設定を解除する
    /// </summary>
    private void againAttack()
    {
        attacker.Dispose();
    }

    /// <summary>
    /// センサー設置
    /// </summary>
    private void useSensor()
    {
        popupMessage.SetMessage("防犯センサー設置!", 2, GamePopUpColor.white);
        AudioManager.Instance.PlaySEClipFromIndex(9, 1f);
        if (PhotonManager.Instance.IsConnect)
        {
            photonView.RPC("syncSensor", PhotonTargets.All);
        }
        else
        {
            syncSensor();
        }
        sensorController.disableSensor();
    }
    [PunRPC]
    private void syncSensor()
    {
        Instantiate(sensorPrefab, transform.position, transform.rotation);
    }

/// <summary>
/// 足跡スキル作動
/// </summary>
    private void useSkill()
    {
        GameObject.FindObjectOfType<GamePopupMessage>().SetMessage("スキル「嗅覚の覚醒」", 2.5f, GamePopUpColor.white);
        skillController.CoolDownToController();
        cameraAnimator.SetBool("usingSkill", true);
        AudioManager.Instance.PlaySEClipFromIndex(6, 0.6f);

        //レイヤー10(footprints)を追加
        mainCamera.cullingMask |= (1 << 10);

        //終了演出は1秒前から
        Observable.Timer(System.TimeSpan.FromMilliseconds(4000)).
            Subscribe(_ => skillAnimEnd());
        //スキル有効時間：5sec
        Observable.Timer(System.TimeSpan.FromMilliseconds(5000)).
            Subscribe(_ => skillEnd());
    }

    private void skillAnimEnd()
    {
        cameraAnimator.SetBool("usingSkill", false);
    }

    private void skillEnd()
    {
        //レイヤー10(footprints)を除く
        mainCamera.cullingMask &= ~(1 << 10);
    }

    /// <summary>
    /// 現在対戦中かどうか
    /// </summary>
    private bool isInBattleState()
    {
        return gameManager.NowGameState == GameStateType.InBattle;
    }

    [PunRPC]
    private void catchCat(int playerId)
    {
        PlayerDataManager.Instance.WinnerPlayer = PhotonManager.Instance.GetPlayerByPlayerId(playerId);
        Debug.Log("犬のPlayerId: " + PlayerDataManager.Instance.WinnerPlayer.ID);
        gameManager.EndBattle();
    }

    //[PunRPC]
    //private void syncPosition(int playerId)
    //{
    //    Debug.Log("PlayerId : " + playerId + ", Position : (" + this.transform.position.x + ", " + this.transform.position.y + ", " + this.transform.position.z + ")");
    //}

    #endregion
}
