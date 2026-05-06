#ifdef _WIN32
#define EXPORT __declspec(dllexport)
#else
#define EXPORT
#endif

#include "sf2Playback.h"

#ifdef __cplusplus
extern "C" {
#endif

    static int g_initialized = 0;

    EXPORT int Audio_Init(const char* path, int sampleRate, float gain)
    {
        if (g_initialized)
            return 1;

        int result = initiate_tsf(path, sampleRate, gain);

        if (result)
            g_initialized = 1;

        return result;
    }

    EXPORT int Audio_Shutdown()
    {
        if (!g_initialized)
            return 0;

        uninitiate_tsf();

        g_initialized = 0;

        return 1;
    }

    EXPORT void Audio_Render(float* buffer, int frames)
    {
        if (!g_initialized)
            return;

        TsfAudioBuffer(buffer, frames);
    }

    EXPORT int Audio_PlayNote(int channel, int key, float velocity)
    {
        return PlayNote(channel, key, velocity);
    }

    EXPORT int Audio_EndNote(int channel, int key)
    {
        return EndNote(channel, key);
    }

    EXPORT int SetChannelPreset(int channel, int preset)
    {
        return Set_Channel_Preset(channel, preset);
    }

    EXPORT int GetChannelPreset(int channel)
    {
        return Get_Channel_Preset(channel);
    }

    EXPORT int GetPresetCount()
    {
        return Get_Preset_Count();
    }

#ifdef __cplusplus
}
#endif