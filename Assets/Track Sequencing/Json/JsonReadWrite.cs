using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TrackSequencingTool
{
    public static class JsonReadWrite
    {
        public static TrackSequencer ReadJSON(TextAsset textAsset) =>
            JsonUtility.FromJson<TrackSequencer>(textAsset.text);
        public static void OutputJSON(TrackSequencer trackSequencer, string fileName)
        {
            string outputStr = JsonUtility.ToJson(trackSequencer, true);
            File.WriteAllText(Application.dataPath + "/Resources/Track Sequencer/" + fileName + ".json", outputStr);
        }
        public static void OutputJSON(TrackSequencer trackSequencer, TextAsset file)
        {
            if (trackSequencer == null || file == null) return;
            File.WriteAllText(
                AssetDatabase.GetAssetPath(file),
                JsonUtility.ToJson(trackSequencer, true)
            );
            AssetDatabase.Refresh();
        }

        public static void ColourUI(Action action, Color color)
        {
            Color colorHold = GUI.backgroundColor;
            GUI.backgroundColor = color;
            action();
            GUI.backgroundColor = colorHold;
        }

    }
    /*[CustomEditor(typeof(JsonEditor))]
    [CanEditMultipleObjects]
    public class JsonEditorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            JsonEditor jsonEditor = (JsonEditor)target;
            if (GUILayout.Button("Write To Json"))
            {
                jsonEditor.OutputJSON();
            }
        }
    }*/
    /*
    ******* JSON LAYOUT *******
    - Each Channel:
        - Start - Playback Settings
        - Each line:
            a) Note
            b) Playback Settings

    ******* DEFINITIONS *******
    - Note: key, velocity, duration (FF-FF-FF)
    - Playback Settings: speed, volume, ??? (FF-FF)
    */
}