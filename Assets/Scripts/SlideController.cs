using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideController : MonoBehaviour
{
    List<GameObject> characterIcons = new List<GameObject>();
    List<GameObject> objectIcons = new List<GameObject>();
    GameObject sceneIcon;
    public GameObject scrollbar;

    private void Start()
    {
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
        var planes = Resources.FindObjectsOfTypeAll<plane>();
        Sprite myImageComponent = planes[0].GetComponent<Image>().sprite;
        if(ac.background != null)
            myImageComponent = ac.background; 
 



}
}
