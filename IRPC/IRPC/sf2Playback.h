#ifndef SF2_PLAYBACK
#define SF2_PLAYBACK

#include "tsf.h"
#include "miniaudio.h"

#include <stdio.h>
#include <string.h>

/*
typedef struct {
    int channel; //0
    int preset_number; //0
    int midi_drums; //0
} ChannelPreset;
typedef struct {
    int key;
    float velocity;
} NoteInfo;*/

int initiate_tsf(const char* path, int sampleRate, float gain);
int uninitiate_tsf();

void data_callback(ma_device* device, void* output, const void* input, ma_uint32 frameCount);
void TsfAudioBuffer(float* buffer, int samples);

void PrintSF2Info();
int Set_Channel_Preset(int channel, int preset);
int Get_Channel_Preset(int channel);
int Get_Preset_Count();
int Get_Channel_Count();

int PlayNote(int channel, int key, float velocity);
int EndNote(int channel, int note);

#endif
