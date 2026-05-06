using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TrackSequencingTool
{
    [Serializable]
    public class Channel
    {
        public InstrumentSettings defineChannelStart;
        public List<ChannelLine> CommandLines = new();
    }
}