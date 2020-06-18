using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupController : MonoBehaviour
{
    public GameObject popup;
    GameObject characterSelectionObj;
    GameObject objectSelectionObj;
    GameObject sceneSelectionObj;

    GameObject currentSlide;

    private void Start() // loads each of the panel selection objects
    {
        objectSelectionObj = popup.transform.Find("ObjectSelection").gameObject;
        characterSelectionObj = popup.transform.Find("CharacterSelection").gameObject;
        sceneSelectionObj = popup.transform.Find("SceneSelection").gameObject;
    }

    public void OpenPopup(GameObject CurrentSlide) // opens the inital popup to character selection
    {
        popup.SetActive(true);
        currentSlide = CurrentSlide;
        characterSelectionObj.SetActive(true);
    }

    public void SwitchToObjectPanel() // switches screen to object panel and deactivates all other panels
    {
        objectSelectionObj.SetActive(true);
        characterSelectionObj.SetActive(false);
        sceneSelectionObj.SetActive(false);
    }

    public void SwitchToCharacterPanel() // switches screen to character panel and deactivates all other panels
    {
        objectSelectionObj.SetActive(false);
        characterSelectionObj.SetActive(true);
        sceneSelectionObj.SetActive(false);
    }

    public void SwitchToScenePanel() // switches screen to scene panel and deactivates all other panels
    {
        objectSelectionObj.SetActive(false);
        characterSelectionObj.SetActive(false);
        sceneSelectionObj.SetActive(true);
    }

    public void ClosePanel() // closes the selection screens
    {
        popup.SetActive(false);
        objectSelectionObj.SetActive(false);
        characterSelectionObj.SetActive(false);
        sceneSelectionObj.SetActive(false);
    }

    void AddCharacter(GameObject button_) // assigns the selected character image to the slide object
    {
        AttributeClass ac = button_.GetComponent<AttributeClass>();
        currentSlide.GetComponent<SlideController>().AddCharacter(ac);
    }

    void RemoveCharacter(GameObject button_) {
        AttributeClass ac = button_.GetComponent<AttributeClass>();
        currentSlide.GetComponent<SlideController>().RemoveCharacter(ac);
    }

    void AddObject(GameObject button_) // assigns the selected object image to the slide object
    {
        AttributeClass ac = button_.GetComponent<AttributeClass>();
        currentSlide.GetComponent<SlideController>().AddObject(ac);
    }

    void RemoveObject(GameObject button_) {
        AttributeClass ac = button_.GetComponent<AttributeClass>();
        currentSlide.GetComponent<SlideController>().RemoveObject(ac);
    }

    public void AddScene(GameObject button_) // assigns the selected object image to the slide object
    {
        AttributeClass ac = button_.GetComponent<AttributeClass>();
        currentSlide.GetComponent<SlideController>().AddScene(ac);
    }

    public void CharacterButtonPressed(GameObject button_) {
        GameObject greenCircle = button_.gameObject.transform.GetChild(0).gameObject;
        if (!greenCircle.activeInHierarchy) {
            AddCharacter(button_);
            greenCircle.SetActive(true);
        }
        else {
            RemoveCharacter(button_);
            greenCircle.SetActive(false);
        }
    }

    public void ObjectButtonPressed(GameObject button_) {
        GameObject greenCircle = button_.gameObject.transform.GetChild(0).gameObject;
        if (!greenCircle.activeInHierarchy) {
            AddObject(button_);
            greenCircle.SetActive(true);
        }
        else {
            RemoveObject(button_);
            greenCircle.SetActive(false);
        }
    }
}
