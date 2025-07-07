using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowerItemUI
{
    public class FlowerBouquetItemUI : FlowerBouquetUI
    {
        [SerializeField] private ItemUIType itemUIType;

        public override void SetFlowerBlockUI(int bouquetId, int slotNumber = -1)
        {
            this.bouquetId = bouquetId;

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

        public override void OnClickBouquetSlotButton()
        {
            // ItemSlot 외에는 클릭 이벤트 발생하지 않음
            if (this.itemUIType != ItemUIType.ItemSlot)
                return;

            FlowerItemUIManager flowerItemUIManager = UIManager.Instance.flowerItemUIManager;

            // 같은 슬롯을 다시 클릭하면 이벤트 발생하지 않음
            if (flowerItemUIManager.GetSelectedBouquetItem() == this)            
                return;
            

            if (flowerItemUIManager.GetSelectedBouquetItem() == null)
            {
                flowerItemUIManager.SetSelectedBouquetItem(this);
                flowerItemUIManager.SetItemSpecificUI(this.bouquetId);
                SetHighlightActive(true);
                return;
            }
            // 클릭한 슬롯이 있고, 다른 슬롯을 클릭한 경우
            else
            {
                // 이전에 클릭한 슬롯의 하이라이트 off
                flowerItemUIManager.GetSelectedBouquetItem().SetHighlightActive(false);

                flowerItemUIManager.SetSelectedBouquetItem(this);
                flowerItemUIManager.SetItemSpecificUI(this.bouquetId);
                SetHighlightActive(true);
            }
        }

        public void SetItemUIType(ItemUIType itemUIType)
        {
            this.itemUIType = itemUIType;
        }
        public ItemUIType GetItemUIType()
        {
            return this.itemUIType;
        }
    }

    public enum ItemUIType
    {
        None,
        Specific,
        ItemSlot
    }
}