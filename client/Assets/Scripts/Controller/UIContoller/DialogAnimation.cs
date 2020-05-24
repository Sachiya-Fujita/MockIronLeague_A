using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;

public class DialogAnimation : MonoBehaviour
{
    [SerializeField]
    private Image dialogBg;

    [SerializeField]
    private Image okButtonImage;

    [SerializeField]
    private Image cancelButtonImage;

    public void InitAnimation()
    {
        Sequence seq = DOTween.Sequence()
        .OnStart(() => {
            dialogBg.rectTransform.localScale = new Vector3(0.5f, 0.5f, 1f);
            dialogBg.color = setTransparentColor();
            okButtonImage.rectTransform.localScale = new Vector3(0.2f, 0.2f, 1f);
            okButtonImage.color = setTransparentColor();
            cancelButtonImage.rectTransform.localScale = new Vector3(0.2f, 0.2f, 1f);
            cancelButtonImage.color = setTransparentColor();
        })
        .Append(
            dialogBg.rectTransform.DOScale(new Vector3(1.0f, 1.0f, 1f), 0.5f).SetEase(Ease.OutElastic)
        )
        .Join(
            dialogBg.DOFade(1f, 0.5f)
        )
        .Insert(
            0.2f, okButtonImage.rectTransform.DOScale(new Vector3(0.7f, 0.7f, 1f), 0.4f).SetEase(Ease.OutElastic)
        )
        .Join(
            okButtonImage.DOFade(1f, 0.3f)
        )
        .Join(
            cancelButtonImage.rectTransform.DOScale(new Vector3(0.7f, 0.7f, 1f), 0.4f).SetEase(Ease.OutElastic)
        )
        .Join(
            cancelButtonImage.DOFade(1f, 0.3f)
        );
    }

    public void CloseAnimation(System.Action action)
    {
        Sequence seq = DOTween.Sequence()
        .Append(
            dialogBg.rectTransform.DOScale(new Vector3(0.5f, 0.5f, 1f), 0.3f).SetEase(Ease.InBack)
        )
        .Join(
            dialogBg.DOFade(0f, 0.3f).SetEase(Ease.InExpo)
        )
        .Join(
            okButtonImage.rectTransform.DOScale(new Vector3(0.2f, 0.2f, 1f), 0.3f).SetEase(Ease.InBack)
        )
        .Join(
            okButtonImage.DOFade(0f, 0.3f).SetEase(Ease.InExpo)
        )
        .Join(
            cancelButtonImage.rectTransform.DOScale(new Vector3(0.2f, 0.2f, 1f), 0.3f).SetEase(Ease.InBack)
        )
        .Join(
            cancelButtonImage.DOFade(0f, 0.3f).SetEase(Ease.InExpo)
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
