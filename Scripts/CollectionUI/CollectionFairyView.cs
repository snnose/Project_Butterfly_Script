using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Events;
using UnityEngine.Localization.Components;
using Defective.JSON;
using RedDot;
using Utils.SpineUtil;

public class CollectionFairyView : MonoBehaviour
{
    [Header("CollectionUIManager")]
    [SerializeField] private CollectionUIManager collectionUIManager;

    [Header("FairyCollections")]
    [SerializeField] private GameObject fairySlotPrefab; // 동적으로 생성할 fairy 아이템 슬롯 프리팹
    [SerializeField] private Transform fairyViewContainer;
    [SerializeField] private GameObject fairyItemSpecific;
    [SerializeField] private Dictionary<int, ItemSlotProp> fairySlotDictionary = new Dictionary<int, ItemSlotProp>();

    [Header("FairySpecific")]
    [SerializeField] private LocalizeStringEvent fairy_name;
    [SerializeField] private Image fairy_color; // 임시 테스트를 위해 Image로 정의
    [SerializeField] private Image fairy_type; // 임시 테스트를 위해 Image로 정의
    [SerializeField] private LocalizeStringEvent fairy_skill;
    [SerializeField] private LocalizeStringEvent fairy_explanation;

    [Header("RedDotNodes")]
    [SerializeField] private RedDotNode fairyTabButtonNode;

    // CollectionUIManager에서 추가하는 onClick 시의 이벤트
    public Action showFairySpecific;
    public Action<string> setSpecificTableReference;

    private void OnEnable()
    {
        foreach (KeyValuePair<int, ItemSlotProp> fairySlot in fairySlotDictionary)
        {
            CheckIsOwnedFairy(fairySlot.Value, fairySlot.Key);
        }
    }

    public void InstantiateFairyView()
    {
        var characterDictionary = CharacterData.GetCharacterDictionary();

        foreach (KeyValuePair<int, Character> characterData in characterDictionary)
        {
            int fairyId = characterData.Key;

            // Character는 생략
            if (characterData.Value.characterType == CharacterType.butterfly)
                continue;

            Character fairyData = characterData.Value;

            string fairyKey = fairyData.key;
            int fairyLevel = UserData.GetUserFairyLevelData(fairyId);

            ItemSlotProp fairySlot = Instantiate(fairySlotPrefab, fairyViewContainer).GetComponent<ItemSlotProp>();
            fairySlot.SetId(fairyId);

            SubscribeRedDotNode(fairyTabButtonNode, fairyId);
            SubscribeRedDotNode(fairySlot.GetComponent<RedDotNode>(), fairyId);

            // PlayerPrefs를 참조해 해당 fairy_slot의 RedDot이 켜져있는지 확인
            RedDotSystem.SetRedDot(fairyId.ToString(), PlayerPrefs.GetInt(fairyId.ToString()));

            SkeletonGraphic fairySlotSkeletonGraphic = fairySlot.GetSkeletonGraphic();
            // 레벨에 따른 스파인 교체 추가
            SpineUtility.SetFromGroupCache(fairySlotSkeletonGraphic, "characters", fairyKey);

            fairySlotSkeletonGraphic.Initialize(true);

            // 스킨 및 애니메이션 설정
            SpineUtility.SetSkeletonSkin(fairySlotSkeletonGraphic, $"cover{fairyLevel}");
            SpineUtility.SetSkeletionAnimation(fairySlotSkeletonGraphic, 0, "rest1", true);
            SpineUtility.SetSkeletionAnimation(fairySlotSkeletonGraphic, 1, "effect_rest1", true);
            fairySlotSkeletonGraphic.SetVerticesDirty();

            fairySlot.GetButton().onClick.AddListener(() => { OnClickFairySlot(fairyId, fairyKey); });
            
            fairySlotDictionary[fairyId] = fairySlot;

            CheckIsOwnedFairy(fairySlot, fairyId);
        }
    }

    private void SubscribeRedDotNode(RedDotNode redDotNode, int fairyId)
    {
        redDotNode.Subscribe.Add(fairyId.ToString());
    }

    private void OnClickFairySlot(int fairyId, string fairyKey)
    {
        UIManager.Instance.PlayUISFX("event:/sfx_common_button_02");
        UIManager.Instance.ActiveGlobalVoume();

        showFairySpecific.Invoke();
        setSpecificTableReference.Invoke(fairyKey);

        // RedDot이 켜져있으면 끈다
        RedDotSystem.ClearRedDot(fairyId.ToString());
    }
    
    private void CheckIsOwnedFairy(ItemSlotProp fairySlot,  int fairyId)
    {
        SkeletonGraphic fairySlotSkeletonGraphic = fairySlot.GetSkeletonGraphic();

        // 유저가 Fairy를 보유중인 경우. 캐릭터를 활성화하고 터치하면 팝업이 생기게 된다.
        if (UserData.GetUserCharacterFairyOwnedData(fairyId))
        {
            fairySlot.GetButton().interactable = true;
            fairySlotSkeletonGraphic.color = Color.white;
        }
        else // 캐릭터를 보유하고 있지 않은 경우
        {
            // 버튼 클릭을 막는다
            fairySlot.GetButton().interactable = false;
            // CollectionUI - Fairy 가 활성화 중인 경우에만 tint 적용되어야 한다.
            fairySlotSkeletonGraphic.color = Color.black;
        }
    }
}
