using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TrackSequencingTool
{
    [Serializable]
    public class InstrumentSettings : TrackerSequenceBaseData
    {
        public string instrument; // Min 0
        public string volume; // Min 0

        protected override void DrawValues()
        {
            EditorGUILayout.LabelField("Inst", GUILayout.Width(CellWidth));
            instrument = int.TryParse(EditorGUILayout.TextField(instrument), out int valI) && valI >= 0 ?
            valI.ToString() :
            "";
            EditorGUILayout.LabelField("Vol", GUILayout.Width(CellWidth));
            volume = float.TryParse(EditorGUILayout.TextField(volume), out float valF) && valF >= 0 && valF <= 1f
            ? valF.ToString()
            : "";
        }

        protected override Color EditorColor() => Color.yellow;
    }
}