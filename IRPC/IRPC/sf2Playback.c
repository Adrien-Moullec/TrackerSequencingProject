#define TSF_IMPLEMENTATION
#include "tsf.h"
#include "sf2Playback.h"

#include <string.h>

static tsf* sf2 = NULL;

/// <summary>
/// Retrieve an sf2 file
/// </summary>
/// <param name="path"> path to sf2 file </param>
/// <param name="sampleRate"> sample rate of audio device </param>
/// <param name="gain"> desired audio gain </param>
/// <returns> pass or fail in loading sf2 file </returns>
int InitiateTsf(const char* path, int sampleRate, float gain)
{
    /// Check if file is loaded
    if (sf2) return 1;
    sf2 = tsf_load_filename(path);
    if (!sf2) return 0;
    
    /// Set sf2 file settings
    tsf_set_output(sf2, TSF_STEREO_INTERLEAVED, sampleRate, 0);
    tsf_set_volume(sf2, gain);

    return 1;
}

/// <summary>
/// Cleanly unload sf2 file
/// </summary>
/// <returns> pass or fail in unloading sf2 file </returns>
int UnitiateTsf()
{
    //if (!sf2) return 0;

    tsf_close(sf2);
    sf2 = NULL;
    return 1;
}

/// <summary>
/// Pass audio samples relating to audio device and sf2 file
/// </summary>
/// <param name="buffer"> Audio data </param>
/// <param name="frames"> Frame data </param>
void TsfAudioBuffer(float* buffer, int frames)
{
    if (!sf2)
    {
        memset(buffer, 0, sizeof(float) * frames * 2);
        return;
    }

    tsf_render_float(sf2, buffer, frames, 0);
}

/// <summary>
/// Play a note in a channel with a certain velocity
/// </summary>
/// <param name="channel"> The channel the audio is played in </param>
/// <param name="key"> The note that is played as a 0-127 value </param>
/// <param name="velocity"> The velocity that a midi-note is 'pressed' </param>
/// <returns> pass or fail in playing a note </returns>
int PlayNote(int channel, int key, float velocity)
{
    if (!sf2) return 0;
    if (channel < 0 || channel > 15) return 0;
    if (key < 0 || key > 127) return 0;

    tsf_channel_note_on(sf2, channel, key, velocity);

    return 1;
}

/// <summary>
/// End a note on a channel
/// </summary>
/// <param name="channel"> Channel target ID </param>
/// <param name="key"> key name to stop </param>
/// <returns> pass or fail in stopping a note </returns>
int EndNote(int channel, int key)
{
    if (!sf2) return 0;
    tsf_channel_note_off(sf2, channel, key);
    return 1;
}

/// <summary>
/// Set a channel's instrument by sf2 preset ID
/// </summary>
/// <param name="channel"> Channel target ID </param>
/// <param name="preset"> Instrument preset ID </param>
/// <returns> Pass/fail when setting channel preset </returns>
int SetChannelPreset(int channel, int preset)
{
    if (!sf2) return 0;
    tsf_channel_set_presetnumber(sf2, channel, preset, 0);
    return 1;
}

/// <summary>
/// Set the Bank (instrument library) preset
/// </summary>
/// <param name="channel"> Channel target ID </param>
/// <param name="bank"> Target bank ID </param>
/// <param name="preset"> Desired instrument preset </param>
/// <returns> pass or fail in setting bank + instrument preset </returns>
int SetBankPreset(int channel, int bank, int preset) {
    if (!sf2) return -1;
    return tsf_channel_set_bank_preset(sf2, channel, bank, preset);
}

/// <summary>
/// Gets the ID of the channel preset of sf2 file
/// </summary>
/// <param name="channel"> Target channel ID </param>
/// <returns> SF2 file instrument preset at "channel" </returns>
int GetChannelPreset(int channel)
{
    if (!sf2) return -1;
    return tsf_channel_get_preset_index(sf2, channel);
}

/// <summary>
/// Gets the preset count of the sf2 file
/// </summary>
/// <returns> Preset Count </returns>
int GetPresetCount()
{
    if (!sf2) return 0;
    return tsf_get_presetcount(sf2);
}