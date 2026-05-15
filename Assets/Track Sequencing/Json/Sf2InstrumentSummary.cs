using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Sf2InstrumentSummary : EditorWindow
{
    TextAsset textFile;
    List<string> instruments;

    [MenuItem("JSON/Track Sequencer Editor")]
    private static void OpenWindow()
    {
        Sf2InstrumentSummary wnd = GetWindow<Sf2InstrumentSummary>();
        wnd.titleContent = new GUIContent("FluidGM summary");
    }

    void OnGUI()
    {
        if (GUILayout.Button("Press to read instruments"))
        {
            if (textFile == null) return;
            instruments = new();
            instruments = textFile.text.Split('\n').ToList();
        }
        if (instruments == null) return;
        foreach (var n in instruments)
        {
            EditorGUILayout.LabelField(n);
        }
    }
}
