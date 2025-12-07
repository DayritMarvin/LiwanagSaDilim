// VoiceCommandBeta.cs (Content from your upload - No Changes Required)

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; 

public class VoiceCommandBeta : MonoBehaviour
{
    // The class name remains the same, so this works!
    VoiceRecognizerBeta recognizer; 
    [SerializeField] TextMeshProUGUI textCommand;

    // NEW FIELD: Reference to the image's Renderer component
    [SerializeField] Image targetRenderer; 

    void Start()
    {
        recognizer = FindObjectOfType<VoiceRecognizerBeta>();
        if (recognizer != null)
        {
            recognizer.OnRecognized += OnRecognized;
        }
    }

    public void OnRecognized(string top, List<string> candidates)
    {
        // This static call works because the new Vosk script includes the MapWordToCommand
        string cmd = VoiceRecognizerBeta.MapWordToCommand(top);
        
        Debug.Log("Command mapped: " + cmd + " (raw: " + top + ")");
        textCommand.text = "Command mapped: " + cmd + " (raw: " + top + ")";

        ApplyCommand(cmd);
    }

    private void ApplyCommand(string command)
    {
        if (targetRenderer == null)
        {
            Debug.LogError("Target Renderer is not assigned! Assign the GameObject showing the image in the Inspector.");
            return;
        }

        switch (command)
        {
            case "COLOR_RED":
                targetRenderer.color = Color.red;
                break;
            case "COLOR_BLUE":
                targetRenderer.color = Color.blue;
                break;
            case "COLOR_GREEN":
                targetRenderer.color = Color.green;
                break;
            case "COMMAND_START":
                // Logic for Start command
                break;
            case "COMMAND_STOP":
                // Logic for Stop command
                break;
            case "COLOR_CYCLE":
                // Handle the multi-word command if you add it to the mapping
                targetRenderer.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                break;
            default:
                // Unknown command
                break;
        }
    }

    public void PressStart() => recognizer?.StartContinuousListening();
    public void PressStop() => recognizer?.StopContinuousListening();
}