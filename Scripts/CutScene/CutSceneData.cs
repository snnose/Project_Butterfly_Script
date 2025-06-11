using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CutSceneData
{
    [Header("[SceneImage]")]
    public Sprite cutSceneSprite;
    [Header("[SoundPath]")]
    public string bgmEventPath;
    public string sfxEventPath;
    [Header("[Effect]")]
    public GameObject effect;
    [Header("[CameraMove]")]
    public CameraMoveData cameraMove;
    [Header("[CameraShake]")]
    public CameraShakeData cameraShake;
    [Header("[CameraZoom]")]
    public CameraZoomData cameraZoom;
    [Header("[FadeIn]")]
    public FadeInData fadeInData;
    [Header("[FadeOut]")]
    public FadeOutData fadeOutData;

    [System.Serializable]
    public struct CameraMoveData
    {
        public bool isMove;
        public Vector3 startPosition;        
        public Vector3 endPosition;
        public float moveSpeed;
        public TransitionType moveTransitionType;
    };

    [System.Serializable]
    public struct CameraShakeData
    {
        public bool isShake;
        public float shakeDurationTime;
        public float shakeIntensity;
        public TransitionType shakeTransitionType;
    };

    [System.Serializable]
    public struct CameraZoomData
    {
        public bool isZoom;
        public float startProjectionSize;
        public float endProjectionSize;
        public float zoomSpeed;
        public float zoomTime;
        public TransitionType zoomTransitionType;
    };

    [System.Serializable]
    public struct FadeInData
    {
        public bool isFadeIn;
        public float fadeTime;
        public TransitionType fadeInTransitionType;
    };

    [System.Serializable]
    public struct FadeOutData
    {
        public bool isFadeOut;
        public float fadeTime;
        public TransitionType fadeOutTransitionType;
    }

    public enum TransitionType
    {
        LINEAR,
        EASE_IN,
        EASE_OUT
    };
}
