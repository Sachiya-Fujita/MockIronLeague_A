using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class StamperController : MonoBehaviour
{
    [SerializeField] GameObject frame;
    [SerializeField] Image stamperImage;
    [SerializeField] GameObject[] stamps;

    private int stamperId;
    public int StamperId
    {
        get{ return stamperId; }
        set{ stamperId = value; }
    }

    public void InitStamper()
    {
        if (stamperId == PhotonManager.Instance.PlayerId)
        {
            frame.SetActive(true);
        }
    }

    public void ShowStamp(int stampIndex)
    {
        stamps[stampIndex].SetActive(true);
        doAnimation(() => stamps[stampIndex].SetActive(false));
    }

    private void doAnimation(System.Action action)
    {
        Sequence seq = DOTween.Sequence()
            .OnStart(() =>
            {
                stamperImage.rectTransform.Rotate(0f, 0f, 20f);
            })
            .Append(
                stamperImage.rectTransform.DORotate(new Vector3(0f, 0f, -20f), 0.2f).SetEase(Ease.Flash).SetLoops(5, LoopType.Yoyo)
            )
            .Append(
                stamperImage.rectTransform.DORotate(new Vector3(0f, 0f, 0), 0.2f).SetEase(Ease.Flash)
            )
            .OnComplete(() => action());
    }
}
