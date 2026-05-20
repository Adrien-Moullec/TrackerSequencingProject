using UnityEngine;

namespace TrackSequencingTool
{
    public static class MusicFunctions
    {
        public static float BeatTime(int bpm) => 60f / bpm;
    }
}