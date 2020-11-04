using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using RockVR.Video;
public class Recorder : MonoBehaviour
{
    private float clipClock = 0f;
    private bool isRecording = false;
    private bool recordA = true;
    private bool doubleRecord = false;
    
    public void Awake()
    {
        StartRecord();
    }
    public void StartRecord()
    {
        VideoCaptureCtrl.instance.StartCapture();
    }

    void LateUpdate()
    {
    }

    public void StopRecord()
    {
    }
    public void PlayRecordClip()
    {
    }
}
