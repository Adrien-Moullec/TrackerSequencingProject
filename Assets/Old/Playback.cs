using UnityEngine;/*
using MeltySynth;
using Unity.VisualScripting;

public class Playback : MonoBehaviour
{
    // Create the synthesizer.
    static int sampleRate = 44100;
    Synthesizer synthesizer = new Synthesizer("Assets/Sonatina_Symphonic_Orchestra.sf2", sampleRate);

    private void Start()
    {
        //synthesizer.ChannelCount
        // Play some notes (middle C, E, G).
        synthesizer.NoteOn(0, 60, 100);
        synthesizer.NoteOn(0, 64, 100);
        synthesizer.NoteOn(0, 67, 100);

        // The output buffer (3 seconds).
        float[] left = new float[3 * sampleRate];
        float[] right = new float[3 * sampleRate];

        // Render the waveform.
        synthesizer.Render(left, right);
    }


    /*
    [SerializeField] AudioSource audioSource;
    double startTime;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        startTime = AudioSettings.dspTime + 5.0; // schedule 1 second ahead
        audioSource.PlayScheduled(startTime);
    }*/
//}
