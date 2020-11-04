using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.XR.WSA.Input;
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
    private Transform spawnBall;
    public List<PlayerEntity> players = new List<PlayerEntity>();
    public List<Goal> goals = new List<Goal>();
    public List<Ball> balls = new List<Ball>();
    [SerializeField]private GameObject _ballPrefab;
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
        spawnBall = GameObject.Find("SpawnBall").transform;
    }
    public void Start()
    {
        LaunchBall();
        StartCoroutine(WaitForSecondBall());
    }

    private void Update()
    {
        if(leftPoints == 2 || rightPoints == 2)
        {
            Destroy(UIManager.Instance.gameObject);
            Destroy(this.gameObject);
            SceneManager.LoadScene("MainMenu");
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
        state = GAME_STATE.FREEZE;
        recorder.PlayRecordClip();
        yield return new WaitForSeconds(6.0f);
        Reset();
        StartCoroutine(WaitForSecondBall());
        state = GAME_STATE.PLAY;
    }
    public void Reset()
    {
        foreach (PlayerEntity p in players)
            p.Reset();
        foreach (Goal g in goals)
            g.Reset();
        balls.Clear();
        LaunchBall();
    }
    public void LaunchBall()
    {
        GameObject instantiatedBall = Instantiate(_ballPrefab, spawnBall.transform.position, Quaternion.identity, recorder.transform);
        recorder.m_Recorder.BindComponentsOfType<Transform>(recorder.gameObject, true);
        instantiatedBall.GetComponent<Ball>().direction = new Vector3(1, 0, 0);
    }

    IEnumerator WaitForSecondBall()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject instantiatedBall2 = Instantiate(_ballPrefab, spawnBall.transform.position, Quaternion.identity, recorder.transform);
        instantiatedBall2.GetComponent<Ball>().direction = new Vector3(-1, 0, 0);
    }
}
