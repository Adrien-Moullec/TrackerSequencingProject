#define TSF_IMPLEMENTATION
#include "tsf.h"
#include "sf2Playback.h"

#include <string.h>

static tsf* g_sf = NULL;

int initiate_tsf(const char* path, int sampleRate, float gain)
{
    if (g_sf)
        return 1;

    g_sf = tsf_load_filename(path);

    if (!g_sf)
        return 0;

    tsf_set_output(g_sf, TSF_STEREO_INTERLEAVED, sampleRate, 0);
    tsf_set_volume(g_sf, gain);

    return 1;
}

int uninitiate_tsf()
{
    if (!g_sf)
        return 0;

    tsf_close(g_sf);
    g_sf = NULL;

    return 1;
}

void TsfAudioBuffer(float* buffer, int frames)
{
    if (!g_sf)
    {
        memset(buffer, 0, sizeof(float) * frames * 2);
        return;
    }

    tsf_render_float(g_sf, buffer, frames, 0);
}

int PlayNote(int channel, int key, float velocity)
{
    if (!g_sf)
        return 0;

    if (channel < 0 || channel > 15)
        return 0;

    if (key < 0 || key > 127)
        return 0;

    tsf_channel_note_on(g_sf, channel, key, velocity);

    return 1;
}

int EndNote(int channel, int key)
{
    if (!g_sf)
        return 0;

    tsf_channel_note_off(g_sf, channel, key);

    return 1;
}

int Set_Channel_Preset(int channel, int preset)
{
    if (!g_sf)
        return 0;

    tsf_channel_set_presetnumber(g_sf, channel, preset, 0);

    return 1;
}

int Get_Channel_Preset(int channel)
{
    if (!g_sf)
        return -1;

    return tsf_channel_get_preset_index(g_sf, channel);
}

int Get_Preset_Count()
{
    if (!g_sf)
        return 0;

    return tsf_get_presetcount(g_sf);
}