using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Defective.JSON;
using DG.Tweening;

public class FlowerPartyEditManager : MonoBehaviour
{
    public GameObject saveButton;
    public GameObject cancelButton;
    public GameObject slotList;
    public Transform partyBouquetSlotContainer;
    public GameObject bouquetSlot;
    public CanvasGroup partyEditCanvasGroup;
    [Header("PlayFab Update")]
    public FlowerPartyPlayFabUpdate flowerPartyPlayFabUpdate;

    [Header("PartySlots")]
    public List<FlowerBouquetPartyUI> partyBouquetSlotList = new List<FlowerBouquetPartyUI>();

    [Header("FocusSlot")]
    public GameObject focusSlotUI;
    public FlowerBouquetPartyUI focusBouquetSlot;
    [SerializeField] private FlowerItemUI.FlowerItemSpecificUI flowerItemSpecific;
    [SerializeField] private bool isInformationUI = false;
    [SerializeField] private float rotateDuration;

    [Header("FlowerItemSlots")]
    public Transform itemContainer;
    [SerializeField] private GameObject flowerItemSlot;
    private Dictionary<int, GameObject> flowerItemDictionary = new Dictionary<int, GameObject>();

    [Header("Current Bouquet List")]
    [SerializeField] private int editPresetNumber;
    [SerializeField] private int[] editBouquetList = new int[5];

    [Header("Swap Option")]
    [SerializeField] private FlowerBouquetPartyUI catchedBouquet;
    public float maxDropDistance;

    private void OnEnable()
    {
        catchedBouquet = null;
        flowerItemSpecific.ClearItemSpecificUI();

        focusSlotUI.transform.DORotate(Vector3.zero, 0f);
        focusSlotUI.SetActive(true);

        flowerItemSpecific.transform.DORotate(new Vector3(0, 90f, 0), 0f);
        flowerItemSpecific.gameObject.SetActive(false);
    }
    public void SetFlowerItemSlots()
    {
        // 컨테이너 초기화 수행
        //ResetFlowerItemSlots();

        foreach (KeyValuePair<int, UserFlowerBouquet> item in UserFlowerBouquetData.userFlowerBouquetDictionary)
        {
            int flowerBouquetId = item.Key;
            if (flowerItemDictionary.ContainsKey(flowerBouquetId))
            {
                if (CheckEquipBouquet(flowerBouquetId))
                {
                    flowerItemDictionary[flowerBouquetId].SetActive(false);
                }
                else
                {
                    flowerItemDictionary[flowerBouquetId].SetActive(true);
                }
                continue;
            }

            GameObject newItem = Instantiate(bouquetSlot, itemContainer);
            flowerItemDictionary[flowerBouquetId] = newItem;
            if (CheckEquipBouquet(flowerBouquetId))
            {
                newItem.SetActive(false);
            }
            else
            {
                newItem.SetActive(true);
            }

            FlowerBouquetPartyUI flowerBouquetUI = newItem.GetComponent<FlowerBouquetPartyUI>();
            flowerBouquetUI.SetBouquetUIType(BouquetUIType.Item);
            flowerBouquetUI.SetFlowerBlockUI(flowerBouquetId);
        }

        /*
        User user = UserData.GetUser(PlayFabUserData.GetPlayFabId());

        // 전체 fairies 배치
        foreach (JSONObject item in user.fairies)
        {
            if(item["isOwned"].boolValue)
            {
                GameObject newItem = Instantiate(flowerItemSlot, itemContainer);
                newItem.SetActive(true);
                string spriteName = DropData.GetDropKey(FairyData.GetFairyInDrop(item["id"].intValue));
                SpriteAtlasManager.Instance.GetSprite("FlowerAtlas", spriteName, sprite =>
                {
                    var image = newItem.GetComponent<Image>();
                    if (image != null)
                        image.sprite = sprite;
                });
                newItem.GetComponent<FlowerItemSlot>().SetItemId(item["id"].intValue);
            }
            else
            {
                Debug.Log("NotOwnedFairies");
            }
        }
        */
    }
    private bool CheckEquipBouquet(int bouquetId)
    {
        int count = editBouquetList.Length;
        bool isEquip = false;

        for (int i = 0; i < count; i++)
        {
            if (bouquetId == editBouquetList[i])
            {
                isEquip = true;
                break;
            }
        }

        return isEquip;
    }
    public void SetFocusSlot(int bouquetId)
    {
        Debug.Log("SetFocusSlot");
        focusBouquetSlot.SetFlowerBlockUI(bouquetId);
    }
    public FlowerBouquetPartyUI GetFocusSlot()
    {
        return focusBouquetSlot;
    }
    public void InitliazePartySlots(int presetNumber, int[] bouquetList)
    {
        editPresetNumber = presetNumber;

        // Deep Copy
        int length = bouquetList.Length;
        Array.Copy(bouquetList, editBouquetList, length);

        ResetPartySlots(); // 이전에 불러왔던 Bouquet 초기화

        int count = editBouquetList.Length;

        for (int i = 0; i < count; i++)
        {
            InitializePartySlot(editBouquetList[i], i + 1);
            Debug.Log("SetPartyFlowerBouqetListUI");
        }
    }
    private void InitializePartySlot(int bouquetId, int slotNumber)
    {
        Debug.Log("SetFlowerBouquet");
        GameObject item = Instantiate(bouquetSlot, partyBouquetSlotContainer);
        item.SetActive(true);
        FlowerBouquetPartyUI flowerBouquetUI = item.GetComponent<FlowerBouquetPartyUI>();
        partyBouquetSlotList.Add(flowerBouquetUI);
        flowerBouquetUI.SetBouquetUIType(BouquetUIType.Party);
        flowerBouquetUI.SetFlowerBlockUI(bouquetId, slotNumber);

        /*
        // 페이지 표시용 listDot도 추가한다.
        GameObject dotItem = Instantiate(slotDot, slotDotContainer);
        slotDotList.Add(dotItem);
        dotItem.SetActive(true);
        slotMaxPage = slotMaxPage + 1;
        if(slotMaxPage == 1) // 1번 listDot라면 검게 칠해줌
        {
            dotItem.GetComponent<Image>().color = Color.black;
        }
        */
    }
    public void SetPartySlot(int bouquetId, int slotNumber)
    {
        int index = slotNumber - 1;
        partyBouquetSlotList[index].SetFlowerBlockUI(bouquetId, slotNumber);
    }
    public void ResetFlowerItemSlots()
    {
        int childCount = itemContainer.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Destroy(itemContainer.GetChild(i).gameObject);
        }
    }
    public void ResetPartySlots()
    {
        partyBouquetSlotList.Clear();
        int childCount = partyBouquetSlotContainer.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Destroy(partyBouquetSlotContainer.GetChild(i).gameObject);
        }
    }
    /// <summary>
    /// slotNumber번 자리 Bouquet을 catchedBouquet으로 변경합니다
    /// </summary>
    public void ReplacePartySlotBouquet(int slotNumber)
    {
        ReleaseBouquet(slotNumber);
        EquipBouquet(slotNumber);
        StopShakePartySlots();
    }
    private void EquipBouquet(int slotNumber)
    {
        int index = slotNumber - 1;
        int bouquetId = catchedBouquet.GetBouquetId();

        catchedBouquet.SetHighlightActive(false);
        catchedBouquet = null;

        editBouquetList[index] = bouquetId;
        SetPartySlot(bouquetId, slotNumber);

        if (flowerItemDictionary.ContainsKey(bouquetId))
            flowerItemDictionary[bouquetId].SetActive(false);
    }
    /// <summary>
    /// slotNumber번 자리 Bouquet을 장착 해제합니다
    /// </summary>
    private void ReleaseBouquet(int slotNumber)
    {
        int index = slotNumber - 1;
        int bouquetId = editBouquetList[index];

        if (flowerItemDictionary.ContainsKey(bouquetId))
            flowerItemDictionary[bouquetId].SetActive(true);
    }

    /// <summary>
    /// 현재 편집중인 프리셋으로 교체합니다
    /// </summary>
    private void ReplacePreset()
    {
        // 클라이언트 편성 데이터 변경
        int[] replaceBouquetList = new int[editBouquetList.Length];
        Array.Copy(editBouquetList, replaceBouquetList, editBouquetList.Length);
        UserFlowerBouquetData.userFlowerBouquetListDictionary[editPresetNumber] = replaceBouquetList;
        // UI 편성 정보 변경
        UIManager.Instance.flowerPartyUIManager.flowerParty.GetComponent<FlowerPartyUI>().SetFlowerBouquetUI(editPresetNumber, editBouquetList);
    }
    /// <summary>
    /// PartySlot에 등록된 부케 슬롯이 흔들립니다.
    /// </summary>
    public void StartShakePartySlots()
    {
        foreach (FlowerBouquetPartyUI partyBouquetSlot in partyBouquetSlotList)
        {
            partyBouquetSlot.StartShakeBouquetUI(3f, 0.5f, 10f);
        }
    }
    public void StopShakePartySlots()
    {
        foreach (FlowerBouquetPartyUI partyBouquetSlot in partyBouquetSlotList)
        {
            partyBouquetSlot.StopShakeBouqeutUI();
        }
    }

    public void OnClickFocusSlot()
    {
        if (focusBouquetSlot.GetBouquetId() == 0)
            return;

        Vector3 vertical = new Vector3(0f, 90f, 0f);
        Vector3 plane = Vector3.zero;

        if (!isInformationUI)
        {
            focusSlotUI.transform.DORotate(vertical, rotateDuration)
                .OnComplete(() => {
                    focusSlotUI.SetActive(false);
                    flowerItemSpecific.gameObject.SetActive(true);
                    flowerItemSpecific.transform.DORotate(plane, rotateDuration);
                });

            isInformationUI = true;
        }
        else
        {
            flowerItemSpecific.transform.DORotate(vertical, rotateDuration)
                .OnComplete(() => {
                    flowerItemSpecific.gameObject.SetActive(false);
                    focusSlotUI.SetActive(true);
                    focusSlotUI.transform.DORotate(plane, rotateDuration);
                });

            isInformationUI = false;
        }
    }

    public void OnClickSaveButton()
    {
        ReplacePreset();
        flowerPartyPlayFabUpdate.UpdateUserFlowerBouquetListData();
        ExitPartyEditUI();
    }

    public void OnClickCancelButton()
    {
        ExitPartyEditUI();
    }
    public void EnterPartyEditUI()
    {
        
    }
    private void ExitPartyEditUI()
    {
        UIManager.Instance.flowerPartyUIManager.SetBackgroundActive(true);
        UIManager.Instance.flowerPartyUIManager.SetFlowerPartyActive(true);
        UIManager.Instance.flowerPartyUIManager.partySlots.SetActive(true);

        focusBouquetSlot.ClearFlowerBlockUI();

        if (catchedBouquet != null)
            catchedBouquet.SetHighlightActive(false);
        catchedBouquet = null;

        gameObject.SetActive(false);
        UIManager.Instance.lobbyUI.SetActive(true);
    }

    public void SetSelectedBouquet(FlowerBouquetPartyUI flowerBouquetUI)
    {
        catchedBouquet = flowerBouquetUI;
    }
    public FlowerBouquetPartyUI GetSelectedBouquet()
    {
        return catchedBouquet;
    }

    public FlowerItemUI.FlowerItemSpecificUI GetFlowerItemSpecificUI()
    {
        return this.flowerItemSpecific;
    }
}
