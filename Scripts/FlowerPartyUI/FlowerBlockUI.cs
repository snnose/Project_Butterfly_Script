using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FlowerBlockUI : MonoBehaviour
{
    [SerializeField] public FlowerBouquetUI flowerBouquet;
    [SerializeField] private int id;
    [SerializeField] private int gridNum;
    [SerializeField] public Image blockSprite;
    [SerializeField] private bool isEmpty = true;
    [SerializeField] private bool isFlowerBouquetEditUI = false;

    public void SetFlowerBlock(int grid, int fairyId)
    {
        id = fairyId;
        gridNum = grid;
        SetSpriteByFairyId(id);
        isEmpty = false;
    }
    
    public void SetFlowerBlockEdit()
    {
        isFlowerBouquetEditUI = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isFlowerBouquetEditUI && !isEmpty)
        {
            // 원하는 동작을 여기에 작성
            Debug.Log($"[편집모드] FlowerBlock({gridNum}) 클릭됨, ID: {id}");

            // 예: 블록 초기화
            //ClearBlock();
        }
    }
    private void ClearBlock()
    {
        blockSprite.sprite = null;
        id = -1;
        isEmpty = true;

        flowerBouquet.IncreaseBlockQuantity(-1); // 수량 차감
    }

    public void OnDrop(PointerEventData eventData)
    {
        // 드래그한 오브젝트에서 FlowerItemSlot을 가져옴
        FlowerItemSlot draggedItem = eventData.pointerDrag?.GetComponent<FlowerItemSlot>();
        if(draggedItem == null)
            return;
        int receivedItemId = draggedItem.GetItemId();

        if (draggedItem != null && isEmpty)
        {
            ApplyItem(receivedItemId);
        }
        else if(!isEmpty)
        {
            EffectManager.Instance.ActivateEffect("InGame", "common_spawned", this.gameObject.GetComponent<RectTransform>()); // 임시 이펙트
            SetSpriteByFairyId(receivedItemId);
        }
    }

    private void ApplyItem(int itemId)
    {
        if(!flowerBouquet.CheckBlockQuantityEmpty()) // 현재 배치 가능한 블록 최대 수 검사
        {
            Debug.Log("더이상 block을 배치할 수 없습니다.");
            return;
        }
        id = itemId;
        SetSpriteByFairyId(itemId);
        flowerBouquet.IncreaseBlockQuantity(1);
        isEmpty = false;
        EffectManager.Instance.ActivateEffect("InGame", "common_spawned", this.gameObject.GetComponent<RectTransform>()); // 임시 이펙트
        // 단순 장착 뿐만 아니라 json 데이터 구조도 바꿔야 함
        Debug.Log($"아이템 장착됨: {itemId}");
    }
    private void SetSpriteByFairyId(int id)
    {
        string spriteName = DropData.GetDropKey(FairyData.GetFairyInDrop(id));
        SpriteAtlasManager.Instance.GetSprite("FlowerAtlas", spriteName, sprite =>
        {
            if(sprite != null)
            {
                blockSprite.sprite = sprite;
            }
            else
            {
                Debug.Log($"There's No Party Image [{sprite}] ");
            }
        });
    }
}
