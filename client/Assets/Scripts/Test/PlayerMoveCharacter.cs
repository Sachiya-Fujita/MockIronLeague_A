using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class PlayerMoveCharacter : MonoBehaviour
{
    // 移動方向
    [SerializeField] private Vector3 velocity;
    // 物理挙動
    [SerializeField] private Rigidbody rb;
    // 移動速度
    [SerializeField] private float moveSpeed = 5.0f;
    // 振り向きの適用速度
    [SerializeField] private float applySpeed = 0.2f;
    // カメラの水平回転を参照する用
    [SerializeField] private PlayerFollowCamera refCamera;

    private const float jumpForce = 5.0f;

    private void Start(){
     setupEvent();
    }

    private void Update () {
        // WASD入力から、XZ平面(水平な地面)を移動する方向(velocity)を得ます
        velocity = Vector3.zero;
        if(Input.GetKey(KeyCode.W))
            velocity.z += 1;
        if(Input.GetKey(KeyCode.A))
            velocity.x -= 1;
        if(Input.GetKey(KeyCode.S))
            velocity.z -= 1;
        if(Input.GetKey(KeyCode.D))
            velocity.x += 1;

        // ジャンプ時重力を考慮
        if(velocity.y > 0){
            velocity.y += Physics.gravity.y * Time.deltaTime;
        }

        // 速度ベクトルの長さを1秒でmoveSpeedだけ進むように調整
        velocity = velocity.normalized * moveSpeed * Time.deltaTime;

        // いずれかの方向に移動している場合
        if(velocity.magnitude > 0)
        {
            // プレイヤーの回転(transform.rotation)の更新
            // 無回転状態のプレイヤーのZ+方向(後頭部)を、
            // カメラの水平回転(refCamera.hRotation)で回した移動の反対方向(-velocity)に回す回転に段々近づける
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(refCamera.hRotation * -velocity),
                                                  applySpeed);
            // プレイヤーの位置(transform.position)の更新
            // カメラの水平回転(refCamera.hRotation)で回した移動方向(velocity)を足し込み
            transform.position += refCamera.hRotation * velocity;

        }
    }

    //イベント登録
    private void setupEvent(){
        this.UpdateAsObservable()
            .Where(_ => Input.GetButtonDown("Jump"))
            .Subscribe(_ => onJump())
            .AddTo(this);
    }

    // jumpしたときの挙動
    private void onJump(){
        var jumpV = new Vector3(0, jumpForce, 0);
        rb.velocity = jumpV;
    }
}
