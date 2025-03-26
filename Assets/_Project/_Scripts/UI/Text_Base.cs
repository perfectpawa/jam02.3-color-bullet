using KBCore.Refs;
using TMPro;
using UnityEngine;

public class Text_Base : ValidatedMonoBehaviour
{
    [SerializeField, Self] private TMP_Text _text;

    public void ChangeText(string text)
    {
        _text.text = text;
    }
    
}
