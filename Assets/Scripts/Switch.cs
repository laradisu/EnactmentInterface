using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public GameObject canvas;
    public void Activate() // opens the inital popup to character selection
    {
        canvas.SetActive(true);

    }
    public void playPressed() // closes the selection screens
    {
        canvas.SetActive(false);
    }
}
