using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Switch : MonoBehaviour {
    public GameObject planningObj;
    public GameObject timelineObj;
    public GameObject enactmentObj;
    public GameObject startScreenObj;
    public GameObject sceneNumText;
    public GameObject backgroundPlane;
    int index = 0;

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

    public void SwitchToEnactmentPhase() {

        if (gameController.GetComponent<ModeController>().GetCurGameMode() == GameModes.PREPLANNED) {
            index = 0;
            Transform currentSlide = timelineObj.transform.GetChild(0);
            if (currentSlide == null)
                return;

            List<AttributeClass> allAttributes = currentSlide.GetComponentsInChildren<AttributeClass>().ToList();
            foreach (AttributeClass ac in allAttributes) {
                if (ac.model != null)
                    Instantiate(ac.model, GetRandomPositionNearZero(), Quaternion.Euler(-90, 0, 0));
                if (ac.background != null)
                    backgroundPlane.GetComponent<Image>().sprite = ac.background;
            }
        }

        SetSceneNumText(index + 1);

        planningObj.SetActive(false);
        enactmentObj.SetActive(true);
        startScreenObj.SetActive(false);
    }
    public void SwitchToEnactmentPhaseNext(bool backwards) {
        Debug.Log("PRESSED " + Time.time);
        int totalSlideCount = timelineObj.transform.childCount;

        if (!backwards && index < totalSlideCount - 1)
            index++;
        else if (backwards && index > 0)
            index--;
        else {
            SwitchToPlanningPhase();
            return;
        }

        List<Tracker> allSpawnedObjs = FindObjectsOfType<Tracker>().ToList();
        foreach (Tracker t in allSpawnedObjs)
            Destroy(t.gameObject);

        Transform currentSlide = timelineObj.transform.GetChild(index);

        if (currentSlide == null)
            return;

        List<AttributeClass> allAttributes = currentSlide.GetComponentsInChildren<AttributeClass>().ToList();

        foreach (AttributeClass ac in allAttributes) {
            if (ac.model != null)
                Instantiate(ac.model, GetRandomPositionNearZero(), Quaternion.Euler(-90, 0, 0));
            if (ac.background != null)
                backgroundPlane.GetComponent<Image>().sprite = ac.background;
        }

        SetSceneNumText(index + 1);
        planningObj.SetActive(false);
        enactmentObj.SetActive(true);
        startScreenObj.SetActive(false);
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