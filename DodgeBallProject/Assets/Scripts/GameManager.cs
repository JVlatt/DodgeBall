using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    public static GameManager Instance
    {
        get { return _instance; }
    }

    public int leftPoints = 0;
    public int rightPoints = 0;
    public float launchTimer = 5.0f;
    private Transform spawnBall;
    public List<PlayerEntity> players = new List<PlayerEntity>();
    public List<Goal> goals = new List<Goal>();
    public List<Ball> balls = new List<Ball>();
    [SerializeField]private GameObject _ballPrefab;
    public enum GAME_STATE
    {
        LAUNCH,
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
        SwitchState(GAME_STATE.LAUNCH);
    }

    private void Update()
    {
        switch (state)
        {
            case GAME_STATE.LAUNCH:
                launchTimer -= Time.deltaTime;
                UIManager.Instance.UpdateLaunchTimer(launchTimer);
                if(launchTimer <= 0)
                {
                    SwitchState(GAME_STATE.PLAY);
                }
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
        if(leftPoints == 2 || rightPoints == 2)
        {
            Destroy(UIManager.Instance.gameObject);
            Destroy(this.gameObject);
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void SwitchState(GAME_STATE newState)
    {
        state = newState;
        switch (state)
        {
            case GAME_STATE.LAUNCH:
                launchTimer = 5.0f;
                UIManager.Instance.launchTimerHolder.SetActive(true);
                break;
            case GAME_STATE.FREEZE:
                Reset();
                break;
            case GAME_STATE.PLAY:
                LaunchBall();
                StartCoroutine(WaitForSecondBall());
                UIManager.Instance.launchTimerHolder.SetActive(false);
                break;
            case GAME_STATE.END:
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
        SwitchState(GAME_STATE.FREEZE);
        yield return new WaitForSeconds(10.0f);
        SwitchState(GAME_STATE.LAUNCH);
    }
    public void Reset()
    {
        foreach (PlayerEntity p in players)
            p.Reset();
        foreach (Goal g in goals)
            g.Reset();

        balls.Clear();
    }
    public void LaunchBall()
    {
        GameObject instantiatedBall = Instantiate(_ballPrefab, spawnBall.transform.position, Quaternion.identity, recorder.transform);
        instantiatedBall.GetComponent<Ball>().direction = new Vector3(1, 0, 0);
    }

    IEnumerator WaitForSecondBall()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject instantiatedBall2 = Instantiate(_ballPrefab, spawnBall.transform.position, Quaternion.identity, recorder.transform);
        instantiatedBall2.GetComponent<Ball>().direction = new Vector3(-1, 0, 0);
    }
}
