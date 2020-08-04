using System.Collections.Generic;
using System.Linq;
using RockVR.Video;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GamePhase {
    START, PLANNING, ENACTMENT
}

public class Switch : MonoBehaviour {
    public GamePhase currentGamePhase = GamePhase.START;
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
    public Material defaultBackgroundMaterial;
    public List<GameObject> allTrackableModels;

    public GameObject biggerSlide;

    GameObject gameController;
    GameObject currentSlide;

    public GameObject sidebarSlidePrefab;


    private void Start() {
        gameController = GameObject.FindGameObjectWithTag("GameController");
    }

    public void SwitchToPlanningPhase() // opens the inital popup to character selection
    {
        DestroySpawnedModels();

        currentGamePhase = GamePhase.PLANNING;
        enactmentObj.SetActive(false);
        instructionObj.SetActive(true);
        planningObj.SetActive(true);
        startScreenObj.SetActive(false);

        timelineObj.transform.GetChild(0).GetComponent<SlideController>().SlideViewButtonPressed(timelineObj.transform.GetChild(0).gameObject);
    }

    public void PlayPressed() // spawns first slide models then closes the selection screens
    {
        SwitchToEnactmentPhase();
    }

    public void StopPlayPressed() {
        SwitchToPlanningPhase();
    }

    public void OpenEnactmentScene(Transform slide) {
        currentGamePhase = GamePhase.ENACTMENT;

        if (slide.GetComponent<SlideController>().isProxySlide)
            Debug.LogWarning("Shouldn't use a sidebarSlide because it may be destroyed; not the original data");
        if (VideoCaptureCtrl.instance.status == VideoCaptureCtrlBase.StatusType.STARTED)
            gameController.GetComponent<VideoPlayerController>().PauseRecordButtonPressed();
        currentSlide = slide.gameObject;
        sceneIndex = slide.GetComponent<SlideController>().indexInTimeline;
        DestroySpawnedModels();

        // reset the preplan icons
        foreach (Transform child in prePlanHelperContent.transform) {
            child.gameObject.GetComponent<Image>().sprite = defaultPlayerIcon;
        }

        // spawn characters and objects, set background, populate PrePlanHelper area
        List<AttributeClass> allAttributes = slide.GetComponentsInChildren<AttributeClass>().ToList();
        foreach (AttributeClass ac in allAttributes) {
            if (ac.model != null)
                SpawnModel(ac.model);
            if (ac.background != null) {
                //backgroundPlane.GetComponent<Image>().sprite = ac.background;
                SetBackground(ac.background);
            }
                
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
        prePlanPanel.SetActive(gameController.GetComponent<ModeController>().GetCurGameMode() == GameModes.PREPLANNED);
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

        OpenEnactmentScene(timelineObj.transform.GetChild(0));

        foreach (GameObject go in allTrackableModels) {
            Instantiate(go, Vector3.zero, Quaternion.Euler(-90, 0, 0));
        }

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
            ExitEnactmentPhaseCleanup();
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

    List<GameObject> GetAllSpawnedModels() {
        List<Tracker> allSpawnedTrackers = FindObjectsOfType<Tracker>().ToList();
        List<GameObject> toReturn = new List<GameObject>();
        foreach (Tracker t in allSpawnedTrackers) {
            toReturn.Add(t.gameObject);
        }

        return toReturn;
    }

    void DestroySpawnedModels() {
        List<GameObject> allSpawnedObjs = GetAllSpawnedModels();
        foreach (GameObject go in allSpawnedObjs)
            Destroy(go);
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
        if (currentGamePhase != GamePhase.ENACTMENT) {
            Debug.Log("Would refresh, but GamePhase isn't Enactment");
            return;
        }

        GameObject slide = currentSlide;

        // clear the preplan icons
        foreach (Transform child in prePlanHelperContent.transform) {
            child.gameObject.GetComponent<Image>().sprite = defaultPlayerIcon;
        }

        // set background, reset PrePlanHelper area
        List<AttributeClass> allAttributes = slide.GetComponentsInChildren<AttributeClass>().ToList();
        List<GameObject> allSpawnedObjs = GetAllSpawnedModels();
        foreach (AttributeClass ac in allAttributes) {
            if (ac.model != null) {
                // spawn new object if it's not present
                if (ac.attributeType == AttributeType.Object) {
                    bool objAlreadySpawned = false;
                    foreach (GameObject spawnedObj in allSpawnedObjs) {
                        if (spawnedObj.name.Contains(ac.model.name)) {
                            objAlreadySpawned = true;
                            break;
                        }
                    }
                    if (!objAlreadySpawned)
                        SpawnModel(ac.model);
                }
            }
            if (ac.background != null) {
                SetBackground(ac.background);
            }
            if (ac.icon != null && ac.model != null) {
                foreach (Transform child in prePlanHelperContent.transform) {
                    if (child.gameObject.GetComponent<Image>().sprite == defaultPlayerIcon) {
                        child.gameObject.GetComponent<Image>().sprite = ac.icon;
                        break;
                    }
                }
            }
        }
    }

    void SpawnModel(GameObject model) {
        Instantiate(model, gameController.GetComponent<ModeController>().IsUsingYOLO() ? Vector3.zero : GetRandomPositionNearZero(), Quaternion.Euler(-90, 0, 0));
    }

    void SetBackground(Sprite img) {
        Material backgroundMat = new Material(defaultBackgroundMaterial);
        backgroundMat.SetTexture("_MainTex", img.texture);
        backgroundPlane.GetComponent<MeshRenderer>().material = backgroundMat;
    }

    void ExitEnactmentPhaseCleanup() {
        if (VideoCaptureCtrl.instance.status == VideoCaptureCtrlBase.StatusType.PAUSED || VideoCaptureCtrl.instance.status == VideoCaptureCtrlBase.StatusType.STARTED)
            gameController.GetComponent<VideoPlayerController>().StopRecordButtonPressed();
    }
}