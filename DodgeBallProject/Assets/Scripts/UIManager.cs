﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance = null;
    public static UIManager Instance
    {
        get { return _instance; }
    }

    public Text leftPoints;
    public Text rightPoints;

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
}