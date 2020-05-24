using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class ResultController : MonoBehaviour
{
    [SerializeField]
    private Image exitButtonImage;

    [SerializeField]
    private Image restartButtonImage;

    [SerializeField]
    private GameObject dogResult;

    [SerializeField]
    private Image dogResultDetailDialog;

    [SerializeField]
    private GameObject catResult;

    [SerializeField]
    private Image catResultDetailDialog;

    [SerializeField]
    private ObservableEventTrigger menuEventTrigger;

    [SerializeField]
    private ObservableEventTrigger restartEventTrigger;

    [SerializeField]
    private Text beforeMassage;

    [SerializeField]
    private Text afterMassage;

    [SerializeField]
    private GameObject massageBgObj;
    [SerializeField]
    private Image massageBg;

    [SerializeField]
    private DogResultDetail dogResultDetail;

    [SerializeField]
    private CatResultDetail catResultDetail;

    private Image resultDetailDialog;

    private ResultState resultState;

    private void Start()
    {
        resultState = new ResultState();
        // photonが接続されている
        if (PhotonManager.Instance.IsConnect)
        {
            // 自分が猫のとき
            if (PhotonManager.Instance.NowPlayerType == PlayerType.Cat)
            {
                // 勝者(猫)が自分のとき
                if ((PlayerType)PlayerDataManager.Instance.WinnerPlayer.CustomProperties["PlayerType"] == PlayerType.Cat)
                {
                    beforeMassage.text = WordMaster.CAT_WIN_TEXT_BEFORE;
                    afterMassage.text = WordMaster.CAT_WIN_TEXT_AFTER;
                    string colorCode = "#ffcc44";
                    Color color = default(Color);
                    ColorUtility.TryParseHtmlString(colorCode, out color);
                    afterMassage.color = color;
                    catResultDetail.Win();
                }
                // 自分以外が勝者(犬)のとき
                else
                {
                    beforeMassage.text = WordMaster.CAT_LOSE_TEXT_BEFORE;
                    afterMassage.text = WordMaster.CAT_LOSE_TEXT_AFTER;
                    catResultDetail.Lose();
                }
                catResult.SetActive(true);
                resultDetailDialog = catResultDetailDialog;
            }
            // 自分が犬のとき
            else
            {
                // 勝者が自分以外(猫)のとき
                if ((PlayerType)PlayerDataManager.Instance.WinnerPlayer.CustomProperties["PlayerType"] == PlayerType.Cat)
                {
                    beforeMassage.text = WordMaster.DOG_LOSE_TEXT_BEFORE;
                    afterMassage.text = WordMaster.DOG_LOSE_TEXT_AFTER;
                    dogResultDetail.Lose();
                }
                // 勝者(犬)が自分のとき
                else
                {
                    beforeMassage.text = WordMaster.DOG_WIN_TEXT_BEFORE;
                    afterMassage.text = WordMaster.DOG_WIN_TEXT_AFTER;
                    string colorCode = "#ffcc44";
                    Color color = default(Color);
                    ColorUtility.TryParseHtmlString(colorCode, out color);
                    afterMassage.color = color;
                    dogResultDetail.Win();
                }
                dogResult.SetActive(true);
                resultDetailDialog = dogResultDetailDialog;
            }
            PhotonManager.Instance.DisConnectedPhotonManager();
        }
        else
        {
            if (PlayerDataManager.Instance.NowPlayerType == PlayerType.Cat)
            {
                catResult.SetActive(true);
                resultDetailDialog = catResultDetailDialog;
            }
            else
            {
                dogResult.SetActive(true);
                resultDetailDialog = dogResultDetailDialog;
            }
        }
        setupEvent();
        doDetailAnimation();
    }

    private void setupEvent()
    {
        menuEventTrigger.OnPointerClickAsObservable()
            .Subscribe(_ => {
                ScreenStateManager.Instance.GoToNextScene(ScreenStateType.Menu);
            });
        restartEventTrigger.OnPointerClickAsObservable()
            .Subscribe(_ => {
                ScreenStateManager.Instance.GoToNextScene(ScreenStateType.Menu);
            });
    }

    private void doDetailAnimation()
    {
        Sequence seq = DOTween.Sequence()
        .Append(
            massageBg.rectTransform.DOMove(new Vector3(-50f, 200f, 0f), 0.5f).SetEase(Ease.Linear)
        )
        .Join(
            beforeMassage.rectTransform.DOLocalMoveX(400f, 0.5f).SetEase(Ease.Linear)
        )
        .Join(
            beforeMassage.DOFade(1f, 0.5f).SetEase(Ease.OutElastic)
        )
        .Join(
            afterMassage.rectTransform.DOLocalMoveX(450f, 1f).SetEase(Ease.Linear)
        )
        .Join(
            afterMassage.DOFade(1f, 0.5f).SetEase(Ease.OutElastic)
        )
        .AppendInterval(1f)
        .Append(
            beforeMassage.DOFade(0f, 1f).SetEase(Ease.Linear)
        )
        .Join(
            afterMassage.DOFade(0f, 1f).SetEase(Ease.Linear)
        )
        .Join(
            massageBg.DOFade(0f, 1f).SetEase(Ease.Linear)
        )
        .AppendCallback(() => {
            massageBgObj.SetActive(false);
        })
        .Append(
            resultDetailDialog.rectTransform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 1.0f).SetEase(Ease.OutElastic)
        )
        .Join(
            resultDetailDialog.DOFade(1f, 1f)
        )
        .Append(
            exitButtonImage.rectTransform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 1.0f).SetEase(Ease.OutElastic)
        )
        .Join(
            exitButtonImage.DOFade(1f, 1f)
        )
        .Join(
            restartButtonImage.rectTransform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 1.0f).SetEase(Ease.OutElastic)
        )
        .Join(
            restartButtonImage.DOFade(1f, 1f)
        );


    }
    
}
