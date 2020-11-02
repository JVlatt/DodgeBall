using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerCtrl : MonoBehaviour
{
    [SerializeField]private int playerID = 0;
    [SerializeField]private Player player;
    [SerializeField]public PlayerEntity entity;
    void Start()
    {
        player = ReInput.players.GetPlayer(playerID);
    }

    void Update()
    {
        float dirX = player.GetAxis("MoveHorizontal");
        float dirZ = player.GetAxis("MoveVertical");

        Vector3 moveDir = new Vector3(dirX, entity.rb.velocity.y, dirZ);
        moveDir.Normalize();
        entity.Move(moveDir);
    }
}
