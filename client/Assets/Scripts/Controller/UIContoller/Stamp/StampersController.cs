using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StampersController : MonoBehaviour
{
    [SerializeField]
    private StamperController[] stampers;

    public StamperController[] Stampers
    {
        get{ return stampers; }
    }
    

    private int[] dogPlayersId;

    private void Start()
    {
        if (PhotonManager.Instance.IsConnect)
        {
            dogPlayersId = PhotonManager.Instance.PhotonPlayers
            .Where(player => (PlayerType)player.CustomProperties["PlayerType"] != PlayerType.Cat)
            .Select(player => player.ID)
            .ToArray();
            for (int i = 0; i < dogPlayersId.Length; i++)
            {
                stampers[i].StamperId = dogPlayersId[i];
                stampers[i].InitStamper();
            }
        }
    }
}
