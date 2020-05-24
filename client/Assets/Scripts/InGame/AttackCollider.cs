using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class AttackCollider : MonoBehaviour
{
    private new Collider collider;
    [SerializeField]
    private DogAnimation dogAnimation;

    void Start()
    {
        collider = GetComponent<Collider>();

        // ねこコライダーに衝突した時
        var OnTriggerEnterAttack = collider.OnTriggerEnterAsObservable()
            .Select(collision => collision.tag)
            .Where(tag => tag == "Cat");
        // 終了,ToDo:どのプレイヤーか判定
        OnTriggerEnterAttack
            .Subscribe(_ =>
            {
                dogAnimation.stateProcessor.state.Value = dogAnimation.stateWin;
                GameObject.Find("GameManager").GetComponent<GameManager>().IsDogWin = true;
            });
    }

    public void ActivateCollider()
    {
        collider.enabled = true;
    }

    public void DisableCollider()
    {
        collider.enabled = false;
    }

    public bool IsActive()
    {
        return collider.enabled;
    }
}
