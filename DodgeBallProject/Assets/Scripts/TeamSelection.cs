﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TeamSelection : MonoBehaviour
{
    public int playerID = 0;
    public Player player;

    public GameObject playerCanvas;
    [HideInInspector]
    public GameObject pressToJoin;
    [HideInInspector]
    public GameObject controllerImage;
    [HideInInspector]
    public List<GameObject> cursorPosList;
    [HideInInspector]
    public bool hasJoin;
    [HideInInspector]
    public bool isReady;

    public static bool canLeave = true;
    private float cd = 0f;
    private float cdc = 0.2f;

    public enum Team
    {
        None,
        Red,
        Blue,
    }

    public Team curTeam;

    void Start()
    {
        playerID = -1;
        curTeam = Team.None;
    }

    void Update()
    {
        if (player == null) return;
        if(cd > 0)
        {
            cd -= Time.deltaTime;
        }

        if (isReady)
        {
            canLeave = false;
        }

        float cursorPos = player.GetAxis("Move");

        if (!hasJoin && player.GetButtonDown("Join"))
        {
            pressToJoin = playerCanvas.transform.GetChild(0).gameObject;
            pressToJoin.SetActive(false);
            controllerImage = playerCanvas.transform.GetChild(1).gameObject;
            controllerImage.SetActive(true);
            hasJoin = true;

            for(int i = 2; i < playerCanvas.transform.childCount; i++)
            {
                cursorPosList.Add(playerCanvas.transform.GetChild(i).gameObject);
            }
        }

        if(hasJoin && player.GetButtonDown("Join") && controllerImage.transform.position != cursorPosList[1].transform.position)
        {
            if(!isReady)
                SoundManager.instance.ButtonClick();

            isReady = true;

            if(controllerImage.transform.position.x > cursorPosList[1].transform.position.x)
            {
                curTeam = Team.Blue;
                controllerImage.GetComponent<Image>().color = Color.blue;
            }

            if (controllerImage.transform.position.x < cursorPosList[1].transform.position.x)
            {
                curTeam = Team.Red;
                controllerImage.GetComponent<Image>().color = Color.red;
            }
        }

        if (hasJoin && player.GetButtonDown("Leave"))
        {
            Reset();
        }

        if (!hasJoin && canLeave && player.GetButtonDown("Leave") && cd <= 0)
        {
            Destroy(GameObject.Find("NumberOfPlayers"));
            SceneManager.LoadScene("MainMenu");
        }

        if (hasJoin && !isReady && cursorPos > 0)
        {
            controllerImage.transform.position = cursorPosList[2].transform.position;
            SoundManager.instance.SelectionEquipe();
        }

        if (hasJoin && !isReady && cursorPos < 0)
        {
            controllerImage.transform.position = cursorPosList[0].transform.position;
            SoundManager.instance.SelectionEquipe();
        }
    }

    public void Reset()
    {
        pressToJoin.SetActive(true);
        controllerImage.SetActive(false);
        controllerImage.transform.position = cursorPosList[1].transform.position;
        controllerImage.GetComponent<Image>().color = Color.white;
        cursorPosList.Clear();
        hasJoin = false;
        isReady = false;
        canLeave = true;
        curTeam = Team.None;
        cd = cdc;
    }
}
