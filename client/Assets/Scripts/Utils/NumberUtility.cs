using System;

public class NumberUtility
{
    public static bool BetweenAAndB(float target, float from, float to){
        return target > from && target < to;
    }

    public static int RandomAToB(int a, int b)
    {
        Random random = new Random();
        return random.Next(a, b);
    }

    /// <summary>
    /// ランダムな値を生成
    /// 0 ~ max-1までの値を生成します
    /// ex) max = 5のとき、{0, 1, 2, 3, 4}
    /// </summary>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int RandomLendth(int length)
    {
        Random random = new Random();
        return random.Next(0, length);
    }

    public static int GetMinBySec(int sec)
    {
        return sec / 60;
    }

    public static int GetSec(int sec)
    {
        return sec % 60;
    }
}
