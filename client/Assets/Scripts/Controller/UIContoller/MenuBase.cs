using UnityEngine;      
using UnityEngine.UI;   
using UniRx;
using System;

public class MenuBase : MonoBehaviour
{
    #region define

    /// <summary>
	/// 状態
	/// </summary>
	public enum MenuStatus {
		/// <summary>無効状態</summary>
		Invalid = -1,
		/// <summary>開いている状態</summary>
		Open,
		/// <summary>開き切った状態</summary>
		Opened,
		/// <summary>閉じている状態</summary>
		Close,
		/// <summary>閉じきっている状態</summary>
		Closed,
    }

    #endregion

    #region variable

    // バックボタン                         
    [SerializeField] 
    protected Button backButton;

    public Subject<MenuStateType> onMenuStateType = new Subject<MenuStateType>();

    #endregion
    // Start is called before the first frame update
    #region method

    private void Start()
    {
        setupEvent();
    }

    public virtual void setupEvent()
    {
        backButton.OnSafeClickAsObservable()
            .Subscribe(_ => onMenuStateType.OnNext(MenuStateType.Home))
            .AddTo(this);
    }

    #endregion
}
