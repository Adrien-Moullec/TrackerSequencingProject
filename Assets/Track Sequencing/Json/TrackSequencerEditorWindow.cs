using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace TrackSequencingTool
{
    using TEF = TrackerEditorFunctions;
    using TDF = TrackerDataFunctions;
    using JRW = JsonReadWrite;

    /// <summary>
    /// Editor window to display JSON file data for track sequencer
    /// </summary>
    public class TrackSequencerEditorWindow : EditorWindow
    {
        #region Variables

        // Json
        TextAsset jsonFile;
        string newJsonFileName = "";

        // Channel settings
        private TrackSequencer sequencer = null;
        private Vector2 channelScroll;
        private float channelWidth = 220;
        int beats = 4;

        // Copy/Paste
        private SequencerSelection selection = new();
        private SequencerClipboard clipboard = new();

        #endregion

        #region Initialize

        /// <summary>
        /// Open track sequencer window
        /// </summary>
        [MenuItem("JSON/Track Sequencer Editor")]
        private static void OpenWindow()
        {
            TrackSequencerEditorWindow wnd = GetWindow<TrackSequencerEditorWindow>();
            wnd.titleContent = new GUIContent("Track Sequencer Editor");
        }

        #endregion

        #region GUI
        /// <summary>
        /// Draw whole window
        /// </summary>
        private void OnGUI()
        {
            /// Handle Keyboard shortcuts
            TEF.KeyboardInputs(
                // Copy
                () => { clipboard.Copy(sequencer, selection); Repaint(); },

                // Paste
                () => { TDF.Paste(sequencer, clipboard, selection, jsonFile); Repaint(); },

                // Undo
                () => { TDF.UndoAction(sequencer, clipboard, jsonFile); Repaint(); },

                // Redo
                () => { TDF.RedoAction(sequencer, clipboard, jsonFile); Repaint(); },

                // Clear Selection
                () => { selection.ClearSelectedRows(sequencer); Repaint(); },

                // Delete
                () => { selection.Clear(); Repaint(); }
            );

            /// File handling area of editorwindow
            EditorGUILayout.BeginVertical();
            DisplayFileArea();
            if (jsonFile != null)
            {
                SequencerSettings();
            }
            else
            {
                EditorGUILayout.EndVertical();
                return;
            }
            EditorGUILayout.EndVertical();

            /// Scroll area of the json file data interface
            channelScroll = EditorGUILayout.BeginScrollView(channelScroll);
            EditorGUILayout.BeginHorizontal();

            /// Display the data that handles overall track settings
            DisplayVerticalDataOutline(MusicSettingsDisplay, "Music Settings");

            if (sequencer.channels != null)
            {
                int channelNum = 0;
                foreach (var channel in sequencer.channels)
                {
                    if (channel == null)
                        continue;

                    /// Display list of editable command lines
                    DisplayVerticalDataOutline(
                        () => DisplayChannel(channel),
                        "Channel " + channelNum
                    );
                    channelNum++;
                }
            }

            EditorGUILayout.EndHorizontal();
            ColourUI(() =>
            {
                /// Delete Last line of file across all channels
                if (GUILayout.Button("X"))
                {
                    TDF.SaveUndoState(sequencer, clipboard);
                    sequencer.musicSettings.RemoveAt(sequencer.musicSettings.Count - 1);
                    foreach (var c in sequencer.channels)
                        c.CommandLines.RemoveAt(c.CommandLines.Count - 1);
                }
            }, Color.red);
            EditorGUILayout.EndScrollView();
        }
        #endregion


        #region Top UI
        /// <summary>
        /// Top part of editor window for json file handling and settings
        /// </summary>
        void DisplayFileArea()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            EditorGUI.BeginChangeCheck();
            /// Json object field
            jsonFile = (TextAsset)EditorGUILayout.ObjectField(
                    "JSON File",
                    jsonFile,
                    typeof(TextAsset),
                    false
                );
            if (EditorGUI.EndChangeCheck())
            {
                JRW.ReadJSON(jsonFile, ref sequencer);
                Repaint();
            }


            /// If jsonFile is set, display Save option
            if (jsonFile != null)
            {
                if (GUILayout.Button("Save Progress"))
                    JRW.OutputJSON(sequencer, jsonFile);
            }
            GUILayout.EndVertical();


            // New file options to right of editor window
            GUILayout.BeginVertical();
            newJsonFileName = GUILayout.TextField(newJsonFileName);
            if (GUILayout.Button("Create New File"))
                JRW.OutputJSON(
                    null,
                    newJsonFileName
                );
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            if (sequencer == null) return;
            beats = Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("Beats"), beats), 2, 16);
        }

        /// <summary>
        /// Channel and command line settings allow addition
        /// </summary>
        void SequencerSettings()
        {
            if (sequencer == null)
                return;
            TDF.IsNull(ref sequencer.channels);
            TDF.IsNull(ref sequencer.musicSettings);

            /// Add command line to each channel
            if (GUILayout.Button("Add Command"))
            {
                TDF.SaveUndoState(sequencer, clipboard);
                Repaint();
                int newLength = TrackSequencer.GetMaxListLengthFromSequencer(sequencer) + 1;
                TDF.RefreshCommandLines(sequencer, newLength);
                JRW.OutputJSON(sequencer, jsonFile);
            }

            /// Add a new channel
            if (GUILayout.Button("Add Channel"))
            {
                TDF.SaveUndoState(sequencer, clipboard);
                Repaint();
                int newLength = TrackSequencer.GetMaxListLengthFromSequencer(sequencer);
                sequencer.channels.Add(
                    new Channel
                    {
                        CommandLines = new List<CommandLine>(newLength),
                        defineChannelStart = new InstrumentSettings()
                    }
                );
                TDF.RefreshCommandLines(sequencer, newLength);
                JRW.OutputJSON(sequencer, jsonFile);
            }
        }

        #endregion

        #region Drawing
        /// <summary>
        /// Display entirety of music settings across the track sequencer
        /// </summary>
        void MusicSettingsDisplay()
        {
            if (sequencer == null) return;
            int num = 0;
            TDF.IsNull(ref sequencer.startingSettings);

            /// Initial settings
            DrawHeaderRow(() => sequencer.startingSettings.EditorDraw());
            DrawBeatSeparator();

            /// Line by line settings
            foreach (var line in sequencer.musicSettings)
            {
                if (num % beats == 0)
                    DrawBeatSeparator();
                DrawSelectableRow(
                    (num + 1).ToString(),
                    () => line.EditorDraw(),
                    selection.IsMusicSelected(num),
                    () => selection.SelectMusic(num, Event.current.shift)
                );
                num++;
            }
        }


        /// <summary>
        /// Display entire channel in json file track sequencer
        /// </summary>
        /// <param name="channel"> Data for single-instrument playback </param>
        void DisplayChannel(Channel channel)
        {
            if (channel == null)
                return;

            int num = 0;
            TDF.IsNull(ref channel.CommandLines);
            TDF.IsNull(ref channel.defineChannelStart);

            /// Display starting settings and channel deletion option
            DrawHeaderRow(() =>
            {
                channel.EditorDraw();
                ColourUI(() =>
                {
                    if (GUILayout.Button("Remove"))
                    {
                        TDF.SaveUndoState(sequencer, clipboard);
                        sequencer.channels.Remove(channel);
                    }
                }, Color.red);
            });
            DrawBeatSeparator();

            /// Display all the command lines / music notes in a channel
            foreach (var line in channel.CommandLines)
            {
                if (num % beats == 0)
                    DrawBeatSeparator();

                DrawSelectableRow(
                    (num + 1).ToString(),
                    () => line?.EditorDraw(),
                    selection.IsChannelSelected(channel, num),
                    () => selection.SelectChannel(
                            channel,
                            num,
                            Event.current.shift
                        )
                );
                num++;
            }
        }

        /// <summary>
        /// Default horizontal layout template for data to be used with keyboard shortcuts
        /// </summary>
        /// <param name="label"> Layout lable name </param>
        /// <param name="drawAction"> The layout contents </param>
        /// <param name="selected"> Whether the contents are currently selected </param>
        /// <param name="onClick"> When the contents are clicked </param>
        void DrawSelectableRow(string label, Action drawAction, bool selected, Action onClick)
        {
            Event e = Event.current;
            Rect rowRect = EditorGUILayout.BeginHorizontal();

            /// Selected and clicked events
            if (selected) EditorGUI.DrawRect(rowRect, new Color(0f, 1f, 1f, 0.15f));
            if (e.type == EventType.MouseDown && e.button == 0 && rowRect.Contains(e.mousePosition) && GUIUtility.hotControl == 0)
            {
                onClick?.Invoke();
                Repaint();
            }

            GUILayout.Label(label, GUILayout.Width(25));

            /// Any changes to the layout will affect the save data
            EditorGUI.BeginChangeCheck();
            drawAction?.Invoke();
            if (EditorGUI.EndChangeCheck())
            {
                TDF.SaveUndoState(sequencer, clipboard);
                JRW.OutputJSON(sequencer, jsonFile);
                Repaint();
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Default layout for header of each column/channel
        /// </summary>
        /// <param name="drawAction"> The contents of the header </param>
        void DrawHeaderRow(Action drawAction)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("-", GUILayout.Width(25));
            drawAction?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// A simple bar to seperate content
        /// </summary>
        void DrawBeatSeparator() =>
            ColourUI(() =>
            {
                GUILayout.Label(
                    "",
                    GUI.skin.box,
                    GUILayout.ExpandWidth(true),
                    GUILayout.Height(8)
                );
            }, Color.black);

        /// <summary>
        /// Layout for column/channel information
        /// </summary>
        /// <param name="action"> draw contents for the column </param>
        /// <param name="channelName"> Display name of the column </param>
        void DisplayVerticalDataOutline(Action action, string channelName) =>
            ColourUI(() =>
            {
                GUI.backgroundColor = Color.cyan;
                EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(channelWidth));
                EditorGUILayout.LabelField(channelName);
                action?.Invoke();
                EditorGUILayout.EndVertical();
                GUI.backgroundColor = Color.white;
            }, Color.black);

        /// <summary>
        /// Draw only a specific layout a certain colour, return to old colour at the end.
        /// </summary>
        /// <param name="action"> Contents to be colour-highlighted </param>
        /// <param name="color"> Colour for the layout </param>
        public static void ColourUI(Action action, Color color)
        {
            Color colorHold = GUI.backgroundColor;
            GUI.backgroundColor = color;
            action();
            GUI.backgroundColor = colorHold;
        }
        #endregion
    }
}