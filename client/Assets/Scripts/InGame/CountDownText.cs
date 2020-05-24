using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class CountDownText : MonoBehaviour {

    [SerializeField]
    private GameManager rxCountDownTimer;

    private Text text;

    public void InitTimer()
    {
        text = GetComponent<Text>();

        //タイマの残り時間を描画する
        rxCountDownTimer
            .CountDownObservable
            .Subscribe(time =>
            {
                //OnNext
                text.text = NumberUtility.GetMinBySec(time) + " : " + NumberUtility.GetSec(time).ToString("00"); 
            }, () =>
            {
                //OnComplete
                text.text = string.Empty;
            });

        //タイマが10秒以下で色を赤くし大きく
        rxCountDownTimer
            .CountDownObservable
            .First(timer => timer <= 10)
            .Subscribe(_ => {
                text.color = Color.red;
                text.fontSize = 50;
            });

        //猫逃げ切り
        rxCountDownTimer
            .CountDownObservable
            .First(timer => timer == 0)
            .Subscribe(_ => rxCountDownTimer.IsCatWin = true);
    }
}