using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.WSA;

public class TeamManager : MonoBehaviour
{
    [HideInInspector]
    public List<TeamSelection> playerList;
    [HideInInspector]
    public bool launchGame;
    [HideInInspector]
    public int i = 0;

    public List<int> blueTeam;
    public List<int> redTeam;
    private bool teamsCreated = false;

    void Start()
    {
        launchGame = false;

        var tmp = GameObject.Find("Controllers");

        for(int i = 0; i < tmp.transform.childCount; i++)
        {
            if (tmp.transform.GetChild(i).gameObject.activeSelf)
            {
                playerList.Add(tmp.transform.GetChild(i).GetComponent<TeamSelection>());
            }
        }
    }

    void Update()
    {
        if (!launchGame)
        {
            PlayerCheck();
        }
        else
        {
            if (!teamsCreated)
            {
                CreateTeams();
                teamsCreated = true;
            }
        }
    }

    void PlayerCheck()
    {
        if (i < playerList.Count && playerList[i].isReady)
        {
            i++;
        }

        if(i == playerList.Count)
        {
            launchGame = true;
        }
    }

    void CreateTeams()
    {
        for(int i = 0; i < playerList.Count; i++)
        {
            if(playerList[i].curTeam == TeamSelection.Team.Blue)
            {
                blueTeam.Add(playerList[i].playerID);
            }

            if(playerList[i].curTeam == TeamSelection.Team.Red)
            {
                redTeam.Add(playerList[i].playerID);
            }
        }
    }
}
