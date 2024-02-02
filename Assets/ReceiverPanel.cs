using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReceiverPanel : MonoBehaviour
{
    private void OnEnable()
    {
        FieldOfView.onSignatureDetected += SetGlyphs;
        FieldOfView.onSignatureEmpty += ClearGlyphs;
    }

    private void OnDisable()
    {
        FieldOfView.onSignatureEmpty -= ClearGlyphs;
        FieldOfView.onSignatureDetected -= SetGlyphs;
    }

    [SerializeField] private Image[] _glyphSlots;

    public void SetGlyphs(Signature signature)
    {
        int glyphCount = signature.glyphs.Count;

        for (int i = 0; i < _glyphSlots.Length; i++)
        {
            if (i < glyphCount)
            {
                _glyphSlots[i].enabled = true;
                _glyphSlots[i].sprite = signature.glyphs[i].glyph;
            }
            else
            {
                _glyphSlots[i].enabled = false;
            }
        }
    }

    public void ClearGlyphs()
    {
        for (int i = 0; i < _glyphSlots.Length; i++)
        {
            _glyphSlots[i].sprite = null;
            _glyphSlots[i].enabled = false;
        }
    }
}
