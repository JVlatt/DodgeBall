using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Animations;

public class Recorder : MonoBehaviour
{
    public AnimationClip clip1;
    private Animation m_player;
    public float clipDuration;
    private float clipClock = 0f;
    private GameObjectRecorder m_Recorder;
    private bool isRecording = false;
    private bool recordA = true;
    private bool doubleRecord = false;
    void Start()
    {
        // Create recorder and record the script GameObject.
        m_Recorder = new GameObjectRecorder(gameObject);
        m_player = GetComponent<Animation>();
        // Bind all the Transforms on the GameObject and all its children.
        m_Recorder.BindComponentsOfType<Transform>(gameObject, true);
    }

    public void StartRecord()
    {
        System.Array.Clear(clip1.events, 0, clip1.events.Length);
        recordA = true;
        isRecording = true;
        Debug.Log("Recording");
    }

    void LateUpdate()
    {
        if (!isRecording)
            return;

        clipClock += Time.deltaTime;
        m_Recorder.TakeSnapshot(Time.deltaTime);
    }

    public void StopRecord()
    {
        m_Recorder.SaveToClip(clip1);
        isRecording = false;
        Debug.Log("Stop Recording");
    }
    public void PlayRecordClip()
    {
        m_player.AddClip(clip1, "record1");
        m_player["record1"].time = m_player["record1"].length - 5.0f;
        m_player.Play("record1");
    }
}
