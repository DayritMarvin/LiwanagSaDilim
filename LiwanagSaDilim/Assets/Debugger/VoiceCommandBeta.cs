using OpenAI;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.Android;

/// <summary>
/// The primary script for the voice command beta.
/// Records audio, transcribes it using Whisper, and sends the text to a command controller.
/// IMPORTANT: This version adds a public field for the OpenAI API Key to be set in the Unity Inspector.
/// </summary>
public class WhisperVoiceCommand : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Enter your OpenAI API Key here to ensure the model can be called. Example: sk-xxxxxxxxxxxxxxxxx")]
    // NEW: Public field for the API Key for easy Inspector setup.
    [SerializeField] private string openAiApiKey = ""; 
    
    [Header("UI References")]
    [Tooltip("Button to start the recording.")]
    [SerializeField] private Button recordButton;
    [Tooltip("UI Image component to show the recording progress.")]
    [SerializeField] private Image progressBar;
    [Tooltip("UI Text component to display the transcription result.")]
    [SerializeField] public TextMeshProUGUI message;

    [Header("Integration")]
    [Tooltip("The script that processes the final text command (ImageColorController).")]
    [SerializeField] private ImageColorController colorController;
    
    // Configuration constants
    private readonly string fileName = "output.wav";
    private readonly int duration = 5; // Recording duration in seconds
    private const int sampleRate = 44100;

    // State
    private AudioClip clip;
    private bool isRecording = false;
    private float time = 0f;
    private OpenAIApi openai; // Now initialized in Start()

    void Awake()
    {
        if (Application.isMobilePlatform)
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
        }
    }

    private void Start()
    {
        // 1. Initialize the OpenAIApi with the key provided in the Inspector
        if (string.IsNullOrEmpty(openAiApiKey))
        {
            message.text = "ERROR: API Key is missing. Please enter it in the Inspector.";
            Debug.LogError("WhisperVoiceCommand: OpenAI API Key is empty. Please enter it in the Inspector.");
            // If the key is missing, fall back to default initialization which might still fail
            openai = new OpenAIApi(); 
        }
        else
        {
            // Initialize the API explicitly with the provided key.
            openai = new OpenAIApi(openAiApiKey);
            message.text = "Press 'Speak' to record a command.";
        }

        progressBar.fillAmount = 0f;
        
        // 2. Setup button listener
        if (recordButton != null)
        {
            recordButton.onClick.AddListener(StartRecording);
        }
        else
        {
            Debug.LogError("Record Button not assigned in WhisperVoiceCommand.");
        }
    }

    private void StartRecording()
    {
        // Guard clause to prevent recording if the API key is known to be missing or if already recording.
        if (isRecording || string.IsNullOrEmpty(openAiApiKey)) 
        {
            if (string.IsNullOrEmpty(openAiApiKey))
            {
                message.text = "Cannot start: API Key is missing.";
            }
            return;
        }
        
        message.text = "Listening for 5 seconds...";
        isRecording = true;
        recordButton.enabled = false;
        time = 0f;

        // Automatically select the first available microphone
        string micDeviceName = null;
        if (Microphone.devices.Length > 0)
        {
            micDeviceName = Microphone.devices.FirstOrDefault();
            Debug.Log($"Recording started with device: {micDeviceName}");
        }
        
        #if !UNITY_WEBGL
        clip = Microphone.Start(micDeviceName, false, duration, sampleRate);
        #else
        // This package is generally not intended for WebGL, but we can prevent a crash.
        message.text = "Recording not supported on WebGL.";
        EndRecording(true); 
        #endif
    }

    private async void EndRecording(bool aborted = false)
    {
        isRecording = false;
        recordButton.enabled = true; // Re-enable button

        if (aborted)
        {
            progressBar.fillAmount = 0;
            return;
        }

        message.text = "Transcribing...";
        
        #if !UNITY_WEBGL
        // Stop the microphone recording
        Microphone.End(null);
        #endif
        
        // Check for clip before proceeding (in case of an immediate failure/error)
        if (clip == null)
        {
            message.text = "Error: Could not capture audio clip.";
            return;
        }

        // Convert the AudioClip to a WAV byte array using the SaveWav utility
        byte[] data = SaveWav.Save(fileName, clip);
        
        // Create the transcription request payload
        var req = new CreateAudioTranscriptionsRequest
        {
            FileData = new FileData() {Data = data, Name = "audio.wav"},
            Model = "whisper-1", // Use the Whisper model
            Language = "en"      // Specify English language
        };

        // try
        // {
            // Call the OpenAI API
            var res = await openai.CreateAudioTranscription(req);

            // Update UI with result
            progressBar.fillAmount = 0;
            message.text = $"Result: {res.Text}";

        //     // Pass the transcription result to the command execution script
        //     if (colorController != null)
        //     {
        //         colorController.HandleVoiceCommand(res.Text);
        //     }
        //     else
        //     {
        //         Debug.LogWarning("Color Controller not assigned. Cannot execute command, but transcription was successful.");
        //     }
        // }
        // catch (System.Exception e)
        // {
        //     Debug.LogError($"OpenAI API Error during transcription: {e.Message}");
        //     message.text = "Transcription Failed. Check API Key/Console for details.";
        // }
    }

    private void Update()
    {
        if (isRecording)
        {
            time += Time.deltaTime;
            // Update progress bar fill amount
            progressBar.fillAmount = time / duration;
            
            // Automatically stop recording after the set duration
            if (time >= duration)
            {
                EndRecording();
            }
        }
    }
}