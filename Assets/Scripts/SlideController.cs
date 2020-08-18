using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class SlideController : MonoBehaviour
{
    List<GameObject> characterIcons = new List<GameObject>();
    List<GameObject> objectIcons = new List<GameObject>();
    string caption = "";
    public GameObject captionTextObj;
    public GameObject titleTextObj;
    public GameObject selectedColorObj;
    GameObject sceneIcon;
    public GameObject scrollbar;
    Sprite defaultCharacterIcon;
    Sprite defaultObjectIcon;
    Sprite defaultSceneIcon;

    public bool isProxySlide = false;
    // A proxy slide is a slide that needs to constantly transfer its changes to 
    // another slide that it was originally copied from (sidebar slide and "bigger slide")
    public int indexInTimeline = 0;
    GameObject isCopyOf;

    bool initialized = false;
    bool isCopying = false;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize() {
        if (initialized)
            return;

        defaultCharacterIcon = GameObject.FindGameObjectWithTag("GameController").GetComponent<Switch>().defaultPlayerIcon;
        defaultObjectIcon = GameObject.FindGameObjectWithTag("GameController").GetComponent<Switch>().defaultObjectIcon;
        defaultSceneIcon = GameObject.FindGameObjectWithTag("GameController").GetComponent<Switch>().defaultSceneIcon;

        // find all character and object icons within the slide and assign them to the appropriate lists
        AttributeClass[] allDescendedAttributeObjs = gameObject.GetComponentsInChildren<AttributeClass>();
        foreach (AttributeClass ac in allDescendedAttributeObjs) {
            if (ac.attributeType == AttributeType.Character)
                characterIcons.Add(ac.gameObject);
            else if (ac.attributeType == AttributeType.Object)
                objectIcons.Add(ac.gameObject);
        }

        /* OLD — here just for reference
        characterIcons[0] = gameObject.transform.Find("CharacterDisplay1").gameObject;
        characterIcons[1] = gameObject.transform.Find("CharacterDisplay2").gameObject;
        characterIcons[2] = gameObject.transform.Find("CharacterDisplay3").gameObject;
        objectIcons[0] = gameObject.transform.Find("ObjectDisplay1").gameObject;
        objectIcons[1] = gameObject.transform.Find("ObjectDisplay2").gameObject;
        objectIcons[2] = gameObject.transform.Find("ObjectDisplay3").gameObject;*/

        sceneIcon = gameObject.transform.Find("SceneDisplay").gameObject;
        initialized = true;
    }

    public void SlideEditButtonPressed(GameObject currentSlide)
    {
        GameObject gc = GameObject.FindWithTag("GameController");
        PopupController pc = gc.GetComponent<PopupController>();
        pc.OpenPopup(currentSlide);
    }

    public void SlideViewButtonPressed(GameObject currentSlide) {
        GameObject gc = GameObject.FindWithTag("GameController");
        GameObject bs = gc.GetComponent<Switch>().biggerSlide;
        bs.GetComponent<SlideController>().CopyOtherSlide(currentSlide);
        foreach (Transform slide in transform.parent) {
            if (slide.gameObject.GetComponent<SlideController>() != null) {
                slide.gameObject.GetComponent<SlideController>().selectedColorObj.SetActive(false);
            }
        }
        selectedColorObj.SetActive(true);
    }

    public void AddCharacter(AttributeClass ac)
    {
        foreach (GameObject ci in characterIcons)
        {
            AttributeClass ciac = ci.GetComponent<AttributeClass>();
            if(ci.GetComponent<AttributeClass>().model == null)
            {
                ciac.icon = ac.icon;
                ciac.model = ac.model;
                ci.GetComponent<Image>().sprite = ciac.icon; 
                break;
            }
        }

        if (!isCopying) {
            if (isProxySlide && isCopyOf != null)
                isCopyOf.GetComponent<SlideController>().AddCharacter(ac);
            if (isProxySlide)
                GameObject.FindGameObjectWithTag("GameController").GetComponent<Switch>().RefreshScene();
        }
    }

    public void RemoveCharacter(AttributeClass ac) {
        foreach (GameObject ci in characterIcons) {
            AttributeClass ciac = ci.GetComponent<AttributeClass>();
            if (ciac.icon == ac.icon && ciac.model == ac.model) {
                ciac.icon = null;
                ciac.model = null;
                ciac.background = null;
                ci.GetComponent<Image>().sprite = defaultCharacterIcon;
                break;
            }
        }

        if (!isCopying) {
            if (isProxySlide && isCopyOf != null)
                isCopyOf.GetComponent<SlideController>().RemoveCharacter(ac);
            if (isProxySlide)
                GameObject.FindGameObjectWithTag("GameController").GetComponent<Switch>().RefreshScene();
        }
    }

    public void AddObject(AttributeClass ac)
    {
        foreach (GameObject ci in objectIcons)
        {
            AttributeClass ciac = ci.GetComponent<AttributeClass>();
            if (ci.GetComponent<AttributeClass>().model == null)
            {
                ciac.icon = ac.icon;
                ciac.model = ac.model;
                ci.GetComponent<Image>().sprite = ciac.icon;
                break;
            }
        }

        if (!isCopying) {
            Debug.Log(isCopyOf + " " + gameObject.name + " " + isProxySlide);
            if (isProxySlide && isCopyOf != null)
                isCopyOf.GetComponent<SlideController>().AddObject(ac);
            if (isProxySlide)
                GameObject.FindGameObjectWithTag("GameController").GetComponent<Switch>().RefreshScene();
        }
    }

    public void RemoveObject(AttributeClass ac) {
        foreach (GameObject ci in objectIcons)
        {
            AttributeClass ciac = ci.GetComponent<AttributeClass>();
            if (ciac.icon == ac.icon && ciac.model == ac.model) {
                ciac.icon = null;
                ciac.model = null;
                ciac.background = null;
                ci.GetComponent<Image>().sprite = defaultObjectIcon;
                break;
            }
                
        }

        if (!isCopying) {
            if (isProxySlide && isCopyOf != null)
                isCopyOf.GetComponent<SlideController>().RemoveObject(ac);
            if (isProxySlide)
                GameObject.FindGameObjectWithTag("GameController").GetComponent<Switch>().RefreshScene();

        }
    }

    public void AddScene(AttributeClass ac)
    {
        AttributeClass ciac = sceneIcon.GetComponent<AttributeClass>();
        sceneIcon.GetComponent<Image>().sprite = ac.icon;
        ciac.icon = ac.icon;
        ciac.model = ac.model;
        ciac.background = ac.background;

        if (!isCopying) {
            if (isProxySlide)
                isCopyOf.GetComponent<SlideController>().AddScene(ac);
            if (isProxySlide)
                GameObject.FindGameObjectWithTag("GameController").GetComponent<Switch>().RefreshScene();
        }

    }

    public void CopyOtherSlide(GameObject otherSlide) {
        isCopying = true;

        SlideController sc = otherSlide.GetComponent<SlideController>();
        sc.Initialize();
        ClearSlideData();
        foreach (GameObject icon in sc.objectIcons) {
            AddObject(icon.GetComponent<AttributeClass>());
        }
        foreach (GameObject icon in sc.characterIcons) {
            AddCharacter(icon.GetComponent<AttributeClass>());
        }
        AddScene(sc.sceneIcon.GetComponent<AttributeClass>());

        SetTitle(sc.titleTextObj.GetComponent<TMP_InputField>().text);
        if (captionTextObj != null) {
            SetCaption(sc.caption);
        }
        isCopyOf = otherSlide;
        isCopying = false;
    }

    public void ClearSlideData() {
        RemoveAllCharacters();
        RemoveAllObjects();
        RemoveBackground();
    }

    public void RemoveAllCharacters() {
        foreach (GameObject ci in characterIcons) {
            AttributeClass ciac = ci.GetComponent<AttributeClass>();
            ciac.icon = null;
            ciac.model = null;
            ciac.background = null;
            ci.GetComponent<Image>().sprite = defaultCharacterIcon;
        }
    }

    public void RemoveAllObjects() {
        foreach (GameObject oi in objectIcons) {
            AttributeClass oiac = oi.GetComponent<AttributeClass>();
            oiac.icon = null;
            oiac.model = null;
            oiac.background = null;
            oi.GetComponent<Image>().sprite = defaultObjectIcon;
        }
    }

    public void RemoveBackground() {
        /*
        AttributeClass ciac = sceneIcon.GetComponent<AttributeClass>();
        sceneIcon.GetComponent<Image>().sprite = defaultSceneIcon;
        ciac.icon = defaultSceneIcon;
        ciac.background = defaultSceneIcon;*/
    }

    public void SlideGotoButtonPressed(GameObject currentSlide) {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<Switch>().OpenEnactmentScene(currentSlide.GetComponent<SlideController>().isCopyOf.transform);
    }

    public void OnCaptionChanged() {
        caption = captionTextObj.GetComponent<TMP_InputField>().text;
        if (isProxySlide && !isCopying) {
            isCopyOf.GetComponent<SlideController>().SetCaption(caption);
        }
    }

    public void SetCaption(string setTo) {
        caption = setTo;
        if (captionTextObj != null)
            captionTextObj.GetComponent<TMP_InputField>().text = setTo;
    }

    public void OnTitleChanged() {
        if (isProxySlide && !isCopying) {
            isCopyOf.GetComponent<SlideController>().SetTitle(titleTextObj.GetComponent<TMP_InputField>().text);
        }
    }

    public void SetTitle(string setTo) {
        titleTextObj.GetComponent<TMP_InputField>().text = setTo;
    }
}
