using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;
    private Text countdown;

    void Start()
    {
        isPaused = false;
        countdown = this.gameObject.transform.GetChild(transform.childCount - 1).gameObject.GetComponentInChildren<Text>();
    }

    public void Pause()
    {
        if(GameManager.Instance.state == GameManager.GAME_STATE.PLAY)
        {
            Time.timeScale = 0;
            isPaused = true;

            for (int i = 0; i < this.gameObject.transform.childCount - 1; i++)
            {
                this.gameObject.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    public void Resume()
    {
        if (GameManager.Instance.state == GameManager.GAME_STATE.PLAY)
        {
            for (int i = 0; i < this.gameObject.transform.childCount - 1; i++)
            {
                this.gameObject.transform.GetChild(i).gameObject.SetActive(false);
            }

            StartCoroutine(GetReady());
        }
    }

    public void Restart()
    {
        Time.timeScale = 1;
        Destroy(UIManager.Instance.gameObject);
        Destroy(GameObject.FindObjectOfType<GameManager>().gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        Destroy(UIManager.Instance.gameObject);
        Destroy(GameObject.FindObjectOfType<GameManager>().gameObject);
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public IEnumerator GetReady()
    {
        this.gameObject.transform.GetChild(transform.childCount - 1).gameObject.SetActive(true);

        float nTime = 3f;
        while (nTime >= 0f)
        {
            nTime -= Time.unscaledDeltaTime;
            if(nTime > 0.5f)
            {
                countdown.text = nTime.ToString("F0");
            }
            else
            {
                countdown.text = "Go !";
            }
            yield return null;
        }

        this.gameObject.transform.GetChild(transform.childCount - 1).gameObject.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }
}
