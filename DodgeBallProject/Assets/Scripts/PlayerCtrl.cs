using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerCtrl : MonoBehaviour
{
    [SerializeField]private int playerID = 0;
    [SerializeField]private Player player;
    void Start()
    {
        player = ReInput.players.GetPlayer(playerID);
    }

    void Update()
    {
        
    }
}
