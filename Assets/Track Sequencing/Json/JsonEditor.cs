using System.IO;
using UnityEditor;
using UnityEngine;

namespace TrackSequencingTool
{
    public class JsonEditor : MonoBehaviour
    {
        [SerializeField] string file;
        public TrackSequencer trackSequencer = new();

        public static void OutputJSON(TrackSequencer trackSequencer, string fileName = "jsonTest")
        {
            string outputStr = JsonUtility.ToJson(trackSequencer, true);
            File.WriteAllText(Application.dataPath + "/Resources/Track Sequencer/" + fileName + ".json", outputStr);
        }
        public static void OutputJSON(TrackSequencer trackSequencer, TextAsset file)
        {
            if (trackSequencer == null || file == null)
            {
                Debug.LogError("TrackSequencer or file is null.");
                return;
            }

            File.WriteAllText(
                AssetDatabase.GetAssetPath(file),
                JsonUtility.ToJson(trackSequencer, true)
            );
            AssetDatabase.Refresh();

            Debug.Log("Saved JSON");
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