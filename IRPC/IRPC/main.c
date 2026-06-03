#ifdef _WIN32
#define EXPORT __declspec(dllexport)
#else
#define EXPORT
#endif

#include "sf2Playback.h"

#ifdef __cplusplus
extern "C" {
#endif

    static int Sf2Initialized = 0; // Consider making this a callable function instead?

    /// Initialize audio using sf2 file
    EXPORT int Tracker_Initialize(const char* path, int sampleRate, float gain)
    {
        if (Sf2Initialized)
            return 1;

        int result = InitiateTsf(path, sampleRate, gain);
        if (result) Sf2Initialized = 1;

        return result;
    }

    /// Uninitialize audio shutdown
    EXPORT int Tracker_Shutdown()
    {
        if (!Sf2Initialized)
            return 0;

        UnitiateTsf();
        Sf2Initialized = 0;
        return 1;
    }

    /// Get initialized state
    EXPORT int Tracker_GetInitializedState()
    {
        return Sf2Initialized;
    }

    /// Audio sample pass-through
    EXPORT void Tracker_AudioRender(float* buffer, int frames)
    {
        if (!Sf2Initialized)
            return;

        TsfAudioBuffer(buffer, frames);
    }

    /// Play note by channel and press-velocity
    EXPORT int Tracker_PlayNote(int channel, int key, float velocity)
    {
        return PlayNote(channel, key, velocity);
    }

    /// End note in channel
    EXPORT int Tracker_EndNote(int channel, int key)
    {
        return EndNote(channel, key);
    }

    /// Set instrument preset in channel
    EXPORT int Tracker_SetChannelPreset(int channel, int preset)
    {
        return SetChannelPreset(channel, preset);
    }

    /// Set bank preset in channel
    EXPORT int Tracker_SetBankPreset(int channel, int bank, int preset)
    {
        return SetBankPreset(channel, bank, preset);
    }

    /// Get channel instrument preset
    EXPORT int Tracker_GetChannelPreset(int channel)
    {
        return GetChannelPreset(channel);
    }

    /// Get preset count in tsf file
    EXPORT int Tracker_GetPresetCount()
    {
        return GetPresetCount();
    }

#ifdef __cplusplus
}
#endif