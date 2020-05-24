using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;

public class NonButtonDialogAnimation : MonoBehaviour
{
    [SerializeField]
    private Image dialogBg;

    [SerializeField]
    private float initScale;

    public void InitAnimation()
    {
        Sequence seq = DOTween.Sequence()
        .OnStart(() => {
            dialogBg.rectTransform.localScale = new Vector3(initScale - 0.5f, initScale - 0.5f, 1f);
            dialogBg.color = setTransparentColor();
        })
        .Append(
            dialogBg.rectTransform.DOScale(new Vector3(initScale, initScale, 1f), 0.5f).SetEase(Ease.OutElastic)
        )
        .Join(
            dialogBg.DOFade(1f, 0.5f)
        );
    }

    public void CloseAnimation(System.Action action)
    {
        Sequence seq = DOTween.Sequence()
        .Append(
            dialogBg.rectTransform.DOScale(new Vector3(initScale - 0.5f, initScale - 0.5f, 1f), 0.3f).SetEase(Ease.InBack)
        )
        .Join(
            dialogBg.DOFade(0f, 0.3f).SetEase(Ease.InExpo)
        )
        .OnComplete(() => {
            action();
        });
    }

    private Color setTransparentColor()
    {
        return new Color(255f, 255f, 255f, 0f);
    }
}
