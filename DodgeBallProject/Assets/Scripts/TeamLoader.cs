using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeamLoader : MonoBehaviour
{
    public List<int> redTeam;
    public List<int> blueTeam;
    public Animator anim;
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "TeamSelection")
        {
            redTeam = GameObject.FindObjectOfType<TeamManager>().redTeam;
            blueTeam = GameObject.FindObjectOfType<TeamManager>().blueTeam;
            DontDestroyOnLoad(this);
            StartCoroutine(LoadSceneCoroutine());
        }

        if(SceneManager.GetActiveScene().name == "TheoLD")
        {

        }
    }

    IEnumerator LoadSceneCoroutine()
    {
        anim.SetTrigger("Transi");
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
