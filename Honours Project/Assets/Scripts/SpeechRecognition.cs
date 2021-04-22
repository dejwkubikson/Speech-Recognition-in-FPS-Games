using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

public class SpeechRecognition : MonoBehaviour
{
    // Text objects to display recognized text and errors
    public Text RecognizedText;
    public Text ErrorText;
    private VoiceControl voiceControl;

    private string recognizedString = "";
    private string errorString = "";

    private bool recognizeText = false;
    private bool recognizedText = false;

    public float textClearTimer = 5.0f;

    // Recognition events are raised in seperate thread. Thread must be locked
    // to avoid deadlocks.
    private System.Object threadLock = new System.Object();

    // Object used for speech recognition
    private SpeechRecognizer recognizer;

    // Start is called before the first frame update
    void Start()
    {
        voiceControl = GameObject.FindGameObjectWithTag("VoiceControl").GetComponent<VoiceControl>();

        StartRecognition();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.T))
        {
            recognizeText = true;
            recognizedText = false;
        }
        else
        {
            recognizeText = false;
        }

        lock (threadLock)
        {
            if (recognizedString.Length > 0 && recognizedString.Contains("GATHERING") == false)
                RecognizedText.text = $"Alpha: {recognizedString}";
            else
                RecognizedText.text = "";
            /*else if (recognizedString.Contains("GATHERING"))
                RecognizedText.text = recognizedString;*/

            if(errorString.Length > 0)
                ErrorText.text = $"ERROR: {errorString}";
        }

        if(RecognizedText.text.Length > 0)
        {
            textClearTimer -= Time.deltaTime;
            if (textClearTimer <= 0)
            {
                recognizedString = "";
                textClearTimer = 5.0f;
            }
        }

    }

    void CreateRecognizer()
    {
        UnityEngine.Debug.Log("CreateRecognizer()");

        if(recognizer == null)
        {
            SpeechConfig config = SpeechConfig.FromSubscription("c0b086eaf0394ee1ad212ab0d731d22e", "uksouth");
            config.SpeechRecognitionLanguage = "en-us";
            recognizer = new SpeechRecognizer(config);

            if(recognizer != null)
            {
                // Speech events
                recognizer.Recognizing += RecognizingHandler;
                recognizer.Recognized += RecognizedHandler;
                recognizer.Canceled += CanceledHandler;
            }
        }

        UnityEngine.Debug.Log("CreateRecognizer() finished");
    }

    private async void StartRecognition()
    {
        UnityEngine.Debug.Log("StartRecognition()");
        CreateRecognizer();

        if(recognizer != null)
        {
            UnityEngine.Debug.Log("Recognizer found, starting");
            await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

            UnityEngine.Debug.Log("Recognizer started");
        }

        UnityEngine.Debug.Log("StartRecognition() finished");
    }

    // Stops the speech recognition.
    // Releasing all events and cleaning up resources
    public async void StopRecognition()
    {
        if (recognizer != null)
        {
            await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
            recognizer.Recognizing -= RecognizingHandler;
            recognizer.Recognized -= RecognizedHandler;
            recognizer.Canceled -= CanceledHandler;
            recognizer.Dispose();
            recognizer = null;
            recognizedString = "Speech recognition stopped.";
        }
    }

    // Speech events
    // "Recognizing" events are fired every time interim results are returned during recognition
    private void RecognizingHandler(object sender, SpeechRecognitionEventArgs e)
    {
        //UnityEngine.Debug.Log("RecognizingHandler() start");
        if (!recognizeText)
            return;
        //UnityEngine.Debug.Log("RecognizingHandler() pass");

        if (e.Result.Reason == ResultReason.RecognizingSpeech)
        {
            lock (threadLock)
            {
                recognizedString = $"GATHERING: {e.Result.Text}";
            }
        }
    }

    // "Recognized" events are fired when the utterance end was detected by the server
    private void RecognizedHandler(object sender, SpeechRecognitionEventArgs e)
    {
        if (recognizedText)
            return;
        else
            recognizedText = true;

        if (e.Result.Reason == ResultReason.RecognizedSpeech)
        {
            lock (threadLock)
            {
                recognizedString = e.Result.Text;
                voiceControl.CheckKeywordsAndFireEvents(recognizedString);

                textClearTimer = 5.0f;
            }
        }
        else if (e.Result.Reason == ResultReason.NoMatch)
        {
            UnityEngine.Debug.LogFormat($"RESULT: Could not recognize speech.");
        }
    }

    // "Canceled" events are fired if the server encounters some kind of error.
    private void CanceledHandler(object sender, SpeechRecognitionCanceledEventArgs e)
    {
        UnityEngine.Debug.Log("CanceledHandler()");
        errorString = e.ToString();
        UnityEngine.Debug.Log("CanceledHandler() finished");
    }
}
