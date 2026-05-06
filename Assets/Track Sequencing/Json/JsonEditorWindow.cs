using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace TrackSequencingTool
{
    public class JsonEditorWindow : EditorWindow
    {
        #region Initialize
        TextAsset jsonFile;
        string newJsonFileName = "";
        int beats = 4;
        private TrackSequencer sequencer = null;
        private Vector2 channelScroll;
        float channelWidth = 200;

        [MenuItem("JSON/Track Sequencer Editor")]
        private static void OpenWindow()
        {
            JsonEditorWindow wnd = GetWindow<JsonEditorWindow>();
            wnd.titleContent = new GUIContent("Track Sequencer Editor");
        }
        #endregion

        #region Data Interaction
        void ReadFromJson()
        {
            if (jsonFile == null) return;
            sequencer = JsonReadWrite.ReadJSON(jsonFile);

            #region Data checker
            CheckNull(ref sequencer);
            CheckNull(ref sequencer.channels);
            CheckNull(ref sequencer.musicSettings);
            foreach (var c in sequencer.channels)
            {
                CheckNull(ref c.CommandLines);
                CheckNull(ref c.defineChannelStart);
            }
            #endregion
        }
        void ReadToJson(TrackSequencer trackSequencer)
        {
            if (trackSequencer == null || jsonFile == null) return;
            JsonReadWrite.OutputJSON(trackSequencer, jsonFile);
        }
        void DisplayFileArea()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            jsonFile = (TextAsset)EditorGUILayout.ObjectField("JSON File", jsonFile, typeof(TextAsset), false);
            if (GUILayout.Button("Read File")) ReadFromJson();
            if (GUILayout.Button("Save Progress")) ReadToJson(sequencer);
            if (GUILayout.Button("Nullify")) sequencer = null;
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            newJsonFileName = GUILayout.TextField(newJsonFileName);
            if (GUILayout.Button("Create New File")) JsonReadWrite.OutputJSON(null, newJsonFileName);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            if (sequencer == null) return;
            beats = Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("Value"), beats), 2, 16);
        }
        #endregion

        #region Text Asset Options
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
                CheckCommandLines(TrackSequencer.GetMaxListLengthFromSequencer(sequencer) + 1);
            }

            if (GUILayout.Button("Add Channel"))
            {
                if (sequencer == null) return;
                int newLength = TrackSequencer.GetMaxListLengthFromSequencer(sequencer);
                sequencer.channels.Add(new Channel
                {
                    CommandLines = new List<CommandLine>(newLength),
                    defineChannelStart = new InstrumentSettings()
                });
                CheckCommandLines(newLength);
            }
        }
        #endregion

        #region Channel layout
        void MusicSettingsDisplay()
        {
            if (sequencer == null) return;
            int num = 1;

            CheckNull(ref sequencer.startingSettings);
            CheckNull(ref sequencer.musicSettings);

            DrawHorizontal(() => sequencer.startingSettings.EditorDraw());
            EditorGUILayout.Space(2);

            foreach (var l in sequencer.musicSettings)
            {
                if (l == null) continue;
                if (num % beats == 0) EditorGUILayout.Space(2);
                DrawHorizontal(() => l.EditorDraw(), num++.ToString());
            }
        }
        void DisplayChannel(Channel channel)
        {
            if (channel == null) return;
            int num = 1;

            CheckNull(ref channel.defineChannelStart);
            CheckNull(ref channel.CommandLines);

            DrawHorizontal(() => { channel.defineChannelStart.EditorDraw(); });
            EditorGUILayout.Space(2);

            foreach (var l in channel.CommandLines)
            {
                if (l == null) continue;
                num %= beats;
                if (num == 0) { EditorGUILayout.Space(2); num++; }

                CheckNull(ref l.instrumentSettings);
                CheckNull(ref l.PlaybackLine);
                DrawHorizontal(() => l.EditorDraw(), num++.ToString());
            }
        }
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
            EditorGUILayout.BeginHorizontal(GUILayout.Width(100));
            EditorGUILayout.LabelField(label, GUILayout.Width(35));
            action();
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #region Variable Settings
        void CheckNull<T>(ref T variable) where T : class, new()
        {
            if (variable == null) variable = new T();
        }
        void CheckNull<T>(ref List<T> list) where T : class, new()
        {
            if (list == null) list = new List<T>();
        }
        void NormalizeListLength<T>(ref List<T> list, int newLength, T paddingValue = default)
        {
            CheckNull(ref list);

            if (list.Count > newLength)
                list.RemoveRange(newLength, list.Count - newLength);
            else if (list.Count < newLength)
                list.AddRange(Enumerable.Repeat(paddingValue, newLength - list.Count));
        }
        void CheckCommandLines(int newLength)
        {
            foreach (var channel in sequencer.channels)
            {
                CheckNull(ref channel.CommandLines);
                NormalizeListLength(ref channel.CommandLines, newLength);
            }
            CheckNull(ref sequencer.musicSettings);
            NormalizeListLength(ref sequencer.musicSettings, newLength);
            ReadToJson(sequencer);
        }
        #endregion
    }
}