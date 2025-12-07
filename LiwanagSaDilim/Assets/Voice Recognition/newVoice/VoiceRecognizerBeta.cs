// VoiceRecognizerBeta.cs - Vosk-based replacement for Android Native API

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vosk; // Vosk API namespace
using TMPro;

public class VoiceRecognizerBeta : MonoBehaviour
{
    // --- Public Interface ---
    public event Action<string, List<string>> OnRecognized;
    [SerializeField] TextMeshProUGUI statusText;

    // --- Vosk and Audio Fields ---
    private Model model;
    private VoskRecognizer recognizer;
    private AudioClip microphoneClip;
    private string microphoneDeviceName;
    private bool isEngineActive = false;
    
    // Vosk requires 16000Hz (16kHz) and 16-bit mono audio
    private const int SAMPLE_RATE = 16000;
    private const int BLOCK_SIZE = 4096; // Chunk size for processing

    // --- Configuration (Grammar List) ---
    // Vosk will only listen for these words/phrases (case-insensitive)
    private readonly string[] commandVocabulary = new string[] 
    { 
        "start", "stop", "change color", "red", "blue", "green", "attack", "run", "pause", "go", "halt"
    };

    // --- Unity Lifecycle ---
    private void Awake()
    {
        // Suppress Vosk console spam
        Vosk.Vosk.SetLogLevel(0); 

        // Select the default microphone
        if (Microphone.devices.Length > 0)
        {
            microphoneDeviceName = Microphone.devices[0];
            LogStatus("Microphone: " + microphoneDeviceName + " selected.");
        }
        else
        {
            LogStatus("Error: No microphone device found!", true);
        }
    }

    private void Start()
    {
        // Request Microphone Permission (Non-blocking, fixes the previous 'await' type issue)
        if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            Application.RequestUserAuthorization(UserAuthorization.Microphone);
            LogStatus("Requesting mic permission...");
        }

        InitializeRecognizer();
        // Start listening automatically as per your previous implementation
        StartContinuousListening(); 
    }

    private void OnDestroy()
    {
        StopContinuousListening();
        recognizer?.Dispose();
        model?.Dispose();
    }
    
    // -------------------------------------------------------------
    // INITIALIZATION
    // -------------------------------------------------------------
    private void InitializeRecognizer()
    {
        try
        {
            // Path to your downloaded Vosk model (adjust the folder name if needed)
            string modelPath = Application.streamingAssetsPath + "/vosk-model-small-en-us-0.22"; 
            
            // 1. Load Vosk Model
            model = new Model(modelPath);
            LogStatus("Vosk Model Loaded: " + modelPath);

            // 2. Create constrained grammar JSON
            string grammarJson = CreateGrammar(commandVocabulary);
            
            // 3. Initialize Recognizer
            recognizer = new VoskRecognizer(model, SAMPLE_RATE, grammarJson);
            recognizer.SetMaxAlternatives(5);
            recognizer.SetWords(false);
            
            LogStatus("Vosk Recognizer Ready");
        }
        catch (Exception e)
        {
            LogStatus($"Vosk Init Error: {e.Message}. Check model path and plugin setup.", true);
        }
    }

    // -------------------------------------------------------------
    // CONTINUOUS LISTENING CONTROL
    // -------------------------------------------------------------
    public void StartContinuousListening()
    {
        if (isEngineActive || model == null) return;

        isEngineActive = true;
        
        // Start Unity Microphone recording into an in-memory AudioClip
        microphoneClip = Microphone.Start(microphoneDeviceName, true, 10, SAMPLE_RATE);
        
        StartCoroutine(ProcessAudioCoroutine());
        LogStatus("Listening...");
    }

    public void StopContinuousListening()
    {
        if (!isEngineActive) return;

        isEngineActive = false;
        StopCoroutine(ProcessAudioCoroutine());
        Microphone.End(microphoneDeviceName);
        
        LogStatus("Voice Recognizer Stopped.");
        
        // Flush any remaining audio to get the final result
        HandleResult(recognizer.FinalResult()); 
    }

    // -------------------------------------------------------------
    // AUDIO PROCESSING COROUTINE
    // -------------------------------------------------------------
    private IEnumerator ProcessAudioCoroutine()
    {
        int audioPosition = 0;
        float[] audioChunk = new float[BLOCK_SIZE];
        byte[] voskInput = new byte[BLOCK_SIZE * 2]; // 16-bit audio = 2 bytes per float

        // Wait until microphoneClip is not null and has started recording
        while (Microphone.GetPosition(microphoneDeviceName) == 0)
        {
            yield return null;
        }

        while (isEngineActive)
        {
            yield return null; 

            int newPosition = Microphone.GetPosition(microphoneDeviceName);
            int samplesToRead = newPosition - audioPosition;

            // Handle wrap-around for the continuous mic buffer
            if (samplesToRead < 0)
            {
                samplesToRead = (microphoneClip.samples - audioPosition) + newPosition;
            }

            // Only process if we have a full block of new data
            if (samplesToRead >= BLOCK_SIZE)
            {
                // Read float data from the AudioClip
                microphoneClip.GetData(audioChunk, audioPosition);

                // Convert float[] to Vosk's required 16-bit PCM byte[]
                ConvertFloatToPcm(audioChunk, voskInput);

                // Core speech recognition call.
                if (recognizer.AcceptWaveform(voskInput, voskInput.Length))
                {
                    HandleResult(recognizer.Result());
                }
                
                // Update the position
                audioPosition = (audioPosition + BLOCK_SIZE) % microphoneClip.samples;
            }
        }
    }
    
    // -------------------------------------------------------------
    // RESULT HANDLER AND EMITTER
    // -------------------------------------------------------------
    private void HandleResult(string jsonResult)
    {
        if (string.IsNullOrEmpty(jsonResult)) return;

        try
        {
            var result = JsonUtility.FromJson<VoskResult>(jsonResult);
            
            if (!string.IsNullOrEmpty(result.text))
            {
                string topResult = result.text.Trim().ToLower();
                
                // Get alternative candidates
                List<string> candidates = new List<string>();
                if (result.alternatives != null)
                {
                    foreach (var alt in result.alternatives)
                    {
                        candidates.Add(alt.text.Trim().ToLower());
                    }
                }
                
                EmitResult(topResult, candidates);
            }
        }
        catch (Exception e)
        {
            LogStatus($"JSON Parsing Error: {e.Message}", true);
        }
    }

    private void EmitResult(string top, List<string> candidates)
    {
        Debug.Log($"[Speech] Top='{top}' Candidates=[{string.Join(",", candidates)}]");
        OnRecognized?.Invoke(top, candidates);
        LogStatus($"Recognized: {top}"); 
    }

    // -------------------------------------------------------------
    // COMMAND MAPPING (Kept for compatibility with VoiceCommandBeta.cs)
    // -------------------------------------------------------------
    public static string MapWordToCommand(string word)
    {
        // Uses your mapping logic from the previous file
        switch (word.ToLower())
        {
            case "start":
            case "go":
                return "COMMAND_START";
            case "stop":
            case "halt":
            case "pause":
                return "COMMAND_STOP";
            case "green":
                return "COLOR_GREEN";
            case "blue":
                return "COLOR_BLUE";
            case "red":
                return "COLOR_RED";
            case "change color":
                // This is an example of a multi-word phrase
                return "COLOR_CYCLE";
            default:
                return "UNKNOWN";
        }
    }
    
    // -------------------------------------------------------------
    // HELPER FUNCTIONS
    // -------------------------------------------------------------

    // Creates the Vosk-required grammar JSON array string: ["word 1", "word 2"]
    private string CreateGrammar(string[] words)
    {
        return $"[\"{string.Join("\", \"", words)}\"]";
    }

    // Converts Unity's float audio to Vosk's required 16-bit PCM byte array
    private void ConvertFloatToPcm(float[] floatArray, byte[] byteArray)
    {
        int bytes = 0;
        for (int i = 0; i < floatArray.Length; i++)
        {
            short s = (short) (floatArray[i] * 32767);
            
            byteArray[bytes++] = (byte)(s & 0xff);
            byteArray[bytes++] = (byte)(s >> 8);
        }
    }

    private void LogStatus(string msg, bool isError = false)
    {
        string prefix = isError ? "[ERROR] Vosk: " : "Vosk: ";
        Debug.Log(prefix + msg);
        if (statusText != null)
        {
            statusText.text = prefix + msg;
            if (isError) statusText.color = Color.red;
            else statusText.color = Color.white;
        }
    }

    // Vosk JSON structure helpers for JsonUtility
    [System.Serializable]
    public class Alternative
    {
        public string text;
    }

    [System.Serializable]
    public class VoskResult
    {
        public string text; 
        public List<Alternative> alternatives; 
    }
}