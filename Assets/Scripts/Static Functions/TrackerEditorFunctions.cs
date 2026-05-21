using System;
using UnityEditor;
using UnityEngine;

namespace TrackSequencingTool
{
#if UNITY_EDITOR
    /// <summary>
    /// Cross-script functions relating to utilizing track sequencer data in editor windows
    /// </summary>
    public static class TrackerEditorFunctions
    {
        public const int CellWidth = 30; // Default cell-width

        /// Note info for text-display
        public static string[] noteTranslation = new string[]
        {
            "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"
        };
        /// Note info for text-display
        public static string[] noteTranslationFlats = new string[]
        {
            "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B"
        };

        /// <summary>
        /// Callable function for any optional variable display in an editor window
        /// </summary>
        /// <param name="label"> Name of the input box </param>
        /// <param name="parameter"> the pointer for a parameter to edit </param>
        /// <param name="minVal"> minimum value </param>
        /// <param name="maxVal"> maximum value </param>
        /// <param name="editorType"> Editor box value type </param>
        /// <param name="defaultVal"> Default if input is not valid </param>
        /// <param name="cellWidth"> width of cells </param>
        /// <returns> string value after editor-clean </returns>
        public static string EditorStringFunction(string label, ref string parameter, float minVal, float maxVal, EditorType editorType, string defaultVal = "", float cellWidth = CellWidth)
        {
            EditorGUILayout.LabelField(label, GUILayout.Width(cellWidth));
            switch (editorType)
            {
                case EditorType.floatBox:
                    return float.TryParse(
                        EditorGUILayout.TextField(parameter, GUILayout.Width(cellWidth)), out float valF
                    ) && valF >= minVal && valF <= maxVal ? valF.ToString() : defaultVal;
                case EditorType.intBox:
                    return int.TryParse(
                        EditorGUILayout.TextField(parameter, GUILayout.Width(cellWidth)), out int ValI
                    ) && ValI >= minVal && ValI <= maxVal ? ValI.ToString() : defaultVal;
                case EditorType.slider:
                    return EditorGUILayout.Slider(
                        float.TryParse(parameter, out float result) && result >= minVal && result <= maxVal ? result : float.Parse(defaultVal), minVal, maxVal, GUILayout.Width(cellWidth)
                    ).ToString();
                default: return defaultVal;
            }
        }

        /// <summary>
        /// Type of editor display for string values
        /// </summary>
        public enum EditorType
        {
            floatBox,
            intBox,
            slider
        }

        /// <summary>
        /// Keyboard options for each common keyboard shortcut
        /// </summary>
        /// <param name="copy"> Copy action </param>
        /// <param name="paste"> Paste action </param>
        /// <param name="undo"> Undo action </param>
        /// <param name="redo"> Redo action </param>
        /// <param name="delete"> Delete action </param>
        /// <param name="deselect"> Deselect action </param>
        public static void KeyboardInputs(Action copy, Action paste, Action undo, Action redo, Action delete, Action deselect)
        {
            Event e = Event.current;
            if (e?.type != EventType.KeyDown)
                return;

            switch (e.keyCode)
            {
                case KeyCode.C:
                    if (e.control)
                    {
                        copy();
                        e.Use();
                    }
                    break;
                case KeyCode.V:
                    if (e.control)
                    {
                        paste();
                        e.Use();
                    }
                    break;
                case KeyCode.Z:
                    if (e.control)
                    {
                        undo();
                        e.Use();
                    }
                    break;
                case KeyCode.Y:
                    if (e.control)
                    {
                        redo();
                        e.Use();
                    }
                    break;
                case KeyCode.Escape:
                    deselect();
                    e.Use();
                    break;
                case KeyCode.Delete:
                    delete();
                    e.Use();
                    break;
            }
        }
    }
#endif
}