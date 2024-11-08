using UnityEngine;
using System.Collections;

public class VoiceDetection : MonoBehaviour
{
    public float sensitivity = 0.1f;  // Threshold to detect voice above this decibel level
    public int sampleWindow = 128;    // Number of samples to analyze
    public Transform player;          // Reference to the player's transform
    public float moveSpeed = 5f;      // Movement speed multiplier

    private AudioClip microphoneClip;
    private bool micInitialized = false;

    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            // Start recording from the default microphone
            microphoneClip = Microphone.Start(null, true, 10, 44100);
            StartCoroutine(WaitForMicrophoneInitialization());
        }
        else
        {
            Debug.LogError("No microphone detected!");
        }
    }

    void Update()
    {
        if (!micInitialized)
        {
            Debug.LogWarning("Microphone buffer not ready. Waiting for sufficient data.");
            return;
        }

        if (player == null)
        {
            Debug.LogError("Player transform not assigned!");
            return;
        }

        float decibelLevel = GetDecibelLevel();
        Debug.Log($"Decibel Level: {decibelLevel} dB, Sensitivity Threshold: {sensitivity}");

        // Check if the decibel level exceeds the sensitivity and is positive
        if (decibelLevel > sensitivity && decibelLevel > 0)
        {
            Debug.Log($"Voice Detected! Decibel Level: {decibelLevel} dB");
            MovePlayer(decibelLevel);
        }
    }

    IEnumerator WaitForMicrophoneInitialization()
    {
        // Wait for the microphone buffer to accumulate enough data
        while (Microphone.GetPosition(null) <= 0 || Microphone.GetPosition(null) < sampleWindow)
        {
            yield return null;  // Wait for the next frame
        }

        yield return new WaitForSeconds(0.1f);  // Additional buffer delay for stability
        micInitialized = true;
        Debug.Log("Microphone initialized and buffer ready.");
    }

    float GetDecibelLevel()
    {
        if (microphoneClip == null)
            return -80f;  // Return minimum decibel if no clip is available

        int micPosition = Microphone.GetPosition(null);
        int startPosition = micPosition - sampleWindow;

        // Ensure the start position is valid
        if (startPosition < 0)
        {
            return -80f; // Skip processing if there's insufficient data
        }

        float[] samples = new float[sampleWindow];
        microphoneClip.GetData(samples, startPosition);

        float sum = 0f;
        foreach (float sample in samples)
        {
            sum += sample * sample;  // Square the amplitude to get power
        }

        float rmsValue = Mathf.Sqrt(sum / sampleWindow);  // Root Mean Square
        float decibel = 20 * Mathf.Log10(rmsValue);       // Convert to decibels

        return Mathf.Clamp(decibel, -80f, 0f);  // Clamp decibel value between -80 and 0
    }

    void MovePlayer(float decibelLevel)
    {
        // Use only positive decibel levels for movement
        float normalizedLevel = decibelLevel / 0f;  // Scale to 0-1
        float movement = normalizedLevel * moveSpeed * Time.deltaTime;

        Debug.Log($"Moving player by: {movement}");

        // Move the player along the Z-axis for demonstration
        player.Translate(Vector3.forward * movement);
    }
}