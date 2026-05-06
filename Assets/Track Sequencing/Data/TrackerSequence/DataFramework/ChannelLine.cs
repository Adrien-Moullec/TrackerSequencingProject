using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TrackSequencingTool
{
    [Serializable]
    public class ChannelLine
    {
        public NoteInfo PlaybackLine;
        public InstrumentSettings channelSettings;
        public void EditorDraw()
        {
            channelSettings.EditorDraw();
            PlaybackLine.EditorDraw();
        }
    }
}