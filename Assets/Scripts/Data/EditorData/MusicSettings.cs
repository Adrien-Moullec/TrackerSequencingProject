using System;
using UnityEditor;
using UnityEngine;

namespace TrackSequencingTool
{
    using TEF = TrackerEditorFunctions;

    /// <summary>
    /// Music data across all channels
    /// </summary>
    [Serializable]
    public class MusicSettings : TrackerSequenceBaseData
    {
        public string tempo; // Playback speed
        public string volume; // Overall Volume     
        public string timestampName; // timestamp(event:?) name

        /// <summary>
        /// Draw values in an editor window
        /// </summary>
        protected override void DrawValues()
        {
            tempo = TEF.EditorStringFunction("Tempo", ref tempo, 30, 1000, TEF.EditorType.intBox);
            volume = TEF.EditorStringFunction("Vol", ref volume, 0, 1, TEF.EditorType.floatBox);

            EditorGUILayout.LabelField("Stamp", GUILayout.Width(TEF.CellWidth));
            timestampName = EditorGUILayout.TextField(timestampName, GUILayout.Width(TEF.CellWidth));
        }

        /// Colour of editor data in window
        protected override Color EditorColor() => Color.blue;

        /// <summary>
        /// Clear data
        /// </summary>
        internal void Clear()
        {
            tempo = "";
            volume = "";
            timestampName = "";
        }
    }
}