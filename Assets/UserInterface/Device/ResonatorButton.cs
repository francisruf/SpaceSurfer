using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ResonatorButton : MonoBehaviour, ISelectHandler
{
    public Action<ResonatorButton> onButtonSelected;

    [SerializeField] private Resonator _resonator;

    public void OnSelect(BaseEventData eventData)
    {
        _resonator.OnSelect();
        onButtonSelected?.Invoke(this);
    }

    public void SelectLeftGlyph()
    {
        _resonator.SelectLeft();
    }

    public void SelectRightGlyph()
    {
        _resonator.SelectRight();
    }
}
