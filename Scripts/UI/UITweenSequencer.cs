using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public class TweenAction
{
    public AnimationType animationType;
    public float duration = 1f;
    public Ease easeType = Ease.Linear;
    public float delay = 0f; // 이 동작이 시작되기 전의 개별 딜레이
    
    [Tooltip("체크하면 이전 동작과 동시에 실행됩니다 (Join).")]
    public bool runWithPrevious = false;

    public Vector3 startPosition;
    public Vector3 targetPosition;
    public Vector3 targetRotation;
    public float targetScale;
    public float targetAlpha;
}

public enum AnimationType
{
    None,
    Move,
    Scale,
    Rotate,
    Fade
}

public class UITweenSequencer : MonoBehaviour
{
    [Header("시퀀스 설정")]
    public int loopCount = 0; // 반복 횟수 (-1이면 무한 반복)
    public LoopType loopType = LoopType.Restart;

    [Header("실행할 애니메이션 목록")]
    public List<TweenAction> actionSequence = new List<TweenAction>();
    private Sequence tweenSequence;

    private RectTransform rectTransform;
    private Image image;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        TryGetComponent(out image);

        InitializeSequence();
    }

    private void OnEnable()
    {
        tweenSequence?.Restart();
    }

    private void OnDisable()
    {
        tweenSequence?.Complete(true);
    }

    private void OnDestroy()
    {
        tweenSequence?.Kill();
    }

    private void InitializeSequence()
    {
        tweenSequence = DOTween.Sequence();
        tweenSequence.SetAutoKill(false).Pause();

        foreach (TweenAction tweenAction in actionSequence)
        {
            Tween currentTween = CreateTweenFromAction(tweenAction);
            if (currentTween == null)
                continue;

            if (tweenAction.runWithPrevious)
            {
                tweenSequence.Join(currentTween);
                continue;
            }

            if (tweenAction.delay > 0)
            {
                tweenSequence.AppendInterval(tweenAction.delay);
            }

            tweenSequence.Append(currentTween);
        }

        tweenSequence.SetLoops(loopCount, loopType);
    }

    private Tween CreateTweenFromAction(TweenAction tweenAction)
    {
        switch (tweenAction.animationType)
        {
            case AnimationType.Move:
                rectTransform.anchoredPosition = tweenAction.startPosition;
                return rectTransform.DOAnchorPos(tweenAction.targetPosition, tweenAction.duration).SetEase(tweenAction.easeType);
            case AnimationType.Scale:
                return rectTransform.DOScale(tweenAction.targetScale, tweenAction.duration).SetEase(tweenAction.easeType);
            case AnimationType.Rotate:
                return rectTransform.DORotate(tweenAction.targetRotation, tweenAction.duration).SetEase(tweenAction.easeType);
            case AnimationType.Fade:
                if (image != null)
                    return image.DOFade(tweenAction.targetAlpha, tweenAction.duration).SetEase(tweenAction.easeType);
                else
                {
                    // 오브젝트가 Image 컴포넌트가 없어서 DOFade가 실행되지 않았다는 경고문
                    Debug.LogWarning($"DOFade did not played because of {name} object not have Image component.");
                    return null;
                }
            default:
                return null;
        }
    }
}
