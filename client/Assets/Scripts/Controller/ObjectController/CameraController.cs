using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class CameraController : MonoBehaviour
{
    #region define

    [SerializeField] PlayerMoveController dogMoveController;
    [SerializeField] PlayerMoveController catMoveController;
    [SerializeField] CameraMoveController cameraMoveController;
    private GameObject player;
    public GameObject Player
    {
        set { player = value; }
    }
    private Vector3 offset;

    // カメラの回転速度
    private const int CAMERA_ROTATE_SPEED = 200;

    private PlayerMoveController playerMoveController;

    #endregion

    #region private method

    public void InitCameraController()
    {
        if (PhotonManager.Instance.IsConnect)
        {
            playerMoveController = PhotonManager.Instance.NowPlayerType == PlayerType.Cat ? catMoveController : dogMoveController;
        }
        else
        {
            playerMoveController = PlayerDataManager.Instance.NowPlayerType == PlayerType.Cat ? catMoveController : dogMoveController;
        }
        offset = transform.position - player.transform.position;
        Observable.Interval(System.TimeSpan.FromMilliseconds(100))
            .Where(_ => !playerMoveController.GetMoveable())
            .Subscribe(_ => followPlayer());
        this.FixedUpdateAsObservable()
            .Where(x => playerMoveController.GetMoveable())
            .Subscribe(x => followPlayer());
        this.FixedUpdateAsObservable()
            .Where(x => cameraMoveController.IsDraging)
            .Subscribe(x => rotateAroundPlayer());
    }

    /// <summary>
    /// プレイヤに追従するメソッド
    /// </summary>
    private void followPlayer()
    {
        transform.position = player.transform.position + offset;
    }

    /// <summary>
    /// プレイヤ中心として回転するメソッド
    /// </summary>
    private void rotateAroundPlayer()
    {
        float deltaX = cameraMoveController.DeltaX * CAMERA_ROTATE_SPEED;
        Vector3 axis = transform.TransformDirection(Vector3.down);
        if (deltaX > 0)
        {
            axis = transform.TransformDirection(Vector3.up);
        }
        transform.RotateAround(player.transform.position, axis, Mathf.Abs(deltaX) * Time.deltaTime);
        offset = transform.position - player.transform.position;
    }

    #endregion
}
