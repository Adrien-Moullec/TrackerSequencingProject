using UnityEngine;

namespace TrackSequencingTool
{
    // Layout for tracker sequence data that will be used in an editor window
    public abstract class TrackerSequenceBaseData
    {
        /// <summary>
        /// Call DrawValues() so this class has a pre-built editorwindow integration
        /// </summary>
        public void EditorDraw() => TrackSequencerEditorWindow.ColourUI(() => DrawValues(), EditorColor());

        /// <summary>
        /// Contains editor window layout
        /// </summary>
        protected abstract void DrawValues();

        /// <summary>
        /// Editor colour for draw values
        /// </summary>
        /// <returns></returns>
        protected abstract Color EditorColor();
        //return Regex.IsMatch(code, @"^\d{3}[a-g]$");
    }
}