using System;
using UnityEngine;

namespace TrackSequencingTool
{
    /// <summary>
    /// A single line of data in a track sequencer channel
    /// </summary>
    [Serializable]
    public class CommandLine
    {
        [Tooltip("Note information to play.")]
        public NoteInfo PlaybackLine;
        [Tooltip("Instrument data for the channel.")]
        public InstrumentSettings instrumentSettings;

#if UNITY_EDITOR
        /// <summary>
        /// Editor window option to display data
        /// </summary>
        public void EditorDraw()
        {
            TrackerDataFunctions.IsNull(ref PlaybackLine);
            TrackerDataFunctions.IsNull(ref instrumentSettings);
            PlaybackLine.EditorDraw();
            instrumentSettings.EditorDraw();
        }
#endif

        /// <summary>
        /// Clear data shorthand
        /// </summary>
        internal void Clear()
        {
            instrumentSettings = new();
            PlaybackLine = new();
        }
    }
}