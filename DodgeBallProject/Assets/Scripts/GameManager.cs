using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    public static GameManager Instance
    {
        get { return _instance; }
    }

    public int leftPoints = 0;
    public int rightPoints = 0;

    public enum GAME_STATE
    {
        PAUSE,
        FREEZE,
        PLAY,
        END
    }

    public GAME_STATE state;
    public Recorder recorder;
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this.gameObject);

        state = GAME_STATE.PLAY;
    }
    
    public void AddPoint(string teamName)
    {
        switch (teamName)
        {
            case "left":
                leftPoints++;
                break;
            case "right":
                rightPoints++;
                break;
            default:
                break;
        }

        UIManager.Instance.UpdateScore();
        StartCoroutine("ScoringCoroutine");
    }

    public IEnumerator ScoringCoroutine()
    {
        state = GAME_STATE.FREEZE;
        recorder.PlayRecordClip();
        yield return new WaitForSeconds(5.0f);
        state = GAME_STATE.PLAY;
    }
    
}
