using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class AttackableObject : MonoBehaviour
{
    private new Collider collider;
    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider>();

        // Player攻撃コライダーに衝突した時
        var OnTriggerEnterAttack = collider.OnTriggerEnterAsObservable()
            .Select(collision => collision.tag)
            .Where(tag => tag == "PlayerAttack");
        // 獲得リストに自身を追加
        OnTriggerEnterAttack
            .Select(_ => GameObject.FindObjectOfType<GameManager>())
            .Subscribe(GameManager => GameManager.attackObjectNames.Add(gameObject.name));
        // 自身をDestroy
        OnTriggerEnterAttack
            .Subscribe(_ => Destroy(gameObject));
    }
}
