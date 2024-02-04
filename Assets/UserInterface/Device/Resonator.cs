using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resonator : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private Image _glyphImage;
    [SerializeField] private Glyph[] _glyphs;

    private int _glyphIndex;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (_glyphs.Length != 6)
            Debug.LogWarning("Warning : Resonator Glyph lenght is not 6.");
    }

    public void OnSelect()
    {
        _glyphImage.enabled = false;
        _animator.SetTrigger("Spin");
    }

    public void SelectRight()
    {
        Debug.Log("RIGHT");
        _glyphIndex = (_glyphIndex + 1) % 6;
        _animator.SetTrigger("SelectRight");
        Debug.Log("Index : " + _glyphIndex);
        _glyphImage.enabled = false;
    }

    public void SelectLeft()
    {
        Debug.Log("LEFT");

        if (_glyphIndex == 0)
            _glyphIndex = 5;
        else
            _glyphIndex -= 1;

        _animator.SetTrigger("SelectLeft");
        Debug.Log("Index : " + _glyphIndex);
        _glyphImage.enabled = false;
    }

    public void OnAnimationComplete()
    {
        Debug.Log("ANIM COMPLETE");
        _glyphImage.enabled = true;
        _glyphImage.sprite = _glyphs[_glyphIndex].glyph;
    }
}
