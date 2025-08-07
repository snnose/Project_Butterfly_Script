using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
using DG.Tweening;

public class CharacterDetailUI : MonoBehaviour
{
    [Header("CharacterUI")]
    [SerializeField] private CharacterUI characterUI;

    [Header("Character Skeleton")]
    [SerializeField] private SkeletonGraphic characterSkeletonGraphic;

    [Header("Slider")]
    [SerializeField] private Slider slider;

    [Header("Animation Slot")]
    [SerializeField] private GameObject animationSlotPrefab;

    [Header("Expression List")] // gathering
    [SerializeField] private GameObject expressionList;
    [SerializeField] private Transform expressionListContent;

    [Header("Motion List")] // idle, rest1, rest2, run
    [SerializeField] private GameObject motionList;
    [SerializeField] private Transform motionListContent;

    [Header("BottomBar")]
    [SerializeField] private Button returnToCharacterUIButton;
    [SerializeField] private Button expressionChangeButton;
    [SerializeField] private Button motionChangeButton;

    public void InitializeCharacterDetailUI()
    {
        InitializeExpressionList();
        InitializeMotionList();
    }

    private void InitializeExpressionList()
    {
        ExposedList<Spine.Animation> animations = characterSkeletonGraphic.Skeleton.Data.Animations;

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
        ExposedList<Spine.Animation> animations = characterSkeletonGraphic.Skeleton.Data.Animations;

        foreach (Spine.Animation animation in animations)
        {
            AnimationSlot animationSlot = Instantiate(animationSlotPrefab, motionListContent).GetComponent<AnimationSlot>();

            // FIXME: 당장은 전체 애니메이션 등록. 나중에 표정과 모션 분리하는 로직 필요함.
            animationSlot.SetAnimationSlot(0, animation.Name, true);
            animationSlot.SetText(animation.Name);
            animationSlot.setSkeletonAnimationState += SetSkeletonAnimationState;
        }
    }

    public void SetSkeletonAnimationState(int trackIndex, string animationName, bool isLoop)
    {
        characterSkeletonGraphic.AnimationState.SetAnimation(trackIndex, animationName, isLoop);
    }

    public void OnValueChanged()
    {
        float baseScale = 0.5f;

        characterSkeletonGraphic.rectTransform.DOScale(baseScale + slider.value, 0f);
    }
}
