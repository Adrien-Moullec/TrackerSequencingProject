using System;
using UnityEngine;

namespace TrackSequencingTool
{
    using TEF = TrackerEditorFunctions;

    /// <summary>
    /// Note information for each line in a channel in a track sequencer
    /// </summary>
    [Serializable]
    public class NoteInfo : TrackerSequenceBaseData
    {
        public string key = ""; // Note to play

        /// <summary>
        /// Editor window draw values
        /// </summary>
        protected override void DrawValues()
        {
            key = TEF.EditorStringFunction(
                int.TryParse(key, out int value) ? (TEF.noteTranslation[value % 12] + "" + Mathf.Floor(value / 12f)) : "",
                ref key, 0, 120, TEF.EditorType.intBox
            );
        }

        /// Colour for editor window
        protected override Color EditorColor() => Color.red;
    }
}