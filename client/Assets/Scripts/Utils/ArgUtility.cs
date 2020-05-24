using UnityEngine;
using System.Collections;

public class ArgUtility
{
    public static readonly int ARGS_PAR_CIRCLE = 360;
    /// <summary>
    /// argがマイナスの時、プラスに変更する
    /// </summary>
    /// <param name="arg">マイナスになる可能性のある角度</param>
    /// <returns></returns>
    public static float CastPlusArgByMinusArg(float arg)
    {
        return arg < 0 ? ARGS_PAR_CIRCLE + arg : arg;
    }

    /// <summary>
    /// 360より小さくする
    /// </summary>
    /// <param name="arg">360を超える可能性のある角度</param>
    /// <returns></returns>
    public static float CastNomalArgByOverArg(float arg)
    {
        return arg > ARGS_PAR_CIRCLE ? arg % ARGS_PAR_CIRCLE : arg;
    }

    public static Vector3 GetVector3ByPlayerId(int PlayerId)
    {
        int arg = 90 * PlayerId;
        return new Vector3(Mathf.Sin(arg * Mathf.Deg2Rad), 0f, Mathf.Cos(arg * Mathf.Deg2Rad));
    }
}
