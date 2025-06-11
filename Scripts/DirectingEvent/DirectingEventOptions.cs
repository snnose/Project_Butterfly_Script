using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace DirectingEventSystem
{
    [System.Serializable]
    public struct CameraMoveOption
    {
        public bool isWaitForCompletion;
        public Transform initTransform;
        public Transform targetTransform;
        public float duration;
        public Ease ease;
        public bool isReturn;
        public bool hideJoyStick;
        public bool hideDialogue;
    }
    [System.Serializable]
    public struct CameraRotateOption
    {
        public bool isWaitForCompletion;
        public Vector3 endRotation;
        public float duration;
        public Ease ease;
    }
    [System.Serializable]
    public struct CameraShakeOption
    {
        public float initStrength;
        public float endStrength;
        public float duration;
        public AnimationCurve shakeAnimationCurve;
    }
    [System.Serializable]
    public struct DialogueOption
    {
        public int startPage;
        public int endPage;
        public int progress;
    }
    [System.Serializable]
    public struct ObjectMoveOption
    {
        public bool isWaitForCompletion;
        public List<DirectingEventObject> targetObjects;
        public Transform endTransform;
        public Vector3 endPosition;
        public float duration;
        public Ease ease;
        public bool isCameraFollow;
        public string effectKey;
        public string animationName;
    }
    [System.Serializable]
    public struct ObjectInstantiateOption
    {
        public bool isWaitForCompletion;
        public GameObject gameObject;
        public string path;
        public Vector3 spawnPosition;
        public Quaternion spawnQuaternion;
        public Vector3 scale;
        public float duration;
        public string effectKey;
        public string animationName;
    }
    [System.Serializable]
    public struct ObjectDestroyOption
    {
        public bool isWaitForCompletion;
        public List<DirectingEventObject> targetObjects;
        public string effectKey;
        public string animationName;
    }
    [System.Serializable]
    public struct ObjectScaleChangeOption
    {
        public bool isWaitForCompletion;
        public List<DirectingEventObject> targetObjects;
        public float scale;
        public float duration;
        public Ease ease;
        public string effectKey;
        public string animationName;
    }
    [System.Serializable]
    [HideInInspector]
    public struct TextOption
    {
        public Vector2 textScale;
        public TMP_FontAsset fontAsset;
        public FontStyles fontStyle;
        public Color textColor;
    }

    public enum OptionType
    {
        ObjectMove,
        ObjectInstantiate,
        ObjectDestroy,
        ObjectScaleChange,
        CameraMove,
        CameraRotate,
        CameraShake,
        DialogueFloat,
        SpeechBubbleFloat,
        FadeOutIn,
    }
}