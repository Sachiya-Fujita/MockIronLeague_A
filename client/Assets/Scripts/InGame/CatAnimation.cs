using UnityEngine;
using CharacterState;
using UniRx;

public class CatAnimation : MonoBehaviour
{
    //変更前のステート名
    private string _prevStateName;

    //ステート
    public StateProcessor stateProcessor { get; set; } = new StateProcessor();
    public CharacterStateIdle stateIdle { get; set; } = new CharacterStateIdle();
    public CharacterStateRun stateRun { get; set; } = new CharacterStateRun();
    public CharacterStateJump stateJump { get; set; } = new CharacterStateJump();

    [SerializeField] private Animator animator;

    private void Start()
    {
        //ステートの初期化
        stateProcessor.state.Value = stateIdle;
        stateIdle.ExecAction = Idle;
        stateRun.ExecAction = Run;
        stateJump.ExecAction = Jump;

        //ステートの値が変更されたら実行処理を行うようにする
        stateProcessor.state
            .Where(_ => stateProcessor.state.Value.GetStateName() != _prevStateName)
            .Subscribe(_ =>
            {
                Debug.Log("Now State:" + stateProcessor.state.Value.GetStateName());
                _prevStateName = stateProcessor.state.Value.GetStateName();
                stateProcessor.Execute();
            })
            .AddTo(this);
    }

    public void Idle()
    {
        Debug.Log("set idle");
        animator.SetInteger("state", (int)CatAnimationStateType.Idle);
    }

    public void Run()
    {
        Debug.Log("set run");
        animator.SetInteger("state", (int)CatAnimationStateType.Run);
    }

    public void Jump()
    {

    }
}