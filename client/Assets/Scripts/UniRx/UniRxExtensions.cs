using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public static class UniRxExtensions
{
    #region method

    /// <summary>
    /// 連打防止用クリック監視
    /// </summary>
    public static IObservable<Unit> OnSafeClickAsObservable(this Button button)
    {
        return button.onClick.AsObservable().ThrottleFirst(System.TimeSpan.FromMilliseconds(250));
    }
    
    /// <summary>
    /// クリック監視(長押し考慮)
    /// </summary>
    public static IObservable<Unit> OnClickAsObservableSafety(this Button button, float duplicateSafetySeconds = 1f, float pressSafetySeconds = 1f)
    {
        return button
            .OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(duplicateSafetySeconds)) // 連打防止
            .SkipUntil(button.OnPointerDownAsObservable())
            .TakeUntil(button.OnLongClickAsObservable(pressSafetySeconds)) // 長押し後に指を離してもタップイベントを発行しない
            .RepeatUntilDestroy(button)
            .AsUnitObservable();
    }
    
    /// <summary>
    /// 長押しタップ監視
    /// </summary>
    public static IObservable<Unit> OnLongClickAsObservable(this Button button, float pressSeconds = 1f)
    {
        return button
            .OnPointerDownAsObservable()
            .Throttle(TimeSpan.FromSeconds(pressSeconds))
            .TakeUntil(button.OnPointerExitAsObservable()) // 押したまま指がボタン領域から離れたら終了
            .TakeUntil(button.OnPointerUpAsObservable())
            .RepeatUntilDestroy(button)
            .AsUnitObservable();
    
    }

#if UNITY_STANDALONE
    /// <summary>
    /// ダブルクリック判定監視(3回以上も許容)
    /// </summary>
    public static IObservable<Unit> OnDoubleClickAsObservable(this Button button)
    {
        return button
            .OnMouseDownAsObservable()
            .Buffer(button.OnMouseDownAsObservable().Throttle(TimeSpan.FromSeconds(0.2f)))
            .Where(x => 2 <= x.Count)
            .AsUnitObservable();
    }
    
    /// <summary>
    /// ダブルクリック判定監視(3回以上も許容)
    /// </summary>
    public static IObservable<bool> OnIsDoubleClickAsObservable(this Button button)
    {
        return button
            .OnMouseDownAsObservable()
            .Buffer(button.OnMouseDownAsObservable().Throttle(TimeSpan.FromSeconds(0.2f)))
            .Select(v => v.Count >=2)
            .AsObservable();
    }
    
    /// <summary>
    /// マルチクリック判定監視
    /// </summary>
    public static IObservable<int> OnMultiClickAsObservable(this Button button)
    {
        return button
            .OnMouseDownAsObservable()
            .Buffer(button.OnMouseDownAsObservable().Throttle(TimeSpan.FromSeconds(0.2f)))
            .Select(v => v.Count)
            .AsObservable();
    }
    
    /// Todo:勉強不足だが似た2つの関数一つにまとめたい．ジェネリック関数とかかな？
    /// <summary>
    /// ダブルクリック判定監視(3回以上も許容)
    /// </summary>
    public static IObservable<Unit> OnDoubleClickAsObservable(this GameObject gameObject)
    {
        return gameObject
            .OnMouseDownAsObservable()
            .Buffer(gameObject.OnMouseDownAsObservable().Throttle(TimeSpan.FromSeconds(0.2f)))
            .Where(x => 2 <= x.Count)
            .AsUnitObservable();
    }
    
    /// <summary>
    /// マルチクリック判定監視
    /// </summary>
    public static IObservable<int> OnMultiTapAsObservable(this GameObject gameObject)
    {
        return gameObject
            .OnMouseDownAsObservable()
            .Buffer(gameObject.OnMouseDownAsObservable().Throttle(TimeSpan.FromSeconds(0.2f)))
            .Select(v => v.Count)
            .AsObservable();
    }
#endif
#endregion
}