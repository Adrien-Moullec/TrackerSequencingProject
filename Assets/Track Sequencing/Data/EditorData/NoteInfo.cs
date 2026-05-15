using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TrackSequencingTool
{
    [Serializable]
    public class NoteInfo : TrackerSequenceBaseData
    {
        public string key = ""; // 0 -> 120
        public string velocity = ""; // 0 -> 1
        public string duration = ""; // Min 0

        protected override void DrawValues()
        {
            bool success = int.TryParse(key, out int value);

            EditorGUILayout.LabelField(success ? (TrackSequencerEditorWindow.noteTranslation[value % 12] + "" + Mathf.Floor(value / 12f)) : "", GUILayout.Width(CellWidth));
            key = int.TryParse(
                EditorGUILayout.TextField(key, GUILayout.Width(CellWidth)),
                out value
            )
            && value >= 0 && value <= 120 ? value.ToString() : "";

            if (!string.IsNullOrEmpty(key))
            {
                EditorGUILayout.LabelField("Vel", GUILayout.Width(CellWidth));
                velocity = EditorGUILayout.Slider(
                    float.TryParse(velocity, out float result) && result >= 0 && result <= 1 ? result : 1f, 0, 1, GUILayout.Width(CellWidth)
                ).ToString();

                EditorGUILayout.LabelField("Dur", GUILayout.Width(CellWidth));
                duration = EditorGUILayout.FloatField(
                    float.TryParse(duration, out float resultDur) && resultDur > 0 ? resultDur : 1,
                    GUILayout.Width(CellWidth)
                ).ToString();
            }
            else
            {
                velocity = ""; duration = "";
            }
        }

        protected override Color EditorColor() => Color.red;
    }
}