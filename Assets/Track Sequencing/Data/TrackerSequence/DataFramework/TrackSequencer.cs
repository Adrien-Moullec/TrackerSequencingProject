using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TrackSequencingTool
{
    [Serializable]
    public class TrackSequencer
    {
        [HideInInspector] public MusicSettings startingSettings = new MusicSettings();
        public List<MusicSettings> musicSettings = new();
        public List<Channel> channels = new();

        public static int GetMaxListLengthFromSequencer(TrackSequencer t)
        {
            if (t == null || t?.channels == null)
                return 0;
            int maxChannel = 0;

            foreach (var c in t.channels)
                if (c?.CommandLines != null)
                    maxChannel = Mathf.Max(maxChannel, c.CommandLines.Count);

            return Mathf.Max(maxChannel, t.musicSettings?.Count ?? 0);
        }
    }
}