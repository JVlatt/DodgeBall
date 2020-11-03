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
    public Text RecordTimer;
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
        RecordTimer.text = clipClock.ToString();
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
    //void LateUpdate()
    //{
    //    if (!isRecording)
    //        return;

    //    RecordTimer.text = clipClock.ToString();
    //    m_Recorder.TakeSnapshot(Time.deltaTime);
    //    clipClock += Time.deltaTime;
    //    if(clipClock >= clipDuration)
    //    {
    //        NextRecord();
    //        clipClock = 0f;
    //        recordA = !recordA;
    //    }
    //}
    //public void NextRecord()
    //{
    //    Debug.Log("Recorder Status : " + m_Recorder.isRecording);
    //    if (m_Recorder.isRecording)
    //    {
    //        if(recordA)
    //        {
    //            m_Recorder.SaveToClip(clip1);
    //            if (clip1.length > clipDuration / 2)
    //            {
    //                System.Array.Clear(clip2.events, 0, clip2.events.Length);
    //            }
    //            else
    //                doubleRecord = true;

    //            Debug.Log("Clip 1 Duration : " + clip1.length);
    //        }
    //        else
    //        {
    //            m_Recorder.SaveToClip(clip2);
    //            if (clip2.length > clipDuration / 2)
    //            {
    //                System.Array.Clear(clip1.events, 0, clip1.events.Length);
    //            }
    //            else
    //                doubleRecord = true;

    //            Debug.Log("Clip 2 Duration : " + clip2.length);
    //        }
    //        m_Recorder.ResetRecording();
    //    }
    //}

    //public void StopRecord()
    //{
    //    NextRecord();
    //    isRecording = false;
    //    clipClock = 0f;
    //    Debug.Log("Stop Recording");
    //}
    //public void StartRecord()
    //{
    //    System.Array.Clear(clip1.events, 0, clip1.events.Length);
    //    System.Array.Clear(clip2.events, 0, clip2.events.Length);
    //    recordA = true;
    //    isRecording = true;
    //    Debug.Log("Recording");
    //}

    //public void PlayRecordClip()
    //{
    //    if(doubleRecord)
    //    {
    //        if(recordA)
    //        {
    //            m_player.AddClip(clip2, "record1");
    //            m_player.AddClip(clip1, "record2");
    //        }
    //        else
    //        {
    //            m_player.AddClip(clip1, "record1");
    //            m_player.AddClip(clip2, "record2");
    //        }

    //        m_player.Play("record1");
    //        m_player.PlayQueued("record2");

    //        Debug.Log("Double Clip Duration : " + clip1.length + clip2.length);
    //    }
    //    else
    //    {
    //        if(recordA)
    //            m_player.AddClip(clip1, "record1");
    //        else
    //            m_player.AddClip(clip2, "record1");

    //        m_player.Play("record1");
    //    }
    //}
}
