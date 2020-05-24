using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class UserNameInputField : MonoBehaviour
{

    #region define

    private InputField inputField;

    #endregion define

    void Start()
    {
        inputField = this.GetComponent<InputField>();
        inputField.text = PlayerDataManager.Instance.UserName;
        // 入力終了で保存
        // ToDo:validation
        inputField.OnEndEditAsObservable()
            .Subscribe(text => PlayerDataManager.Instance.UserName = text);

    }
}
