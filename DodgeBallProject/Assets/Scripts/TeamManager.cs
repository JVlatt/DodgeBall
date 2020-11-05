using Rewired;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public List<TeamSelection> playerList;
    List<TeamSelection> copyPlayerList = new List<TeamSelection>();
    [HideInInspector]
    public bool launchGame;
    [HideInInspector]
    public int i = 0;

    //[HideInInspector]
    public List<int> blueTeam;
    //[HideInInspector]
    public List<int> redTeam;
    private bool teamsCreated = false;

    [HideInInspector]
    public List<GameObject> displayList;
    [HideInInspector]
    public List<GameObject> displayPosList;

    public GameObject teamLoader;
    private List<GameObject> controllers = new List<GameObject>();
    [SerializeField]private List<int> listIndex;
    private int nbPlayers = 0;

    void Start()
    {
        launchGame = false;

        var controllersHolder = GameObject.Find("Controllers");
        var controllersDisplay = GameObject.Find("Canvas Selection");
        nbPlayers = GameObject.FindObjectOfType<NumberOfPlayers>().numberOfPlayers;

        if(nbPlayers != 0)
        {
            for (int i = 0; i < nbPlayers; i++)
            {
                controllersHolder.transform.GetChild(i).gameObject.SetActive(true);
                controllers.Add(controllersHolder.transform.GetChild(i).gameObject);
                controllersDisplay.transform.GetChild(i + 7).gameObject.SetActive(true);
                displayList.Add(controllersDisplay.transform.GetChild(i + 7).gameObject);
                listIndex.Add(i);
            }
        }

        for(int i = 0; i < controllersHolder.transform.childCount; i++)
        {
            if (controllersHolder.transform.GetChild(i).gameObject.activeSelf)
            {
                playerList.Add(controllersHolder.transform.GetChild(i).GetComponent<TeamSelection>());
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

        foreach (TeamSelection t in playerList)
            copyPlayerList.Add(t);
    }

    void Update()
    {
        for(int i = 0; i < listIndex.Count; i++)
        {
            if (ReInput.players.GetPlayer(listIndex[i]).GetButtonDown("Join"))
            {
                if(copyPlayerList.Count > 0)
                {
                    copyPlayerList[0].player = ReInput.players.GetPlayer(listIndex[i]);
                    copyPlayerList[0].playerID = listIndex[i];
                    copyPlayerList.RemoveAt(0);
                    listIndex.RemoveAt(i);
                }
            }
        }

        if (!launchGame)
        {
            PlayerCheck();
        }
        else
        {
            if (!teamsCreated)
            {
                CreateTeams();

                if(redTeam.Count == 0 || blueTeam.Count == 0)
                {
                    //reset
                    launchGame = false;
                    i = 0;
                    redTeam.Clear();
                    blueTeam.Clear();

                    for(int i = 0; i < playerList.Count; i++)
                    {
                        playerList[i].Reset();
                    }
                }
                else
                {
                    teamLoader.SetActive(true);
                    teamsCreated = true;
                }
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
