// using System.Collections;
// using System.Collections.Generic;
// using TextSpeech;
// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.UI;
// using UnityEngine.Android;
// using UnityEngine.Windows.Speech;
// using System.Linq;
// using TMPro;
// using System;

// public class VoiceControl : MonoBehaviour
// {
//     const string LANG_CODE = "en-US";

//     string speechResult;
//     public TextMeshProUGUI text;

//     KeywordRecognizer keywordRecognizer;
//     Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

//     void Start()
//     {
//         keywords.Add("Hello", PartialSpeechResult);
//         keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
//         keywordRecognizer.OnPhraseRecognized += StartVoice2;

//         keywordRecognizer.Start();


//         Setup(LANG_CODE);

// #if UNITY_ANDROID

//         //SpeechToText.Instance.onPartialResultsCallback = PartialSpeechResult;

// #endif

//         SpeechToText.Instance.onResultCallback = FinalSpeechResult;

//         CheckPermission();
//     }

//     void CheckPermission()
//     {
//         #if UNITY_ANDROID
//         if(!Permission.HasUserAuthorizedPermission(Permission.Microphone))
//         {
//             Permission.RequestUserPermission(Permission.Microphone);
//         }
//         #endif
//     }

//     void Update()
//     {    
//         if(speechResult == null) return;
//         Debug.Log(speechResult);

        
//     }


//     void Setup(string code)
//     {
//         TextToSpeech.Instance.Setting(code, 1, 1);
//         SpeechToText.Instance.Setting(code);
//     }

//     public void StartVoice()
//     {
//         SpeechToText.Instance.StartRecording();
//         Debug.Log("Speek");
//     }

//     public void EndVoice()
//     {
//         SpeechToText.Instance.StopRecording();
//         Debug.Log("End");
//     }

//     void FinalSpeechResult(string result)
//     {
//         text.text = speechResult;
//     }
//     void PartialSpeechResult()
//     {
//         text.text = speechResult;
//     }

//     public void StartVoice2(PhraseRecognizedEventArgs phrase)
//     {
//         keywords[phrase.text].Invoke();
//         speechResult = phrase.text;
//     }

//     public void DikoAlam()
//     {
//         keywordRecognizer.Start();
//     }

//     public void EndVoice2()
//     {
//         keywordRecognizer.Stop();
//     }
// }
