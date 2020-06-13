using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AttributeType { Character, Object, Scene };

public class AttributeClass : MonoBehaviour
{
    public Sprite icon;
    public GameObject model;
    public Sprite background;
    public AttributeType attributeType = AttributeType.Character;
}
