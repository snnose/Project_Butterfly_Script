using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class FlowerBouquetUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] public List<GameObject> flowerBlockList;
    [SerializeField] public List<FlowerBlockUI> flowerBlockUIList;
    public RectTransform rectTransform;
    [SerializeField] public Button button;
    public Outline outline;
    public Color outlineColor;

    [SerializeField] private int bouquetId;
    [SerializeField] private int slotNumber;  // -1 => 편성 X, 1 ~ 5 => 편성 O
    //[SerializeField] private int minBlockQuantity = 3;
    [SerializeField] private int maxBlockQuantity = 5;
    [SerializeField] private int blockQuantity = 0;
    [SerializeField] private BouquetUIType bouquetUIType;

    [SerializeField] private GameObject catchedBouquetUI;

    public void SetFlowerBlockUI(int bouquetId, int slotNumber = -1)
    {
        this.bouquetId = bouquetId;
        this.slotNumber = slotNumber;

        int[] blockList = UserFlowerBouquetData.GetUserFlowerBouquetList(bouquetId);
        for (int i = 0; i < 9; i++) // gridNum은 9까지 잇다.
        {
            FlowerBlockUI flowerBlockUI = flowerBlockList[i].GetComponent<FlowerBlockUI>();
            flowerBlockUI.flowerBouquet = this;
            flowerBlockUIList.Add(flowerBlockUI);

            if(blockList[i] == -1)
            {
                //flowerBlockList[item.gridNum - 1].SetActive(false);
                Debug.Log("FIXME :: 일단 false 하는것은 넘기고, 실제로는 투명 이미지로 교체해줘야 함");
                flowerBlockUI.blockSprite.sprite = null;
                continue;
            }
            else
            {
                flowerBlockUI.SetFlowerBlock(i, blockList[i]);
                IncreaseBlockQuantity(1); // 배치한 block 수 증가
            }
        }
    }
    public bool CheckBlockQuantityEmpty()
    {
        if(blockQuantity >= maxBlockQuantity)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public void IncreaseBlockQuantity(int num)
    {
        blockQuantity = blockQuantity + num;
    }
    public void SetBouquetUIType(BouquetUIType bouquetUIType)
    {
        this.bouquetUIType = bouquetUIType;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (bouquetUIType != BouquetUIType.Item)
            return;

        FlowerPartyEditManager partyEditor = UIManager.Instance.flowerPartyEditManager;

        partyEditor.partyEditCanvasGroup.blocksRaycasts = false;  // 드래그 중에는 Raycast 차단
        catchedBouquetUI = Instantiate(gameObject, partyEditor.transform);

        partyEditor.SetCatchedBouquet(catchedBouquetUI.GetComponent<FlowerBouquetUI>());
        FlowerBouquetUI catchedBouquet = partyEditor.GetCatchedBouquet();
        catchedBouquet.rectTransform = catchedBouquetUI.GetComponent<RectTransform>();
        catchedBouquet.rectTransform.sizeDelta = new Vector2(180f, 180f); // 하드코딩 : FlowerItemSlots의 CellSize 사양을 따름
        // 잡힌 UI Anchor preset을 middle center로 변경
        catchedBouquet.rectTransform.anchorMin = 0.5f * Vector2.one;
        catchedBouquet.rectTransform.anchorMax = 0.5f * Vector2.one;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (bouquetUIType != BouquetUIType.Item)
            return;

        if (UIManager.Instance.flowerPartyUIManager.flowerPartyUICanvas == null) 
            return;

        // 마우스 위치를 UI 좌표로 변환
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            UIManager.Instance.flowerPartyUIManager.flowerPartyUICanvas.transform as RectTransform, eventData.position, UIManager.Instance.GetUICamera(), out localPoint);

        // 드래그 중인 UI 요소를 마우스 위치로 이동
        UIManager.Instance.flowerPartyEditManager.GetCatchedBouquet().rectTransform.anchoredPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (bouquetUIType != BouquetUIType.Item)
            return;

        FlowerBouquetUI catchedBouquet = UIManager.Instance.flowerPartyEditManager.GetCatchedBouquet();

        FlowerBouquetUI nearestTarget = FindNearestDropTarget(catchedBouquet.transform);
        if (nearestTarget != null &&
            CalculateUIDistance(catchedBouquet.transform, nearestTarget.transform) <= UIManager.Instance.flowerPartyEditManager.maxDropDistance)
        {
            Debug.Log($"교체 성공! / 거리 : {CalculateUIDistance(catchedBouquet.transform, nearestTarget.transform)}");
            UIManager.Instance.flowerPartyEditManager.ReplacePartySlotBouquet(nearestTarget.slotNumber);
        }
        else
        {
            Debug.Log("교체하기엔 너무 멀다...");
        }

        Destroy(catchedBouquetUI);
        UIManager.Instance.flowerPartyEditManager.partyEditCanvasGroup.blocksRaycasts = true;  // 드래그 종료 후 다시 활성화
    }

    private FlowerBouquetUI FindNearestDropTarget(Transform dragTransform)
    {
        return UIManager.Instance.flowerPartyEditManager.partyBouquetSlotList
            .OrderBy(target => CalculateUIDistance(dragTransform, target.transform))
            .FirstOrDefault();
    }

    private float CalculateUIDistance(Transform a, Transform b)
    {
        Vector2 aLocalPos;
        Vector2 bLocalPos;
        RectTransform parentRect = UIManager.Instance.flowerPartyEditManager.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, a.position, null, out aLocalPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, b.position, null, out bLocalPos);

        return Vector2.Distance(aLocalPos, bLocalPos);
    }

    public void OnClickBouquetSlotButton()
    {
        // 현재 Focus 중인 Bouquet을 다시 클릭 시 Focus 해제
        if (UIManager.Instance.flowerPartyEditManager.GetFocusSlot() != null
            && UIManager.Instance.flowerPartyEditManager.GetFocusSlot().bouquetId == this.bouquetId)
        {

        }

        UIManager.Instance.flowerPartyEditManager.SetFocusSlot(bouquetId);

        if (UIManager.Instance.flowerPartyEditManager.GetCatchedBouquet() == null)
        {
            UIManager.Instance.flowerPartyEditManager.SetCatchedBouquet(this);
            SetHighlightActive(true);
        }
        else
        {
            if (bouquetUIType == UIManager.Instance.flowerPartyEditManager.GetCatchedBouquet().bouquetUIType)
                return;

            if (bouquetUIType == BouquetUIType.Item)
            {
                int num = UIManager.Instance.flowerPartyEditManager.GetCatchedBouquet().slotNumber;
                UIManager.Instance.flowerPartyEditManager.GetCatchedBouquet().SetHighlightActive(false);

                UIManager.Instance.flowerPartyEditManager.SetCatchedBouquet(this);
                UIManager.Instance.flowerPartyEditManager.ReplacePartySlotBouquet(num);
            }

            if (bouquetUIType == BouquetUIType.Party)
            {
                UIManager.Instance.flowerPartyEditManager.ReplacePartySlotBouquet(slotNumber);
            }
        }
    }

    public void SetHighlightActive(bool isActive)
    {
        if (isActive)
        {
            outlineColor.a = 1;
        }
        else
        {
            outlineColor.a = 0;
        }

        outline.effectColor = outlineColor;
    }

    public int GetBouquetId()
    {
        return bouquetId;
    }
}

public enum BouquetUIType
{
    None,
    Party,
    Focus,
    Item,
}