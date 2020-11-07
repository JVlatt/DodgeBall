using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.Networking;

public class PlayerCtrl : MonoBehaviour
{
    public int playerID = 99;
    [HideInInspector]public Player player;

    public PauseMenu pauseCanvas;
    private PlayerEntity entity;
    private bool stopMove;

    void Start()
    {
        entity = this.gameObject.transform.parent.GetComponent<PlayerEntity>();
        pauseCanvas = GameObject.FindObjectOfType<PauseMenu>();
    }

    void Update()
    {
        if (player == null) return;
        Inputs();

        if (GameManager.Instance.state != GameManager.GAME_STATE.PLAY)
        {
            stopMove = true;
            entity.rb.velocity = Vector3.zero;
        }
        else
        {
            stopMove = false;
        }
    }

    public void Inputs()
    {
        if (!stopMove)
        {
            MoveInputs();
        }

        OrientInputs();
        if(GameManager.Instance.state == GameManager.GAME_STATE.PLAY)
        {
            ActionsInputs();
        }
    }

    public void MoveInputs()
    {
        float dirX = player.GetAxis("MoveHorizontal");
        float dirZ = player.GetAxis("MoveVertical");

        if (entity.isOnGround)
        {
            entity.stopModelOrient = false;
            Vector3 moveDir = new Vector3(dirX, 0, dirZ);
            moveDir.Normalize();
            entity.Move(moveDir);
        }
        else
        {
            entity.stopModelOrient = true;
            Vector3 moveDir = new Vector3(dirX, entity.rb.velocity.y, dirZ);
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
        {
            if(entity.playerBall == null)
            {
                entity.TryCatch();
            }
            else
            {
                entity.LaunchBall();
            }
        }

        if (player.GetButtonDown("Dash"))
            entity.StartCoroutine(entity.Dash());

        if (player.GetButtonDown("Pause"))
        {
            if (PauseMenu.isPaused)
            {
                pauseCanvas.Resume();
            }
            else
            {
                pauseCanvas.Pause();
            }
        }
    }

    public void UpdateID(int id)
    {
        playerID = id;
        player = ReInput.players.GetPlayer(playerID);
    }
}
