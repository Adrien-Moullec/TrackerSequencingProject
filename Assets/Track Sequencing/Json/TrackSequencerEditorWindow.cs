using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace TrackSequencingTool
{
    public class TrackSequencerEditorWindow : EditorWindow
    {
        #region Initialize
        TextAsset jsonFile;
        string newJsonFileName = "";
        int beats = 4;
        private TrackSequencer sequencer = null;
        private Vector2 channelScroll;
        float channelWidth = 200;
        public static string[] noteTranslation = new string[]
        {
            "C",
            "C#",
            "D",
            "D#",
            "E",
            "F",
            "F#",
            "G",
            "G#",
            "A",
            "A#",
            "B"
        };

        [MenuItem("JSON/Track Sequencer Editor")]
        private static void OpenWindow()
        {
            TrackSequencerEditorWindow wnd = GetWindow<TrackSequencerEditorWindow>();
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
            int num = 0;

            CheckNull(ref sequencer.startingSettings);
            CheckNull(ref sequencer.musicSettings);
            DrawHorizontal(() => sequencer.startingSettings.EditorDraw(), -1, "-", false);

            var cd = GUI.color;
            GUI.color = Color.black;
            GUILayout.Label("", GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.Height(10));
            GUI.color = cd;

            foreach (var l in sequencer.musicSettings)
            {
                if (l == null) continue;
                if (num % (beats) == 0)
                {
                    var c = GUI.color;
                    GUI.color = Color.black;
                    GUILayout.Label("", GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.Height(10));
                    GUI.color = c;
                }
                DrawHorizontal(() => l.EditorDraw(), num, (num++ + 1).ToString());
            }
        }
        void DisplayChannel(Channel channel)
        {
            if (channel == null) return;
            int num = 0;

            CheckNull(ref channel.defineChannelStart);
            CheckNull(ref channel.CommandLines);

            DrawHorizontal(() => { channel.defineChannelStart.EditorDraw(); }, -1, "-", false);
            var cd = GUI.color;
            GUI.color = Color.black;
            GUILayout.Label("", GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.Height(10));
            GUI.color = cd;

            foreach (var l in channel.CommandLines)
            {
                if (l == null)
                {
                    num++;
                    continue;
                }

                if (num % beats == 0)
                {
                    var c = GUI.color;
                    GUI.color = Color.black;
                    GUILayout.Label("", GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.Height(10));
                    GUI.color = c;
                }

                CheckNull(ref l.instrumentSettings);
                CheckNull(ref l.PlaybackLine);

                DrawHorizontal(() => l.EditorDraw(), num, (num + 1).ToString());

                num++;
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

        void DrawHorizontal(Action action, int lineIndex, string label = "-", bool drawButtons = true)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(100));
            EditorGUILayout.LabelField(label, GUILayout.Width(25));

            if (drawButtons)
            {
                GUI.enabled = lineIndex > 0;
                if (GUILayout.Button("▲", GUILayout.Width(25)))
                    MoveLine(lineIndex, lineIndex - 1);
                GUI.enabled = true;

                if (GUILayout.Button("▼", GUILayout.Width(25)))
                    MoveLine(lineIndex, lineIndex + 1);

                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    DeleteLine(lineIndex);
                    GUI.backgroundColor = Color.white;

                    EditorGUILayout.EndHorizontal();
                    return;
                }
                GUI.backgroundColor = Color.white;
            }
            action();
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;
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

        #region Line Operations
        void DeleteLine(int index)
        {
            if (sequencer == null)
                return;

            // Remove music setting
            if (sequencer.musicSettings != null &&
                index >= 0 &&
                index < sequencer.musicSettings.Count)
            {
                sequencer.musicSettings.RemoveAt(index);
            }

            // Remove command line from every channel
            foreach (var channel in sequencer.channels)
            {
                if (channel?.CommandLines == null)
                    continue;

                if (index >= 0 && index < channel.CommandLines.Count)
                    channel.CommandLines.RemoveAt(index);
            }

            ReadToJson(sequencer);
        }

        void MoveLine(int from, int to)
        {
            if (sequencer == null)
                return;

            int max = TrackSequencer.GetMaxListLengthFromSequencer(sequencer);

            if (from < 0 || from >= max)
                return;

            if (to < 0 || to >= max)
                return;

            // Move music settings
            if (sequencer.musicSettings != null &&
                from < sequencer.musicSettings.Count &&
                to < sequencer.musicSettings.Count)
            {
                var item = sequencer.musicSettings[from];
                sequencer.musicSettings.RemoveAt(from);
                sequencer.musicSettings.Insert(to, item);
            }

            // Move every channel command line
            foreach (var channel in sequencer.channels)
            {
                if (channel?.CommandLines == null)
                    continue;

                if (from >= channel.CommandLines.Count ||
                    to >= channel.CommandLines.Count)
                    continue;

                var item = channel.CommandLines[from];
                channel.CommandLines.RemoveAt(from);
                channel.CommandLines.Insert(to, item);
            }

            ReadToJson(sequencer);
        }
        #endregion
    }
}