#define TSF_IMPLEMENTATION
#include "tsf.h"
#define MINIAUDIO_IMPLEMENTATION
#include "miniaudio.h"
#include "sf2Playback.h"

#include <stdio.h>
#include <string.h>

static tsf* g_sf = NULL;
ma_device_config config;
ma_device device;


#pragma region Initialize & De-initialize
int initiate_tsf(const char* path, int sampleRate, float gain) {
    //Sonatina_Symphonic_Orchestra.sf2
    //FluidR3_GM.sf2
    g_sf = tsf_load_filename(path);

    if (!g_sf) // Failed to load SF2 file        
        return 0;//NULL    

    tsf_set_output(g_sf, TSF_STEREO_INTERLEAVED, sampleRate, 0);
    tsf_set_volume(g_sf, gain);

    //Default preset (piano) on channel 0
    tsf_channel_set_presetnumber(g_sf, 0, 40, 0);

    config = ma_device_config_init(ma_device_type_playback);
    config.sampleRate = sampleRate;
    config.playback.format = ma_format_f32;
    config.playback.channels = 2;
    config.dataCallback = data_callback;

    if (ma_device_init(NULL, &config, &device) != MA_SUCCESS) { //Playback device failed
        tsf_close(g_sf);
        g_sf = NULL;
        return 2;
    }

    if (ma_device_start(&device) != MA_SUCCESS) { // device start not successful
        ma_device_uninit(&device);
        tsf_close(g_sf);
        g_sf = NULL;
        return 3;
    }

    return 1;
}

int uninitiate_tsf() {
    ma_device_uninit(&device);
    if (!g_sf)
        return 0;
    tsf_close(g_sf);
    g_sf = NULL;
    return 1;
}

void data_callback(ma_device* device, void* output, const void* input, ma_uint32 frameCount)
{
    (void)input;

    if (g_sf)
        tsf_render_float(g_sf, (float*)output, frameCount, 0);
    else
        memset(output, 0, frameCount * device->playback.channels * sizeof(float));
}
#pragma endregion


#pragma region Play notes
int PlayNote(int channel, int key, float velocity)
{
    if (!g_sf) return -1;

    tsf_channel_note_on( g_sf, channel, key, velocity);
    return 1;
}
int EndNote(int channel, int note)
{
    if (!g_sf) return 0;
    tsf_channel_note_off(g_sf, channel, note);
    return 1;
}
#pragma endregion


#pragma region Channels
int Set_Channel_Preset(int channel, int preset) {

    if (!g_sf) return 0;


    tsf_channel_set_presetnumber(
        g_sf, // Soundfont object
        channel, // Current track being played on
        preset, // MIDI channel number (instrument) 0 -> sf2 amount x
        0 // 0/1, should be using drums?
    );
    return preset;
}
int Get_Channel_Preset(int channel) {
    if (!g_sf) return -1;
    return tsf_channel_get_preset_index(g_sf, channel);
}
int Get_Preset_Count() {
    if (!g_sf) return -1;
    return tsf_get_presetcount(g_sf);
}
int Get_Channel_Count()
{
    if (!g_sf) return -1;
    if (!g_sf->channels)
        return 0;

    return g_sf->channels->channelNum;
}
#pragma endregion

#pragma region Old Code
void PrintSF2Info()
{
    if (!g_sf) {
        printf("SoundFont not loaded.\n");
        return;
    }

    int presetCount = tsf_get_presetcount(g_sf);

    printf("\n==== SoundFont Info ====\n");
    printf("Preset count: %d\n", presetCount);

    for (int i = 0; i < presetCount; i++) {
        const char* name = tsf_get_presetname(g_sf, i);

        printf("[%3d] %s\n",
            i, name ? name : "(null)");
    }

    printf("========================\n\n");
}

/*char* GetInstruments() {
    const int presetCount = tsf_get_presetcount(g_sf);
    char** a[presetCount];

    //allocate the array
    int** arr = new int* [row];
    for (int i = 0; i < row; i++)
        arr[i] = new int[col];

    // use the array

    //deallocate the array
    for (int i = 0; i < row; i++)
        delete[] arr[i];
    delete[] arr;
    if (!g_sf) return;

}*/

/*void TsfAudioBuffer(float* buffer, int samples)
{
    tsf_render_float(g_sf, buffer, samples, 0);
}*/

#pragma endregion