using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
public class WalkMenu : MenuBase
{
    #region define

    #endregion

    #region variable

    [SerializeField]
    private Button dogButton;

    [SerializeField]
    private Button catButton;

    #endregion

    #region method

    void Start(){
        setupEvent();
    }

    public override void setupEvent(){
        base.setupEvent();

        dogButton.OnSafeClickAsObservable()
            .Subscribe(_ => {
                PhotonManager.Instance.NowPlayerType = PlayerType.Oliver;
                ScreenStateManager.Instance.GoToNextScene(0);
            })
            .AddTo(this);

        catButton.OnSafeClickAsObservable()
            .Subscribe(_ => {
                Debug.Log("cat");
                PhotonManager.Instance.NowPlayerType = PlayerType.Cat;
                ScreenStateManager.Instance.GoToNextScene(0);
            })
            .AddTo(this);
    }

    #endregion
}
