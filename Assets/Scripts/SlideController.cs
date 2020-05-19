using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideController : MonoBehaviour
{
    public GameObject[] characterIcons = new GameObject[3];
    public GameObject[] objectIcons = new GameObject[3];
    public GameObject sceneIcon;
    public GameObject scrollbar;

    private void Start()
    {
        characterIcons[0] = gameObject.transform.Find("CharacterDisplay1").gameObject;
        characterIcons[1] = gameObject.transform.Find("CharacterDisplay2").gameObject;
        characterIcons[2] = gameObject.transform.Find("CharacterDisplay3").gameObject;
        objectIcons[0] = gameObject.transform.Find("ObjectDisplay1").gameObject;
        objectIcons[1] = gameObject.transform.Find("ObjectDisplay2").gameObject;
        objectIcons[2] = gameObject.transform.Find("ObjectDisplay3").gameObject;
        sceneIcon = gameObject.transform.Find("SceneDisplay").gameObject;
    }

    public void SlideEditButtonPressed(GameObject CurrentSlide)
    {
        GameObject gc = GameObject.FindWithTag("GameController");
        PopupController pc = gc.GetComponent<PopupController>();
        pc.OpenPopup(CurrentSlide);
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
    }
    public void AddScene(AttributeClass ac)
    {

        AttributeClass ciac = sceneIcon.GetComponent<AttributeClass>();
        sceneIcon.GetComponent<Image>().sprite = ac.icon;
        ciac.icon = ac.icon;
        ciac.model = ac.model;
       
    }
}
