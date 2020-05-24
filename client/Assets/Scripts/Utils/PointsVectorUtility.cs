using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PointsVectorUtility
{
    /// <summary>
    /// 2点間の距離を求めるメソッド
    /// </summary>
    /// <param name="pivot">軸ベクトル</param>
    /// <param name="wheel">輪ベクトル</param>
    /// <returns></returns>
    public static float GetDistance(Vector3 pivot, Vector3 wheel)
    {
        Vector3 dVector = wheel - pivot;
        return dVector.magnitude;
    }

    /// <summary>
    /// 2点間の角度を求めるメソッド
    /// </summary>
    /// <param name="pivot">軸ベクトル</param>
    /// <param name="wheel">輪ベクトル</param>
    /// <returns></returns>
    public static float GetArg(Vector3 pivot, Vector3 wheel)
    {
        Vector3 dVector = wheel - pivot;
        return Mathf.Atan2(dVector.x, dVector.y) * Mathf.Rad2Deg;
    }
}
