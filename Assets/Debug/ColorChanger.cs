using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Color[] _colors = new Color[] { Color.red, Color.blue, Color.yellow, Color.green };
    private int _colorIndex;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
            Debug.LogError("Color Changer on : " + gameObject.name + " requires a Sprite Renderer!");
    }

    public void CycleColor()
    {
        Color newColor = _colors[_colorIndex];
        _spriteRenderer.color = newColor;

        _colorIndex = (_colorIndex + 1) % _colors.Length;
    }

}
