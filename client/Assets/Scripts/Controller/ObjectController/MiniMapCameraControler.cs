using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class MiniMapCameraControler : MonoBehaviour
{

    private const float MINI_MAP_RANGE = 30f;
    // Start is called before the first frame update
    private void Start()
    {
        if (PhotonManager.Instance.IsConnect)
        {
            Observable.Interval(System.TimeSpan.FromMilliseconds(100))
            .Where(_ => PhotonManager.Instance.PlayerObj != null)
            .Subscribe(_ => followPlayer());
        }
    }

    private void followPlayer()
    {
        Vector3 playerPos = PhotonManager.Instance.PlayerObj.transform.position;
        playerPos.y = MINI_MAP_RANGE;
        transform.position = playerPos;
    }
}
