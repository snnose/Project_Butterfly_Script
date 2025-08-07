using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class AnimationSlot : MonoBehaviour
{
    [SerializeField] private Button slotButton;
    [SerializeField] private TextMeshProUGUI slotText;

    int animationTrackIndex;
    string animationName;
    bool isLoop;

    // fairyDetailUI로부터 받는 Action 이벤트
    public Action<int, string, bool> setSkeletonAnimationState;

    public void SetAnimationSlot(int trackIndex, string animationName, bool isLoop)
    {
        animationTrackIndex = trackIndex;
        this.animationName = animationName;
        this.isLoop = isLoop;
    }

    // 나중에 stringKey로 Localization을 통한 텍스트 변경으로 개선해야함.
    public void SetText(string text)
    {
        slotText.text = text;
    }

    public void OnClickAnimationSlot()
    {
        setSkeletonAnimationState?.Invoke(animationTrackIndex, animationName, isLoop);
    }
}
