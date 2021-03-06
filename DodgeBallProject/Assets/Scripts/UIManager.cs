﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance = null;
    public static UIManager Instance
    {
        get { return _instance; }
    }

    public Text leftPoints;
    public Text rightPoints;
    public GameObject launchTimerHolder;
    public TextMeshProUGUI launchTimer;
    public GameObject endCanvas;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this.gameObject);
    }

    public void UpdateScore()
    {
        leftPoints.text = GameManager.Instance.leftPoints.ToString();
        rightPoints.text = GameManager.Instance.rightPoints.ToString();
    }

    public void UpdateLaunchTimer(float value)
    {
        if(value > 0.5f)
        {
            launchTimer.text = value.ToString("F0");
        }
        else
        {
            launchTimer.SetText("Go !", true);
        }
    }

    public void ReturnToMenu()
    {
        SoundManager.instance.ButtonClick();
        Destroy(UIManager.Instance.gameObject);
        Destroy(GameManager.Instance);
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        SoundManager.instance.ButtonClick();
        Time.timeScale = 1;
        Destroy(UIManager.Instance.gameObject);
        Destroy(GameObject.FindObjectOfType<GameManager>().gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
