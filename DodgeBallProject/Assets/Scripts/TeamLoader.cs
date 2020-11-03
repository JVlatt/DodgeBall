using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeamLoader : MonoBehaviour
{
    public List<int> redTeam;
    public List<int> blueTeam;

    void Start()
    {
        if(SceneManager.GetActiveScene().name == "TeamSelection")
        {
            redTeam = GameObject.FindObjectOfType<TeamManager>().redTeam;
            blueTeam = GameObject.FindObjectOfType<TeamManager>().blueTeam;
            DontDestroyOnLoad(this);
            SceneManager.LoadScene("MattLD");
        }
    }
}
