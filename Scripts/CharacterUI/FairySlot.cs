using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine;
using Spine.Unity;
using Utils.SpineUtil;
using DG.Tweening;

public class FairySlot : MonoBehaviour
{
    public SkeletonGraphic fairySkeletonGraphic;
    public GameObject textBox;
    public TextMeshProUGUI textBoxText;

    [Header("Slot")]
    [SerializeField] private RectTransform slotRectTransform;

    [Header("FairyUI")]
    private FairyUI fairyUI;

    [Header("Buttons")]
    public Button slotButton;
    [SerializeField] private Button changeButton;
    [SerializeField] private TextMeshProUGUI changeButtonText;

    [Header("FairyData")]
    [SerializeField]
    private int fairyId;
    [SerializeField]
    private int fairyLevel;

    [Header("Move Option")]
    [SerializeField] public Vector2 selectedMovePosition;
    [SerializeField] public Vector2 initPosition;

    // FairyChangeUI에서 추가하는 Action 이벤트
    public Action<int, int> setSkeletonGraphic;
    public Action<int> setRequireExpText;

    public void SetFairyData(int id, int level)
    {
        fairyId = id;
        fairyLevel = level;
    }
    public int GetFairySlotId()
    {
        return fairyId;
    }
    public int GetFairySlotLevel()
    {
        return fairyLevel;
    }
    public Button GetSlotButton()
    {
        return slotButton;
    }
    public Button GetChangeButton()
    {
        return changeButton;
    }
    public void EnableChangeButton()
    {
        changeButton.interactable = true;
        changeButtonText.text = "선택";
    }
    public void DisableChangeButton()
    {
        changeButton.interactable = false;
        changeButtonText.text = "사용 중";
    }

    public SkeletonGraphic GetSkeletonGraphic()
    {
        return fairySkeletonGraphic;
    }
    public TextMeshProUGUI GetText()
    {
        return textBoxText;
    }
    public void SetSkeletonGraphic(int fairyId, int fairyLevel)
    {
        string characterFairyKey = CharacterData.GetCharacterKey(fairyId);
        SpineUtility.SetFromGroupCache(fairySkeletonGraphic, "characters", characterFairyKey);

        fairySkeletonGraphic.Initialize(true);

        // RayCast 설정
        fairySkeletonGraphic.raycastTarget = true;

        // level값에 따라 skin 적용
        string skinname = "cover" + fairyLevel.ToString();
        Skin newSkin = fairySkeletonGraphic.SkeletonData.FindSkin(skinname);
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

    public void OnClickSlotButton()
    {
        FairySlot selectedFairySlot = fairyUI.GetSelectedFairySlot();

        if (selectedFairySlot == this)
            return;

        // 이전에 선택 중인 슬롯 처리
        if (selectedFairySlot != null)
        {
            selectedFairySlot.GetSlotRectTransform().DOAnchorPos(selectedFairySlot.initPosition, 0.5f);

            fairyUI.ClearSelectedFairySlot();
        }

        setRequireExpText?.Invoke(fairyId);
        setSkeletonGraphic?.Invoke(fairyId, fairyLevel);
        fairyUI.SetSelectedFairySlot(this);

        slotRectTransform.DOAnchorPos(initPosition + selectedMovePosition, 0.5f);
    }
    public void SetInitPosition(Vector2 initPosition)
    {
        this.initPosition = initPosition;
    }
    public void SetText(string text)
    {
        this.textBoxText.text = text;
    }
    public void SetActiveTextBox(bool value)
    {
        textBox.SetActive(value);
    }
    public void SetFairyUI(FairyUI fairyUI)
    {
        this.fairyUI = fairyUI;
    }
    public RectTransform GetSlotRectTransform()
    {
        return slotRectTransform;
    }
}
