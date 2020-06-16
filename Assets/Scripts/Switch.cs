using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.UI;

public class Switch : MonoBehaviour {
    public GameObject planningObj;
    public GameObject timelineObj;
    public GameObject instructionObj;
    public GameObject enactmentObj;
    public GameObject startScreenObj;
    public GameObject sceneNumText;
    public GameObject backgroundPlane;
    public GameObject prePlanHelperContent;
    public GameObject prePlanPanel;
    int sceneIndex = 0;

    public Sprite defaultPlayerIcon;
    public List<GameObject> allTrackableModels;

    GameObject gameController;


    private void Start() {
        gameController = GameObject.FindGameObjectWithTag("GameController");
    }

    public void SwitchToPlanningPhase() // opens the inital popup to character selection
    {
        List<Tracker> allSpawnedObjs = FindObjectsOfType<Tracker>().ToList();
        foreach (Tracker t in allSpawnedObjs)
            Destroy(t.gameObject);

        enactmentObj.SetActive(false);
        instructionObj.SetActive(true);
        planningObj.SetActive(true);
        startScreenObj.SetActive(false);
    }

    public void PlayPressed() // spawns first slide models then closes the selection screens
    {
        SwitchToEnactmentPhase();
    }

    public void StopPlayPressed() {
        SwitchToPlanningPhase();
    }

    void OpenEnactmentScene(int newIndex) {
        sceneIndex = newIndex;
        Transform currentSlide = timelineObj.transform.GetChild(sceneIndex);
        if (currentSlide == null)
            return;

        // reset the preplan icons
        foreach (Transform child in prePlanHelperContent.transform) {
            child.gameObject.GetComponent<Image>().sprite = defaultPlayerIcon;
        }

        List<AttributeClass> allAttributes = currentSlide.GetComponentsInChildren<AttributeClass>().ToList();
        foreach (AttributeClass ac in allAttributes) {
            if (ac.model != null)
                Instantiate(ac.model, gameController.GetComponent<ModeController>().IsUsingYOLO() ? Vector3.zero : GetRandomPositionNearZero(), Quaternion.Euler(-90, 0, 0));
            if (ac.background != null)
                backgroundPlane.GetComponent<Image>().sprite = ac.background;
            if (ac.icon != null && ac.model != null) {
                foreach (Transform child in prePlanHelperContent.transform) {
                    if (child.gameObject.GetComponent<Image>().sprite == defaultPlayerIcon) {
                        child.gameObject.GetComponent<Image>().sprite = ac.icon;
                        break;
                    }
                }
            }
        }

        SetSceneNumText(sceneIndex);

        planningObj.SetActive(false);
        enactmentObj.SetActive(true);
        prePlanPanel.SetActive(true);
        startScreenObj.SetActive(false);
    }

    public void SwitchToEnactmentPhase() {

        if (gameController.GetComponent<ModeController>().GetCurGameMode() == GameModes.PREPLANNED) {
            OpenEnactmentScene(0);
        }
        else {
            BeginContinuousEnactment();
        }
    }

    void BeginContinuousEnactment() {

        foreach (GameObject go in allTrackableModels) {
            Instantiate(go, Vector3.zero, Quaternion.Euler(-90, 0, 0));
        }

        SetSceneNumText(0);

        planningObj.SetActive(false);
        enactmentObj.SetActive(true);
        prePlanPanel.SetActive(false);
        startScreenObj.SetActive(false);
    }

    public void SwitchToEnactmentPhaseNext(bool backwards) {
        Debug.Log("PRESSED " + Time.time);
        int totalSlideCount = timelineObj.transform.childCount;

        if (!backwards && sceneIndex < totalSlideCount - 1)
            sceneIndex++;
        else if (backwards && sceneIndex > 0)
            sceneIndex--;
        else {
            SwitchToPlanningPhase();
            return;
        }

        OpenEnactmentScene(sceneIndex);
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    Vector3 GetRandomPositionNearZero() {
        return new Vector3(Random.Range(0, 15f) - 7.5f, Random.Range(0, 15f) + 2f, 0);
    }

    void SetSceneNumText(int num) {
        sceneNumText.GetComponent<TextMeshProUGUI>().text = "Scene #" + num;
    }
}