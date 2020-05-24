using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;


public class GameManager : MonoBehaviour
{
    #region define

    private PhotonView m_photonView = null;

    public static readonly string COIN = "coin";
    private const int COIN_MAX = 5;

    [SerializeField]
    private int defaultGameTimerSecs = 120;

    public GameStateType NowGameState {get; set; }

    private IConnectableObservable<int> _countDownObservable;

    public ReactiveCollection<string> attackObjectNames = new ReactiveCollection<string>();
    public ReactiveDictionary<string, int> obtainedObjectCounts = new ReactiveDictionary<string, int>();

    private GamePopupMessage popupMessage;

    private int catJewelCount;

    [SerializeField]
    private Text coinNumText;
    [SerializeField]
    private Image[] catJewelImages;
    [SerializeField]
    private Image[] dogJewelImages;
    [SerializeField]
    private GameObject controllerUI;
    [SerializeField]
    private GameObject endMessageUI;
    [SerializeField]
    private GameObject dogMoveController;
    [SerializeField]
    private GameObject catMoveCotroller;

    private SweetController sweetController;
    private SensorController sensorController;

    private Text endMessageText;

    private Image[] jewelImages;

    private bool isDogWin = false;
    public bool IsDogWin
    {
        get{ return isDogWin; }
        set{ isDogWin = value; }
    }

    private bool isCatWin = false;
    public bool IsCatWin
    {
        get{ return isCatWin; }
        set{ isCatWin = value; }
    }

    /// <summary>
    /// カウントダウンストリーム
    /// このObservableを各クラスがSubscribeする
    /// </summary>
    public IObservable<int> CountDownObservable {
        get {
            return _countDownObservable.AsObservable();
        }
    }

    #endregion define

    #region private method

    void Awake() {
        //カウントのストリームを作成
        //PublishでHot変換
        _countDownObservable = createCountDownObservable(defaultGameTimerSecs).Publish();
        m_photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        isDogWin = false;
        isCatWin = false;
        catJewelCount = 0;
        initObtainedObjectCounts();
        popupMessage = GameObject.FindObjectOfType<GamePopupMessage>();

        if (PhotonManager.Instance.NowPlayerType == PlayerType.Cat)
        {
            dogMoveController.SetActive(false);
            catMoveCotroller.SetActive(true);
            jewelImages = catJewelImages;
        }
        else
        {
            dogMoveController.SetActive(true);
            catMoveCotroller.SetActive(false);
            jewelImages = dogJewelImages;
        }

        sweetController = GameObject.FindObjectOfType<SweetController>();
        sensorController = GameObject.FindObjectOfType<SensorController>();

        // 攻撃したオブジェクトを購読
        attackedObserve();
        // 獲得したオブジェクトを購読
        obtainedObserve();
        //とりあえず即座にゲーム始まる
        startBattle();
    }

    void initObtainedObjectCounts()
    {
        obtainedObjectCounts.Add(GameItemType.coin.ToString(), 0);
        obtainedObjectCounts.Add(GameItemType.jewel.ToString(), 0);
        obtainedObjectCounts.Add(GameItemType.securityCamera.ToString(), 0);
        obtainedObjectCounts.Add(GameItemType.bread.ToString(), 0);
        obtainedObjectCounts.Add(GameItemType.treasureCoin.ToString(), 0);
        obtainedObjectCounts.Add(GameItemType.empty.ToString(), 0);
    }

    private void startBattle()
    {
        NowGameState = GameStateType.InBattle;
        _countDownObservable.Connect();
    }

    /// <summary>
    /// ゲーム終了処理
    /// </summary>
    public void EndBattle()
    {
        AudioManager.Instance.PlaySEClipFromIndex(1, 0.6f);
        setEndUI();
        NowGameState = GameStateType.BattleEnd;
        if (PhotonManager.Instance.NowPlayerType == PlayerType.Cat)
        {
            PlayerDataManager.Instance.JewelCount = catJewelCount;
        }
        //3秒後リザルト遷移
        Observable.Timer(TimeSpan.FromMilliseconds(3000))
            .Subscribe(_ => {
                AudioManager.Instance.StopBGM();
                ScreenStateManager.Instance.GoToNextScene(0);
            });
    }

    /// <summary>
    /// CountTimeだけカウントダウンするストリーム
    /// </summary>
    /// <param name="CountTime"></param>
    /// <returns></returns>
    private IObservable<int> createCountDownObservable(int CountTime) {
        return Observable
            .Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1))
            .Select(x => (int)(CountTime - x))
            .TakeWhile(x => x >= 0);
    }

    private void attackedObserve() {
        attackObjectNames.ObserveAdd().Subscribe(x => Debug.Log(x + "を攻撃"));
    }

    private void obtainedObserve() {
        obtainedObjectCounts.ObserveReplace()
            .Subscribe(x =>
            {
                //ToDo: キーごとの場合分け
                coinNumText.text = obtainedObjectCounts[GameItemType.coin.ToString()].ToString();
            });

    }

    public void SetObtainedObject(GameItemType itemType) {
        // 宝箱コインはランダム
        int itemCount = 1;
        if (itemType == GameItemType.treasureCoin) {
            itemCount = UnityEngine.Random.Range(0,5);
        }
        // 獲得リストに追加
        int newCount = obtainedObjectCounts[itemType.ToString()] + itemCount;
        obtainedObjectCounts[itemType.ToString()] = newCount;
        itemsEffectApplying(itemType);
        viewTreasureUI(itemType);
        Debug.Log(itemType.ToString() + "を獲得");
    }

    private void viewTreasureUI(GameItemType itemType)
    {
        string str;
        switch (itemType)
        {
            case GameItemType.coin:
                return;
            case GameItemType.empty:
                str = "ハズレ";
                break;
            case GameItemType.jewel:
                str = "宝石獲得！";
                break;
            case GameItemType.bread:
                str = "ミルク獲得！";
                break;
            case GameItemType.securityCamera:
                str = "防犯センサー獲得！";
                break;
            case GameItemType.treasureCoin:
                str = "コイン獲得！";
                break;
            default:
                str = "空っぽだ...(ハズレ)";
                break;
        }
        popupMessage.SetMessage(str, 2, GamePopUpColor.white);
    }

    private void itemsEffectApplying(GameItemType itemType)
    {
        switch (itemType)
        {
            case GameItemType.jewel:
                if (PhotonManager.Instance.IsConnect)
                {
                    m_photonView.RPC("syncJewel", PhotonTargets.All);
                }
                else
                {
                    syncJewel();
                }

                if (catJewelCount >= 3 && PhotonManager.Instance.NowPlayerType == PlayerType.Cat)
                {
                    IsCatWin = true;
                }
                break;
            case GameItemType.bread:
                sweetController.addSweet();
                break;
            case GameItemType.securityCamera:
                sensorController.addSensor();
                break;
        }
    }

    /// <summary>
    /// ToDo:同期　ジュエルの獲得状況をUIに反映する
    /// </summary>
    [PunRPC]
    private void syncJewel()
    {
        DOTween.Sequence()
            .OnStart(() =>
            {
                jewelImages[catJewelCount].rectTransform.localScale = new Vector3(0.5f, 0.5f, 1f);
            })
            .Append(jewelImages[catJewelCount].rectTransform.DOScale(new Vector3(1f, 1f, 1f), 1f).SetEase(Ease.OutElastic))
            .Join(jewelImages[catJewelCount].DOFade(1f, 1f));
        catJewelCount++;
    }

    /// <summary>
    /// ゲーム終了時の表示切替
    /// </summary>
    private void setEndUI()
    {
        controllerUI.SetActive(false);
        if (isDogWin)
        {
            popupMessage.SetMessage("怪盗キャットが捕まった！", 2.5f, GamePopUpColor.yellow);
            AudioManager.Instance.PlaySEClipFromIndex(10, 0.8f);
        }
        else if(isCatWin)
        {
            popupMessage.SetMessage("怪盗キャットが逃げ切った！", 2.5f, GamePopUpColor.yellow);
        }
        else
        {
            popupMessage.SetMessage("ゲーム終了！", 2.5f, GamePopUpColor.yellow);
        }
        //endMessageUI.SetActive(true);
    }
}

#endregion private method
