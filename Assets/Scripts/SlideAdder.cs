using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideAdder : MonoBehaviour
{
    public GameObject slidePrefab;
    public GameObject currentCanvas;
    
    public void AddSlide() {
        GameObject slideInstance = Instantiate(slidePrefab, transform.position, GameObject.Find("Canvas").transform.rotation, currentCanvas.transform);
        slideInstance.transform.SetParent(transform);
        slideInstance.GetComponent<SlideController>().indexInTimeline = transform.childCount - 1;
    }
}
