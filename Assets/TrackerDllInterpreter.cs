using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

public class TrackerDllInterpreter : MonoBehaviour
{
    [SerializeField] int preset = 0;
    const string DdlReference = "IRPC";

    #region Export functions
    [DllImport(DdlReference)]
    private static extern int Audio_PlayNote(int channel, int key, float velocity);

    [DllImport(DdlReference)]
    private static extern int Audio_EndNote(int channel, int note);

    [DllImport(DdlReference)]
    private static extern int SetChannelPreset(int channel, int presetNumber);
    [DllImport(DdlReference)]
    private static extern int GetChannelPreset(int channel);
    [DllImport(DdlReference)]
    private static extern int GetPresetCount();
    [DllImport(DdlReference)]
    private static extern int GetChannelCount();

    [DllImport(DdlReference)]
    private static extern int Audio_Init(string sf2Path, int sampleRate, float gain);

    [DllImport(DdlReference)]
    private static extern int Audio_Shutdown();
    #endregion


    public void Start()
    {
        Debug.Log("INIT = " + Audio_Init(Path.Combine(Application.streamingAssetsPath, "FluidR3_GM.sf2"), AudioSettings.outputSampleRate, 0.5f));
        SetChannelPreset(0, 0);
    }

    public void OnSetChannelPreset() =>
        SetChannelPreset(0, preset);

    public void PlayTestNote() => StartCoroutine(PlayNote(0, 60, 1, 1));
    public IEnumerator PlayNote(int channel, int note, float velocity, float duration)
    {
        SetChannelPreset(channel, 0);
        Debug.Log("PlayNote = " + Audio_PlayNote(channel, note, velocity));
        yield return new WaitForSeconds(duration);
        Audio_EndNote(channel, note);
    }

    public void OnDestroy()
    {
        Audio_Shutdown();
    }
}

[CustomEditor(typeof(TrackerDllInterpreter))]
[CanEditMultipleObjects]
public class TrackerDllInterpreterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        TrackerDllInterpreter trackerDllInterpreter = (TrackerDllInterpreter)target;

        GUILayout.Space(20);
        GUILayout.Label("PLAY");
        GUILayout.Space(10);
        if (GUILayout.Button("Play Note"))
        {
            trackerDllInterpreter.PlayTestNote();
        }
        if (GUILayout.Button("On Set Channel Preset"))
        {
            trackerDllInterpreter.OnSetChannelPreset();
        }

        GUILayout.Space(10);
        GUILayout.Label("SETUP");
        GUILayout.Space(10);
        if (GUILayout.Button("Init"))
        {
            trackerDllInterpreter.Start();
        }
        if (GUILayout.Button("Shut down"))
        {
            trackerDllInterpreter.OnDestroy();
        }
    }
}