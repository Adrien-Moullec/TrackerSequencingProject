using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace TrackSequencingTool
{
    public class JsonEditorWindow : EditorWindow
    {
        TextAsset jsonFile;
        private TrackSequencer sequencer = null;
        private Vector2 channelScroll;
        [SerializeField] float channelWidth = 200;

        [MenuItem("JSON/Track Sequencer Editor")]
        private static void OpenWindow()
        {
            JsonEditorWindow wnd = GetWindow<JsonEditorWindow>();
            wnd.titleContent = new GUIContent("Track Sequencer Editor");
        }

        #region Data Interaction
        void ReadFromJson()
        {
            if (jsonFile == null) return;
            sequencer = JsonUtility.FromJson<TrackSequencer>(jsonFile.text);

            #region Data checker
            if (sequencer == null)
                sequencer = new TrackSequencer();

            if (sequencer.channels == null)
                sequencer.channels = new List<Channel>();

            if (sequencer.musicSettings == null)
                sequencer.musicSettings = new List<MusicSettings>();

            foreach (var c in sequencer.channels)
            {
                if (c.CommandLines == null)
                    c.CommandLines = new List<MusicCommand>();

                if (c.defineChannelStart == null)
                    c.defineChannelStart = new ChannelSettings();
            }
            #endregion
        }

        void ReadToJson(TrackSequencer trackSequencer)
        {
            if (trackSequencer == null || jsonFile == null) return;
            JsonEditor.OutputJSON(trackSequencer, jsonFile);
        }

        void DisplayFileArea()
        {
            jsonFile = (TextAsset)EditorGUILayout.ObjectField("JSON File", jsonFile, typeof(TextAsset), false);

            if (GUILayout.Button("Read File"))
                ReadFromJson();

            if (GUILayout.Button("Save Progress"))
                ReadToJson(sequencer);

            if (GUILayout.Button("Nullify"))
                sequencer = null;

            if (sequencer == null)
                return;

            GUILayout.Label("Sequencer Settings");

            if (GUILayout.Button("Clear"))
            {
                sequencer = new TrackSequencer();
                ReadToJson(sequencer);
                ReadFromJson();
            }
        }
        #endregion

        #region GUI Layout
        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            DisplayFileArea();
            SequencerSettings();
            EditorGUILayout.EndVertical();

            if (sequencer == null)
                return;

            #region Channel Layout
            channelScroll = EditorGUILayout.BeginScrollView(channelScroll);
            EditorGUILayout.BeginHorizontal();
            DisplayVerticalData(MusicSettingsDisplay, "Music Settings");
            if (sequencer.channels != null)
            {
                int channelNum = 0;
                foreach (var channel in sequencer.channels)
                    if (channel != null)
                        DisplayVerticalData(() => DisplayChannel(channel), "Channel " + channelNum++.ToString());
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
            #endregion
        }

        void SequencerSettings()
        {
            if (sequencer == null) return;

            CheckNull(ref sequencer.channels);
            CheckNull(ref sequencer.musicSettings);

            if (GUILayout.Button("Add command"))
            {
                if (sequencer == null) return;
                ReadToJson(sequencer);
                int newListLength = GetMaxListLengthFromSequencer(sequencer) + 1;

                foreach (var channel in sequencer.channels)
                {
                    CheckNull(ref channel.CommandLines);
                    NormalizeListLength(ref channel.CommandLines, newListLength);
                }
                NormalizeListLength(ref sequencer.musicSettings, newListLength);
            }

            if (GUILayout.Button("Add Channel"))
            {
                if (sequencer == null) return;
                ReadToJson(sequencer);
                int newLength = GetMaxListLengthFromSequencer(sequencer);
                sequencer.channels.Add(new Channel
                {
                    CommandLines = new List<MusicCommand>(newLength),
                    defineChannelStart = new ChannelSettings()
                });

                foreach (var channel in sequencer.channels)
                {
                    CheckNull(ref channel.CommandLines);
                    NormalizeListLength(ref channel.CommandLines, newLength);
                }
                CheckNull(ref sequencer.musicSettings);
                NormalizeListLength(ref sequencer.musicSettings, newLength);
            }
        }

        void MusicSettingsDisplay()
        {
            if (sequencer == null) return;
            int num = 0;

            CheckNull(ref sequencer.startingSettings);
            CheckNull(ref sequencer.musicSettings);

            DrawHorizontal(() => sequencer.startingSettings.EditorDraw());
            foreach (var l in sequencer.musicSettings)
            {
                if (l == null) continue;
                DrawHorizontal(() => l.EditorDraw(), num++.ToString()); //
            }
        }

        void DisplayChannel(Channel channel)
        {
            if (channel == null) return;
            int num = 0;

            CheckNull(ref channel.defineChannelStart);
            CheckNull(ref channel.CommandLines);

            DrawHorizontal(() => { channel.defineChannelStart.EditorDraw(); });

            foreach (var l in channel.CommandLines)
            {
                if (l == null) continue;

                CheckNull(ref l.channelSettings);
                CheckNull(ref l.PlaybackLine);
                DrawHorizontal(() => l.EditorDraw(num++.ToString()), num.ToString());
            }
        }
        #endregion

        #region Layout Structures
        void DisplayVerticalData(Action action, string channelName)
        {
            GUI.backgroundColor = Color.cyan;
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(channelWidth));

            EditorGUILayout.LabelField(channelName);
            action?.Invoke();

            EditorGUILayout.EndVertical();
            GUI.backgroundColor = Color.white;
        }
        void DrawHorizontal(Action action, string label = "-")
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(35));
            action();
            EditorGUILayout.EndHorizontal();
        }
        void CheckNull<T>(ref T variable) where T : class, new()
        {
            if (variable == null) variable = new T();
        }
        void CheckNull<T>(ref List<T> list) where T : class, new()
        {
            if (list == null) list = new List<T>();
        }
        #endregion

        #region List Settings
        int GetMaxListLengthFromSequencer(TrackSequencer t)
        {
            if (t == null || t?.channels == null)
                return 0;
            int maxChannel = 0;

            foreach (var c in t.channels)
                if (c?.CommandLines != null)
                    maxChannel = Mathf.Max(maxChannel, c.CommandLines.Count);

            return Mathf.Max(maxChannel, t.musicSettings?.Count ?? 0);
        }

        void NormalizeListLength<T>(ref List<T> list, int newLength, T paddingValue = default)
        {
            CheckNull(ref list);

            if (list.Count > newLength)
                list.RemoveRange(newLength, list.Count - newLength);
            else if (list.Count < newLength)
                list.AddRange(Enumerable.Repeat(paddingValue, newLength - list.Count));
        }
        #endregion
    }
}