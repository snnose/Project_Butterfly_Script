using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace FlowerItemUI
{
    public class FlowerItemSpecificUI : MonoBehaviour
    {
        [Header("Item Slot")]
        [SerializeField] private FlowerBouquetItemUI itemSlot;
      
        [Header("Item Overview")]
        [SerializeField] private TextMeshProUGUI itemRarity;
        [SerializeField] private TextMeshProUGUI itemNameandLevel;
        [SerializeField] private Image expBar;
        [SerializeField] private TextMeshProUGUI exp;

        [Header("Item Option")]
        [SerializeField] private TextMeshProUGUI[] itemOptionNames = new TextMeshProUGUI[3];
        [SerializeField] private BubblePopupTrigger[] itemOptionNamesPopupTrigger = new BubblePopupTrigger[3];
        [SerializeField] private TextMeshProUGUI[] itemOptionLevels = new TextMeshProUGUI[3];
        [SerializeField] private TextMeshProUGUI itemSetOptionName;
        [SerializeField] private BubblePopupTrigger itemSetOptionNamePopupTrigger;
        [SerializeField] private TextMeshProUGUI itemSetOptionDescription;
        [SerializeField] private BubblePopupTrigger itemSetOptionDescriptionPopupTrigger;

        [Header("Button")]
        [SerializeField] private Button enhanceButton;
        [SerializeField] private Button resetButton;

        [Header("Enhance Controller")]
        [SerializeField] private FlowerItemEnhanceController flowerItemEnhanceController;
        [Header("Reset Controller")]
        [SerializeField] private FlowerItemResetController flowerItemResetController;

        private StringBuilder stringBuilder = new StringBuilder();
        // private LocalizedString localizedString = new LocalizedString();

        public void SetItemSpecificUI(int bouquetId)
        {
            UserFlowerBouquet userFlowerBouquet = UserFlowerBouquetData.GetUserFlowerBouquet(bouquetId);

            SetItemSlot(bouquetId);
            SetItemOverview(userFlowerBouquet);
            SetItemOption(userFlowerBouquet);
        }

        public void ClearItemSpecificUI()
        {
            SetItemSlot();
            ClearItemOverview();
            ClearItemOption();
        }

        public void OnClickEnhanceButton()
        {
            UserFlowerBouquet userFlowerBouquet = UserFlowerBouquetData.GetUserFlowerBouquet(itemSlot.GetBouquetId());

            if (userFlowerBouquet.level == userFlowerBouquet.maxlevel)
            {
                ObjectPoolManager.Instance.RequestObject("ShadowPopup", "Shadow_Popup",
                            "이미 최대 레벨에 도달했습니다");
                return;
            }

            flowerItemEnhanceController.SetUserFlowerBouquet(userFlowerBouquet);
            flowerItemEnhanceController.gameObject.SetActive(true);
        }

        public void OnClickResetButton()
        {
            UserFlowerBouquet userFlowerBouquet = UserFlowerBouquetData.GetUserFlowerBouquet(itemSlot.GetBouquetId());

            if (userFlowerBouquet.level <= 1)
            {
                ObjectPoolManager.Instance.RequestObject("ShadowPopup", "Shadow_Popup", "이미 초기화된 아이템입니다");
                return;
            }

            flowerItemResetController.SetUserFlowerBouquet(userFlowerBouquet);
            flowerItemResetController.gameObject.SetActive(true);
        }


        private void SetItemSlot(int bouquetId = 0)
        {
            itemSlot.SetFlowerBlockUI(bouquetId);
        }

        private void SetItemOverview(UserFlowerBouquet userFlowerBouquet)
        {
            if (userFlowerBouquet.id == 0)
                return;

            SetRarity(userFlowerBouquet.rarity);
            SetNameAndLevel("", userFlowerBouquet.level - 1);

            int demandExp = userFlowerBouquet.expbase + (userFlowerBouquet.level - 1) * userFlowerBouquet.expadd;

            SetExp(userFlowerBouquet.exp, demandExp);
            SetExpBar(userFlowerBouquet.exp, demandExp);
        }

        private void SetRarity(FlowerBouquetRarity flowerBouquetRarity)
        {
            /// FIXME : 추후 LocalizedStringEvent 사용??
            itemRarity.text = "";

            switch (flowerBouquetRarity)
            {
                case FlowerBouquetRarity.none:
                    return;
                case FlowerBouquetRarity.story:
                    itemRarity.text += "스토리";
                    break;
                case FlowerBouquetRarity.normal:
                    itemRarity.text += "노멀";
                    break;
                case FlowerBouquetRarity.rare:
                    itemRarity.text += "레어";
                    break;
                case FlowerBouquetRarity.epic:
                    itemRarity.text += "에픽";
                    break;
                case FlowerBouquetRarity.legend:
                    itemRarity.text += "전설";
                    break;                    
            }

            itemRarity.text += " 등급";
        }

        private void SetNameAndLevel(string itemName, int itemLevel)
        {
            stringBuilder.Append(itemName);
            stringBuilder.Append(" (+");
            stringBuilder.Append(itemLevel);
            stringBuilder.Append(")");

            itemNameandLevel.text = stringBuilder.ToString();
            stringBuilder.Clear();
        }
        // expBase = 기본 요구 경험치 / expAdd = 꽃의 레벨만큼 요구 경험치 증가 (itemLevel * expAdd)
        private void SetExp(int currentExp, int demandExp)
        {
            stringBuilder.Append(currentExp);
            stringBuilder.Append(" / ");
            stringBuilder.Append(demandExp);

            exp.text = stringBuilder.ToString();
            stringBuilder.Clear();
        }

        private void SetExpBar(int currentExp, int demandExp)
        {
            float ratio = (float)currentExp / (float)demandExp;
            expBar.fillAmount = ratio;
        }

        private void SetItemOption(UserFlowerBouquet userFlowerBouquet)
        {
            int[] optionIdArray = new int[] { userFlowerBouquet.mainoption, userFlowerBouquet.suboption1, userFlowerBouquet.suboption2 };
            int[] optionLevelArray = new int[] { userFlowerBouquet.mainoptionlevel, userFlowerBouquet.suboption1level, userFlowerBouquet.suboption2level };

            SetItemOptionNames(optionIdArray);
            SetItemOptionLevels(optionLevelArray);
            SetCollectionName(userFlowerBouquet.collection);
            SetCollectionDescription(userFlowerBouquet.collection);
        }

        private void SetItemOptionNames(int[] optionIdArray)
        {
            // 옵션은 총 3개까지 이므로 i < 3
            for (int i = 0; i < 3; i ++)
            {
                if (optionIdArray[i] == 0)
                {
                    itemOptionNames[i].text = "-";
                    continue;
                }

                string stringReference = "";
                stringBuilder.Append("string_partyinfo_option_");
                stringBuilder.Append(OptionData.GetOptionKey(optionIdArray[i]));
                stringReference = stringBuilder.ToString();
                stringBuilder.Clear();

                itemOptionNames[i].text = ConvertStringReference(stringReference);
                itemOptionNamesPopupTrigger[i].SetTitleKey(stringReference);

                // 추가로 해당 아이템 옵션의 상세 효과, Description키도 설정해야한다.
                // 그런데 현재 키가 없음.
            }
        }

        private void SetItemOptionLevels(int[] optionLevelArray)
        {
            for (int i = 0; i < 3; i++)
            {
                if (optionLevelArray[i] == 0)
                {
                    itemOptionLevels[i].text = "";
                    continue;
                }

                itemOptionLevels[i].text = optionLevelArray[i].ToString();
            }
        }

        /// <summary>
        /// 세트 옵션의 이름을 설정합니다.
        /// </summary>
        /// <param name="collection"></param>
        private void SetCollectionName(int collection)
        {
            string stringRefernce;
            stringBuilder.Append("string_partyinfo_setoption_title_");
            stringBuilder.Append(CollectionData.GetCollectionKey(collection));
            stringRefernce = stringBuilder.ToString();
            stringBuilder.Clear();

            itemSetOptionName.text = ConvertStringReference(stringRefernce);
        }
        /// <summary>
        /// 세트 옵션 효과 설명을 설정합니다.
        /// </summary>
        /// <param name="collection"></param>
        private void SetCollectionDescription(int collection)
        {
            int optionId = CollectionData.GetCollection(collection).optionid;

            string stringRefernce;
            stringBuilder.Append("string_partyinfo_setoption_");
            stringBuilder.Append(OptionData.GetOptionKey(optionId));
            stringRefernce = stringBuilder.ToString();
            stringBuilder.Clear();

            itemSetOptionDescription.text = ConvertStringReference(stringRefernce);
        }

        private void ClearItemOverview()
        {
            ClearRarity();
            ClearNameAndLevel();
            ClearExp();
            ClearExpBar();
        }

        private void ClearRarity()
        {
            SetRarity(FlowerBouquetRarity.none);
        }

        private void ClearNameAndLevel()
        {
            itemNameandLevel.text = "";
        }

        private void ClearExp()
        {
            exp.text = "";
        }

        private void ClearExpBar()
        {
            SetExpBar(0, 1);
        }

        private void ClearItemOption()
        {
            ClearItemOptionNames();
            ClearItemOptionLevels();
            ClearSetOptionName();
            ClearSetOptionDescription();
        }

        private void ClearItemOptionNames()
        {
            foreach (TextMeshProUGUI optionName in itemOptionNames)
            {
                optionName.text = "옵션";
            }
        }

        private void ClearItemOptionLevels()
        {
            foreach (TextMeshProUGUI optionLevel in itemOptionLevels)
            {
                optionLevel.text = "-";
            }
        }

        private void ClearSetOptionName()
        {
            itemSetOptionName.text = "";
        }

        private void ClearSetOptionDescription()
        {
            itemSetOptionDescription.text = "";
        }

        public FlowerBouquetItemUI GetItemSlot()
        {
            return this.itemSlot;
        }

        public RectTransform GetItemSlotRectTransform()
        {
            return this.itemSlot.GetComponent<RectTransform>();
        }

        private string ConvertStringReference(string stringReference)
        {
            LocalizedString localizedString = new LocalizedString("ui", stringReference);

            string text = localizedString.GetLocalizedString();

            return text;
        }
    }
}