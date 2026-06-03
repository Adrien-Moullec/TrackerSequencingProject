using System.Collections.Generic;
using UnityEngine;

namespace TrackSequencingTool
{
    public class SequencerClipboard
    {
        [Tooltip("The stored stack data of previous window iterations to loop back through later on Ctrl Z.")]
        public Stack<string> undoStack = new Stack<string>();
        [Tooltip("The stored stack data of undone window iterations to loop back through later on Ctrl Y.")]
        public Stack<string> redoStack = new Stack<string>();
        [Tooltip("Stored copy data after a selected column of music commands undergo Ctrl C.")]
        private SaveSequencerChannelData seqCopyData = new();
        [Tooltip("Maximum allowed number of undo steps.")]
        public const int MaxUndoSteps = 100;

        /// <summary>
        /// Copy selected data from TrackSequencer between 2 selected indexes.
        /// </summary>
        /// <param name="seq"> Track Sequencer class </param>
        /// <param name="sel"> Track Sequencer selection data </param>
        public void Copy(TrackSequencer seq, SequencerSelection sel)
        {
            seqCopyData = new SaveSequencerChannelData();

            // Copy music settings data from the sequencer from copied selection of commands
            foreach (int i in sel.GetMusicSelection())
            {
                if (seq.musicSettings == null)
                    continue;

                if (i >= 0 && i < seq.musicSettings.Count)
                    seqCopyData.musicSettings.Add(CloneMusic(seq.musicSettings[i]));
            }

            // Copy music command data from the sequencer from copied selection of commands
            var channel = sel.GetActiveChannel();

            if (channel != null)
            {
                foreach (int i in sel.GetChannelSelection())
                {
                    if (channel.CommandLines == null)
                        continue;

                    if (i >= 0 && i < channel.CommandLines.Count)
                        seqCopyData.commands.Add(CloneCommand(channel.CommandLines[i]));
                }
            }
        }

        /// <summary>
        /// Paste copied data to TrackSequencer from a selected index.
        /// </summary>
        /// <param name="seq"> Track Sequencer class </param>
        /// <param name="sel"> Track Sequencer selection data </param>
        public void Paste(TrackSequencer seq, SequencerSelection sel)
        {
            if (seq == null)
                return;

            // Paste music settings down the sequencer from copied command list
            int musicStart = sel.GetFirstSelectedMusicIndex();
            if (musicStart >= 0 && seq.musicSettings != null)
            {
                for (int i = 0; i < seqCopyData.musicSettings.Count; i++)
                {
                    if (musicStart + i >= seq.musicSettings.Count)
                        break;

                    seq.musicSettings[musicStart + i] = CloneMusic(seqCopyData.musicSettings[i]);
                }
            }

            // Paste music command data down the sequencer from copied command list
            Channel channel = sel.GetActiveChannel();
            int channelStart = sel.GetFirstSelectedChannelIndex();

            if (channel != null && channel.CommandLines != null && channelStart >= 0)
            {
                for (int i = 0; i < seqCopyData.commands.Count; i++)
                {
                    if (channelStart + i >= channel.CommandLines.Count)
                        break;

                    channel.CommandLines[channelStart + i] =
                        CloneCommand(seqCopyData.commands[i]);
                }
            }
        }

        #region Data Clones
        /// <summary>
        /// Return deep-cloned Music Settings using Json methodology. 
        /// </summary>
        /// <param name="obj"> MusicSettings to clone </param>
        /// <returns> Deep-cloned MusicSettings </returns>
        MusicSettings CloneMusic(MusicSettings obj)
        {
            if (obj == null) return null;
            return JsonUtility.FromJson<MusicSettings>(JsonUtility.ToJson(obj));
        }

        /// <summary>
        /// Return deep-cloned CommandLine using Json methodology. 
        /// </summary>
        /// <param name="obj"> CommandLine to clone </param>
        /// <returns> Deep-cloned CommandLine </returns>
        CommandLine CloneCommand(CommandLine obj)
        {
            if (obj == null) return null;
            return JsonUtility.FromJson<CommandLine>(JsonUtility.ToJson(obj));
        }
        #endregion

        /// <summary>
        /// Save sequence data for copying and pasting, a separate class to Channel.cs due to requiring less data.
        /// </summary>
        [System.Serializable]
        private class SaveSequencerChannelData
        {
            public List<MusicSettings> musicSettings = new();
            public List<CommandLine> commands = new();
        }
    }
}