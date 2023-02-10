using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace PerlinTesting
{

    /// <summary>
    /// This is super non-optimized, but is a great example of how perlin noise can be simply read and interpretated.
    /// To get better performances, colors should be cached in an array and applied at once with Texture2D.SetPixels
    /// </summary>

    public class PerlinNoise1DController : MonoBehaviour
    {
        private TextMeshPro _noiseText;
        [SerializeField] private Transform _circle;
        [SerializeField] private Renderer _planeRenderer;

        [SerializeField] private float _noiseScale;
        [SerializeField] private float _amplitude;
        [SerializeField] private float _speed;

        [SerializeField] private int textureWidth = 256;
        [SerializeField] private int textureHeight = 256;

        private Texture2D _planeTexture;
        private Color[] _colorMap;
        private bool[,] _blackPixels;

        private void Awake()
        {
            _noiseText = GetComponentInChildren<TextMeshPro>();
        }

        private void Start()
        {
            _blackPixels = new bool[textureWidth, textureHeight];
            _colorMap = new Color[textureWidth * textureHeight];
            _planeTexture = new Texture2D(textureWidth, textureHeight);
            _planeRenderer.material.mainTexture = _planeTexture;
        }

        float time = 0f;
        private void Update()
        {
            float noise = Mathf.PerlinNoise(time * _noiseScale, time * _noiseScale);
            time += Time.deltaTime;

            if (time - (int)time <= 0.001f)
            {
                time += 0.01f;
            }

            float circleHeight = noise * _amplitude;
            _circle.transform.position = new Vector2(0f, circleHeight);
            _noiseText.text = circleHeight.ToString();

            //_planeRenderer.material.mainTexture = GenerateTexture(noise);
            GenerateTexture(noise * _amplitude);
        }

        private void GenerateTexture(float currentNoiseValue)
        {
            bool[,] newBlackPixels = new bool[textureWidth, textureHeight];
            //Texture2D texture = new Texture2D(textureWidth, textureHeight);
            for (int x = 0; x < textureWidth; x++)
            {
                for (int y = 0; y < textureHeight; y++)
                {
                    if (_blackPixels[x, y])
                    {
                        int newX = (int)(x + Time.deltaTime * _speed * (textureWidth / 2));
                        //Debug.Log(newX);
                        if (newX <= textureWidth - 1)
                        {
                            newBlackPixels[newX, y] = true;
                        }
                    }
                    _blackPixels[x, y] = false;
                }
            }
            _blackPixels = newBlackPixels;

            int pixelY = (int)(currentNoiseValue * textureHeight);

            for (int x = 0; x < textureWidth; x++)
            {
                for (int y = 0; y < textureHeight; y++)
                {
                    if (x > textureWidth / 2 - 4 && x < textureWidth / 2 + 4)
                        if (y > pixelY / 2 - 4 && y < pixelY / 2 + 4)
                        {
                            Mathf.Clamp(y, 0, textureHeight);
                            _planeTexture.SetPixel(x, y, Color.black);
                            _blackPixels[x, y] = true;
                            continue;
                        }
                    if (_blackPixels[x, y])
                    {
                        _planeTexture.SetPixel(x, y, Color.black);
                        continue;
                    }
                    _planeTexture.SetPixel(x, y, Color.white);
                }
            }

            _planeTexture.Apply();
            //return texture;
        }
    }
}