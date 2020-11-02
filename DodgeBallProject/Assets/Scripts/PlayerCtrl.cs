using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerCtrl : MonoBehaviour
{
    [SerializeField]private int playerID = 0;
    [SerializeField]private Player player;
    private PlayerEntity entity;
    void Start()
    {
        player = ReInput.players.GetPlayer(playerID);
        entity = this.gameObject.transform.parent.GetComponent<PlayerEntity>();
    }

    void Update()
    {
        float dirX = player.GetAxis("MoveHorizontal");
        float dirZ = player.GetAxis("MoveVertical");

        Vector3 moveDir = new Vector3(dirX, entity.rb.velocity.y, dirZ);
        moveDir.Normalize();
        entity.Move(moveDir);

        float oriX = player.GetAxis("AimHorizontal");
        float oriZ = player.GetAxis("AimVertical");
        Vector3 oriDir = new Vector3(oriX, 0, oriZ);
        oriDir.Normalize();
        entity.Orient(oriDir);

        if(oriX == 0 && oriZ == 0)
        {
            entity.rightAxisTouch = false;
        }
        else
        {
            entity.rightAxisTouch = true;
        }
    }
}
