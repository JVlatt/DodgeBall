using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NumberOfPlayers : MonoBehaviour
{
    public int numberOfPlayers;

    public void TwoPlayers()
    {
        numberOfPlayers = 2;
        DontDestroyOnLoad(this);
        SceneManager.LoadScene("TeamSelection");
    }

    public void ThreePlayers()
    {
        numberOfPlayers = 3;
        DontDestroyOnLoad(this);
        SceneManager.LoadScene("TeamSelection");
    }

    public void FourPlayers()
    {
        numberOfPlayers = 4;
        DontDestroyOnLoad(this);
        SceneManager.LoadScene("TeamSelection");
    }

    public void FivePlayers()
    {
        numberOfPlayers = 5;
        DontDestroyOnLoad(this);
        SceneManager.LoadScene("TeamSelection");
    }

    public void SixPlayers()
    {
        numberOfPlayers = 6;
        DontDestroyOnLoad(this);
        SceneManager.LoadScene("TeamSelection");
    }
}
