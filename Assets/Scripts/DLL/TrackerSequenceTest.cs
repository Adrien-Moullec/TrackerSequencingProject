using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

public class AudioDLL : MonoBehaviour
{
    [Header("TSF INIT")]
    [SerializeField, Min(0)] int channel = 0;
    [SerializeField, Min(0)] int preset_num = 0;

    [Space]
    [Header("Note info")]
    [SerializeField, Range(0, 120)] int note = 60;
    [SerializeField, Range(0f, 1f)] float velocity = 1f;
    [SerializeField, Min(0.3f)] float duration = 1f;

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
        Debug.Log("INIT: " + Audio_Init(Path.Combine(Application.streamingAssetsPath, "FluidR3_GM.sf2"), AudioSettings.outputSampleRate, 0.5f));
    }
    public void OnPlayNote() =>
        StartCoroutine(PlayNote(note));

    public void OnSetChannelPreset() =>
        Debug.Log("SET CHANNEL - " + SetChannelPreset(channel, preset_num));

    public void OnGetChannelInfo()
    {
        Debug.Log(
            "Preset on channel " + channel + ", " + GetChannelPreset(channel) + "\n" +
            "Preset Count = " + GetPresetCount() + "\n" +
            "Channel Count = " + GetChannelCount()
        );
    }
    public IEnumerator PlayNote(int playNote)
    {
        Debug.Log("PLAY NOTE = " + playNote);
        Audio_PlayNote(channel, playNote, velocity);
        yield return new WaitForSeconds(duration);
        Audio_EndNote(channel, playNote);
    }

    public void OnPlayChord(bool major = true)
    {
        StartCoroutine(PlayNote(note));
        StartCoroutine(PlayNote(note + (major ? 4 : 3)));
        StartCoroutine(PlayNote(note + 7));
    }

    public void OnDestroy()
    {
        Debug.Log("Destroy: " + Audio_Shutdown());
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(AudioDLL))]
[CanEditMultipleObjects]
public class AudioDLLEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        AudioDLL audioDLL = (AudioDLL)target;

        #region Play Notes
        GUILayout.Space(20);
        GUILayout.Label("PLAY");
        GUILayout.Space(10);
        if (GUILayout.Button("Play Note"))
        {
            audioDLL.OnPlayNote();
        }
        GUILayout.Space(10);
        if (GUILayout.Button("Play Major Chord"))
        {
            audioDLL.OnPlayChord();
        }
        if (GUILayout.Button("Play Minor Chord"))
        {
            audioDLL.OnPlayChord(false);
        }
        GUILayout.Space(20);
        #endregion

        #region Channel Settings
        GUILayout.Label("Channels");
        GUILayout.Space(10);
        if (GUILayout.Button("Set Channel Preset"))
        {
            audioDLL.OnSetChannelPreset();
        }
        if (GUILayout.Button("Get Channel Info"))
        {
            audioDLL.OnGetChannelInfo();
        }
        GUILayout.Space(20);
        #endregion

        #region Setup            
        GUILayout.Label("SETUP");
        GUILayout.Space(10);
        if (GUILayout.Button("Init"))
        {
            audioDLL.Start();
        }
        if (GUILayout.Button("Shut down"))
        {
            audioDLL.OnDestroy();
        }
        #endregion
    }
}
#endif