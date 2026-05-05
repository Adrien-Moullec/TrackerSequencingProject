#ifdef _WIN32
#define EXPORT __declspec(dllexport)
#else
#define EXPORT
#endif

#include "sf2Playback.h"
#include <stdio.h>

void PrintSF2Info();

int initiate_tsf();
int uninitiate_tsf();

int PlayNote(int channel, int key, float velocity);
int EndNote(int channel, int note);

void TsfAudioBuffer(float* buffer, int samples);
int Set_Channel_Preset(int channel, int preset);
int Get_Channel_Preset(int channel);
int Get_Preset_Count();
int Get_Channel_Count();

#ifdef __cplusplus
extern "C" {
#endif

    /*
    *** Important Links ***
    * BUFFERS
    Audio quality through buffers - https://discussions.unity.com/t/passing-audio-buffers-between-audiosources/667229/6 *
    
    * ALLOCATING ARRAYS
    https://stackoverflow.com/questions/9219712/c-array-expression-must-have-a-constant-value
    */


#pragma region Initialize & De-initialize

    static int g_initialized = 0;

    EXPORT int Audio_Init(const char* path, int sampleRate, float gain)
    {
        if (g_initialized == 1)
            return 0;

        int test = initiate_tsf(path, sampleRate, gain);
        if (test == 1)
            g_initialized = 1;
        return test;
    }

    EXPORT int Audio_Shutdown()
    {
        if (!g_initialized)
            return 0;

        uninitiate_tsf();
        g_initialized = 0;
        return 1;
    }

#pragma endregion

#pragma region Initialize & De-initialize
    EXPORT int SetChannelPreset(int channel, int presetNumber) {
        if (!g_initialized)
            return 0;

        return Set_Channel_Preset(channel, presetNumber);
    }
    EXPORT int GetChannelPreset(int channel) {
        return Get_Channel_Preset(channel);
    }
    EXPORT int GetPresetCount() {
        return Get_Preset_Count();
    }
    EXPORT int GetChannelCount() {
        return Get_Channel_Count();
    }

#pragma endregion


#pragma region Note playback logic

    EXPORT int Audio_PlayNote(int channel, int key, float velocity)
    {
        if (!g_initialized)
            return -1;

        return PlayNote(channel, key, velocity);
    }

    EXPORT int Audio_EndNote(int channel, int key)
    {
        if (!g_initialized)
            return 0;

        return EndNote(channel, key);
    }

#pragma endregion


#pragma region Old Code

    EXPORT int Test(int input1, int input2)
    {
        return input1 + input2;
    }

    /*EXPORT int Audio_Render(float* buffer, int samples)
    {
        if (!g_initialized)
            return 0;
        TsfAudioBuffer(buffer, samples);
        return 1;
    }*/
#pragma endregion

#ifdef __cplusplus
}
#endif