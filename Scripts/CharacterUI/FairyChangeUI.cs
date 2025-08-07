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

public class FairyChangeUI : MonoBehaviour
{
    [SerializeField] private FairyUI fairyUI;

    [Header("Fairy")]
    [SerializeField] private Button fairyBackground;
    [SerializeField] private SkeletonGraphic fairySkeletonGraphic;
    [SerializeField] private float fairyMoveDuration;

    [Header("Move Position")]
    [SerializeField] Vector2 initPosition;
    [SerializeField] Vector2 movePosition;

    [Header("Active Skill")]
    [SerializeField] private GameObject activeSkillBox;
    [SerializeField] private TextMeshProUGUI activeSkillName;

    [Header("Fairy List")]
    [SerializeField] private GameObject fairySlotPrefab;
    [SerializeField] private GameObject fairyList;
    [SerializeField] private Transform fairyListContent;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;

    [Header("Fairy Evolution")]
    [SerializeField] private FairyEvolutionPopup fairyEvolutionPopup;
    [SerializeField] private TextMeshProUGUI requireExpText;

    [Header("BottomBar")]
    [SerializeField] private Button evolutionButton;
    [SerializeField] private Button fairyChangeButton;

    [Header("User Currency")]
    [SerializeField] private TextMeshProUGUI userExperience;
    [SerializeField] private TextMeshProUGUI itemExperience;

    private Dictionary<int, FairySlot> fairyListDictionary = new Dictionary<int, FairySlot>();

    // FairyUI에서 설정하는 Action 이벤트
    public Action<int, int, int> updatePlayFabCharacter;

    private IEnumerator entranceAnimation;

    private void OnEnable()
    {
        // FairyChangeUI 진입 시 정령 입장 애니매이션
        if (entranceAnimation != null)        
           StopCoroutine(entranceAnimation);
        
        entranceAnimation = EntranceAnimation();
        StartCoroutine(EntranceAnimation());

        // SkeletonGraphic 설정
        int fairyId = fairyUI.GetCurrentFairyId();
        int fairyLevel = fairyUI.GetCurrentFairyLevel();

        SetSkeletonGraphic(fairyId, fairyLevel);

        // 재화 텍스트 설정
        SetUserExperienceText(UserParameterData.GetUserParameter("experience"));
        SetItemExperienceText(UserParameterData.GetUserParameter("itemexperience"));
    }

    private void OnDisable()
    {
        fairySkeletonGraphic.rectTransform.DOComplete();
    }

    public void InitializeFairyChangeUI()
    {
        //InitializeFairySkeletonGraphic();
        UpdateFairyList();
    }

    private void InitializeFairySkeletonGraphic()
    {
        int currentUserFairyId = UserCharacterData.GetUserCharacterSlotId(1);
        int currentUserFairyLevel = UserData.GetUserFairyLevelData(currentUserFairyId);

        SetSkeletonGraphic(currentUserFairyId, currentUserFairyLevel);
    }

    public void UpdateFairyList()
    {
        // 현재 유저가 사용하고 있는 fairy의 Id
        int currentUserFairyId = UserCharacterData.GetUserCharacterSlotId(1); // Fairy는 1번 슬롯 고정
        int count = 0;

        foreach (KeyValuePair<int, bool> fairyData in UserData.GetUserCharacterFairy())
        {
            if (fairyData.Value)
            {
                int fairyId = fairyData.Key;
                int fairyLevel = UserData.GetUserFairyLevelData(fairyId);

                // id가 100 미만이면 character이므로 패스
                if (fairyId < 100 || fairyListDictionary.ContainsKey(fairyId))
                    continue;

                // 슬롯 prefab 생성
                FairySlot fairySlot = Instantiate(fairySlotPrefab, fairyListContent).GetComponent<FairySlot>();
                fairyListDictionary[fairyId] = fairySlot;

                fairySlot.SetFairyUI(fairyUI);
                fairySlot.SetInitPosition(new Vector2(gridLayoutGroup.padding.left + 0.5f * gridLayoutGroup.cellSize.x + count * (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x),
                                                    -(gridLayoutGroup.padding.top + 0.5f * gridLayoutGroup.cellSize.y)));

                fairySlot.setSkeletonGraphic += SetSkeletonGraphic;
                fairySlot.setRequireExpText += SetRequireExpText;
                fairySlot.SetFairyData(fairyId, fairyLevel);
                fairySlot.SetSkeletonGraphic(fairyId, fairyLevel);

                // 서버 상 유저가 사용하는 정령과 같을 때 처리
                if (fairyId == currentUserFairyId)
                {
                    SetRequireExpText(fairyId);
                    fairyUI.SetUsingFairySlot(fairySlot);
                    fairySlot.SetText("사용 중");
                    fairySlot.SetActiveTextBox(true);
                }

                count++;
            }
        }
    }

    public void SetSkeletonGraphic(int fairyId, int fairyLevel)
    {
        // 설정 전 default 스킨 설정
        fairySkeletonGraphic.initialSkinName = "default";

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

    private IEnumerator EntranceAnimation()
    {
        // 애벌레UI 진입 시 애벌레가 가운데로 뛰어오는 것처럼 보이게 설정
        fairySkeletonGraphic.AnimationState.SetAnimation(0, "run", true);
        fairySkeletonGraphic.rectTransform.DOAnchorPos(initPosition, 0f);
        fairySkeletonGraphic.rectTransform.DOAnchorPos(movePosition, fairyMoveDuration);

        yield return new WaitForSeconds(fairyMoveDuration);

        // 애벌레가 가운데로 도착하면 대기 애니메이션
        fairySkeletonGraphic.AnimationState.SetAnimation(0, "idle", true);

        yield break;
    }

    public void OnClickFairyBackground()
    {
        
    }

    public void OnClickFairyChangeButton()
    {
        if (fairyUI.GetSelectedFairySlot() == null)
        {
            ObjectPoolManager.Instance.RequestObject("ShadowPopup", "Shadow_Popup", "정령을 선택해주세요!");
            return;
        }

        if (fairyUI.GetUsingFairySlot() == fairyUI.GetSelectedFairySlot())
        {
            ObjectPoolManager.Instance.RequestObject("ShadowPopup", "Shadow_Popup", "다른 정령을 선택해주세요!");
            return;
        }

        fairyUI.ClearUsingFairySlot();
        fairyUI.SetUsingFairySlot(fairyUI.GetSelectedFairySlot());
        ChangeFairy(fairyUI.GetUsingFairySlot().GetFairySlotId(), fairyUI.GetUsingFairySlot().GetFairySlotLevel());

        fairyUI.GetUsingFairySlot().GetSlotRectTransform().DOAnchorPos(fairyUI.GetUsingFairySlot().initPosition, 0.5f)
            .OnComplete(() =>
            {
                // 사용 중 텍스트 출력
                fairyUI.GetUsingFairySlot().SetActiveTextBox(true);
                fairyUI.GetUsingFairySlot().SetText("사용 중");
            });
    }
    public void OnClickEvolutionButton()
    {
        FairySlot selectedFairySlot = fairyUI.GetSelectedFairySlot();
        FairySlot usingFairySlot = fairyUI.GetUsingFairySlot();

        // 선택 중인 애벌레의 Id, Level을 가져온다. 없다면 사용 중인 애벌레의 Id, Level로 대체한다.
        int selectedFairyId = selectedFairySlot != null ? selectedFairySlot.GetFairySlotId() : usingFairySlot.GetFairySlotId();
        int selectedFairyLevel = selectedFairySlot != null ? selectedFairySlot.GetFairySlotLevel() : usingFairySlot.GetFairySlotLevel();

        fairyEvolutionPopup.SetSelectedFairyId(selectedFairyId);
        fairyEvolutionPopup.SetSelectedFairyLevel(selectedFairyLevel);
        fairyEvolutionPopup.SetFairyEvolutionPopup();
    }
    private void ChangeFairy(int fairyid, int fairylevel)
    {
        // 광클로 ChangeFariy가 여러번 호출되지 않도록 방지
        StartCoroutine(CharacterChangeDelay());

        // 스킨 교체 성공 이펙트 출력
        EffectManager.Instance.ActivateEffect("UI", "Character_Change", fairySkeletonGraphic.rectTransform);
        //ActivateCharacterChangeEffect();

        // FIXME :: 변경과 동시에 서버 송신?? 갱신 시점이 너무 잦지 않게 변경 필수
        updatePlayFabCharacter?.Invoke(1, fairyid, fairylevel);
        //GetCharacterUIManager().playfabUpdateCharacter.UpdateUserCharacterOrFairy(1, fairyid, fairylevel);
    }

    private IEnumerator CharacterChangeDelay()
    {
        UIManager.Instance.DisableCanvasGroup();

        yield return new WaitForSeconds(1.5f);

        UIManager.Instance.EnableCanvasGroup();
    }

    private void SetRequireExpText(int fairyId)
    {
        int userCurrency = UserParameterData.GetUserParameter("experience");
        int levelupcostperlevel = CharacterData.GetCharacterLevelupCostPerLevel(fairyId);
        int level = UserData.GetUserFairyLevelData(fairyId);
        int requireExp = levelupcostperlevel * (1 + level); // FIXME:: 레벨에 따른 진화 비용 공식

        Debug.LogWarning($"진화 비용 : {requireExp}");

        requireExpText.text = requireExp.ToString();

        if (userCurrency >= requireExp)
        {
            requireExpText.color = Color.green;
        }
        else
        {
            requireExpText.color = Color.red;
        }
    }

    public void SetUserExperienceText(int userExp)
    {
        userExperience.text = userExp.ToString();
    }
    public void SetItemExperienceText(int itemExp)
    {
        itemExperience.text = itemExp.ToString();
    }

    public SkeletonGraphic GetFairySkeletonGraphic()
    {
        return fairySkeletonGraphic;
    }
}
