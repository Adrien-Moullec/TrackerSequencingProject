using UnityEngine;
using UnityEditor;
using System.Collections;
using TrackSequencingTool;

public class TrackPlayback : MonoBehaviour
{
    public TextAsset textAsset;
    public TrackSequencer trackSequencer;
    public TrackerDllInterpreter trackerDllInterpreter;
    public void PlayTrack() => StartCoroutine(PlayTrackEnum());
    public IEnumerator PlayTrackEnum()
    {
        int totalCommandLines = TrackSequencer.GetMaxListLengthFromSequencer(trackSequencer);
        ChannelLine channelLine = null;
        Debug.Log("Start");
        for (int lineInt = 0; lineInt < totalCommandLines; lineInt++)
        {
            Debug.Log("Line " + lineInt);
            for (int chan = 0; chan < trackSequencer.channels.Count; chan++)
            {
                channelLine = trackSequencer.channels[chan].CommandLines[lineInt];
                if (channelLine == null || channelLine.PlaybackLine.key == "") continue;
                Debug.Log("channel " + chan);
                trackerDllInterpreter.PlayNote(chan, int.Parse(channelLine.PlaybackLine.key), float.Parse(channelLine.PlaybackLine.velocity), float.Parse(channelLine.PlaybackLine.duration));
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}

[CustomEditor(typeof(TrackPlayback))]
[CanEditMultipleObjects]
public class TrackPlaybackEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        TrackPlayback trackPlayback = (TrackPlayback)target;
        if (GUILayout.Button("Read JSON"))
            trackPlayback.trackSequencer = JsonReadWrite.ReadJSON(trackPlayback.textAsset);

        if (GUILayout.Button("Play") && trackPlayback.trackSequencer != null)
            trackPlayback.PlayTrack();
    }
}