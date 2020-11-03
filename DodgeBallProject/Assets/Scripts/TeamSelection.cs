using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.UI;

public class TeamSelection : MonoBehaviour
{
    public int playerID = 0;
    [SerializeField] private Player player;

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

    public enum Team
    {
        None,
        Red,
        Blue,
    }

    public Team curTeam;

    void Start()
    {
        player = ReInput.players.GetPlayer(playerID);
        curTeam = Team.None;
    }

    void Update()
    {
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
            isReady = true;

            if(controllerImage.transform.position == cursorPosList[2].transform.position)
            {
                curTeam = Team.Blue;
                controllerImage.GetComponent<Image>().color = Color.blue;
            }

            if (controllerImage.transform.position == cursorPosList[0].transform.position)
            {
                curTeam = Team.Red;
                controllerImage.GetComponent<Image>().color = Color.red;
            }
        }

        if (hasJoin && player.GetButtonDown("Leave"))
        {
            pressToJoin.SetActive(true);
            controllerImage.SetActive(false);
            controllerImage.transform.position = cursorPosList[1].transform.position;
            controllerImage.GetComponent<Image>().color = Color.white;
            cursorPosList.Clear();
            hasJoin = false;
            isReady = false;
            curTeam = Team.None;
        }

        if (hasJoin && !isReady && cursorPos > 0)
        {
            controllerImage.transform.position = cursorPosList[2].transform.position;
        }

        if (hasJoin && !isReady && cursorPos < 0)
        {
            controllerImage.transform.position = cursorPosList[0].transform.position;
        }
    }
}
