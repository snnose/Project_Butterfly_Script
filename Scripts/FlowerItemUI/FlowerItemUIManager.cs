using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace FlowerItemUI
{
    public class FlowerItemUIManager : MonoBehaviour
    {
        [Header("FlowerBouquetItemSlot Prefab")]
        [SerializeField] private GameObject flowerBouquetItemSlotPrefab;

        [Header("FlowerItemSpecific")]
        [SerializeField] private FlowerItemSpecificUI flowerItemSpecificUI;

        [Header("FlowerItemSlots")]
        public Transform itemContainer;
        [SerializeField] private GameObject flowerItemSlot;
        private Dictionary<int, FlowerBouquetItemUI> flowerItemDictionary = new Dictionary<int, FlowerBouquetItemUI>();

        [Header("Selected Item")]
        [SerializeField] private FlowerBouquetItemUI selectedBouquetItem;

        /// <summary>
        /// FlowerItemUI를 초기화합니다.
        /// </summary>
        public void InitializeFlowerItemUI()
        {
            SetFlowerItemSlots();
            InitializeItemSpecificUI();
        }

        private void InitializeItemSpecificUI()
        {
            if (flowerItemSpecificUI.GetItemSlot().GetBouquetId() == 0)
            {
                SetItemSpecificUI(flowerItemDictionary.Keys.Min());
            }
        }

        /// <summary>
        /// 보유한 Bouquet 아이템 데이터에 맞게 보유 ItemSlotUI를 갱신합니다.
        /// </summary>
        public void SetFlowerItemSlots()
        {
            foreach (KeyValuePair<int, UserFlowerBouquet> item in UserFlowerBouquetData.userFlowerBouquetDictionary)
            {
                int flowerBouquetId = item.Key;
                if (flowerItemDictionary.ContainsKey(flowerBouquetId))
                {
                    continue;
                }

                GameObject newItem = Instantiate(flowerBouquetItemSlotPrefab, itemContainer);

                FlowerBouquetItemUI flowerBouquetItemUI = newItem.GetComponent<FlowerBouquetItemUI>();
                flowerItemDictionary[flowerBouquetId] = flowerBouquetItemUI;
                flowerBouquetItemUI.SetItemUIType(ItemUIType.ItemSlot);
                flowerBouquetItemUI.SetFlowerBlockUI(flowerBouquetId);
            }

        }

        /// <summary>
        /// ItemSpecificUI를 bouquetId에 해당하는 bouquet의 아이템 정보로 갱신합니다.
        /// </summary>
        public void SetItemSpecificUI(int bouquetId)
        {
            flowerItemSpecificUI.SetItemSpecificUI(bouquetId);
        }
        /// <summary>
        /// ItemSpecificUI를 비웁니다.
        /// </summary>
        public void ClearItemSpecificUI()
        {
            flowerItemSpecificUI.ClearItemSpecificUI();
        }

        public void SetSelectedBouquetItem(FlowerBouquetItemUI flowerBouquetItemUI)
        {
            this.selectedBouquetItem = flowerBouquetItemUI;
        }
        public FlowerBouquetItemUI GetSelectedBouquetItem()
        {
            return this.selectedBouquetItem;
        }
    }
}