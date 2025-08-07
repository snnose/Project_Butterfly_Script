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

public class FairySkinChangeUI : MonoBehaviour
{
    [Header("FairyUI")]
    [SerializeField] private FairyUI fairyUI;

    [Header("Fairy Skeleton")]
    [SerializeField] private SkeletonGraphic fairySkeletonGraphic;

    [Header("Fairy Skin List")]
    [SerializeField] private GameObject fairySkinSlotPrefab;
    [SerializeField] private GameObject fairySkinList;
    [SerializeField] private Transform fairySkinListContent;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;

    [Header("BottomBar")]
    [SerializeField] private Button returnToFairyDetailButton;
    [SerializeField] private Button skinChangeButton;

    private Dictionary<int, List<FairySkinSlot>> fairySkinListDictionary = new Dictionary<int, List<FairySkinSlot>>();

    [Header("Current Skin")]
    [SerializeField] private FairySkinSlot selectedSkinSlot;
    [SerializeField] private FairySkinSlot usingSkinSlot;

    // FairyUI에서 설정하는 Action 이벤트
    public Action<int, int, int> updatePlayFabCharacter;

    private void OnEnable()
    {
        int fairyId = fairyUI.GetCurrentFairyId();
        int fairyLevel = fairyUI.GetCurrentFairyLevel();

        SetSkeletonGraphic(fairyId, fairyLevel);
        UpdateFairySkinList();
        ShowSelectedFairySkinSlot();
    }

    public void UpdateFairySkinList()
    {
        // 현재 유저가 사용하고 있는 fairy의 Id
        int currentUserFairyId = UserCharacterData.GetUserCharacterSlotId(1); // Fairy는 1번 슬롯 고정
        int currentUserFairyLevel = UserData.GetUserFairyLevelData(currentUserFairyId);

        FairySkinSlot fairySkinSlot;
        int count = 0;

        foreach (KeyValuePair<int, bool> fairyData in UserData.GetUserCharacterFairy())
        {
            if (fairySkinListDictionary.ContainsKey(fairyData.Key))
                continue;

            if (fairyData.Value)
            {
                List<FairySkinSlot> fairySkinSlotList = new List<FairySkinSlot>();
                int fairyId = fairyData.Key;
                int fairyLevel = UserData.GetUserFairyLevelData(fairyId);    // fairy의 현재 레벨
                int fairyMaxLevel = UserData.GetUserFairyLevelData(fairyId); // fairy의 최대 레벨 상태??

                for (int i = 0; i <= fairyMaxLevel; i++)
                {
                    fairySkinSlot = Instantiate(fairySkinSlotPrefab, fairySkinListContent).GetComponent<FairySkinSlot>();
                    fairySkinSlot.gameObject.SetActive(false);

                    fairySkinSlot.SetInitPosition(new Vector2(gridLayoutGroup.padding.left + 0.5f * gridLayoutGroup.cellSize.x + count * (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x),
                                                    -(gridLayoutGroup.padding.top + 0.5f * gridLayoutGroup.cellSize.y)));
                    fairySkinSlot.SetFairySkinChangeUI(this);
                    fairySkinSlot.SetFairyData(fairyId, i);
                    fairySkinSlot.SetSkeletonGraphic(fairyId, i);
                    fairySkinSlot.onClickSkinSlot += OnClickSkinSlot;

                    fairySkinSlotList.Add(fairySkinSlot);
                }

                // Empty Fairy Skin Slot 하나 생성
                // => 해당 슬롯 터치 시, 스킨 추가 예정임을 알리는 Shadow Popup 출력
                fairySkinSlot = Instantiate(fairySkinSlotPrefab, fairySkinListContent).GetComponent<FairySkinSlot>();
                fairySkinSlot.SetFairyData(-1, -1);
                fairySkinSlot.fairySkeletonGraphic.gameObject.SetActive(false);
                fairySkinSlot.GetComponent<Image>().color = Color.gray;
                fairySkinSlotList.Add(fairySkinSlot);

                fairySkinListDictionary[fairyId] = fairySkinSlotList;

                count++;
            }
        }
    }

    private void ShowSelectedFairySkinSlot()
    {
        int fairyId = fairyUI.GetCurrentFairyId();
        int fairyLevel = fairyUI.GetCurrentFairyLevel();

        foreach (KeyValuePair<int, List<FairySkinSlot>> fairySkinSlotList in fairySkinListDictionary)
        {
            foreach (FairySkinSlot fairySkinSlot in fairySkinSlotList.Value)
            {
                // Emtpy Fairy Skin Slot의 경우
                if (fairyId == -1)
                {
                    fairySkinSlot.gameObject.SetActive(true);
                    continue;
                }

                if (fairyId == fairySkinSlot.GetFairySlotId())
                {
                    fairySkinSlot.gameObject.SetActive(true);

                    if (fairyLevel == fairySkinSlot.GetFairySlotLevel())
                    {
                        usingSkinSlot = fairySkinSlot;
                        fairySkinSlot.SetActiveTextBox(true);
                        fairySkinSlot.SetText("사용 중");
                    }
                }
                else
                {
                    fairySkinSlot.gameObject.SetActive(false);
                }
            }
        }
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

    public void OnClickChangeButton()
    {
        int fairyId = selectedSkinSlot.GetFairySlotId();
        int fairyLevel = selectedSkinSlot.GetFairySlotLevel();

        ChangeFairy(fairyId, fairyLevel);

        // 기존에 사용 중인 슬롯 처리
        usingSkinSlot.SetActiveTextBox(false);
        usingSkinSlot.SetText("");

        selectedSkinSlot.GetSlotRectTransform().DOAnchorPos(selectedSkinSlot.initPosition, 0.5f)
            .OnComplete(() =>
            {
                selectedSkinSlot.SetActiveTextBox(true);
                selectedSkinSlot.SetText("사용 중");
            });
        usingSkinSlot = selectedSkinSlot;
    }

    /// <summary>
    /// SkinSlot 버튼에 추가할 OnClick 이벤트.
    /// </summary>
    private void OnClickSkinSlot(int fairyId, int fairyLevel, FairySkinSlot fairySkinSlot)
    {
        SetSkeletonGraphic(fairyId, fairyLevel);
        SetSelectedSkinSlot(fairySkinSlot);
    }

    private void ChangeFairy(int fairyid, int fairylevel)
    {
        // 광클로 ChangeFariy가 여러번 호출되지 않도록 방지
        StartCoroutine(CharacterChangeDelay());

        // FairyUI의 selectedFairy 정보 교체 (없으면 usingFairy)
        FairySlot fairySlot = fairyUI.GetSelectedFairySlot() != null ? fairyUI.GetSelectedFairySlot() : fairyUI.GetUsingFairySlot();
        fairySlot.SetSkeletonGraphic(fairyid, fairylevel);
        fairySlot.SetFairyData(fairyid, fairylevel);
        fairyUI.SetSelectedFairySlot(fairySlot);

        // 스킨 교체 성공 이펙트 출력
        EffectManager.Instance.ActivateEffect("UI", "Character_Change", fairySkeletonGraphic.rectTransform);

        updatePlayFabCharacter?.Invoke(1, fairyid, fairylevel);
    }

    private IEnumerator CharacterChangeDelay()
    {
        UIManager.Instance.DisableCanvasGroup();

        yield return new WaitForSeconds(1.5f);

        UIManager.Instance.EnableCanvasGroup();
    }

    public void SetSelectedSkinSlot(FairySkinSlot fairySkinSlot)
    {
        selectedSkinSlot = fairySkinSlot;
    }
    public FairySkinSlot GetSelectedSkinSlot()
    {
        return selectedSkinSlot;
    }
}
