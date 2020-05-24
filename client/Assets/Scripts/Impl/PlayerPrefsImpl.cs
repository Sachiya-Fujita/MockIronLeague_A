using System;
using UnityEngine;

public class PlayerPrefsImpl
{
    /// <summary>
    /// stringを設定する
    /// </summary>
    /// <param name="playerPrefsKey"></param>
    /// <param name="setValue"></param>
    public static void SetString(PlayerPrefsKey playerPrefsKey, string setValue)
    {
        PlayerPrefs.SetString(playerPrefsKey.ToString(), setValue);
    }

    public static void SetInteger(PlayerPrefsKey playerPrefsKey, int setValue)
    {
        PlayerPrefs.SetInt(playerPrefsKey.ToString(), setValue);
    }

    /// <summary>
    /// stringのValueを受け取る
    /// </summary>
    /// <param name="playerPrefsKey"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static string GetStringValue(PlayerPrefsKey playerPrefsKey, string defaultValue)
    {
        return PlayerPrefs.GetString(playerPrefsKey.ToString(), defaultValue);
    }

    public static int GetIntegerValue(PlayerPrefsKey playerPrefsKey, int defaultValue)
    {
        return PlayerPrefs.GetInt(playerPrefsKey.ToString(), defaultValue);
    }
}
