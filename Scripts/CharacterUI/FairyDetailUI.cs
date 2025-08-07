using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine;
using Spine.Unity;
using DG.Tweening;
using Utils.SpineUtil;

public class FairyDetailUI : MonoBehaviour
{
    [Header("FairyUI")]
    [SerializeField] private FairyUI fairyUI;

    [Header("Fairy Skeleton")]
    [SerializeField] private SkeletonGraphic fairySkeletonGraphic;

    [Header("Slider")]
    [SerializeField] private Slider slider;

    [Header("Animation Slot")]
    [SerializeField] private GameObject animationSlotPrefab;

    [Header("Expression List")] // effect_rest1, face_eye_blinking, face_eye_closed
    [SerializeField] private GameObject expressionList;
    [SerializeField] private Transform expressionListContent;

    [Header("Motion List")] // idle, rest1, rest2, run
    [SerializeField] private GameObject motionList;
    [SerializeField] private Transform motionListContent;

    [Header("BottomBar")]
    [SerializeField] private Button returnToFairyChangeButton;
    [SerializeField] private Button expressionChangeButton;
    [SerializeField] private Button motionChangeButton;

    private void OnEnable()
    {
        int fairyId = fairyUI.GetCurrentFairyId();
        int fairyLevel = fairyUI.GetCurrentFairyLevel();

        SetSkeletonGraphic(fairyId, fairyLevel);
    }

    public void InitializeFairyDetailUI()
    {
        InitializeExpressionList();
        InitializeMotionList();
    }

    private void InitializeExpressionList()
    {
        ExposedList<Spine.Animation> animations = fairySkeletonGraphic.Skeleton.Data.Animations;

        foreach (Spine.Animation animation in animations)
        {
            AnimationSlot animationSlot = Instantiate(animationSlotPrefab, expressionListContent).GetComponent<AnimationSlot>();

            // FIXME: 당장은 전체 애니메이션 등록. 나중에 표정과 모션 분리하는 로직 필요함.
            animationSlot.SetAnimationSlot(1, animation.Name, true);
            animationSlot.SetText(animation.Name);
            animationSlot.setSkeletonAnimationState += SetSkeletonAnimationState;
        }
    }

    private void InitializeMotionList()
    {
        ExposedList<Spine.Animation> animations = fairySkeletonGraphic.Skeleton.Data.Animations;

        foreach (Spine.Animation animation in animations)
        {
            AnimationSlot animationSlot = Instantiate(animationSlotPrefab, motionListContent).GetComponent<AnimationSlot>();

            // FIXME: 당장은 전체 애니메이션 등록. 나중에 표정과 모션 분리하는 로직 필요함.
            animationSlot.SetAnimationSlot(0, animation.Name, true);
            animationSlot.SetText(animation.Name);
            animationSlot.setSkeletonAnimationState += SetSkeletonAnimationState;
        }
    }

    public void OnValueChanged()
    {
        float baseScale = 1.0f;

        fairySkeletonGraphic.rectTransform.DOScale(baseScale + slider.value, 0f);
    }

    public void SetSkeletonGraphic(int fairyId, int fairyLevel)
    {
        string characterFairyKey = CharacterData.GetCharacterKey(fairyId);
        SpineUtility.SetFromGroupCache(fairySkeletonGraphic, "characters", characterFairyKey);

        fairySkeletonGraphic.Initialize(true);

        // level값에 따라 skin 적용
        string skinname = $"cover{fairyLevel}";
        Skin newSkin = fairySkeletonGraphic.Skeleton.Data.FindSkin(skinname);
        if (newSkin != null)
        {
            // 스킨이 존재하는 경우 변경
            fairySkeletonGraphic.initialSkinName = skinname;
            fairySkeletonGraphic.Skeleton.SetSkin(newSkin);
            Debug.Log($"Skin changed to: {skinname}");
        }
        else
        {
            // 스킨이 없는 경우, Default 스킨으로 변경
            Debug.LogWarning($"fairyId = '{fairyId}' Skin '{skinname}' not found! Reverting to default skin.");
            fairySkeletonGraphic.Skeleton.SetSkin(fairySkeletonGraphic.Skeleton.Data.DefaultSkin); // 기본 스킨 설정
        }
        // 스킨 변경한 경우 반드시 호출하여 변경사항 적용
        fairySkeletonGraphic.Skeleton.SetSlotsToSetupPose();

        // animation 교체
        // FIXME :: animation 네임 통일 필요, 없는 경우 예외 처리 필요
        if (fairySkeletonGraphic.AnimationState == null)
        {
            Debug.LogError("AnimationState가 초기화되지 않았습니다.");
            return;
        }
        // 애니메이션 이름이 존재하는지 확인
        if (fairySkeletonGraphic.SkeletonData.FindAnimation("idle") != null)
        {
            fairySkeletonGraphic.AnimationState.SetAnimation(0, "idle", true);
            Debug.Log($"애니메이션 재생 시작");
        }
        else
        {
            Debug.LogWarning($"애니메이션이(가) 존재하지 않습니다. 애니메이션 재생을 건너뜁니다.");
        }
    }

    public void SetSkeletonAnimationState(int trackIndex, string animationName, bool isLoop)
    {
        fairySkeletonGraphic.AnimationState.SetAnimation(trackIndex, animationName, isLoop);
    }
}
