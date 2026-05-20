using System;
using UnityEngine;

namespace TrackSequencingTool
{
    using TEF = TrackerEditorFunctions;

    /// <summary>
    /// Instrument settings to be used line-by-line and at the start of each channel to set how playback sounds.
    /// </summary>
    [Serializable]
    public class InstrumentSettings : TrackerSequenceBaseData
    {
        public string bank; // Library of instruments in sf2 file
        public string instrument; // Instrument ID
        public string velocity; // Note press speed
        public string duration = ""; // Note duration

        /// <summary>
        /// Draw data for editor window
        /// </summary>
        protected override void DrawValues()
        {
            bank = TEF.EditorStringFunction("Bank", ref bank, 0, 1000, TEF.EditorType.intBox);
            instrument = TEF.EditorStringFunction("Inst", ref instrument, 0, 1000, TEF.EditorType.intBox);
            velocity = TEF.EditorStringFunction("Vel", ref velocity, 0, 1, TEF.EditorType.floatBox);
            duration = TEF.EditorStringFunction("Dur", ref duration, 0, 100, TEF.EditorType.floatBox);
        }

        // Editor colour
        protected override Color EditorColor() => Color.yellow;
    }
}