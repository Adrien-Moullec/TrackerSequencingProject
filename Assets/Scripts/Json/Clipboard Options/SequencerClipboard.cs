using System.Collections.Generic;
using UnityEngine;

namespace TrackSequencingTool
{
    public class SequencerClipboard
    {
        public Stack<string> undoStack = new Stack<string>();
        public Stack<string> redoStack = new Stack<string>();
        private Data data = new();
        public const int MaxUndoSteps = 100;

        [System.Serializable]
        private class Data
        {
            public List<MusicSettings> music = new();
            public List<CommandLine> commands = new();
        }
        public void Copy(TrackSequencer seq, SequencerSelection sel)
        {
            data = new Data();

            // copy music line
            foreach (int i in sel.GetMusicSelection())
            {
                if (seq.musicSettings == null)
                    continue;

                if (i >= 0 && i < seq.musicSettings.Count)
                    data.music.Add(CloneMusic(seq.musicSettings[i]));
            }

            // Copy channel
            var channel = sel.GetActiveChannel();

            if (channel != null)
            {
                foreach (int i in sel.GetChannelSelection())
                {
                    if (channel.CommandLines == null)
                        continue;

                    if (i >= 0 && i < channel.CommandLines.Count)
                        data.commands.Add(CloneCommand(channel.CommandLines[i]));
                }
            }
        }

        public void Paste(TrackSequencer seq, SequencerSelection sel)
        {
            if (seq == null)
                return;

            // MUSIC PASTE
            int musicStart = sel.GetFirstSelectedMusicIndex();

            if (musicStart >= 0 && seq.musicSettings != null)
            {
                for (int i = 0; i < data.music.Count; i++)
                {
                    if (musicStart + i >= seq.musicSettings.Count)
                        break;

                    seq.musicSettings[musicStart + i] = CloneMusic(data.music[i]);
                }
            }

            // CHANNEL PASTE
            Channel channel = sel.GetActiveChannel();

            int channelStart = sel.GetFirstSelectedChannelIndex();

            if (channel != null && channel.CommandLines != null && channelStart >= 0)
            {
                for (int i = 0; i < data.commands.Count; i++)
                {
                    if (channelStart + i >= channel.CommandLines.Count)
                        break;

                    channel.CommandLines[channelStart + i] =
                        CloneCommand(data.commands[i]);
                }
            }
        }

        #region Data Clones
        MusicSettings CloneMusic(MusicSettings obj)
        {
            if (obj == null) return null;

            return JsonUtility.FromJson<MusicSettings>(
                JsonUtility.ToJson(obj)
            );
        }

        CommandLine CloneCommand(CommandLine obj)
        {
            if (obj == null) return null;

            return JsonUtility.FromJson<CommandLine>(
                JsonUtility.ToJson(obj)
            );
        }
        #endregion
    }
}