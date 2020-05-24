using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class MenuController : SingletonMonoBehaviour<MenuController>
{
    #region SerializeField Define

    [SerializeField]
	private HomeMenu homeMenu;

	[SerializeField]
	private GameObject home;

	[SerializeField]
	private CharacterMenu characterMenu;

	[SerializeField]
	private GameObject character;

	[SerializeField]
	private CollectionMenu collectionMenu;

	[SerializeField]
	private GameObject collection;

	[SerializeField]
	private RankingMenu rankingMenu;

	[SerializeField]
	private GameObject ranking;

	[SerializeField]
	private WalkMenu walkMenu;

	[SerializeField]
	private GameObject walk;

	[SerializeField]
	private RoomMatch roomMatchMenu;

	[SerializeField]
	private GameObject roomMatch;

	// [SerializeField]
	// private RoomMatchDialog roomMatchDialog;

	// [SerializeField]
	// private GameObject roomMatchDialogObject;

	//[SerializeField]
 //   private Button mainButton;

	#endregion

	#region private Define

	private MenuState menuState;

	private MenuStateType currentMenuTypeState = MenuStateType.Home;

	private MenuStateType defaultMenuTypeState = MenuStateType.Home;

	#endregion


	#region method

	private void Start()

    {
			menuState = new MenuState();
			ChangeMenu(defaultMenuTypeState);
			SetupEvent();
    }

	private void SetupEvent()
	{
        // debug用
        // TODO: 削除予定
		//mainButton.OnClickAsObservable()
		//	.Subscribe(_ => {
		//		ScreenStateManager.Instance.GoToNextScene(0);
		//	});

		homeMenu.OnMenuStateTypeAsObservable
			.Subscribe(type => {
				ChangeMenu(type);
				home.SetActive(false);
				})
			.AddTo(this);

		characterMenu.onMenuStateType
			.Where(type => MenuStateType.Home == type)
			.Subscribe(_ => {
				ChangeMenu(MenuStateType.Home);
				character.SetActive(false);
				})
			.AddTo(this);

		collectionMenu.onMenuStateType
			.Where(type => MenuStateType.Home == type)
			.Subscribe(_ => {
				ChangeMenu(MenuStateType.Home);
				collection.SetActive(false);
				})
			.AddTo(this);

		rankingMenu.onMenuStateType
			.Where(type => MenuStateType.Home == type)
			.Subscribe(_ => {
				ChangeMenu(MenuStateType.Home);
				ranking.SetActive(false);
				})
			.AddTo(this);

		walkMenu.onMenuStateType
			.Where(type => MenuStateType.Home == type)
			.Subscribe(_ => {
				ChangeMenu(MenuStateType.Home);
				walk.SetActive(false);
				})
			.AddTo(this);

		roomMatchMenu.onMenuStateType
			.Where(type => MenuStateType.Home == type)
			.Subscribe(_ => {
				ChangeMenu(MenuStateType.Home);
				roomMatch.SetActive(false);
				})
			.AddTo(this);
	}

	private void ChangeMenu(MenuStateType type){

		switch(type) {
			case MenuStateType.Character:
				Debug.Log("キャラ変");
				character.SetActive(true);
				break;
			case MenuStateType.Collection:
				Debug.Log("コレクション");
				collection.SetActive(true);
				break;
			case MenuStateType.FreeMatch:
				Debug.Log("フリー対戦");
				connectPhoton(ConnectType.RandomJoin);
				break;
			case MenuStateType.Info:
				Debug.Log("お知らせ");
				break;
			case MenuStateType.Walk:
				Debug.Log("お散歩");
				walk.SetActive(true);
				break;
			case MenuStateType.Home:
				Debug.Log("ホーム");
				homeMenu.OnCharacterTypeAsObservable.OnNext((int)PlayerDataManager.Instance.NowPlayerType);
				home.SetActive(true);
				break;
			case MenuStateType.Ranking:
				Debug.Log("ランキング");
				ranking.SetActive(true);
				break;
			case MenuStateType.RoomMatch:
				Debug.Log("ルームマッチ");
				roomMatch.SetActive(true);
				break;
			case MenuStateType.Setting:
				Debug.Log("設定");
				break;
			default:
				break;
		}

		currentMenuTypeState = type;
	}

	private void connectPhoton(ConnectType connectType)
	{
		PhotonManager.Instance.Connect(connectType);
	}
	#endregion
}
