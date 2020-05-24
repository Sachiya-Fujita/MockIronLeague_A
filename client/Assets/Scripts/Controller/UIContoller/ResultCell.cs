using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultCell : MonoBehaviour
{
    [SerializeField] Text reasonText;
    [SerializeField] Text coinText;

    public void InitCell(string reason, int coin)
    {
        reasonText.text = reason;
        if (coin == 0)
        {
            coinText.text = "";
        }
        else
        {
            coinText.text = "+" + coin.ToString();
        }
    }
}
