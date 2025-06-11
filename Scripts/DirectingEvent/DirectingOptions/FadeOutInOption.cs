using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DirectingEventSystem
{
    [System.Serializable]
    public struct FadeOutInOption
    {
        public FadeType fadeType;
        public float fadeOutDuration;
        public float fadeInDuration;
        public Color fadeOutColor;
        public List<DirectingEvent> gapEvents;
        public List<string> stringKeys;
        public float stringDuration;
        public List<TextOption> textOption;
    }

    public enum FadeType
    {
        Circle,
        Full,
    }
}