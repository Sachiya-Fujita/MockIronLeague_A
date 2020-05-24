using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CatResultDetail : MonoBehaviour
{
    [SerializeField] ResultCell[] resultCells;
    [SerializeField] Image[] jewelCells;
    [SerializeField] Image[] getJewelCells;
    [SerializeField] Text text;

    public void Win()
    {
        resultCells[0].InitCell("参加賞", 100);
        resultCells[1].InitCell("Aチームからの賄賂", 500);
        resultCells[2].InitCell("逃げ切ったで賞", 1000);
        resultCells[3].InitCell("", 0);

        for (int i = 1; i < jewelCells.Length; i++)
        {
            jewelCells[i].enabled = false;
            if (i < PlayerDataManager.Instance.JewelCount){
                getJewelCells[i].DOFade(1f, 1f);
            }
        }
        PlayerDataManager.Instance.MyCoin += 1100;
        text.text = "+" + 1100;
        PlayerDataManager.Instance.SetAndSaveInteger(PlayerPrefsKey.MyCoin);
    }

    public void Lose()
    {
        resultCells[0].InitCell("参加賞", 100);
        for (int i = 1; i < resultCells.Length; i++)
        {
            resultCells[i].InitCell("", 0);
        }
        for (int i = 1; i < jewelCells.Length; i++)
        {
            if (i >= PlayerDataManager.Instance.JewelCount)
                jewelCells[i].enabled = false;
            jewelCells[i].DOFade(0f, 1f);
        }
        PlayerDataManager.Instance.MyCoin += 100;
        text.text = "+" + 100;
        PlayerDataManager.Instance.SetAndSaveInteger(PlayerPrefsKey.MyCoin);
    }
}
