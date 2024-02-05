using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resonator : MonoBehaviour
{
    // TODO : Refactor this button in different class shit, and have all the main debug logic be in the device class.

    private Animator _animator;
    [SerializeField] private Image _glyphImage;
    [SerializeField] private Glyph[] _glyphs;
    private bool _debugEnabled;

    private int _glyphIndex;
    [SerializeField] private int _debugTextIndex;

    private DebugManager _debugManager;

    private void OnEnable()
    {
        DebugManager.onDebugEnabled += OnDebugEnabled;
        DebugManager.onDebugDisabled += OnDebugDisabled;
    }

    private void OnDisable()
    {
        DebugManager.onDebugDisabled -= OnDebugDisabled;
        DebugManager.onDebugEnabled -= OnDebugEnabled;
    }

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
        _glyphIndex = (_glyphIndex + 1) % 6;
        _animator.SetTrigger("SelectRight");
        _glyphImage.enabled = false;
    }

    public void SelectLeft()
    {
        if (_glyphIndex == 0)
            _glyphIndex = 5;
        else
            _glyphIndex -= 1;

        _animator.SetTrigger("SelectLeft");
        _glyphImage.enabled = false;
    }

    public void OnAnimationComplete()
    {
        _glyphImage.enabled = true;
        _glyphImage.sprite = _glyphs[_glyphIndex].glyph;
        if (_debugEnabled)
            _debugManager.EnableDebugText(_debugTextIndex, _glyphs[_glyphIndex].word);
    }

    public void OnDebugEnabled(DebugManager debugManager)
    {
        _debugManager = debugManager;
        _debugEnabled = true;
        _debugManager.EnableDebugText(_debugTextIndex, _glyphs[_glyphIndex].word);
    }

    public void OnDebugDisabled(DebugManager debugManager)
    {
        _debugManager = debugManager;
        _debugEnabled = false;
        _debugManager.DisableDebugText(_debugTextIndex);
    }
}
