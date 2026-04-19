using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Receives the transcribed voice command and executes a simple action:
/// changing the color of a target image component based on keywords.
/// </summary>
public class ImageColorController : MonoBehaviour
{
    [Tooltip("The UI Image component whose color will be changed.")]
    [SerializeField] private Image targetImage;
    [Tooltip("Reference to the TextMeshProUGUI to show execution status.")]
    [SerializeField] private TextMeshProUGUI messageDisplay;
    WhisperVoiceCommand whisperVoiceCommand;

    private void Start()
    {
        if (targetImage == null)
        {
            Debug.LogError("Target Image is not assigned in ImageColorController.");
        }
        whisperVoiceCommand = GetComponent<WhisperVoiceCommand>();
    }

    void Update()
    {
        HandleVoiceCommand(whisperVoiceCommand.message.text);
    }

    /// <summary>
    /// Processes the transcribed voice command and attempts to change the color.
    /// This method is called by WhisperVoiceCommand after successful transcription.
    /// </summary>
    /// <param name="command">The text transcribed by Whisper.</param>
    public void HandleVoiceCommand(string command)
    {
        if (targetImage == null) return;
        
        string lowerCommand = command.ToLowerInvariant();
        Color newColor = targetImage.color;
        string statusText = "Command executed: ";

        // Simple keyword matching for demonstration
        if (lowerCommand.Contains("red"))
        {
            newColor = Color.red;
            statusText += "Set to Red.";
        }
        else if (lowerCommand.Contains("blue"))
        {
            newColor = Color.blue;
            statusText += "Set to Blue.";
        }
        else if (lowerCommand.Contains("green"))
        {
            newColor = Color.green;
            statusText += "Set to Green.";
        }
        else if (lowerCommand.Contains("yellow"))
        {
            newColor = Color.yellow;
            statusText += "Set to Yellow.";
        }
        else
        {
            statusText = $"Command not recognized: '{command}'. Try 'Set color to red'.";
        }

        targetImage.color = newColor;
        if (messageDisplay != null)
        {
            // Use the same message display as the main script for consistency
            messageDisplay.text = statusText;
        }
        else
        {
            Debug.Log(statusText);
        }
    }
}