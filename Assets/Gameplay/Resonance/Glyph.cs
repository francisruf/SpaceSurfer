using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Glyph", menuName = "Resonance/Glyph")]
public class Glyph : ScriptableObject
{
    public string word;
    public Sprite glyph;
    public glyphClass glyphClass;
}

[System.Serializable]
public class Signature
{
    public List<Glyph> glyphs;
    public string GetSignatureString()
    {
        string signatureStr = "";

        for (int i = 0; i < glyphs.Count; i++)
        {
            if (i > 0)
                signatureStr += " ";
            signatureStr += glyphs[i].word;
        }

        return signatureStr;
    }
}

[System.Serializable]
public enum glyphClass
{
    noun,
    verb,
    pronoun,
    adjective,
    adverb,
    determiner
}
