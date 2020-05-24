using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;

public class StartController : MonoBehaviour
{
    [SerializeField]
    private ObservableEventTrigger transitionTrigger;

    [SerializeField]
    private Image logoImage;

    private StartState startState;

    private bool isFadeOut;

    private void Start()
    {
        startState = new StartState();
        isFadeOut = false;
        // フェードイン
        var tweener = logoImage.DOFade(1.0f, 5.0f).SetEase(Ease.OutQuart)
            .OnComplete(() =>
            {
                setColor();
                isFadeOut = true;
                fadeOut();
            });
        transitionTrigger.OnPointerClickAsObservable()
            .Where(_ => !isFadeOut)
            .Subscribe(_ => {
                setColor();
                tweener.Complete();
                fadeOut();
            });
    }

    private void fadeOut()
    {
        // フェードアウト
        logoImage.DOFade(0.0f, 2.0f).SetEase(Ease.InQuart)
                    .OnComplete(() =>
                    {
                        ScreenStateManager.Instance.GoToNextScene(0);
                    });
    }

    private void setColor()
    {
        Color color = logoImage.color;
        color.a = 1.0f;
        logoImage.color = color;
    }
}
