﻿using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// ポップに押されるボタン
/// </summary>
public class PopButton : MonoBehaviour
{
    [SerializeField]
    private ObservableEventTrigger popButton;
    Tweener tweener = null;

    private void Start()
    {
        // 再生中のアニメーションを停止/初期化
        if (tweener != null)
        {
            tweener.Kill();
            tweener = null;
            transform.localScale = Vector3.one;
        }
        popButton.OnPointerDownAsObservable()
            .Subscribe(_ =>
            {
                tweener = transform.DOPunchScale(
                    punch: Vector3.one * 0.1f,
                    duration: 0.2f,
                    vibrato: 1
                ).SetEase(Ease.OutExpo);
            });   
    }
}