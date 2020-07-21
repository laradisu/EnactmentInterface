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
    public GameObject instructionObj;
    public GameObject enactmentObj;
    public GameObject startScreenObj;
    public GameObject sceneNumText;
    public GameObject backgroundPlane;
    public GameObject prePlanHelperContent;
    public GameObject prePlanPanel;
    public GameObject sceneSidebarContent;
    public GameObject sceneSidebarScrollView;
    public GameObject sceneSidebarOpenButton;
    public GameObject sceneSidebarCloseButton;
    int sceneIndex = 0;

    public Sprite defaultPlayerIcon;
    public Sprite defaultObjectIcon;
    public Sprite defaultSceneIcon;
    public List<GameObject> allTrackableModels;

    GameObject gameController;
    GameObject currentSlide;

    public GameObject sidebarSlidePrefab;


    private void Start() {
        gameController = GameObject.FindGameObjectWithTag("GameController");
    }

    public void SwitchToPlanningPhase() // opens the inital popup to character selection
    {
        DestroySpawnedModels();

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

    public void OpenEnactmentScene(Transform slide) {
        if (slide.GetComponent<SlideController>().isSidebarSlide)
            Debug.LogWarning("Shouldn't use a sidebarSlide because it may be destroyed; not the original data");

        currentSlide = slide.gameObject;
        DestroySpawnedModels();

        // reset the preplan icons
        foreach (Transform child in prePlanHelperContent.transform) {
            child.gameObject.GetComponent<Image>().sprite = defaultPlayerIcon;
        }

        // spawn characters and objects, set background, populate PrePlanHelper area
        List<AttributeClass> allAttributes = slide.GetComponentsInChildren<AttributeClass>().ToList();
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

        // delete existing slides in sidebar
        foreach (Transform child in sceneSidebarContent.transform) {
            Destroy(child.gameObject);
        }

        // populate sidebar
        foreach (Transform child in timelineObj.transform) {
            GameObject curSlide = Instantiate(sidebarSlidePrefab, sceneSidebarContent.transform);
            curSlide.GetComponent<SlideController>().Initialize();
            curSlide.GetComponent<SlideController>().CopyOtherSlide(child.gameObject);
        }
    }

    void OpenEnactmentSceneByIndex(int newIndex) {
        

        sceneIndex = newIndex;
        Transform slide = timelineObj.transform.GetChild(sceneIndex);
        if (slide == null)
            return;

        OpenEnactmentScene(slide);
    }

    public void SwitchToEnactmentPhase() {

        if (gameController.GetComponent<ModeController>().GetCurGameMode() == GameModes.PREPLANNED) {
            OpenEnactmentSceneByIndex(0);
        }
        else {
            BeginContinuousEnactment();
        }
    }

    void BeginContinuousEnactment() {
        DestroySpawnedModels();

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
        int totalSlideCount = timelineObj.transform.childCount;

        if (!backwards && sceneIndex < totalSlideCount - 1)
            sceneIndex++;
        else if (backwards && sceneIndex > 0)
            sceneIndex--;
        else {
            SwitchToPlanningPhase();
            return;
        }

        OpenEnactmentSceneByIndex(sceneIndex);
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

    void DestroySpawnedModels() {
        List<Tracker> allSpawnedObjs = FindObjectsOfType<Tracker>().ToList();
        foreach (Tracker t in allSpawnedObjs)
            Destroy(t.gameObject);
    }

    public void OpenSceneSidebar() {
        sceneSidebarOpenButton.SetActive(false);
        sceneSidebarScrollView.SetActive(true);
        sceneSidebarCloseButton.SetActive(true);
    }

    public void CloseSceneSidebar() {
        sceneSidebarOpenButton.SetActive(true);
        sceneSidebarScrollView.SetActive(false);
        sceneSidebarCloseButton.SetActive(false);
    }

    public void RefreshScene() {
        Debug.Log("Start refresh");
        GameObject slide = currentSlide;
        Debug.Log("Spot1");

        // reset the preplan icons
        foreach (Transform child in prePlanHelperContent.transform) {
            child.gameObject.GetComponent<Image>().sprite = defaultPlayerIcon;
        }
        Debug.Log("Spot11");


        // spawn characters and objects, set background, populate PrePlanHelper area
        Debug.Log(slide);
        Debug.Log(slide.name);
        List<AttributeClass> allAttributes = slide.GetComponentsInChildren<AttributeClass>().ToList();
        Debug.Log("Spot12");
        foreach (AttributeClass ac in allAttributes) {
            if (ac.model != null) {

            }
            if (ac.background != null) {
                backgroundPlane.GetComponent<Image>().sprite = ac.background;
            }
            if (ac.icon != null && ac.model != null) {
                Debug.Log("Spot2");
                foreach (Transform child in prePlanHelperContent.transform) {
                    Debug.Log("Spot3");
                    if (child.gameObject.GetComponent<Image>().sprite == defaultPlayerIcon) {
                        Debug.Log("Spot4");
                        child.gameObject.GetComponent<Image>().sprite = ac.icon;
                        Debug.Log("Spot5");
                        break;
                    }
                    Debug.Log("Spot6");
                }
            }
        }
        Debug.Log("End refresh");
    }
}