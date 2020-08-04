using System.Collections;
using System.Collections.Generic;
using RockVR.Video;
using UnityEngine;

public class VideoPlayerController : MonoBehaviour
{
    public GameObject stopRecordingButton;
    public GameObject startRecordingButton;
    public GameObject pauseRecordingButton;

    public void StartRecordButtonPressed() {
        if (VideoCaptureCtrl.instance.status == VideoCaptureCtrlBase.StatusType.PAUSED) {
            TogglePauseRecording();
            pauseRecordingButton.SetActive(true);
            startRecordingButton.SetActive(false);
            stopRecordingButton.SetActive(true);
        }
        else {
            StartRecording();
            startRecordingButton.SetActive(false);
            stopRecordingButton.SetActive(true);
            pauseRecordingButton.SetActive(true);
        }
    }

    public void StopRecordButtonPressed() {
        StopRecording();
        startRecordingButton.SetActive(true);
        stopRecordingButton.SetActive(false);
    }

    public void PauseRecordButtonPressed() {
        TogglePauseRecording();
        pauseRecordingButton.SetActive(false);
        startRecordingButton.SetActive(true);
    }

    void StartRecording() {
        if (VideoCaptureCtrl.instance.status == VideoCaptureCtrlBase.StatusType.NOT_START || VideoCaptureCtrl.instance.status == VideoCaptureCtrlBase.StatusType.PAUSED)
            VideoCaptureCtrl.instance.StartCapture();
    }

    void StopRecording() {
        if (VideoCaptureCtrl.instance.status == VideoCaptureCtrlBase.StatusType.STARTED || VideoCaptureCtrl.instance.status == VideoCaptureCtrlBase.StatusType.PAUSED)
            VideoCaptureCtrl.instance.StopCapture();
    }

    void TogglePauseRecording() {
        VideoCaptureCtrl.instance.ToggleCapture();
    }

    public void PlayRecordedVideo() {

    }

    public void StopRecordedVideo() {

    }

    private void Update() {
        /*if (Input.GetKey(KeyCode.Q))
            Debug.Log(VideoCaptureCtrl.instance.status);*/
    }
}
