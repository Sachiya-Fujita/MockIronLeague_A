/*
 * enum管理用
 */

//
public enum ScreenStateType
{
    Start,      // 初期状態
    Title,      // タイトル画面
    Menu,       // セレクト画面
    Matching,   // マッチング画面
    Main,       // インゲーム
    Result,     // 結果
}

/// <summary>
/// インゲーム時間状態
/// </summary>
public enum GameStateType
{
    SyncPending, //同期待ち
    CountDown,   //スタート前
    InBattle,    //プレイ中
    BattleEnd,   //ゲーム終了
}

/// <summary>
/// インゲーム犬アニメーションstate
/// </summary>
public enum DogAnimationStateType
{
    Idle,   //待機
    Run,    //走る
    Attack, //攻撃
    Win,    //勝利
}

/// <summary>
/// インゲーム猫アニメーションstate
/// </summary>
public enum CatAnimationStateType
{
    Idle,   //待機
    Run,    //走る
    jump,   //ジャンプ
}

public enum MenuStateType
{
    Home,       // ホーム画面
    Info,       // お知らせ画面
    Setting,    // 設定画面
    Ranking,    // ランキング画面
    Character,  // キャラクターセレクト画面
    Gacha,      // ガチャ画面
    Collection, //コレクション画面
    RoomMatch,  // ルームマッチ画面
    FreeMatch,  // フリーマッチ画面

    Walk,       // お散歩画面
}

public enum GameItemType
{
    coin,   //コイン
    jewel, //猫用 宝石
    securityCamera, //犬用 監視カメラ
    bread,          //犬用 菓子パン
    treasureCoin,   //犬用 枚数ランダムコイン
    empty,          //犬用 ハズレ
}

/// <summary>
/// ポップアップメッセージで使う色
/// </summary>
public enum GamePopUpColor
{
    white,
    red,
    yellow,
}

public enum DialogState
{
    Invalid,    // 無効
    Open,       // 開いている
    Close,      // 閉じた
}

public enum SelectIndex{
    Cancel = 0, // キャンセル
    OK = 1,     // OK
}

public enum PlayerPrefsKey
{
    UserName,
    MyCoin
}

public enum ConnectType
{
    CreateRoom,
    Join,
    RandomJoin
}

/// <summary>
/// PlayerTypeの指定
/// タイプ名はそのままPathになる
/// </summary>
public enum PlayerType
{
    Oliver,
    George,
    Harry,
    Jack,
    Charlie,
    Arthur,
    Unfinished,
    Cyberagent,
    Cat,

    Achan,
    Kozupid,
    Yoshipons,
    Tomy,
    Sacha,

    Gorilla,
    Dog_1,
}


public enum ItemType
{
    ORANGE_JEWELRY,
    PINK_BIG_JEWELRY,
    PINK_JEWELRY,
    RED_JEWELRY,
    TRIANGLE,
    DIAMOND,
    YELLOW_JEWELRY,
    RUBY,
    COIN_ICON,
    RANK_ICON,
}
