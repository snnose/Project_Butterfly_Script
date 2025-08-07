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

public class CollectionFlowerView : MonoBehaviour
{
    [Header("CollectionUIManager")]
    [SerializeField] private CollectionUIManager collectionUIManager;

    [Header("Each Collections")]
    [SerializeField] private GameObject flowerSlotPrefab; // 동적으로 생성할 아이템 슬롯 프리팹
    [SerializeField] private GameObject flowerEachView;
    [SerializeField] private Transform eachViewContent;
    [SerializeField] private Dictionary<int, ItemSlotProp> flowerSlotDictionary = new Dictionary<int, ItemSlotProp>();

    [Header("Bundle Collections")]
    [SerializeField] private GameObject flowerBundlePrefab;
    [SerializeField] private GameObject flowerBundleView;
    [SerializeField] private Transform bundleViewContent;

    [Header("RedDotNodes")]
    [SerializeField] private RedDotNode flowerTabButtonNode;

    [Header("View Change Button")]
    [SerializeField] private Image bundleButtonImage;
    [SerializeField] private Image eachButtonImage;

    // CollectionUIManager에서 추가하는 onClick 이벤트 (SpecificUI 활성화/비활성화)
    public Action showFlowerSpecific;
    public Action<int, string> setSpecificTableReference;

    private void OnEnable()
    {
        OnClickBundleButton();
    }

    public void InstantiateFlowerView()
    {
        InstantiateFlowerEachView();
        InstantiateFlowerBundleView();
    }

    private void InstantiateFlowerEachView()
    {
        Dictionary<int, Drop> dropDictionary = DropData.GetDropDictionary();

        Debug.LogWarning($"dropDictionary Count: {dropDictionary.Count}");

        foreach (KeyValuePair<int, Drop> dropData in dropDictionary)
        {
            int flowerId = dropData.Key;

            // 이미 생성한 슬롯이면 생략.
            if (flowerSlotDictionary.ContainsKey(flowerId))
                continue;

            Drop flowerData = dropData.Value;
            string flowerKey = flowerData.key;

            ItemSlotProp flowerSlot = Instantiate(flowerSlotPrefab, eachViewContent).GetComponent<ItemSlotProp>();
            flowerSlot.SetId(flowerId);
            flowerSlot.SetSprite(flowerKey);

            SubscribeRedDotNode(flowerTabButtonNode, flowerId);
            SubscribeRedDotNode(flowerSlot.GetComponent<RedDotNode>(), flowerId);
            // PlayerPrefs를 참조해 해당 fairy_slot의 RedDot이 켜져있는지 확인
            RedDotSystem.SetRedDot(flowerId.ToString(), PlayerPrefs.GetInt(flowerId.ToString()));

            flowerSlot.GetButton().onClick.AddListener(() => { OnClickFlowerSlot(flowerId, flowerKey); });

            flowerSlotDictionary[flowerId] = flowerSlot;

            CheckIsOwnedFlower(flowerSlot, flowerId);
        }
    }

    private void InstantiateFlowerBundleView()
    {
        Dictionary<int, Drop> dropDictionary = DropData.GetDropDictionary();
        Dictionary<string, CollectionFlowerBundle> flowerBundleDictionary = new Dictionary<string, CollectionFlowerBundle>();

        foreach (KeyValuePair<int, Drop> dropData in dropDictionary)
        {
            int flowerId = dropData.Key;
            string dropKey = dropData.Value.key;
            string flowerKey = FairyData.GetFairyKey(FairyData.GetFairyIdByDropId(flowerId));
            CollectionFlowerBundle flowerBundle;

            if (flowerKey == null)
                continue;

            if (!flowerBundleDictionary.ContainsKey(flowerKey))
            {
                flowerBundle = Instantiate(flowerBundlePrefab, bundleViewContent).GetComponent<CollectionFlowerBundle>();
                flowerBundleDictionary[flowerKey] = flowerBundle;

                // LocalizeStringKey 설정
                flowerBundle.SetFlowerTitleText(flowerKey);
                //flowerBundle.SetFlowerTitleStringKey(flowerKeyLocalizeStringKey);
            }
            else
            {
                flowerBundle = flowerBundleDictionary[flowerKey];
            }

            ItemSlotProp flowerSlot = Instantiate(flowerSlotPrefab, flowerBundle.GetFlowerGridTransform()).GetComponent<ItemSlotProp>();
            flowerSlot.SetId(flowerId);
            flowerSlot.SetSprite(dropKey);

            SubscribeRedDotNode(flowerTabButtonNode, flowerId);
            SubscribeRedDotNode(flowerSlot.GetComponent<RedDotNode>(), flowerId);
            // PlayerPrefs를 참조해 해당 fairy_slot의 RedDot이 켜져있는지 확인
            RedDotSystem.SetRedDot(flowerId.ToString(), PlayerPrefs.GetInt(flowerId.ToString()));

            flowerSlot.GetButton().onClick.AddListener(() => { OnClickFlowerSlot(flowerId, dropKey); });
            CheckIsOwnedFlower(flowerSlot, flowerId);
        }
    }

    private void SubscribeRedDotNode(RedDotNode redDotNode, int flowerId)
    {
        redDotNode.Subscribe.Add(flowerId.ToString());
    }

    private void OnClickFlowerSlot(int flowerId, string flowerKey)
    {
        // 버튼 터치 시 사운드 출력
        UIManager.Instance.PlayUISFX("event:/sfx_common_button_02");
        UIManager.Instance.ActiveGlobalVoume();

        showFlowerSpecific.Invoke();
        setSpecificTableReference(flowerId, flowerKey);

        // RedDot이 켜져있으면 끈다
        RedDotSystem.ClearRedDot(flowerId.ToString());
    }

    private void CheckIsOwnedFlower(ItemSlotProp flowerSlot, int flowerId)
    {
        Image flowerImage = flowerSlot.GetImage();

        // 유저가 Fairy를 보유중인 경우. 캐릭터를 활성화하고 터치하면 팝업이 생기게 된다.
        if (UserData.GetUserCharacterFairyOwnedData(flowerId))
        {
            flowerSlot.GetButton().interactable = true;
            flowerImage.color = Color.white;
        }
        else // 캐릭터를 보유하고 있지 않은 경우
        {
            // 버튼 클릭을 막는다
            flowerSlot.GetButton().interactable = false;
            // CollectionUI - Fairy 가 활성화 중인 경우에만 tint 적용되어야 한다.
            flowerImage.color = Color.black;
        }
    }

    public void OnClickViewChangeButton()
    {
        if (flowerEachView.activeSelf)
        {
            OnClickBundleButton();
        }
        else
        {
            OnClickEachButton();
        }
    }

    public void OnClickEachButton()
    {
        flowerEachView.SetActive(true);
        flowerBundleView.SetActive(false);

        eachButtonImage.color = Color.white;
        bundleButtonImage.color = new Color(0, 0, 0, 0.33f);
    }

    public void OnClickBundleButton()
    {
        flowerEachView.SetActive(false);
        flowerBundleView.SetActive(true);

        eachButtonImage.color = new Color(1, 1, 1, 0.33f);
        bundleButtonImage.color = Color.black;
    }
}
