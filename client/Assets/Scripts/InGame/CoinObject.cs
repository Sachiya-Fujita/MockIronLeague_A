using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class CoinObject : MonoBehaviour
{
    private new Collider collider;

    void Start()
    {
        collider = GetComponent<Collider>();

        // Playerコライダーに衝突した時
        var OnTriggerEnterPlayer = collider.OnTriggerEnterAsObservable()
            .Select(collision => collision.tag)
            .Where(tag => tag == "Cat" || tag == "Dog");
        // 獲得リストに自身を追加しDestroy
        OnTriggerEnterPlayer
            .Select(_ => GameObject.FindObjectOfType<GameManager>())
            .Subscribe(GameManager => {
                GameManager.SetObtainedObject(GameItemType.coin);
                Destroy(gameObject);
            })
            .AddTo(this);

    }
}
