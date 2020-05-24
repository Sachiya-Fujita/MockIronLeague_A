using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SmokeAnim : MonoBehaviour
{

    private Image image;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        DOTween.Sequence()
            .Append
            (
                image.rectTransform.DOScale(new Vector3(1.5f, 1.5f, 1.0f), Random.Range(0.5f, 2f)).SetEase(Ease.OutBounce).SetLoops(2, LoopType.Yoyo)
            )
            .Append
            (
                image.DOFade(0f, 1f)
            );
    }
}
