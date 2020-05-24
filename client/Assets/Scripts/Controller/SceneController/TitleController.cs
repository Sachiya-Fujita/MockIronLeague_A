using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class TitleController : MonoBehaviour
{
    [SerializeField] Button button;
    private TitleState titleState;

    private void Start()
    {
        titleState = new TitleState();
        button.OnClickAsObservable()
            .Subscribe(_ => {
                ScreenStateManager.Instance.GoToNextScene(0);
            });
    }
}
