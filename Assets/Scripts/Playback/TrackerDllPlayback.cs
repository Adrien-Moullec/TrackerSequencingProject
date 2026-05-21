using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Collections.Generic;

namespace TrackSequencingTool
{
    /// <summary>
    /// Tracker sequencer playback script using DLL file for SF2 file.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class TrackerDllPlayback : MonoBehaviour
    {
        /// <summary>
        /// All Dll C scripts that utilise SF2 files and TinySoundFont library
        /// </summary>
        #region Export functions
        const string DdlReference = "IRPC";
        [DllImport(DdlReference)]
        private static extern int Tracker_Initialize(string sf2Path, int sampleRate, float gain);

        [DllImport(DdlReference)]
        protected static extern int Tracker_Shutdown();
        [DllImport(DdlReference)]
        protected static extern int Tracker_PlayNote(int channel, int key, float velocity);

        [DllImport(DdlReference)]
        protected static extern int Tracker_EndNote(int channel, int note);

        [DllImport(DdlReference)]
        protected static extern int Tracker_SetChannelPreset(int channel, int presetNumber);
        [DllImport(DdlReference)]
        protected static extern int Tracker_SetBankPreset(int channel, int bank, int presetNumber);
        [DllImport(DdlReference)]
        protected static extern void Tracker_AudioRender(IntPtr buffer, int frames);

        [DllImport(DdlReference)]
        protected static extern int Tracker_GetInitializedState();
        [DllImport(DdlReference)]
        protected static extern int Tracker_GetChannelPreset(int channel);
        [DllImport(DdlReference)]
        protected static extern int Tracker_GetPresetCount();
        #endregion

        #region Playback Variables
        [Header("Tracker Sequence Json File")]
        [SerializeField] protected TextAsset textAsset;
        protected TrackSequencer sequencer;
        #endregion

        #region Private variables
        float interval;
        bool isPlaying = false;
        float[] nativeBuffer;
        AudioSource audioSource;
        IEnumerator playTrackEnum;
        List<Channel> playableChannels;
        MusicSettingsInfo musicSettingsInfo;
        Dictionary<int, CommandLineInfo> channelInfo = new();
        CommandLineInfo commandLineInfo;
        int preset;
        int bank;
        float floatVar;
        #endregion

        // On Begin
        public void Start() => InitializeAudioSource();

        // Play the default track loaded in serialized textasset
        public void PlayTrack()
        {
            sequencer = JsonReadWrite.ReadJSON(textAsset);
            playTrackEnum = PlayTrackEnum(sequencer);
            StartCoroutine(playTrackEnum);
        }

        // Play custom track sequencer from TrackSequencer class
        public void PlayTrack(TrackSequencer trackSequencer)
        {
            playTrackEnum = PlayTrackEnum(trackSequencer);
            StartCoroutine(playTrackEnum);
        }
        // Play custom track sequencer from textasset
        public void PlayTrack(TextAsset textAsset)
        {
            playTrackEnum = PlayTrackEnum(JsonReadWrite.ReadJSON(textAsset));
            StartCoroutine(playTrackEnum);
        }

        /// Play through track sequencer line by line
        private IEnumerator PlayTrackEnum(TrackSequencer trackSequencer)
        {

            /// Initialize if not already done.
            if (Tracker_GetInitializedState() == 0) InitializeAudioSource();

            /// If already playing a track, leave track playback
            if (isPlaying) yield break;

            /// If there are no channels, leave track playback
            playableChannels = new();
            foreach (var n in trackSequencer.channels)
                if (n.playback)
                {
                    playableChannels.Add(n);
                }
            if (playableChannels.Count < 1) yield break;

            /// Setup base information for the track sequencer
            isPlaying = true;
            int totalCommandLines = TrackSequencer.GetMaxListLengthFromSequencer(trackSequencer);
            interval = MusicFunctions.BeatTime(60);
            TrySetMusicSettings(trackSequencer.startingSettings);
            channelInfo.Clear();

            /// Setup each channel for playback - bpm, base duration/note velocity
            for (int i = 0; i < playableChannels.Count; i++)
            {
                musicSettingsInfo.bpm = 300;
                int.TryParse(trackSequencer.startingSettings.tempo, out musicSettingsInfo.bpm);
                commandLineInfo = new(1, 1);
                if (channelInfo.ContainsKey(i)) channelInfo[i] = commandLineInfo;
                else channelInfo.Add(i, commandLineInfo);
                TrySetChannelBankPreset(playableChannels[i].defineChannelStart, i);
            }

            /// Playback music
            for (int lineInt = 0; lineInt < totalCommandLines; lineInt++)
            {
                TrySetMusicSettings(trackSequencer.musicSettings[lineInt]);

                for (int chan = 0; chan < playableChannels.Count; chan++)
                    TryPlayCommandLine(playableChannels[chan].CommandLines[lineInt], chan);

                yield return new WaitForSeconds(interval);
            }
            isPlaying = false;
        }


        /// <summary>
        /// Setup AudioSource component, read json file info and setup sf2 file for playback
        /// </summary>
        public void InitializeAudioSource()
        {
            isPlaying = false;
            Tracker_Initialize(Path.Combine(Application.streamingAssetsPath, "FluidR3_GM.sf2"), AudioSettings.outputSampleRate, 0.5f);
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = AudioClip.Create("Test SF2 playback file", AudioSettings.outputSampleRate, 2, AudioSettings.outputSampleRate, true);
            audioSource.Play();
            if (textAsset != null) sequencer = JsonReadWrite.ReadJSON(textAsset);
        }

        /// <summary>
        /// Audio data handler
        /// </summary>
        /// <param name="data"></param>
        /// <param name="channels"></param>
        void OnAudioFilterRead(float[] data, int channels)
        {
            int frames = data.Length / channels;

            if (nativeBuffer == null || nativeBuffer.Length != data.Length)
                nativeBuffer = new float[data.Length];

            GCHandle handle = GCHandle.Alloc(nativeBuffer, GCHandleType.Pinned);

            try
            {
                Tracker_AudioRender(handle.AddrOfPinnedObject(), frames);
                Array.Copy(nativeBuffer, data, data.Length);
            }
            finally
            {
                handle.Free();
            }
        }

        /// <summary>
        /// Command line playback
        /// </summary>
        /// <param name="command"> command line info for playing a note </param>
        /// <param name="chan"> channel number </param>
        void TryPlayCommandLine(CommandLine command, int chan)
        {
            TrySetChannelBankPreset(command.instrumentSettings, chan);
            TryPlayNote(command?.PlaybackLine, chan);
        }

        /// <summary>
        /// Play a note if one is written in the data
        /// </summary>
        /// <param name="noteInfo"> note data </param>
        /// <param name="channel"> channel number </param>
        void TryPlayNote(NoteInfo noteInfo, int channel)
        {
            if (noteInfo?.key != "")
                StartCoroutine(
                    PlayNote(
                        channel,
                        int.Parse(noteInfo.key),
                        channelInfo[channel].velocity,
                        channelInfo[channel].duration
                    )
                );
        }

        /// <summary>
        /// Read instrument settings and set new parameters if any input data
        /// </summary>
        /// <param name="instrumentSettings"> channel instrument settings </param>
        /// <param name="channel"> channel ID </param>
        void TrySetChannelBankPreset(InstrumentSettings instrumentSettings, int channel)
        {
            if (instrumentSettings == null) return;

            /// Try set velocity and duration of notes
            if (float.TryParse(instrumentSettings.velocity, out floatVar))
                channelInfo[channel].velocity = floatVar;
            if (float.TryParse(instrumentSettings.duration, out floatVar))
                channelInfo[channel].duration = MusicFunctions.BeatTime(musicSettingsInfo.bpm) * floatVar;

            /// Set instrument and bank/library of channel
            if (int.TryParse(instrumentSettings.instrument, out preset))
                if (int.TryParse(instrumentSettings.bank, out bank))
                {
                    Debug.Log("Bank set " + bank + "," + preset);
                    Tracker_SetBankPreset(channel, bank, preset);
                }
                else
                {
                    Debug.Log("Channel set " + bank + "," + preset);
                    Tracker_SetBankPreset(channel, 0, preset);
                }
        }

        /// <summary>
        /// Set overall track settings across channels
        /// </summary>
        /// <param name="musicSettings"> Music data </param>
        void TrySetMusicSettings(MusicSettings musicSettings)
        {
            if (int.TryParse(musicSettings.tempo, out int speed))
                interval = MusicFunctions.BeatTime(speed);
        }

        /// <summary>
        /// Play a note for a duration
        /// </summary>
        /// <param name="channel"> channel ID </param>
        /// <param name="note"> note ID </param>
        /// <param name="velocity"> note velocity </param>
        /// <param name="duration"> note duration </param>
        /// <returns></returns>
        IEnumerator PlayNote(int channel, int note, float velocity, float duration)
        {
            Tracker_PlayNote(channel, note, velocity);
            yield return new WaitForSeconds(duration);
            Tracker_EndNote(channel, note);
        }

        /// <summary>
        /// End current track-sequencer playback
        /// </summary>
        public void Stop()
        {
            StopCoroutine(playTrackEnum);
            isPlaying = false;
        }

        /// <summary>
        /// Test-play a middle-C note
        /// </summary>
        public void PlayTestNote() => StartCoroutine(PlayNote(0, 60, 1, 1));

        /// <summary>
        /// Shut down audio tsf file
        /// </summary>
        public void OnDestroy() =>
            Tracker_Shutdown();

        /// <summary>
        /// Information from last line for each of duration and velocity
        /// </summary>
        [Serializable]
        private class CommandLineInfo
        {
            public float duration;
            public float velocity;
            public CommandLineInfo(float duration, float velocity)
            {
                this.duration = duration;
                this.velocity = velocity;
            }
        }

        /// <summary>
        /// Information from last music setting line containing bpm
        /// </summary>
        [Serializable]
        private struct MusicSettingsInfo
        {
            public int bpm;
        }
    }

    /// <summary>
    /// Editor window for default tracker Dll playback class.
    /// Controls playback, loading and stop logic
    /// </summary>
    [CustomEditor(typeof(TrackerDllPlayback))]
    [CanEditMultipleObjects]
    public class TrackerDllPlaybackEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            /// This try/catch currently avoids error on init load
            try
            {
                DrawDefaultInspector();
            }
            catch { }
            DrawDefaultTrackerDllPlayback((TrackerDllPlayback)target);
        }
        public static void DrawDefaultTrackerDllPlayback(TrackerDllPlayback target)
        {
            GUILayout.Space(20);
            GUILayout.Label("PLAY");
            GUILayout.Space(10);
            if (GUILayout.Button("Play Test Note"))
                target.PlayTestNote();

            if (GUILayout.Button("Play Sequence"))
                target.PlayTrack();

            if (GUILayout.Button("Stop"))
                target.Stop();

            GUILayout.Space(10);
            GUILayout.Label("SETUP");
            GUILayout.Space(10);
            if (GUILayout.Button("Init"))
                target.InitializeAudioSource();

            if (GUILayout.Button("Shut down"))
                target.OnDestroy();
        }
    }
}