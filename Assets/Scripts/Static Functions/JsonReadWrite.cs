using System.IO;
using UnityEditor;
using UnityEngine;

namespace TrackSequencingTool
{
    /// <summary>
    /// Callable functions relating to Json files
    /// </summary>
    public static class JsonReadWrite
    {
        /// <summary>
        /// Read track sequencer data from a json file
        /// </summary>
        /// <param name="jsonFile"> Json file </param>
        /// <returns> Track sequence data </returns>
        public static TrackSequencer ReadJSON(TextAsset jsonFile) => JsonUtility.FromJson<TrackSequencer>(jsonFile.text);

        /// <summary>
        /// Apply a json file's data to a track sequencer variable
        /// </summary>
        /// <param name="jsonFile"> Json file to read </param>
        /// <param name="trackSequencer"> track sequence variable pointer </param>
        /// <returns> Returns true if data was successfully extracted </returns>
        public static bool ReadJSON(TextAsset jsonFile, ref TrackSequencer trackSequencer)
        {
            /// Try read JSON file
            if (jsonFile == null)
                return false;
            trackSequencer = ReadJSON(jsonFile);

            /// Check that data isn't completely null
            if (TrackerDataFunctions.IsNull(ref trackSequencer) && TrackerDataFunctions.IsNull(ref trackSequencer.channels) && TrackerDataFunctions.IsNull(ref trackSequencer.musicSettings))
                return false;

            /// Check that there are functional channels
            bool foundValid = false;
            foreach (var c in trackSequencer.channels)
            {
                if (c == null) continue;
                foundValid = TrackerDataFunctions.IsNull(ref c.CommandLines) || TrackerDataFunctions.IsNull(ref c.defineChannelStart) ?
                    foundValid : true;
            }
            return foundValid;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Write track sequencer data to a JSON file or create a new JSON file
        /// </summary>
        /// <param name="trackSequencer"> Output track sequencer data </param>
        /// <param name="fileName"> Name of the JSON file </param>
        public static void OutputJSON(TrackSequencer trackSequencer, string fileName)
        {
            string outputStr = JsonUtility.ToJson(trackSequencer, true);
            File.WriteAllText(Application.dataPath + "/Resources/Track Sequencer/" + fileName + ".json", outputStr);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Write track sequencer data to an existing JSON file
        /// </summary>
        /// <param name="trackSequencer"> Track sequencer data </param>
        /// <param name="file"> The text asset that is being written to </param>
        public static void OutputJSON(TrackSequencer trackSequencer, TextAsset file)
        {
            if (trackSequencer == null || file == null) return;
            File.WriteAllText(
                AssetDatabase.GetAssetPath(file),
                JsonUtility.ToJson(trackSequencer, true)
            );
            AssetDatabase.Refresh();
        }
#endif
    }
}