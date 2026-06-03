using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TrackSequencingTool
{
    public class SequencerSelection
    {
        [Tooltip("Collection of music command indexes currently selected.")]
        private HashSet<int> musicSelected = new();
        [Tooltip("Collection of channel IDs currently selected.")]
        private HashSet<int> channelSelected = new();
        [Tooltip("Currently selected channel.")]
        private Channel activeChannel = null;
        [Tooltip("Last selected command line index.")]
        private int lastMusicIndex = -1;
        [Tooltip("Last selected Channel ID.")]
        private int lastChannelId = -1;

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

                for (int x = Mathf.Min(lastMusicIndex, i); x <= Mathf.Max(lastMusicIndex, i); x++)
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
                lastChannelId = -1;
            }

            activeChannel = c;

            if (!shift || lastChannelId < 0)
            {
                channelSelected.Clear();
                channelSelected.Add(i);
            }
            else
            {
                channelSelected.Clear();

                int min = System.Math.Min(lastChannelId, i);
                int max = System.Math.Max(lastChannelId, i);

                for (int x = min; x <= max; x++)
                    channelSelected.Add(x);
            }

            lastChannelId = i;
            musicSelected.Clear();
        }

        #region Selection Data Checks
        /// Get the list of selected music command indexes.
        public List<int> GetMusicSelection() => musicSelected.ToList();
        /// Get the list of currently selected channels.
        public List<int> GetChannelSelection() => channelSelected.ToList();
        /// Get the current active channel.
        public Channel GetActiveChannel() => activeChannel;
        /// Check whether the command line of index i is currently selected.
        public bool IsMusicSelected(int i) => musicSelected.Contains(i);
        #endregion


        /// <summary>
        /// Clear selection data
        /// </summary>
        public void Clear()
        {
            channelSelected.Clear();
            musicSelected.Clear();
        }
        /// <summary>
        /// Deselect just the channel.
        /// </summary>
        public void DeselectChannel()
        {
            channelSelected.Clear();
        }

        /// <summary>
        /// Get the first selected channel index
        /// </summary>
        /// <returns> First channel index </returns>
        public int GetFirstSelectedChannelIndex()
        {
            if (channelSelected.Count == 0) return -1;
            return channelSelected.Min();
        }
        /// <summary>
        /// Get the first selected music command index
        /// </summary>
        /// <returns> First channel index </returns>
        public int GetFirstSelectedMusicIndex()
        {
            if (musicSelected.Count == 0) return -1;
            return musicSelected.Min();
        }

        /// <summary>
        /// Clear the music commands or channels that are selected
        /// </summary>
        /// <param name="sequencer"></param>
        public void ClearSelectedRows(TrackSequencer sequencer)
        {
            if (sequencer == null)
                return;

            // Clear selected music rows
            if (sequencer.musicSettings != null)
                foreach (int i in musicSelected)
                    if (i >= 0 && i < sequencer.musicSettings.Count)
                        sequencer.musicSettings[i].Clear();

            // Clear selected channel rows
            if (activeChannel != null && activeChannel.CommandLines != null)
                foreach (int i in channelSelected)
                    if (i >= 0 && i < activeChannel.CommandLines.Count)
                        activeChannel.CommandLines[i].Clear();

            Clear();
        }
    }
}