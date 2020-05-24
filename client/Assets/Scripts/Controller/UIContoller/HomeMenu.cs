using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class HomeMenu : MonoBehaviour
{
    #region variable

	// キャラ変更
    [SerializeField] Button changeCharacterButton;

    // コレクションボタン
    [SerializeField] Button collectionButton;

    // フリーマッチボタン
    [SerializeField] Button freeMatchButton;

    // お知らせボタン
    [SerializeField] Button infoButton;

    // おさんぽボタン
    [SerializeField] Button walkButton;

    // ランキングボタン
    [SerializeField] Button rankingButton;

    // ルームマッチボタン
    [SerializeField] Button roomMatchButton;

    // ショップボタン
    [SerializeField] Button settingButton;

    // 名前変更ボタン
    [SerializeField] Button renameButton;

    // キャラ詳細ボタン
    [SerializeField] Button characterButton;

    // キャラアイコン
    [SerializeField] Image characterIcon;

    //ユーザーネーム
    [SerializeField] Text userName;

    //ユーザーの所持コイン
    [SerializeField] Text coinText;

    // 名前変更ダイアログ
    [SerializeField] EditNameDialog editNameDialog;

    // キャラ詳細ダイアログ
    [SerializeField] CharacterDetailDialog characterDetailDialog;

    private Subject<MenuStateType> onMenuStateType = new Subject<MenuStateType>();
	public IObservable<MenuStateType> OnMenuStateTypeAsObservable { get { return onMenuStateType; } }

    private Subject<DialogState> onDialogResult = new Subject<DialogState>();
    public IObservable<DialogState> OnDialogResultAsObservable { get { return onDialogResult; }}

    public Subject<int> OnCharacterTypeAsObservable = new Subject<int>();

    #endregion

    #region private method

    private void Start()
    {
        userName.text = PlayerDataManager.Instance.UserName;
        PlayerDataManager.Instance.NowPlayerType = PlayerType.Oliver;
        //AudioManager.Instance.PlayBGMClipFromIndex(1, 0.4f);
        AudioManager.Instance.ChangeBGM(1, 0.4f);
        // Prefsからコインの合計を取得
        coinText.text = PlayerDataManager.Instance.MyCoin.ToString();
        DownloadIcon();
        setup();
        setupEvent();
    }

    private void setup(){
        changeCharacter((int)PlayerDataManager.Instance.NowPlayerType);
    }

    private IObservable<Unit> StartSEAsobservable(){
        Debug.Log("SE");
        AudioManager.Instance.PlaySEClipFromIndex(1, 0.6f);

        return Observable.ReturnUnit();
    }

    private void setupEvent()
    {
        changeCharacterButton.OnSafeClickAsObservable()
            .SelectMany(_ => StartSEAsobservable())
            .Subscribe(_ => {
                Debug.Log("test");
                // AudioManager.Instance.PlaySEClipFromIndex(1, 0.6f);
                onMenuStateType.OnNext(MenuStateType.Character);
            })
            .AddTo(this);

        collectionButton.OnSafeClickAsObservable()
            .Subscribe(_ => onMenuStateType.OnNext(MenuStateType.Collection))
            .AddTo(this);

        freeMatchButton.OnSafeClickAsObservable()
            .Subscribe(_ => onMenuStateType.OnNext(MenuStateType.FreeMatch))
            .AddTo(this);

        infoButton.OnSafeClickAsObservable()
            .Subscribe(_ => onMenuStateType.OnNext(MenuStateType.Info))
            .AddTo(this);

        walkButton.OnSafeClickAsObservable()
            .Subscribe(_ => onMenuStateType.OnNext(MenuStateType.Walk))
            .AddTo(this);

        rankingButton.OnSafeClickAsObservable()
            .Subscribe(_ => onMenuStateType.OnNext(MenuStateType.Ranking))
            .AddTo(this);

        roomMatchButton.OnSafeClickAsObservable()
            .Subscribe(_ => onMenuStateType.OnNext(MenuStateType.RoomMatch))
            .AddTo(this);

        settingButton.OnSafeClickAsObservable()
            .Subscribe(_ => onMenuStateType.OnNext(MenuStateType.Setting))
            .AddTo(this);

        renameButton.OnSafeClickAsObservable()
            .Subscribe(_ => {
                Debug.Log("name");
                nameEditDialog();
            })
            .AddTo(this);

        characterButton.OnClickAsObservable()
            .Subscribe(_ => {
                characterDetail();
                Debug.Log("キャラ詳細");
            })
            .AddTo(this);

        editNameDialog.OnSelectIndex
            .Where(index => SelectIndex.OK == index)
            .Subscribe(_ => {
                userName.text = editNameDialog.UserName;
                PlayerDataManager.Instance.SetAndSaveString(PlayerPrefsKey.UserName, editNameDialog.UserName);
            })
            .AddTo(this);

        onMenuStateType
            .Where(state => state == MenuStateType.Home)
            .Subscribe(_ => Debug.Log("back Home"))
            .AddTo(this);

        OnCharacterTypeAsObservable
            .Where(id => id <= 7)
            .Subscribe(id => {
                changeCharacter(id);}
                )
            .AddTo(this);
    }
    private void nameEditDialog()
    {
        editNameDialog.OpenAsObservable()
            .Subscribe(x => {
                Debug.Log(x);
            })
            .AddTo(this);
    }

    private void characterDetail(){
        characterDetailDialog.SetupAsObservable((int)PlayerDataManager.Instance.NowPlayerType)
            .SelectMany(_ => characterDetailDialog.OpenAsObservable())
            .Subscribe()
            .AddTo(this);

        characterDetailDialog.OpenAsObservable()
            .Subscribe(x => {
                Debug.Log(x);
            })
            .AddTo(this);
    }

    private void changeCharacter(int id){
        characterIcon.sprite = CharacterMenu.characterModelDic[id].homeIcon;
    }

    private void DownloadIcon(){
        foreach (var key in CharacterMenu.characterModelDic.Keys)
        {
            var homeIcon = Resources.Load<Sprite>(PathMaster.CHARACTER_ICON_ROOT + CharacterMenu.characterModelDic[key].homeIconPath) as Sprite;
            CharacterMenu.characterModelDic[key].homeIcon = homeIcon;
            var icon = Resources.Load<Sprite>(PathMaster.CHARACTER_ICON_ROOT + CharacterMenu.characterModelDic[key].iconPath) as Sprite;
            CharacterMenu.characterModelDic[key].icon = icon;
            Debug.Log("resource icon is : " + CharacterMenu.characterModelDic[key].icon);

        }
    }

    #endregion
}
