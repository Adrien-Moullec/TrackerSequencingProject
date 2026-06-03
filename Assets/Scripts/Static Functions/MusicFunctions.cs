using UnityEngine;

namespace TrackSequencingTool
{
    public static class MusicFunctions
    {
        /// <summary>
        /// Get inter-note time from the current beats per minute value
        /// </summary>
        /// <param name="bpm"> Beats per minute </param>
        /// <returns> Playback command interval </returns>
        public static float BeatTime(int bpm) => 60f / bpm;
    }
}