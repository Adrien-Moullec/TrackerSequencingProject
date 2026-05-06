using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TrackSequencingTool
{
    [Serializable]
    public class CommandLine
    {
        public NoteInfo PlaybackLine;
        public InstrumentSettings instrumentSettings;
        public void EditorDraw()
        {
            instrumentSettings.EditorDraw();
            PlaybackLine.EditorDraw();
        }
    }
}