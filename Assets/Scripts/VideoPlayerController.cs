using System.Collections;
using System.Collections.Generic;
using RockVR.Video;
using UnityEngine;

public class VideoPlayerController : MonoBehaviour
{
    public GameObject stopRecordingButton;
    public GameObject startRecordingButton;
    public GameObject pauseRecordingButton;
    public GameObject resumeRecordingButton;

    public void StartRecordButtonPressed() {
        StartRecording();
        startRecordingButton.SetActive(false);
        stopRecordingButton.SetActive(true);
    }

    public void StopRecordButtonPressed() {
        StopRecording();
        startRecordingButton.SetActive(true);
        stopRecordingButton.SetActive(false);
    }

    public void PauseRecordButtonPressed() {
        TogglePauseRecording();
        pauseRecordingButton.SetActive(false);
        resumeRecordingButton.SetActive(true);
    }

    public void ResumeRecordButtonPressed() {
        TogglePauseRecording();
        pauseRecordingButton.SetActive(true);
        resumeRecordingButton.SetActive(false);
    }

    void StartRecording() {
        VideoCaptureCtrl.instance.StartCapture();
    }

    void StopRecording() {
        VideoCaptureCtrl.instance.StopCapture();
    }

    void TogglePauseRecording() {
        VideoCaptureCtrl.instance.ToggleCapture();
    }
}
