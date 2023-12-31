using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "ItemSettings", menuName = "ItemsSettings/ItemSettings",order = 2)]
public class ScriptableItemSettings : ScriptableObject
{
    [SerializeField] private string _type;
    [SerializeField] private string _name;
    [SerializeField] private Sprite _sprite;

    public string Type => _type;
    public string Name => _name;
    public Sprite Sprite => _sprite;
}
