using OpenAI;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.Android;
using System.Collections.Generic;

public class PlayerVoiceCommand : MonoBehaviour
{
    [Header("OpenAI Configuration")]
    [SerializeField] private string openAiApiKey = ""; 
    [SerializeField] private string modelName = "whisper-1";

    [Header("Voice Settings")]
    [Tooltip("How many seconds each recording segment lasts. 3-4s is usually best for responsiveness.")]
    [SerializeField] private int segmentDuration = 4; 

    [Header("Integration")]
    [Tooltip("Drag the Player object with the PlayerMovements script here.")]
    PlayerMovements movements;
    [SerializeField] private VoiceCommandDatabase voiceDb;

    private OpenAIApi openai;
    private AudioClip clip;
    private bool isListening = false;
    private float timer = 0f;
    private const int sampleRate = 44100;
    private readonly string tempFileName = "temp_segment.wav";

    void Awake()
    {
        // Ensure microphone permissions are handled before Start
        if (Application.isMobilePlatform && !Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
    }

    private void Start()
    {
        movements = GetComponent<PlayerMovements>();

         if (string.IsNullOrEmpty(openAiApiKey))
        {
            Debug.LogError("Voice Command Error: Missing API Key! Always-listening will not start.");
            return;
        }
        Debug.Log("Always-Listening Can be activated.");
    }

    public void Active()
    {
        if(isListening) return;
        Invoke(nameof(StartVoiceCommand), .5f); // Delay to ensure everything is initialized
    }

    void StartVoiceCommand()
    {
        openai = new OpenAIApi(openAiApiKey);
        
        // // AUTO-START LISTENING
        isListening = true;
        Debug.Log("Always-Listening Mode: Activated.");
        StartNewRecordingSegment();
    }

    public void StartNewRecordingSegment()
    {
        if (!isListening) return;

        timer = 0f;
        string mic = Microphone.devices.Length > 0 ? Microphone.devices[0] : null;
        
        // Non-looping recording clip
        clip = Microphone.Start(mic, false, segmentDuration, sampleRate);
    }

    public void FixedUpdate()
    {
        if (isListening && Microphone.IsRecording(null))
        {
            timer += Time.deltaTime;

            if (timer >= segmentDuration)
            {
                // Capture the current clip and IMMEDIATELY start the next one 
                // to minimize the "gap" where the mic isn't hearing anything.
                AudioClip recordedClip = clip;
                StartNewRecordingSegment(); 
                
                // Process the audio in a background task so the game doesn't lag
                _ = ProcessAndFilterAudio(recordedClip);
            }
        }
    }

    private async System.Threading.Tasks.Task ProcessAndFilterAudio(AudioClip segment)
    {
        if (segment == null || voiceDb == null) return;

        byte[] data = SaveWav.Save(tempFileName, segment);
        var request = new CreateAudioTranscriptionsRequest
        {
            FileData = new FileData() { Data = data, Name = "audio.wav" },
            Model = modelName,
            Language = "en"
        };

        try
        {
            var response = await openai.CreateAudioTranscription(request);
            if (string.IsNullOrEmpty(response.Text)) return;

            string transcript = response.Text.ToLower().Trim();
            string masterWakeWord = voiceDb.wakeWord.ToLower();

            // --- WAKE WORD CHECK ---
            if (!transcript.Contains(masterWakeWord))
            {
                // The wake word wasn't heard, so we ignore everything else.
                return; 
            }

            Debug.Log($"Wake Word Detected! Full phrase: {transcript}");

            // --- DATABASE SEARCH LOGIC (Only runs if Wake Word was heard) ---
            foreach (var mapping in voiceDb.mappings)
            {
                if (mapping.keywords.Any(key => transcript.Contains(key.ToLower())))
                {
                    // Safety check: Don't trigger if already active
                    if (movements != null && movements.currentPower != mapping.power)
                    {
                        TriggerPower(mapping.power);
                        return; 
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Whisper Error: {e.Message}");
        }
    }

    private void TriggerPower(PowerUpType power)
    {
        if (movements != null)
        {
            Debug.Log($"Power Triggered: {power}");
            movements.ImprovedActivatePower(power);
        }
    }
}

[System.Serializable]
public class VoiceMapping
{
    public PowerUpType power;
    public List<string> keywords; // Example: "red", "strength", "power"
}

[CreateAssetMenu(fileName = "VoiceDatabase", menuName = "ScriptableObjects/VoiceDatabase")]
public class VoiceCommandDatabase : ScriptableObject
{
    public string wakeWord = "light";
    public List<VoiceMapping> mappings;
}