using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;

public class FlowerBouquetPartyUI : FlowerBouquetUI, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private int slotNumber;  // -1 => 편성 X, 1 ~ 5 => 편성 O
    [SerializeField] private BouquetUIType bouquetUIType;

    // Shake
    private IEnumerator shakeBouquetUI;

    [SerializeField] private GameObject catchedBouquetUI;

    public override void SetFlowerBlockUI(int bouquetId, int slotNumber = -1)
    {
        this.bouquetId = bouquetId;
        this.slotNumber = slotNumber;

        if (bouquetId <= 0)
        {
            ClearFlowerBlockUI();
            return;
        }
        flowerBlockUIList.Clear();
        this.blockQuantity = 0;

        int[] blockList = UserFlowerBouquetData.GetUserFlowerBouquetList(bouquetId);
        for (int i = 0; i < 9; i++) // gridNum은 9까지 있다.
        {
            FlowerBlockUI flowerBlockUI = flowerBlockList[i].GetComponent<FlowerBlockUI>();
            flowerBlockUI.flowerBouquet = this;
            flowerBlockUIList.Add(flowerBlockUI);

            if (blockList[i] == -1)
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

    public override void ClearFlowerBlockUI()
    {
        this.bouquetId = 0;

        for (int i = 0; i < 9; i++)
        {
            FlowerBlockUI flowerBlockUI = flowerBlockList[i].GetComponent<FlowerBlockUI>();
            flowerBlockUI.flowerBouquet = this;
            flowerBlockUI.blockSprite.sprite = null;
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (bouquetUIType != BouquetUIType.Item)
            return;

        if (UIManager.Instance.flowerPartyEditManager.GetSelectedBouquet() != null)
        {
            UIManager.Instance.flowerPartyEditManager.GetSelectedBouquet().SetHighlightActive(false);
        }

        FlowerPartyEditManager partyEditor = UIManager.Instance.flowerPartyEditManager;

        partyEditor.StartShakePartySlots();

        partyEditor.partyEditCanvasGroup.blocksRaycasts = false;  // 드래그 중에는 Raycast 차단
        catchedBouquetUI = Instantiate(gameObject, partyEditor.transform);

        partyEditor.SetSelectedBouquet(catchedBouquetUI.GetComponent<FlowerBouquetPartyUI>());
        FlowerBouquetPartyUI catchedBouquet = partyEditor.GetSelectedBouquet();
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
        UIManager.Instance.flowerPartyEditManager.GetSelectedBouquet().rectTransform.anchoredPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (bouquetUIType != BouquetUIType.Item)
            return;

        FlowerPartyEditManager partyEditManager = UIManager.Instance.flowerPartyEditManager;
        FlowerBouquetPartyUI catchedBouquet = partyEditManager.GetSelectedBouquet();

        FlowerBouquetPartyUI nearestTarget = FindNearestDropTarget(catchedBouquet.transform);
        if (nearestTarget != null
            && CalculateUIDistance(catchedBouquet.transform, nearestTarget.transform) <= partyEditManager.maxDropDistance)
        {
            Debug.Log($"교체 성공! / 거리 : {CalculateUIDistance(catchedBouquet.transform, nearestTarget.transform)}");
            partyEditManager.ReplacePartySlotBouquet(nearestTarget.GetSlotNumber());
        }
        else
        {
            partyEditManager.StopShakePartySlots();
            Debug.Log("교체하기엔 너무 멀다...");
        }

        Destroy(catchedBouquetUI);
        partyEditManager.partyEditCanvasGroup.blocksRaycasts = true;  // 드래그 종료 후 다시 활성화
    }

    private FlowerBouquetPartyUI FindNearestDropTarget(Transform dragTransform)
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

    public override void OnClickBouquetSlotButton()
    {
        FlowerPartyEditManager partyEditManager = UIManager.Instance.flowerPartyEditManager;

        // 현재 Focus 중인 Bouquet을 다시 클릭 시 Focus 해제
        if (partyEditManager.GetSelectedBouquet() != null
            && partyEditManager.GetFocusSlot().bouquetId == this.bouquetId)
        {
            partyEditManager.GetSelectedBouquet().SetHighlightActive(false);
            partyEditManager.GetFocusSlot().ClearFlowerBlockUI();
            partyEditManager.SetSelectedBouquet(null);
            partyEditManager.StopShakePartySlots();
            partyEditManager.GetFlowerItemSpecificUI().ClearItemSpecificUI();
            return;
        }

        partyEditManager.SetFocusSlot(bouquetId);

        if (partyEditManager.GetSelectedBouquet() == null)
        {
            // 선택된 Bouquet가 없을 때, 빈 슬롯은 클릭할 수 없다.
            if (this.bouquetId == 0)
                return;

            partyEditManager.SetSelectedBouquet(this);
            partyEditManager.GetFlowerItemSpecificUI().SetItemSpecificUI(this.bouquetId);
            SetHighlightActive(true);

            // Item 슬롯 선택 시, 파티 슬롯에 Shake 연출 추가
            if (bouquetUIType == BouquetUIType.Item)
            {
                partyEditManager.StartShakePartySlots();
            }
        }
        else
        {
            // 이전에 클릭한 슬롯의 하이라이트 제거
            partyEditManager.GetSelectedBouquet().SetHighlightActive(false);

            // 이전 클릭 슬롯과 같은 타입의 슬롯이면
            if (bouquetUIType == partyEditManager.GetSelectedBouquet().bouquetUIType)
            {
                // 클릭한 슬롯 갱신
                partyEditManager.SetSelectedBouquet(this);
                partyEditManager.GetFlowerItemSpecificUI().SetItemSpecificUI(this.bouquetId);
                SetHighlightActive(true);
                return;
            }
            // Party 슬롯 선택 후, Item 슬롯을 클릭한 경우
            if (bouquetUIType == BouquetUIType.Item)
            {
                int num = partyEditManager.GetSelectedBouquet().slotNumber;
                partyEditManager.GetSelectedBouquet().SetHighlightActive(false);

                partyEditManager.SetSelectedBouquet(this);
                partyEditManager.ReplacePartySlotBouquet(num);
            }
            // Item 슬롯 선택 후, Party 슬롯을 클릭한 경우
            if (bouquetUIType == BouquetUIType.Party)
            {
                partyEditManager.ReplacePartySlotBouquet(slotNumber);
            }
        }
    }

    public void StartShakeBouquetUI(float interval, float duration, float strength = 90f)
    {
        // shake 중에 호출하면 아무것도 하지 않음
        if (shakeBouquetUI != null)
        {
            return;
        }

        shakeBouquetUI = ShakeBouquetUI(interval, duration, strength);
        StartCoroutine(shakeBouquetUI);
    }

    private IEnumerator ShakeBouquetUI(float interval, float duration, float strength = 90f)
    {
        WaitForSeconds repeatInterval = new WaitForSeconds(interval);

        while (true)
        {
            yield return new WaitForSeconds(0.5f * interval);

            this.rectTransform.DOShakeRotation(duration, new Vector3(0f, 0f, strength));

            yield return new WaitForSeconds(0.5f * interval);
        }
    }

    public void StopShakeBouqeutUI()
    {
        if (shakeBouquetUI != null)
        {
            StopCoroutine(shakeBouquetUI);
            this.transform.DOComplete();
            shakeBouquetUI = null;
        }
    }

    public void SetBouquetUIType(BouquetUIType bouquetUIType)
    {
        this.bouquetUIType = bouquetUIType;
    }
    public void SetSlotNumber(int slotNumber)
    {
        this.slotNumber = slotNumber;
    }
    public int GetSlotNumber()
    {
        return slotNumber;
    }
    public BouquetUIType GetBouquetUIType()
    {
        return bouquetUIType;
    }
}

public enum BouquetUIType
{
    None,
    Party,
    Focus,
    Item,
}
