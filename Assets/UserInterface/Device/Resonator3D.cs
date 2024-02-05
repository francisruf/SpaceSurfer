using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resonator3D : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    [SerializeField] private Texture2D _testTexture;

    private void Awake()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    void Start()
    {
        List<Material> materials = new List<Material>();
        _meshRenderer.GetMaterials(materials);


        int width = materials[1].mainTexture.width;
        int height = materials[1].mainTexture.height;
        int glyhphWidth = _testTexture.width;
        int glyphHeight = _testTexture.height;

        Texture2D texture = new Texture2D(width, height);
        Color[] colors = new Color[width * height];
        materials[1].SetTexture("_MainTex", texture);

        //for (int x = 0; x < glyhphWidth; x++)
        //{
        //    for (int y = 0; y < glyphHeight; y++)
        //    {
        //        colors[x + width * y] = _testTexture.GetPixel(x, y);
        //    }
        //}

        Debug.Log("Height : " + height + " // Width : " + width);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color newColor = Color.black;
                newColor.a = 1f;
                newColor.r = x / (float)width;
                Debug.Log(newColor.r);
                colors[x + width * y] = newColor;
            }
        }

        texture.SetPixels(colors);
        texture.Apply();
    }
}
