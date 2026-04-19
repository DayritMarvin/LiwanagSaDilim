using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Utility class to convert a Unity AudioClip into a byte array representing a WAV file.
/// This is crucial for sending audio data to the OpenAI Whisper API.
/// </summary>
public static class SaveWav
{
    // A standard 44.1 kHz, 16-bit mono WAV header size is 44 bytes.
    private const int HeaderSize = 44;

    /// <summary>
    /// Converts a Unity AudioClip into a byte array of WAV data.
    /// </summary>
    /// <param name="filename">The name to use for the file.</param>
    /// <param name="clip">The AudioClip to convert.</param>
    /// <returns>A byte array containing the WAV file data.</returns>
    public static byte[] Save(string filename, AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("SaveWav: Input AudioClip is null.");
            return null;
        }

        // 1. Get the raw float samples from the AudioClip
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        using (var stream = new MemoryStream())
        {
            // 2. Write the WAV header and audio data to the memory stream.
            WriteWavHeader(stream, clip.samples, clip.channels, clip.frequency);
            WriteSamples(stream, samples);

            // 3. Convert the memory stream to a byte array for the API call.
            return stream.ToArray();
        }
    }

    /// <summary>
    /// Writes the 44-byte WAV header to the stream.
    /// </summary>
    private static void WriteWavHeader(MemoryStream stream, int totalSamples, int channels, int sampleRate)
    {
        // Calculate data sizes for 16-bit PCM (standard)
        int bytesPerSample = 2; 
        int totalAudioLength = totalSamples * channels * bytesPerSample;
        int totalFileLength = totalAudioLength + HeaderSize - 8;

        var writer = new BinaryWriter(stream);

        // RIFF chunk
        writer.Write(new char[] { 'R', 'I', 'F', 'F' }); // Chunk ID: 'RIFF'
        writer.Write(totalFileLength);                   // Chunk Size: (Total file size - 8)
        writer.Write(new char[] { 'W', 'A', 'V', 'E' }); // Format: 'WAVE'

        // fmt chunk
        writer.Write(new char[] { 'f', 'm', 't', ' ' }); // Sub-chunk 1 ID: 'fmt '
        writer.Write(16);                                // Sub-chunk 1 Size (16 for PCM)
        writer.Write((ushort)1);                         // Audio Format (1 for PCM)
        writer.Write((ushort)channels);                  // Number of Channels
        writer.Write(sampleRate);                        // Sample Rate (e.g., 44100)
        writer.Write(sampleRate * channels * bytesPerSample); // Byte Rate
        writer.Write((ushort)(channels * bytesPerSample)); // Block Align
        writer.Write((ushort)(bytesPerSample * 8));      // Bits per Sample (16)

        // data chunk
        writer.Write(new char[] { 'd', 'a', 't', 'a' }); // Sub-chunk 2 ID: 'data'
        writer.Write(totalAudioLength);                  // Sub-chunk 2 Size
    }

    /// <summary>
    /// Converts and writes the floating-point audio samples as 16-bit PCM data.
    /// </summary>
    private static void WriteSamples(MemoryStream stream, float[] samples)
    {
        var writer = new BinaryWriter(stream);

        // Convert float samples (-1.0 to 1.0) to Int16 samples (-32768 to 32767)
        for (int i = 0; i < samples.Length; i++)
        {
            short sample = (short)(samples[i] * short.MaxValue);
            writer.Write(sample);
        }
    }
}