using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class StampButtonsController : MonoBehaviour
{
    [SerializeField]
    private ObservableEventTrigger[] stampEventTrigger;

    public ObservableEventTrigger[] StampEventTrigger
    {
        get{ return stampEventTrigger; }
    }
}
