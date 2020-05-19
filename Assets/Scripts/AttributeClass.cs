using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttributeType { Character, Object, Scene };

public class AttributeClass : MonoBehaviour
{
    public Sprite icon;
    public GameObject model;
    public AttributeType attributeType = AttributeType.Character;
}
