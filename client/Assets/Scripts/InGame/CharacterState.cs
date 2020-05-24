//参考：https://www.hanachiru-blog.com/entry/2019/04/20/010740
using UnityEngine;
using System;
using UniRx;

namespace CharacterState
{
    /// <summary>
    /// ステートの実行を管理するクラス
    /// </summary>
    public class StateProcessor
    {
        //ステート本体
        public ReactiveProperty<CharacterState> state { get; set; } = new ReactiveProperty<CharacterState>();

        //実行ブリッジ
        public void Execute() => state.Value.Execute();
    }

    public abstract class CharacterState
    {
        public Action ExecAction
        {
            get; set;
        }

        //実行処理
        public virtual void Execute()
        {
            if (ExecAction != null) ExecAction();
        }

        //ステート名を取得するメソッド
        public abstract string GetStateName();
    }

    //以下状態クラス


    public class CharacterStateIdle : CharacterState
    {
        public override string GetStateName()
        {
            return "State:Idle";
        }
    }

    public class CharacterStateRun : CharacterState
    {
        public override string GetStateName()
        {
            return "State:Run";
        }
    }

    public class CharacterStateAttack : CharacterState
    {
        public override string GetStateName()
        {
            return "State:Attack";
        }
    }

    public class CharacterStateJump : CharacterState
    {
        public override string GetStateName()
        {
            return "State:Jump";
        }
    }

    public class CharacterStateWin : CharacterState
    {
        public override string GetStateName()
        {
            return "State:Win";
        }
    }

    public class CharacterStateSample : CharacterState
    {
        public override string GetStateName()
        {
            return "State:Sample";
        }

        public override void Execute()
        {
            Debug.Log("なにか特別な処理をしたいときは派生クラスにて処理をしても良い");
            if (ExecAction != null) ExecAction();
        }
    }
}