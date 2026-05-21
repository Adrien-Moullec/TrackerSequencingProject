using System.Collections.Generic;
using UnityEngine;

namespace TrackSequencingTool
{

    /// <summary>
    /// Sequence tracker data functions
    /// </summary>
    public static class TrackerDataFunctions
    {
#if UNITY_EDITOR
        #region Line functionality
        /// <summary>
        /// Check that all command lines and music setting lengths are of the same length
        /// </summary>
        /// <param name="sequencer"> Track sequence data </param>
        /// <param name="newLength"> The target list length </param>
        public static void RefreshCommandLines(TrackSequencer sequencer, int newLength)
        {
            /// Channel length
            foreach (var channel in sequencer.channels)
            {
                IsNull(ref channel.CommandLines);
                while (channel.CommandLines.Count < newLength)
                    channel.CommandLines.Add(null);
            }

            /// Music Settings
            IsNull(ref sequencer.musicSettings);
            while (sequencer.musicSettings.Count < newLength)
                sequencer.musicSettings.Add(null);
        }

        /// <summary>
        /// Delete sequencer line at an index
        /// </summary>
        /// <param name="sequencer"> Track sequencer data </param>
        /// <param name="jsonFile"> json file reference </param>
        /// <param name="index"> list id target </param>
        public static void DeleteLine(TrackSequencer sequencer, TextAsset jsonFile, int index)
        {
            if (sequencer == null)
                return;

            /// Remove music settings line at index
            if (!IsNull(ref sequencer.musicSettings) && index >= 0 && index < sequencer.musicSettings.Count)
                sequencer.musicSettings.RemoveAt(index);

            /// Remove command lines that exist at index
            foreach (var channel in sequencer.channels)
            {
                if (channel?.CommandLines == null)
                    continue;
                if (index >= 0 && index < channel.CommandLines.Count)
                    channel.CommandLines.RemoveAt(index);
            }

            /// output track sequencer to json file
            JsonReadWrite.OutputJSON(sequencer, jsonFile);
        }

        /// <summary>
        /// Move a track sequencer's command line from a to b
        /// </summary>
        /// <param name="sequencer"> track sequencer data </param>
        /// <param name="clipboard"> clipboard data reference </param>
        /// <param name="jsonFile"> json file reference </param>
        /// <param name="from"> command line index to be moved </param>
        /// <param name="to"> index to move command line to </param>
        public static void MoveLine(TrackSequencer sequencer, SequencerClipboard clipboard, TextAsset jsonFile, int from, int to)
        {
            /// Check line movement is legal
            SaveUndoState(sequencer, clipboard);
            int max = TrackSequencer.GetMaxListLengthFromSequencer(sequencer);
            if (sequencer == null || from < 0 || from >= max || to < 0 || to >= max)
                return;

            /// Remove and reinsert music setting line into data
            if (!IsNull(ref sequencer.musicSettings) && from < sequencer.musicSettings.Count && to < sequencer.musicSettings.Count)
            {
                var item = sequencer.musicSettings[from];
                sequencer.musicSettings.RemoveAt(from);
                sequencer.musicSettings.Insert(to, item);
            }

            // Remove and reinsert music command lines into data
            foreach (var channel in sequencer.channels)
            {
                if (channel?.CommandLines == null || from >= channel.CommandLines.Count || to >= channel.CommandLines.Count)
                    continue;

                // Move to
                var item = channel.CommandLines[from];
                channel.CommandLines.RemoveAt(from);
                channel.CommandLines.Insert(to, item);
            }

            /// Output data to json file
            JsonReadWrite.OutputJSON(sequencer, jsonFile);
        }
        #endregion

        #region Undo / Redo
        /// <summary>
        /// Paste clipboard data into channel at selected location
        /// </summary>
        /// <param name="sequencer"> Track sequencer data reference </param>
        /// <param name="clipboard"> clipboard/stored information </param>
        /// <param name="selection"> Selected data information </param>
        /// <param name="jsonFile"> json file reference </param>
        public static void Paste(TrackSequencer sequencer, SequencerClipboard clipboard, SequencerSelection selection, TextAsset jsonFile)
        {
            SaveUndoState(sequencer, clipboard);
            clipboard.Paste(sequencer, selection);
            JsonReadWrite.OutputJSON(sequencer, jsonFile);
        }

        /// <summary>
        /// Add current editor state to save data stack
        /// </summary>
        /// <param name="sequencer"> track sequencer data </param>
        /// <param name="clipboard"> stored data reference </param>
        public static void SaveUndoState(TrackSequencer sequencer, SequencerClipboard clipboard)
        {
            if (sequencer == null)
                return;

            /// Adds current json file state to undo stack
            string json = JsonUtility.ToJson(sequencer);
            clipboard.undoStack.Push(json);

            /// Reset undo steps in clipboard if max undo steps exceed an amount
            while (clipboard.undoStack.Count > SequencerClipboard.MaxUndoSteps)
            {
                var temp = new Stack<string>();

                /// Re-add undostack back to itself
                while (clipboard.undoStack.Count > 1)
                    temp.Push(clipboard.undoStack.Pop());
                clipboard.undoStack.Clear();
                while (temp.Count > 0)
                    clipboard.undoStack.Push(temp.Pop());
            }
            clipboard.redoStack.Clear();
        }


        /// <summary>
        /// Return track sequencer data to state one stack back
        /// </summary>
        /// <param name="sequencer"> Track sequencer data reference </param>
        /// <param name="clipboard"> stored old data reference </param>
        /// <param name="jsonFile"> json asset file reference </param>
        public static void UndoAction(TrackSequencer sequencer, SequencerClipboard clipboard, TextAsset jsonFile)
        {
            if (clipboard.undoStack.Count == 0) return;
            if (sequencer != null) clipboard.redoStack.Push(JsonUtility.ToJson(sequencer));

            string json = clipboard.undoStack.Pop();
            sequencer = JsonUtility.FromJson<TrackSequencer>(json);
            JsonReadWrite.OutputJSON(sequencer, jsonFile);
        }

        /// <summary>
        /// Pushes track sequence data forwards
        /// </summary>
        /// <param name="sequencer"> Track sequence data </param>
        /// <param name="clipboard"> clipboard data </param>
        /// <param name="jsonFile"> json file reference </param>
        public static void RedoAction(TrackSequencer sequencer, SequencerClipboard clipboard, TextAsset jsonFile)
        {
            if (clipboard.redoStack.Count == 0) return;
            if (sequencer != null) clipboard.undoStack.Push(JsonUtility.ToJson(sequencer));

            string json = clipboard.redoStack.Pop();
            sequencer = JsonUtility.FromJson<TrackSequencer>(json);
            JsonReadWrite.OutputJSON(sequencer, jsonFile);
        }
        #endregion
#endif

        #region Variable change
        /// <summary>
        /// Checks if variable reference is null and assigns a default value if it is
        /// </summary>
        /// <typeparam name="T"> Any class type </typeparam>
        /// <param name="variable"> A class variable reference </param>
        /// <returns> whether input class is null: null=true, !null=false </returns>
        public static bool IsNull<T>(ref T variable)
            where T : class, new()
        {
            if (variable == null)
            {
                variable = new T();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if list reference is null and assigns a default value if it is
        /// </summary>
        /// <typeparam name="T"> Any class type </typeparam>
        /// <param name="variable"> A list variable reference </param>
        /// <returns> whether input list is null: null=true, !null=false </returns>
        public static bool IsNull<T>(ref List<T> list)
            where T : class, new()
        {
            if (list == null)
            {
                list = new List<T>();
                return true;
            }
            return false;
        }
        #endregion
    }
}