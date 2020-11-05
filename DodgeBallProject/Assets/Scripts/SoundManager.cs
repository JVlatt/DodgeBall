using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("General Sounds")]
    public AudioSource main_theme;
    public AudioSource button_click;

    [Header("Environement Sounds")]
    public AudioSource lastCrystalState;
    public AudioSource crystalBreak;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        instance.MainTheme();
    }

    //General
    public void MainTheme()
    {
        main_theme.Play();
    }

    public void ButtonClick()
    {
        button_click.Play();
    }

    //Environement
    public void LastCrystalState()
    {
        lastCrystalState.Play();
    }

    public void CrystalBreak()
    {
        crystalBreak.Play();
    }
}
