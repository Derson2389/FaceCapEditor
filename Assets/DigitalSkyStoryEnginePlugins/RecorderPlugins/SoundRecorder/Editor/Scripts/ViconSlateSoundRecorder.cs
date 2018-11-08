using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ViconSlateSoundRecorder : SlateSoundRecorder
{
    private AudioClip m_AudioClip = null;

    const int HEADER_SIZE = 44;
    const int RECORD_TIME = 600;
    const int RECORD_RATE = 44100; //录音采样率

    private static ViconSlateSoundRecorder m_Current = null;

    public static ViconSlateSoundRecorder Current
    {
        get
        {
            return m_Current;
        }
    }

    public ViconSlateSoundRecorder()
    {
        m_Current = this;
    }

    public override void Refresh()
    {

    }

    public override void Cleanup()
    {
        if (Microphone.IsRecording(null))
        {
            Microphone.End(null);
        }
    }

    public override void UpdateRecord()
    {

    }

    public override void StartRecord(float startTime)
    {
        m_AudioClip = Microphone.Start(SoundRecorderManager.Instance.Device, false, RECORD_TIME, RECORD_RATE);
        //while (!(Microphone.GetPosition(null) > 0))
        //{
        //}
    }

    public override void StopRecord(float startTime, float endTime, int? recordIdx = null)
    {
#if UNITY_EDITOR
        float time = Mathf.Min(endTime - startTime, m_AudioClip.length);
        if(m_AudioClip != null && time > 0.0f)
        {
            string fileName = DateTime.Now.ToString("MMddHHmmssfff");
            string filePath = "Assets/" + SoundRecorderManager.Instance.GetRecordSavePath() + fileName + ".wav";

            string fileDir = Application.dataPath + "/" + SoundRecorderManager.Instance.GetRecordSavePath();
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }

            if(WaveSaver.SaveWithLength(fileDir + fileName + ".wav", m_AudioClip, time))
            {
                AssetDatabase.Refresh();
            }
            
            AudioClip clip = AssetDatabase.LoadMainAssetAtPath(filePath) as AudioClip;

            //             Slate.DirectorAudioTrack audioTrack = Slate.CutsceneEditor.current.cutscene.directorGroup.tracks.Find(t => t is Slate.DirectorAudioTrack) as Slate.DirectorAudioTrack;
            //             if (audioTrack == null)
            //             {
            //                 audioTrack = Slate.CutsceneEditor.current.cutscene.directorGroup.AddTrack<Slate.DirectorAudioTrack>();
            //             }

            Slate.DirectorAudioTrack audioTrack = Slate.CutsceneEditor.current.cutscene.directorGroup.AddTrack<Slate.DirectorAudioTrack>("Audio Track " + fileName);
            audioTrack.IsAutoRecord = true;
            audioTrack.RecordIdx = recordIdx;
            Slate.ActionClips.PlayAudio audio = audioTrack.AddAction<Slate.ActionClips.PlayAudio>(time);
            audio.audioClip = clip;
            audio.startTime = startTime;
            audio.length = time;
           
        }

#endif
        Microphone.End(null);
    }

    public override void Record(float currentTime, float totalTime)
    {

    }
}
