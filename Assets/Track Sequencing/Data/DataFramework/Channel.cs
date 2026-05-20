using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TrackSequencingTool
{
    using TEF = TrackerEditorFunctions;

    /// <summary>
    /// Channel information for a track sequencer file
    /// </summary>
    [Serializable]
    public class Channel
    {
        public bool playback = true; // Can be played
        public InstrumentSettings defineChannelStart; // Instrument data for the channel
        public List<CommandLine> CommandLines = new(); // List of commands for the channel

        /// <summary>
        /// A display option for editor windows
        /// </summary>
        internal void EditorDraw()
        {
            playback = EditorGUILayout.Toggle(playback, GUILayout.Width(TEF.CellWidth));
            defineChannelStart.EditorDraw();
        }
    }
}