using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class GamePopupMessage : MonoBehaviour
{
    #region define
    [SerializeField] private Text text;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject animatorRootObject;
    private Queue<string> strQueue = new Queue<string>() { };
    private Queue<float> secQueue = new Queue<float>() { };
    private Queue<GamePopUpColor> colorQueue = new Queue<GamePopUpColor>() { };
    private bool isViewing;
    #endregion define

    /// <summary>
    /// ゲーム中のポップアップメッセージをセット
    /// </summary>
    /// <param name="str">表示する文章</param>
    /// <param name="viewSec">表示する秒数</param>
    /// <param name="textColor">文字の色</param>
    public void SetMessage(string str, float viewSec, GamePopUpColor textColor)
    {
        strQueue.Enqueue(str);
        secQueue.Enqueue(viewSec);
        colorQueue.Enqueue(textColor);

        //キューの一つ目のメッセージなら表示
        if (!isViewing)
        {
            viewMessage();
        }
    }

    #region private method

    /// <summary>
    /// 画面に表示
    /// </summary>
    private void viewMessage()
    {
        isViewing = true;
        animatorRootObject.SetActive(true);
        text.text = strQueue.Dequeue();
        GamePopUpColor c = colorQueue.Dequeue();
        switch (c)
        {
            case GamePopUpColor.white:
                text.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                break;
            case GamePopUpColor.yellow:
                text.color = new Color(1.0f, 0.8f, 0.267f, 1.0f);
                break;
            case GamePopUpColor.red:
                text.color = new Color(1.0f, 0.05f, 0, 1.0f);
                break;
        }


        Observable.Timer(System.TimeSpan.FromMilliseconds(secQueue.Dequeue() * 1000f))
            .Subscribe(_ => endView());
    }

    /// <summary>
    /// 現在表示を終了
    /// </summary>
    private void endView()
    {
        animatorRootObject.SetActive(false);
        //まだメッセージがあるなら続けて表示
        if (strQueue.Count > 0)
        {
            Observable.Timer(System.TimeSpan.FromMilliseconds(500))
            .Subscribe(_ => viewMessage());
        }
        isViewing = false;
    }
    #endregion private method
}
