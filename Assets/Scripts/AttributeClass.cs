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

    public static bool operator ==(AttributeClass lhs, AttributeClass rhs) {
        if (lhs.icon == rhs.icon && lhs.model == rhs.model && lhs.background == rhs.background && lhs.attributeType == rhs.attributeType)
            return true;
        return false;
    }

    public static bool operator !=(AttributeClass lhs, AttributeClass rhs) {
        return !(lhs == rhs);
    }
}
