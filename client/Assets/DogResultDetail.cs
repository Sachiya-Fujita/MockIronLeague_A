using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DogResultDetail : MonoBehaviour
{
    [SerializeField] ResultCell[] resultCells;
    [SerializeField] Text text;

    public void Win()
    {
        resultCells[0].InitCell("参加賞", 100);
        resultCells[1].InitCell("Aチームからの賄賂", 500);
        resultCells[2].InitCell("警視総監賞", 300);
        resultCells[3].InitCell("", 0);
        PlayerDataManager.Instance.MyCoin += 300;
        text.text = "+" + 300;
        PlayerDataManager.Instance.SetAndSaveInteger(PlayerPrefsKey.MyCoin);
    }

    public void Lose()
    {
        resultCells[0].InitCell("参加賞", 100);
        for (int i = 1; i < resultCells.Length; i++)
        {
            resultCells[i].InitCell("", 0);
        }
        PlayerDataManager.Instance.MyCoin += 100;
        text.text = "+" + 100;
        PlayerDataManager.Instance.SetAndSaveInteger(PlayerPrefsKey.MyCoin);
    }
}
