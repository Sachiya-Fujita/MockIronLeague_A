using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class CatGroundingDecision : MonoBehaviour
{
    [SerializeField]
    private new Collider collider;
    //接地数
    private int enterNum = 0;

    private CatController catController;

    void Start()
    {
        collider = GetComponent<Collider>();
        catController = GameObject.FindObjectOfType<CatController>();

        // 地面衝突時
        collider.OnTriggerEnterAsObservable()
            .Where(collider => collider.CompareTag("Ground"))
            .Subscribe(_ => onGround());
        //地面離れた時
        collider.OnTriggerExitAsObservable()
            .Where(collider => collider.CompareTag("Ground"))
            .Subscribe(_ => outGround());
    }

    private void onGround()
    {
        enterNum++;
        checkNowGround();
    }

    private void outGround()
    {
        enterNum--;
        checkNowGround();
    }

    private void checkNowGround()
    {
        if (enterNum <= 0)
        {
            catController.IsGround = false;
            Debug.Log("地面離れた");
        }
        else
        {
            catController.IsGround = true;
            Debug.Log("地面いる");
        }
    }
}
