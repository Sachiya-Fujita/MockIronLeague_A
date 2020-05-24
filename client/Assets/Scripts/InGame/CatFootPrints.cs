using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class CatFootPrints : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private int desappearingCount;
    //足跡時間：7秒
    private const int lifeSpanSec = 7;
    private int step;

    void Start()
    {
        desappearingCount = 0;
        step = lifeSpanSec * 10;
        meshRenderer = this.GetComponent<MeshRenderer>();
        disappearing();
    }

    private void disappearing()
    {
        if (desappearingCount == step)
        {
            Destroy(gameObject);
            return;
        }

        meshRenderer.material.color = new Color(1, 1, 1, 1 - 1.0f * desappearingCount / step);
        desappearingCount++;
        Observable.Timer(TimeSpan.FromMilliseconds(100))
            .Subscribe(_ => disappearing());
    }
}
