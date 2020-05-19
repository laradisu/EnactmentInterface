using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionController : MonoBehaviour
{
    public GameObject instructions;
    public void OpenInstructions() // opens the inital popup to character selection
    {
        instructions.SetActive(true);

    }
    public void ClosePanel() // closes the selection screens
    {
        instructions.SetActive(false);
    }
}
