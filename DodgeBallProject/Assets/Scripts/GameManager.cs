using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    public static GameManager Instance
    {
        get { return _instance; }
    }

    public int leftPoints = 0;
    public int rightPoints = 0;
    public float launchTimer = 3.0f;
    public float launchGameTimer = 15.0f;
    private Transform spawnBall;
    public List<PlayerEntity> players = new List<PlayerEntity>();
    public List<Goal> goals = new List<Goal>();
    public List<Ball> balls = new List<Ball>();
    public List<Destructible> destructibles = new List<Destructible>();
    public List<GameObject> redTeam;
    public List<GameObject> blueTeam;
    [SerializeField]private GameObject _ballPrefab;
    public enum GAME_STATE
    {
        LAUNCH_GAME,
        LAUNCH_ROUND,
        END_ROUND,
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

        spawnBall = GameObject.Find("SpawnBall").transform;
    }
    public void Start()
    {
        List<int> redTeamIds = FindObjectOfType<TeamLoader>().redTeam;
        List<int> blueTeamIds = FindObjectOfType<TeamLoader>().blueTeam;
        for(int i = 0; i < redTeamIds.Count; i++)
        {
            redTeam[i].SetActive(true);
            redTeam[i].GetComponentInChildren<PlayerCtrl>().UpdateID(redTeamIds[i]);
        }
        for (int i = 0; i < blueTeamIds.Count; i++)
        {
            blueTeam[i].SetActive(true);
            blueTeam[i].GetComponentInChildren<PlayerCtrl>().UpdateID(blueTeamIds[i]);
        }
        SwitchState(GAME_STATE.LAUNCH_GAME);
    }

    private void Update()
    {
        switch (state)
        {
            case GAME_STATE.LAUNCH_GAME:
                launchGameTimer -= Time.deltaTime;
                if (launchGameTimer <= 0)
                {
                    SwitchState(GAME_STATE.LAUNCH_ROUND);
                }
                break;
            case GAME_STATE.LAUNCH_ROUND:
                launchTimer -= Time.deltaTime;
                UIManager.Instance.UpdateLaunchTimer(launchTimer);
                if(launchTimer <= 0)
                {
                    SwitchState(GAME_STATE.PLAY);
                }
                break;
            case GAME_STATE.END_ROUND:
                break;
            case GAME_STATE.FREEZE:
                break;
            case GAME_STATE.PLAY:
                break;
            case GAME_STATE.END:
                break;
            default:
                break;
        }
    }

    public void SwitchState(GAME_STATE newState)
    {
        state = newState;
        switch (state)
        {
            case GAME_STATE.LAUNCH_ROUND:
                launchTimer = 3.0f;
                UIManager.Instance.launchTimerHolder.SetActive(true);
                break;
            case GAME_STATE.FREEZE:
                UIManager.Instance.leftPoints.gameObject.SetActive(true);
                UIManager.Instance.rightPoints.gameObject.SetActive(true);
                Reset();
                break;
            case GAME_STATE.PLAY:
                UIManager.Instance.launchTimerHolder.SetActive(false);
                UIManager.Instance.leftPoints.gameObject.SetActive(false);
                UIManager.Instance.rightPoints.gameObject.SetActive(false);
                break;
            case GAME_STATE.END:
                UIManager.Instance.endCanvas.SetActive(true);
                UIManager.Instance.endCanvas.transform.GetChild(0).gameObject.SetActive(true);

                if(leftPoints == 3)
                {
                    UIManager.Instance.endCanvas.GetComponentInChildren<Text>().text = "Red Team wins !";
                    UIManager.Instance.endCanvas.GetComponentInChildren<Text>().color = Color.red;
                }
                else
                {
                    UIManager.Instance.endCanvas.GetComponentInChildren<Text>().text = "Blue Team wins !";
                    UIManager.Instance.endCanvas.GetComponentInChildren<Text>().color = Color.blue;
                }
                break;
            default:
                break;
        }
    }

    public void AddPoint(string teamName)
    {
        switch (teamName)
        {
            case "left":
                rightPoints++;
                break;
            case "right":
                leftPoints++;
                break;
            default:
                break;
        }

        UIManager.Instance.UpdateScore();
        StartCoroutine("ScoringCoroutine");
    }

    public IEnumerator ScoringCoroutine()
    {
        SwitchState(GAME_STATE.END_ROUND);
        yield return new WaitForSeconds(2.0f);
        SwitchState(GAME_STATE.FREEZE);
        yield return new WaitForSeconds(5.0f);

        if (leftPoints == 3 || rightPoints == 3)
        {
            SwitchState(GAME_STATE.END);
        }
        else
        {
            SwitchState(GAME_STATE.LAUNCH_ROUND);
        }
    }
    public void Reset()
    {
        foreach (PlayerEntity p in players)
            p.Reset();
        foreach (Goal g in goals)
            g.Reset();
        foreach (Destructible d in destructibles)
            d.Reset();
        foreach (Ball b in balls)
            b.Reset();
    }
}
