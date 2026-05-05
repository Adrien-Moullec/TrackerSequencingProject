using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TrackSequencingTool
{
    #region Summaries
    [Serializable]
    public class TrackSequencer
    {
        [HideInInspector] public MusicSettings startingSettings = new MusicSettings();
        public List<MusicSettings> musicSettings = new();
        public List<Channel> channels = new();
    }

    [Serializable]
    public class Channel
    {
        public ChannelSettings defineChannelStart;
        public List<MusicCommand> CommandLines = new();
    }

    [Serializable]
    public class MusicCommand
    {
        public ChannelPlaybackLine PlaybackLine;
        public ChannelSettings channelSettings;
        public void EditorDraw(string message)
        {
            channelSettings.EditorDraw();
            PlaybackLine.EditorDraw();
        }
    }
    #endregion

    #region Base Definitions
    [Serializable]
    public class MusicSettings
    {
        public int tempo;

        public void EditorDraw()
        {
            tempo = EditorGUILayout.IntField(tempo);
        }
    }
    [Serializable]
    public class ChannelSettings
    {
        public string playback;
        public void EditorDraw()
        {
            playback = EditorGUILayout.TextField(playback);
        }
    }
    [Serializable]
    public class ChannelPlaybackLine
    {
        public string key;
        public float velocity;
        public float duration;

        public void EditorDraw(string message = "")
        {
            key = int.TryParse(EditorGUILayout.TextField(key), out int value) ? value.ToString() : "";
            velocity = EditorGUILayout.Slider(velocity, 0, 1);
            duration = EditorGUILayout.FloatField(duration);

        }
    }
    #endregion
    /*
    [Serializable]
    public class Playback
    {
        [Range(0, 120)] public int key = 60;
        [Range(0f, 1f)] public float velocity = 1f;
        [Min(0)] public float duration = 5;
    }

    [Serializable]
    public class DefineMusic
    {
        [Range(0, 120)] public int tempo = 60;
    }
    */
}