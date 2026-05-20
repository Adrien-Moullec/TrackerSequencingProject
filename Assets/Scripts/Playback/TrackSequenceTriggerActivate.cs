using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Events;

namespace TrackSequencingTool
{
    public class TrackerSequenceTriggerActivate : TrackerDllPlayback
    {
        [Header("Enter Area Events")]
        [SerializeField] UnityEvent onEnter;
        // For level area activation
        void OnTriggerEnter(Collider other)
        {
            if (other.tag != "Player") return;

            onEnter.Invoke();
            PlayTrack();
        }
    }
    /// <summary>
    /// Editor window for default tracker Dll playback class.
    /// Controls playback, loading and stop logic
    /// </summary>
    [CustomEditor(typeof(TrackerSequenceTriggerActivate))]
    [CanEditMultipleObjects]
    public class TrackerSequenceTriggerActivateEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            /// This try/catch currently avoids error on init load
            try
            {
                DrawDefaultInspector();
            }
            catch { }
            TrackerDllPlaybackEditor.DrawDefaultTrackerDllPlayback((TrackerSequenceTriggerActivate)target);
        }
    }
}