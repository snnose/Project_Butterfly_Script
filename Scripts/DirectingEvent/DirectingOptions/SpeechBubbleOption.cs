using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DirectingEventSystem
{
    [System.Serializable]
    public struct SpeechBubbleOption
    {

        public GameObject speechBubble;
        public GameObject cameraTarget;
        public bool isInBoundary;
        public Vector2 lensShift;
        public SpeechBubbleType speechBubbleType;
        public GameObject speechTarget;
        public Vector3 speechBubbleOffset;
        public List<string> stringKeys;
        public StringPrintType stringPrintType;
        public float printSpeed;
        public float remainingDuration;
        public bool isBubbleShake;
        public SpeechBubbleShake speechBubbleShake;
        public bool isStringShake;
        public StringShake stringShake;
        public List<TextOption> textOptionList;
    }
    [System.Serializable]
    [HideInInspector]
    public struct SpeechBubbleShake
    {
        public float strength;
        public float duration;
    }
    [System.Serializable]
    [HideInInspector]
    public struct StringShake
    {
        public float strength;
        public float duration;
    }
    public enum SpeechBubbleType
    {
        Talk,
        Think,
        Shout,
    }
    public enum StringPrintType
    {
        OneByOne,
        All
    }
}