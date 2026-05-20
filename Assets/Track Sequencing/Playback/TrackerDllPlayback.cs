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
        private static extern int Tracker_Shutdown();
        [DllImport(DdlReference)]
        private static extern int Tracker_PlayNote(int channel, int key, float velocity);

        [DllImport(DdlReference)]
        private static extern int Tracker_EndNote(int channel, int note);

        [DllImport(DdlReference)]
        private static extern int Tracker_SetChannelPreset(int channel, int presetNumber);
        [DllImport(DdlReference)]
        private static extern int Tracker_SetBankPreset(int channel, int bank, int presetNumber);
        [DllImport(DdlReference)]
        private static extern void Tracker_AudioRender(IntPtr buffer, int frames);

        [DllImport(DdlReference)]
        private static extern int Tracker_GetInitializedState();
        [DllImport(DdlReference)]
        private static extern int Tracker_GetChannelPreset(int channel);
        [DllImport(DdlReference)]
        private static extern int Tracker_GetPresetCount();
        #endregion

        #region Playback Variables
        public TextAsset textAsset;
        public TrackSequencer trackSequencer;
        public float interval;
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

        // For level area activation
        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player") PlayTrack();
        }

        // Play the default track loaded in serialized textasset
        public void PlayTrack()
        {
            trackSequencer = JsonReadWrite.ReadJSON(textAsset);
            playTrackEnum = PlayTrackEnum(trackSequencer);
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
        public IEnumerator PlayTrackEnum(TrackSequencer trackSequencer)
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
            trackSequencer = JsonReadWrite.ReadJSON(textAsset);
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
        public void TryPlayCommandLine(CommandLine command, int chan)
        {
            TrySetChannelBankPreset(command.instrumentSettings, chan);
            TryPlayNote(command?.PlaybackLine, chan);
        }

        /// <summary>
        /// Play a note if one is written in the data
        /// </summary>
        /// <param name="noteInfo"> note data </param>
        /// <param name="channel"> channel number </param>
        public void TryPlayNote(NoteInfo noteInfo, int channel)
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
        public void TrySetChannelBankPreset(InstrumentSettings instrumentSettings, int channel)
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
        public void TrySetMusicSettings(MusicSettings musicSettings)
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
        public IEnumerator PlayNote(int channel, int note, float velocity, float duration)
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
        public void OnDestroy() =>
            Tracker_Shutdown();

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
        [Serializable]
        private struct MusicSettingsInfo
        {
            public int bpm;
        }
    }


    [CustomEditor(typeof(TrackerDllPlayback))]
    [CanEditMultipleObjects]
    public class TrackerDllInterpreterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            try
            {
                DrawDefaultInspector();
            }
            catch { }
            TrackerDllPlayback trackerDllInterpreter = (TrackerDllPlayback)target;

            GUILayout.Space(20);
            GUILayout.Label("PLAY");
            GUILayout.Space(10);
            if (GUILayout.Button("Play Test Note"))
                trackerDllInterpreter.PlayTestNote();

            if (GUILayout.Button("Play Sequence"))
                trackerDllInterpreter.PlayTrack();

            if (GUILayout.Button("Stop"))
                trackerDllInterpreter.Stop();

            GUILayout.Space(10);
            GUILayout.Label("SETUP");
            GUILayout.Space(10);
            if (GUILayout.Button("Init"))
                trackerDllInterpreter.InitializeAudioSource();

            if (GUILayout.Button("Shut down"))
                trackerDllInterpreter.OnDestroy();

        }
    }
}