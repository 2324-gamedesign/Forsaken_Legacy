using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BarkScriptableObject", menuName = "ForsakenLegacy/BarkScriptableObject", order = 0)]
public class BarkScriptableObject : ScriptableObject 
{
    public Bark[] barks;

    [System.Serializable]
    public class Bark
    {
        public string text;
        public Emotion emotion;
    }

    [System.Serializable]
    public enum Emotion
    {
        Neutral,
        Happy,
        Sad
    }
}
