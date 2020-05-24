using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Sensor : MonoBehaviour
{
    private Animator animator;
    [SerializeField]
    private new Collider collider;
    private bool alearted;
    private GamePopupMessage popupMessage;

    void Start()
    {
        popupMessage = GameObject.FindObjectOfType<GamePopupMessage>();
        alearted = false;
        animator = GetComponent<Animator>();

        // 猫に衝突した時
        var OnTriggerEnterPlayer = collider.OnTriggerEnterAsObservable()
            .Where(collider => (collider.CompareTag("Cat")) && !alearted)
            .Subscribe(_ => onAleart());
    }

    void onAleart()
    {
        AudioManager.Instance.PlaySEClipFromIndex(11, 1f);
        popupMessage.SetMessage("猫がセンサーに見つかった！", 4, GamePopUpColor.yellow);
        alearted = true;
        animator.SetTrigger("on");
    }
}
