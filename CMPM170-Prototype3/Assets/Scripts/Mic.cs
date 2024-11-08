using UnityEngine;

public class VoiceDetection : MonoBehaviour
{
    public float sensitivity = 0.1f;  // Threshold to detect voice
    public int sampleWindow = 128;    // Number of samples to analyze
    private AudioClip microphoneClip;

    void Start()
    {
        // Start recording from the default microphone
        microphoneClip = Microphone.Start(null, true, 1, 44100);
    }

    void Update()
    {
        if (Microphone.IsRecording(null))
        {
            float volumeLevel = GetMaxVolume();
            if (volumeLevel > sensitivity)
            {
                Debug.Log("Voice Detected!");
            }
        }
    }

    float GetMaxVolume()
    {
        float[] samples = new float[sampleWindow];
        microphoneClip.GetData(samples, Microphone.GetPosition(null) - sampleWindow);
        float maxVolume = 0f;

        foreach (float sample in samples)
        {
            if (sample > maxVolume)
            {
                maxVolume = sample;
            }
        }
        return maxVolume;
    }
}

