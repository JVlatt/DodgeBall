using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    [HideInInspector]
    public List<TeamSelection> playerList;
    [HideInInspector]
    public bool launchGame;
    [HideInInspector]
    public int i = 0;

    [HideInInspector]
    public List<int> blueTeam;
    [HideInInspector]
    public List<int> redTeam;
    private bool teamsCreated = false;

    [HideInInspector]
    public List<GameObject> displayList;
    [HideInInspector]
    public List<GameObject> displayPosList;

    public GameObject teamLoader;

    void Start()
    {
        launchGame = false;

        var controllers = GameObject.Find("Controllers");
        var controllersDisplay = GameObject.Find("Canvas Selection");
        var nbPlayers = GameObject.FindObjectOfType<NumberOfPlayers>().numberOfPlayers;

        if(nbPlayers != null)
        {
            for (int i = 0; i < nbPlayers; i++)
            {
                controllers.transform.GetChild(i).gameObject.SetActive(true);
                controllersDisplay.transform.GetChild(i + 7).gameObject.SetActive(true);
                displayList.Add(controllersDisplay.transform.GetChild(i + 7).gameObject);
            }
        }

        for(int i = 0; i < controllers.transform.childCount; i++)
        {
            if (controllers.transform.GetChild(i).gameObject.activeSelf)
            {
                playerList.Add(controllers.transform.GetChild(i).GetComponent<TeamSelection>());
            }
        }

        displayPosList.Add(GameObject.Find("2playersCase"));
        displayPosList.Add(GameObject.Find("3playersCase"));
        displayPosList.Add(GameObject.Find("4playersCase"));
        displayPosList.Add(GameObject.Find("5playersCase"));

        if(nbPlayers < 6)
        {
            Display(nbPlayers);
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
                teamLoader.SetActive(true);
                teamsCreated = true;
            }
        }
    }

    void Display(int nbPlayers)
    {
        for(int i = 0; i < nbPlayers; i++)
        {
            displayList[i].transform.position = displayPosList[nbPlayers - 2].transform.GetChild(i).gameObject.transform.position;
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
