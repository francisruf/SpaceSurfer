using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ForceSortingOrder : MonoBehaviour
{
    [SerializeField] private string _sortingLayer = "default";
    [SerializeField] private int _sortingOrder;

    private void Awake()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.sortingLayerID = SortingLayer.NameToID(_sortingLayer);
        renderer.sortingOrder = _sortingOrder;
    }
}
