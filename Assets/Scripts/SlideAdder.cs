using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideAdder : MonoBehaviour
{
    public GameObject slidePrefab;
    public GameObject currentCanvas;
    
    public void AddSlide() {
        GameObject slideInstance = Instantiate(slidePrefab, transform.position, Quaternion.identity, currentCanvas.transform);
        slideInstance.transform.SetParent(transform);
    }
}
