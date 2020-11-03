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
    private GameObjectRecorder m_Recorder;
    private bool isRecording = false;
    
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

        // Take a snapshot and record all the bindings values for this frame.
        m_Recorder.TakeSnapshot(Time.deltaTime);
    }
    public void StopRecord()
    {
        isRecording = false;
        if (m_Recorder.isRecording)
        {
            // Save the recorded session to the clip.
            m_Recorder.SaveToClip(clip1);
        }
    }
    
    public void StartRecord()
    {
        System.Array.Clear(clip1.events, 0, clip1.events.Length);
        isRecording = true;
    }

    public void PlayRecordClip()
    {
        m_player.AddClip(clip1,"record1");
        m_player.Play("record1");
    }
}
