﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchingState : ScreenState
{
    private ScreenStateType screenStateType;
    private ScreenStateType[] nextScreenStateTypes;

    public MatchingState()
    {
        this.screenStateType = ScreenStateType.Matching;
        ScreenStateManager.Instance.NowScreenStateType = screenStateType;
        this.nextScreenStateTypes = new ScreenStateType[] { ScreenStateType.Main, ScreenStateType.Menu };
        ScreenStateManager.Instance.ChangeScreenState(this);
    }

    public override void GoToNextScene(int screenIndex)
    {
        if (nextScreenStateTypes[screenIndex].Equals(null))
        {
            // エラー処理
            Debug.LogError("指定されたIndex = " + screenIndex + "は存在しません。");
        }
        else
        {
            ScreenStateManager.Instance.ChangeScreenStateType(nextScreenStateTypes[screenIndex]);
        }
    }

    public override void GoToNextScene(ScreenStateType screenStateType)
    {
        if (!nextScreenStateTypes.Any(state => state == screenStateType))
        {
            // エラー処理
            Debug.LogError("指定されたState = " + screenStateType + "は存在しません。");
        }
        else
        {
            ScreenStateManager.Instance.ChangeScreenStateType(screenStateType);
        }
    }
}