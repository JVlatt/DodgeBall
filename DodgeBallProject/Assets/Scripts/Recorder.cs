using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
public class Recorder : MonoBehaviour
{
    [Serializable]
    public class RecordInfo
    {
        public Vector3 position;
        public Quaternion rotation;

        public RecordInfo(Vector3 _position, Quaternion _rotation)
        {
            position = _position;
            rotation = _rotation;
        }
    }
    public Dictionary<int, List<RecordInfo>> recordInfos;
    public List<GameObject> objectsToRecord = new List<GameObject>(1);
    public float clipDuration = 20f;
    private bool isRecording = false;
    private bool isPlayingClip = false;

    void FixedUpdate()
    {
        if (isRecording)
        {
            Record();
        }
        else if (isPlayingClip)
            Play();
    }

    public void Play()
    {
        for (int i = 0; i < objectsToRecord.Count - 1; i++)
        {
                if (recordInfos[i].Count > 0)
                {
                    Debug.Log(recordInfos[i].Count);
                    objectsToRecord[i].transform.position = recordInfos[i][recordInfos[i].Count - 1].position;
                    objectsToRecord[i].transform.rotation = recordInfos[i][recordInfos[i].Count - 1].rotation;
                    recordInfos[i].RemoveAt(recordInfos[i].Count - 1);
                }
                else
                    StopRecordClip();
        }
    }

    public void Record()
    {
        for (int i = 0; i < objectsToRecord.Count - 1; i++)
        {
            if (recordInfos[i].Count > Mathf.Round(clipDuration / Time.fixedDeltaTime))
            {
                recordInfos[i].RemoveAt(recordInfos[i].Count - 1);
            }
            recordInfos[i].Insert(0, new RecordInfo(objectsToRecord[i].transform.position, objectsToRecord[i].transform.rotation));
        }
    }
    public void StartRecord()
    {
        recordInfos = new Dictionary<int, List<RecordInfo>>();
        for (int i = 0; i < objectsToRecord.Count - 1; i++)
        {
            recordInfos[i] = new List<RecordInfo>();
        }
        isRecording = true;
    }
    public void AddObjectToRecord(GameObject objectToAdd)
    {
        objectsToRecord.Add(objectToAdd);
        recordInfos[objectsToRecord.Count - 1] = new List<RecordInfo>();
    }

    public void RemoveObjectToRecord(GameObject objectToRemove)
    {
        recordInfos.Remove(objectsToRecord.IndexOf(objectToRemove));
        objectsToRecord.Remove(objectToRemove);
    }

    public void StopRecord()
    {
        isRecording = false;
    }
    public void StopRecordClip()
    {
        isPlayingClip = false;
    }
    public void PlayRecordClip()
    {
        isPlayingClip = true;
    }
}
