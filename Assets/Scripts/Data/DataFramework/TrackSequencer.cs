using System;
using System.Collections.Generic;
using UnityEngine;

namespace TrackSequencingTool
{
    /// <summary>
    /// Data for a track sequencer to be read into a JSON file
    /// </summary>
    [Serializable]
    public class TrackSequencer
    {
        [Tooltip("Starting overall music settings.")]
        [HideInInspector] public MusicSettings startingSettings = new MusicSettings();
        [Tooltip("Line-by-line music settings.")]
        public List<MusicSettings> musicSettings = new();
        [Tooltip("List of channel information.")]
        public List<Channel> channels = new();

        /// <summary>
        /// Returns the max list length out of all the channels for data management
        /// </summary>
        /// <param name="t"> track sequencer check </param>
        /// <returns> max list length </returns>
        public static int GetMaxListLengthFromSequencer(TrackSequencer t)
        {
            // Check validity
            if (t == null || t?.channels == null)
                return 0;
            int maxChannel = 0;

            // Check for new longest value in each channel
            foreach (var c in t.channels)
                if (c?.CommandLines != null)
                    maxChannel = Mathf.Max(maxChannel, c.CommandLines.Count);

            // Return max value
            return Mathf.Max(maxChannel, t.musicSettings?.Count ?? 0);
        }
    }
}