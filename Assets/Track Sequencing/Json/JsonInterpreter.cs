using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using UnityEditor;
using System;

namespace TrackSequencingTool
{
    public class JsonInterpreter : MonoBehaviour
    {

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    [Serializable]
    public class NoteInfo
    {
        public string note;

        //return Regex.IsMatch(code, @"^\d{3}[a-g]$");
    }

    [CustomEditor(typeof(JsonInterpreter))]
    [CanEditMultipleObjects]
    public class JsonInterpreterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}