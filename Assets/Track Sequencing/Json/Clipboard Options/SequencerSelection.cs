using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TrackSequencingTool
{
    public class SequencerSelection
    {
        private HashSet<int> musicSelected = new();
        private HashSet<int> channelSelected = new();
        private Channel activeChannel = null;
        private int lastMusicIndex = -1;
        private int lastChannelIndex = -1;

        public bool IsMusicSelected(int i) => musicSelected.Contains(i);

        public void SelectMusic(int i, bool shift)
        {
            if (!shift || lastMusicIndex < 0)
            {
                musicSelected.Clear();
                musicSelected.Add(i);
            }
            else
            {
                musicSelected.Clear();

                int min = Mathf.Min(lastMusicIndex, i);
                int max = Mathf.Max(lastMusicIndex, i);

                for (int x = min; x <= max; x++)
                    musicSelected.Add(x);
            }

            lastMusicIndex = i;
        }
        public bool IsChannelSelected(Channel c, int i) => c == activeChannel && channelSelected.Contains(i);
        public void SelectChannel(Channel c, int i, bool shift)
        {
            if (c == null)
                return;

            if (activeChannel != c)
            {
                channelSelected.Clear();
                lastChannelIndex = -1;
            }

            activeChannel = c;

            if (!shift || lastChannelIndex < 0)
            {
                channelSelected.Clear();
                channelSelected.Add(i);
            }
            else
            {
                channelSelected.Clear();

                int min = System.Math.Min(lastChannelIndex, i);
                int max = System.Math.Max(lastChannelIndex, i);

                for (int x = min; x <= max; x++)
                    channelSelected.Add(x);
            }

            lastChannelIndex = i;
            musicSelected.Clear();
        }
        public void DeselectChannel()
        {
            channelSelected.Clear();
        }

        public List<int> GetMusicSelection() => musicSelected.OrderBy(x => x).ToList();
        public List<int> GetChannelSelection() => channelSelected.OrderBy(x => x).ToList();
        public Channel GetActiveChannel() => activeChannel;
        public void Clear()
        {
            channelSelected.Clear();
            musicSelected.Clear();
        }

        public int GetFirstSelectedChannelIndex()
        {
            if (channelSelected.Count == 0)
                return -1;
            return channelSelected.Min();
        }
        public int GetFirstSelectedMusicIndex()
        {
            if (musicSelected.Count == 0)
                return -1;

            return musicSelected.Min();
        }
        public void ClearSelectedRows(TrackSequencer sequencer)
        {
            if (sequencer == null)
                return;

            // Clear selected music rows
            if (sequencer.musicSettings != null)
            {
                foreach (int i in musicSelected)
                {
                    if (i >= 0 && i < sequencer.musicSettings.Count)
                        sequencer.musicSettings[i].Clear();
                }
            }

            // Clear selected channel rows
            if (activeChannel != null && activeChannel.CommandLines != null)
                foreach (int i in channelSelected)
                    if (i >= 0 && i < activeChannel.CommandLines.Count)
                        activeChannel.CommandLines[i].Clear();

            Clear();
        }
    }
}