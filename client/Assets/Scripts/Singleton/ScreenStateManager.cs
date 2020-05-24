using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenStateManager : SingletonMonoBehaviour<ScreenStateManager>
{
    #region define
    
    // TODO: 初期値をStartに変更
    // Stateクラスの管理変数
    private ScreenState nowScreenState = null;
    public ScreenState NowScreenState
    {
        get{ return this.nowScreenState; }
    }

    // StateTypeの管理変数
    private ScreenStateType nowScreenStateType = ScreenStateType.Start;
    public ScreenStateType NowScreenStateType
    {
        get{ return this.nowScreenStateType; }
        set{ nowScreenStateType = value; }
    }

    #endregion define

    #region public method

    /// <summary>
    /// nowScreenStateの変更を行う
    /// </summary>
    /// <param name="nowScreenState"></param>
    public void ChangeScreenState(ScreenState nowScreenState)
    {
        this.nowScreenState = nowScreenState;
    }

    /// <summary>
    /// nowScreenStateTypeの変更と遷移を行う
    /// </summary>
    /// <param name="nowScreenStateType"></param>
    public void ChangeScreenStateType(ScreenStateType nowScreenStateType)
    {
        this.nowScreenStateType = nowScreenStateType;
        if (!SceneManager.GetActiveScene().name.Equals(nowScreenStateType.ToString()))
        {
            SceneManager.LoadSceneAsync(nowScreenStateType.ToString());
        }
    }

    public void GoToNextScene(int screenIndex)
    {
        nowScreenState.GoToNextScene(screenIndex);
    }

    public void GoToNextScene(ScreenStateType screenStateType)
    {
        nowScreenState.GoToNextScene(screenStateType);
    }

    #endregion public method

    #region private method

    private void Start()
    {
        ChangeScreenStateType(ScreenStateType.Start);
    }

    #endregion private method
}
