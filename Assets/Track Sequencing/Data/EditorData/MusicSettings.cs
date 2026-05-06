using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TrackSequencingTool
{
    [Serializable]
    public class MusicSettings : TrackerSequenceBaseData
    {
        public string tempo; // 30 -> 240
        protected override void DrawValues()
        {
            EditorGUILayout.LabelField("Temp", GUILayout.Width(CellWidth));
            tempo = int.TryParse(EditorGUILayout.TextField(tempo/*, GUILayout.Width(CellWidth)*/), out int value) && value >= 30 && value <= 240 ? value.ToString() : "";
        }

        protected override Color EditorColor() => Color.blue;
    }
}