using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("General Sounds")]
    public AudioSource main_theme;
    public AudioSource button_click;
    public AudioSource selectionEquipe;

    [Header("Player Sounds")]
    public AudioSource catchBall;
    public AudioSource throwBall;
    public AudioSource hitPlayer;
    public AudioSource fall;
    public AudioSource dash;

    [Header("Events Sounds")]
    public AudioSource hitWall;
    public AudioSource wallFall;
    public AudioSource scorePoint;
    public AudioSource lastCrystalState;
    public AudioSource crystalTouch;
    public AudioSource crystalBreak;
    public AudioSource winGame;

    [Header("Environement Sounds")]
    public AudioSource ambiance;

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

    public void SelectionEquipe()
    {
        selectionEquipe.Play();
    }

    //Player
    public void CatchBall()
    {
        catchBall.Play();
    }

    public void ThrowBall()
    {
        throwBall.Play();
    }

    public void HitPlayer()
    {
        hitPlayer.Play();
    }

    public void Fall()
    {
        fall.Play();
    }

    public void Dash()
    {
        dash.Play();
    }

    //Events
    public void HitWall()
    {
        hitWall.Play();
    }
    public void WallFall()
    {
        wallFall.Play();
    }
    public void ScorePoint()
    {
        scorePoint.Play();
    }
    public void LastCrystalState()
    {
        lastCrystalState.Play();
    }

    public void CrystalTouch()
    {
        crystalTouch.Play();
    }

    public void CrystalBreak()
    {
        crystalBreak.Play();
    }

    public void WinGame()
    {
        winGame.Play();
    }

    //Evironement
    public void Ambiance()
    {
        ambiance.Play();
    }
}
