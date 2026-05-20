#ifndef SF2_PLAYBACK
#define SF2_PLAYBACK

#include "tsf.h"
#include "miniaudio.h"

#include <stdio.h>
#include <string.h>

int InitiateTsf(const char* path, int sampleRate, float gain);
int UnitiateTsf();

void TsfAudioBuffer(float* buffer, int samples);

int PlayNote(int channel, int key, float velocity);
int EndNote(int channel, int note);

int SetChannelPreset(int channel, int preset);
int SetBankPreset(int channel, int bank, int preset);

int GetChannelPreset(int channel);
int GetPresetCount();
int GetChannelCount();

#endif
