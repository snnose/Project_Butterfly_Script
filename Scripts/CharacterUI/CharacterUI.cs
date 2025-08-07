using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;
using DG.Tweening;

public class CharacterUI : MonoBehaviour
{
    [Header("CharacterUIManager")]
    [SerializeField] private CharacterUIManager characterUIManager;

    [Header("Character")]
    [SerializeField] private Button characterBackground;
    [SerializeField] private SkeletonGraphic characterSkeletonGraphic;
    [SerializeField] private Vector2 characterInitPosition = new Vector2();
    [SerializeField] private float characterMoveDuration;

    [Header("Slider")]
    [SerializeField] private Slider slider;

    private IEnumerator entranceAnimation;

    private void Awake()
    {
        characterInitPosition = characterSkeletonGraphic.rectTransform.anchoredPosition;
    }

    private void OnEnable()
    {
        if (entranceAnimation != null)
        {
            StopCoroutine(entranceAnimation);
        }

        entranceAnimation = EntranceAnimation();

        StartCoroutine(EntranceAnimation());
    }

    private void OnDisable()
    {
        characterSkeletonGraphic.rectTransform.DOComplete();
        characterSkeletonGraphic.rectTransform.anchoredPosition = characterInitPosition;
    }

    public void OnClickCharacterBackground()
    {
        
    }

    private IEnumerator EntranceAnimation()
    {
        // 캐릭터 UI 진입 시 캐릭터가 가운데로 뛰어오는 것처럼 보이게 설정
        characterSkeletonGraphic.AnimationState.SetAnimation(0, "run2", true);
        characterSkeletonGraphic.rectTransform.DOAnchorPos(new Vector2(0, -250f), characterMoveDuration);

        yield return new WaitForSeconds(characterMoveDuration);

        // 캐릭터가 가운데로 도착하면 대기 애니메이션
        characterSkeletonGraphic.AnimationState.SetAnimation(0, "idle", true);

        entranceAnimation = null;
    }
}
