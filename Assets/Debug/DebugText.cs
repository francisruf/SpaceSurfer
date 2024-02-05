using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DebugText : MonoBehaviour
{
    private Image _image;
    [SerializeField] private TextMeshProUGUI _text;

    private void Awake()
    {
        _image = GetComponent<Image>();
        HideText();
    }

    public void DisplayText(string debugText)
    {
        _image.enabled = true;
        _text.enabled = true;
        _text.text = debugText;
        _text.ForceMeshUpdate();
    }

    public void HideText()
    {
        _image.enabled = false;
        _text.text = "Undefined";
        _text.enabled = false;
    }
}
