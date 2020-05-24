using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MenuState : ScreenState
{
    private ScreenStateType screenStateType;
    private ScreenStateType[] nextScreenStateTypes;

    public MenuState()
    {
        this.screenStateType = ScreenStateType.Menu;
        ScreenStateManager.Instance.NowScreenStateType = screenStateType;
        this.nextScreenStateTypes = new ScreenStateType[] { ScreenStateType.Main, ScreenStateType.Matching };
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