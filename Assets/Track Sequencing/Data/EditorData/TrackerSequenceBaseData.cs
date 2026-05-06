using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TrackSequencingTool
{
    //return Regex.IsMatch(code, @"^\d{3}[a-g]$");
    public abstract class TrackerSequenceBaseData
    {
        public const int CellWidth = 30;
        public void EditorDraw()
        {
            JsonReadWrite.ColourUI(() =>
            {
                DrawValues();
            },
            EditorColor()
            );
        }
        protected abstract void DrawValues();
        protected abstract Color EditorColor();
    }
}