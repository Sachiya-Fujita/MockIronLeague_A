using System;
using UnityEngine;

// 画面状態インターフェース
abstract public class ScreenState
{
    public virtual void GoToNextScene(int screenIndex){ }
    public virtual void GoToNextScene(ScreenStateType screenStateType){ }
}