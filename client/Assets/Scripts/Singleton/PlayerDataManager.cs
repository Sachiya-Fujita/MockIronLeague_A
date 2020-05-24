using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : SingletonMonoBehaviour<PlayerDataManager>
{
    #region define

    private string userName;
    public string UserName {
        get { return userName; }
        set { userName = value; }
    }

    //TODO: Serialize外す
    [SerializeField] private PlayerType nowPlayerType = PlayerType.Oliver;
    public PlayerType NowPlayerType
    {
        get{ return nowPlayerType; }
        set{ nowPlayerType = value; }
    }

    private PhotonPlayer winnerPlayer;
    public PhotonPlayer WinnerPlayer
    {
        get{ return winnerPlayer; }
        set{ winnerPlayer = value; }
    }

    private int myCoin = 0;
    public int MyCoin
    {
        get{ return myCoin; }
        set{ myCoin = value; }
    }

    private int jewelCount = 0;
    public int JewelCount
    {
        get{ return jewelCount; }
        set{ jewelCount = value; }
    }

    #endregion define

    public void SetAndSaveString(PlayerPrefsKey playerPrefsKey, string text)
    {
        userName = text;
        PlayerPrefsImpl.SetString(playerPrefsKey, text);
    }

    public void SetAndSaveInteger(PlayerPrefsKey playerPrefsKey)
    {
        PlayerPrefsImpl.SetInteger(playerPrefsKey, myCoin);
    }

    private void Start()
    {
        userName = PlayerPrefsImpl.GetStringValue(PlayerPrefsKey.UserName, "オリバー");
        Debug.Log("userName : " + userName);

        myCoin = PlayerPrefsImpl.GetIntegerValue(PlayerPrefsKey.MyCoin, 0);
        Debug.Log("myCoin : " + myCoin);
    }
}
