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
        Inputs();
    }

    public void Inputs()
    {
        MoveInputs();
        OrientInputs();
        ActionsInputs();
    }

    public void MoveInputs()
    {
        float dirX = player.GetAxis("MoveHorizontal");
        float dirZ = player.GetAxis("MoveVertical");

        if (entity.isOnGround)
        {
            Vector3 moveDir = new Vector3(dirX, 0, dirZ);
            moveDir.Normalize();
            entity.Move(moveDir);
        }
        else
        {
            Vector3 moveDir = entity.rb.velocity;
            moveDir.Normalize();
            entity.Move(moveDir);
        }
    }

    public void OrientInputs()
    {
        float oriX = player.GetAxis("AimHorizontal");
        float oriZ = player.GetAxis("AimVertical");
        Mathf.Clamp01(oriX);
        Mathf.Clamp01(oriZ);
        Vector3 oriDir = new Vector3(oriX, 0, oriZ);
        oriDir.Normalize();
        entity.Orient(oriDir);

        if (Mathf.Abs(oriX) <= 0.2f && Mathf.Abs(oriZ) <= 0.2f)
        {
            entity.rightAxisTouch = false;
        }
        else
        {
            entity.rightAxisTouch = true;
        }
    }

    public void ActionsInputs()
    {
        if (player.GetButtonDown("Catch"))
            entity.TryCatch();

        if (player.GetButtonDown("Dash"))
            entity.StartCoroutine(entity.Dash());
    }
}
