using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class Recorder : MonoBehaviour
{
    public AnimationClip clip1;
    public AnimationClip clip2;
    private Animation m_player;
    public float clipDuration;
    private float clipClock;
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

    void LateUpdate()
    {
        if (!isRecording)
            return;

        m_Recorder.TakeSnapshot(Time.deltaTime);
        clipClock += Time.deltaTime;
        if(clipClock >= clipDuration)
        {
            NextRecord();
            clipClock = 0f;
            recordA = !recordA;
        }
    }
    public void NextRecord()
    {
        if (m_Recorder.isRecording)
        {
            if(recordA)
            {
                m_Recorder.SaveToClip(clip1);
                if (clip1.length > clipDuration / 2)
                {
                    System.Array.Clear(clip2.events, 0, clip2.events.Length);
                }
                else
                    doubleRecord = true;
            }
            else
            {
                m_Recorder.SaveToClip(clip2);
                if (clip2.length > clipDuration / 2)
                {
                    System.Array.Clear(clip1.events, 0, clip2.events.Length);
                }
                else
                    doubleRecord = true;
            }
            m_Recorder.ResetRecording();
        }
    }
    
    public void StopRecord()
    {
        isRecording = false;
        NextRecord();
    }
    public void StartRecord()
    {
        System.Array.Clear(clip1.events, 0, clip1.events.Length);
        System.Array.Clear(clip2.events, 0, clip2.events.Length);
        isRecording = true;
    }

    public void PlayRecordClip()
    {
        if(doubleRecord)
        m_player.AddClip(clip1,"record1");
        m_player.Play("record1");
    }
}
