using UnityEngine;
using CharacterState;
using UniRx;

public class DogAnimation : MonoBehaviour
{
    //変更前のステート名
    private string _prevStateName;

    //ステート
    public StateProcessor stateProcessor { get; set; } = new StateProcessor();
    public CharacterStateIdle stateIdle { get; set; } = new CharacterStateIdle();
    public CharacterStateRun stateRun { get; set; } = new CharacterStateRun();
    public CharacterStateAttack stateAttack { get; set; } = new CharacterStateAttack();
    public CharacterStateWin stateWin { get; set; } = new CharacterStateWin();

    [SerializeField] private Animator animator;

    private void Start()
    {
        //ステートの初期化
        stateProcessor.state.Value = stateIdle;
        stateIdle.ExecAction = Idle;
        stateRun.ExecAction = Run;
        stateAttack.ExecAction = Attack;
        stateWin.ExecAction = Win;

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
        animator.SetInteger("state", (int)DogAnimationStateType.Idle);
    }

    public void Run()
    {
        Debug.Log("set run");
        animator.SetInteger("state", (int)DogAnimationStateType.Run);
    }

    public void Attack()
    {
        Debug.Log("set attack");
        animator.SetInteger("state", (int)DogAnimationStateType.Attack);
    }

    public void Win()
    {
        Debug.Log("set win");
        animator.SetInteger("state", (int)DogAnimationStateType.Win);
    }
}