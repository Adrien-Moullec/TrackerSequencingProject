using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace TrackSequencingTool
{
    public class TrackerDllPlayback : MonoBehaviour
    {
        #region Export functions
        const string DdlReference = "IRPC";
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

        #region Playback Variables
        public TextAsset textAsset;
        public TrackSequencer trackSequencer;
        public float interval;
        bool isPlaying = false;
        #endregion

        public void PlayTrack() =>
            StartCoroutine(PlayTrackEnum());
        public void SetTempo(int speed)
        {
            interval = MusicFunctions.BeatTime(speed);
            Debug.Log(interval);
        }
        public IEnumerator PlayTrackEnum()
        {
            if (isPlaying) yield break;
            isPlaying = true;
            int totalCommandLines = TrackSequencer.GetMaxListLengthFromSequencer(trackSequencer);
            int preset;
            int intVal;

            for (int i = 0; i < trackSequencer.channels.Count; i++) //trackSequencer.channels.Count; i++)
                if (int.TryParse(trackSequencer.channels[i].defineChannelStart.instrument, out preset))
                    OnSetChannelPreset(i, preset);

            if (int.TryParse(trackSequencer.startingSettings.tempo, out intVal)) SetTempo(intVal);
            else SetTempo(60);

            CommandLine channelLine;
            NoteInfo noteInfo;
            InstrumentSettings instrSettings;
            MusicSettings musicSettings;
            for (int lineInt = 0; lineInt < totalCommandLines; lineInt++) //totalCommandLines
            {
                musicSettings = trackSequencer.musicSettings[lineInt];
                if (int.TryParse(musicSettings.tempo, out intVal))
                    SetTempo(intVal);

                //Debug.Log("command line " + lineInt);
                for (int chan = 0; chan < trackSequencer.channels.Count; chan++)
                {
                    channelLine = trackSequencer.channels[chan].CommandLines[lineInt];
                    noteInfo = channelLine.PlaybackLine;
                    instrSettings = channelLine.instrumentSettings;

                    // Set Channel Preset
                    if (int.TryParse(instrSettings.instrument, out preset))
                        OnSetChannelPreset(chan, preset);

                    // Play note
                    if (channelLine == null || noteInfo.key == "") continue;

                    StartCoroutine(
                        PlayNote(
                            chan,
                            int.Parse(noteInfo.key),
                            float.Parse(noteInfo.velocity),
                            float.Parse(noteInfo.duration)
                        )
                    );
                }
                yield return new WaitForSeconds(interval);
            }
            isPlaying = false;
        }

        public void Start()
        {
            Debug.Log("INIT = " + Audio_Init(Path.Combine(Application.streamingAssetsPath, "FluidR3_GM.sf2"), AudioSettings.outputSampleRate, 0.5f));
            trackSequencer = JsonReadWrite.ReadJSON(textAsset);
        }

        public void OnSetChannelPreset(int channel, int presets) =>
            SetChannelPreset(channel, presets);

        public void PlayTestNote() => StartCoroutine(PlayNote(0, 60, 1, 1));
        public IEnumerator PlayNote(int channel, int note, float velocity, float duration)
        {
            Audio_PlayNote(channel, note, velocity);
            yield return new WaitForSeconds(duration);
            Audio_EndNote(channel, note);
        }

        public void OnDestroy()
        {
            Audio_Shutdown();
        }
    }

    [CustomEditor(typeof(TrackerDllPlayback))]
    [CanEditMultipleObjects]
    public class TrackerDllInterpreterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            TrackerDllPlayback trackerDllInterpreter = (TrackerDllPlayback)target;

            GUILayout.Space(20);
            GUILayout.Label("PLAY");
            GUILayout.Space(10);
            if (GUILayout.Button("Play Test Note"))
                trackerDllInterpreter.PlayTestNote();

            if (GUILayout.Button("Play Sequence"))
                trackerDllInterpreter.PlayTrack();

            if (GUILayout.Button("On Set Channel Preset"))
                trackerDllInterpreter.OnSetChannelPreset(0, 0);

            GUILayout.Space(10);
            GUILayout.Label("SETUP");
            GUILayout.Space(10);
            if (GUILayout.Button("Init"))
                trackerDllInterpreter.Start();

            if (GUILayout.Button("Shut down"))
                trackerDllInterpreter.OnDestroy();

        }
    }
}