using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseController : MonoBehaviour
{
    [SerializeField] private int _textureWidth, _textureHeight;
    [SerializeField] private float _perlinScale, _xOffset, _yOffset;

    private Renderer _quadRenderer;

    private void Awake()
    {
        _quadRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        _quadRenderer.material.mainTexture = GenerateTexture();
    }

    private Texture2D GenerateTexture()
    {
        Texture2D QuadTexture = new Texture2D(_textureWidth, _textureHeight);
        for (int x = 0; x < _textureWidth; x++)
        {
            for (int y = 0; y < _textureHeight; y++)
            {
                float xCoord = (((float)x / _textureWidth) * _perlinScale) + _xOffset;
                float yCoord = (((float)y / _textureHeight) * _perlinScale) + _yOffset;
                float perlinOutput = Mathf.PerlinNoise(xCoord, yCoord);
                QuadTexture.SetPixel(x, y, new Color(perlinOutput, perlinOutput, perlinOutput));
            }
        }
        QuadTexture.filterMode = FilterMode.Point;
        QuadTexture.Apply();
        return QuadTexture;
    }
}
