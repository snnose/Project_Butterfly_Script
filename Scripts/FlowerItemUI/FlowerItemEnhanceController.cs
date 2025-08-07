using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab.Json;
using PlayFab;
using PlayFab.ClientModels;

namespace FlowerItemUI
{
    public class FlowerItemEnhanceController : MonoBehaviour
    {
        [Header("flowerItemSpecific")]
        [SerializeField] private FlowerItemSpecificUI flowerItemSpecificUI;

        [Header("FlowerItemEnhanceControllerUI")]
        [SerializeField] private Button enhanceButton;      // 강화 버튼
        [SerializeField] public Slider enhanceSlider;             // 강화 수치를 조절하는 슬라이더
        [SerializeField] public TextMeshProUGUI sliderValueText;      // 슬라이더 값 텍스트

        [SerializeField] private UserFlowerBouquet userFlowerBouquet;
        [SerializeField] private int itemMaxEnhanceValue;   // 선택된 아이템의 레벨업 요구 경험치
        [SerializeField] private int userCurrency;

        private void OnEnable()
        {
            userCurrency = UserParameterData.GetUserParameter("itemexperience");
            itemMaxEnhanceValue = userFlowerBouquet.expbase + userFlowerBouquet.expadd * (userFlowerBouquet.level - 1);
            
            SetSliderMaxValue();
            UpdateButtonAndText();
        }

        public void OnClickEnhanceButton()
        {
            ExecuteItemExpUp(userFlowerBouquet, int.Parse(enhanceSlider.value.ToString()));  
            //ItemExpUp(int.Parse(enhanceSlider.value.ToString()));
            this.gameObject.SetActive(false);
        }

        public void OnClickPlusButton()
        {
            enhanceSlider.value++;
            sliderValueText.text = enhanceSlider.value.ToString();
        }

        public void OnClickMinusButton()
        {
            enhanceSlider.value--;
            sliderValueText.text = enhanceSlider.value.ToString();
        }

        public void OnClickMinButton()
        {
            enhanceSlider.value = 0;
            sliderValueText.text = enhanceSlider.value.ToString();
        }

        public void OnClickMaxButton()
        {
            SetSliderMaxValue();
        }

        public bool CheckItemLevel(UserFlowerBouquet userFlowerBouquet)
        {
            // 아이템 최고 레벨 도달 시
            if (userFlowerBouquet.level >= userFlowerBouquet.maxlevel)
            {
                return true;
            }

            return false;
        }

        private void SetSliderMaxValue()
        {
            int demandExp = itemMaxEnhanceValue - userFlowerBouquet.exp;
            int maxValue = Mathf.Min(userCurrency, demandExp);

            enhanceSlider.maxValue = maxValue;
            enhanceSlider.value = maxValue;
            sliderValueText.text = maxValue.ToString();
        }

        public void OnSliderValueChanged()
        {
            // 슬라이더 값이 userCurrency 이상으로 올라가지 않도록 제한
            if (enhanceSlider.value > userCurrency)
            {
                enhanceSlider.value = userCurrency;  // 최대 가능한 값으로 조정
                // 또는 팝업 경고창 출력
            }

            sliderValueText.text = enhanceSlider.value.ToString();  // 슬라이더 값 텍스트 업데이트
            UpdateButtonAndText();  // 버튼 상태 및 텍스트 색상 업데이트
        }
 
        public void UpdateButtonAndText()
        {
            // 보유 재화 부족 시 텍스트 색상 변경 및 버튼 비활성화
            if (userCurrency <= enhanceSlider.value)
            {
                sliderValueText.color = Color.red;  // 재화 부족 시 텍스트를 빨간색으로 표시
                enhanceButton.interactable = false; // 버튼 비활성화
            }
            else
            {
                sliderValueText.color = Color.black; // 재화 충분 시 텍스트를 기본 색으로 표시
                enhanceButton.interactable = true;   // 버튼 활성화
            }
        }

        private void ExecuteItemExpUp(UserFlowerBouquet userFlowerBouquet, int addedExp)
        {
            // 우선 UI 입력을 막는다.
            UIManager.Instance.DisableCanvasGroup();

            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "updateItemExp",
                FunctionParameter = new {
                    EntityId = PlayFabUserData.GetEntityTokenId(),
                    itemId = userFlowerBouquet.id,
                    addedExp = addedExp 
                },
                GeneratePlayStreamEvent = true    // (선택) 클라우드스트림 이벤트 생성
            };
            PlayFabClientAPI.ExecuteCloudScript(request,
            result => {

                Debug.Log("CloudScript raw result: " + result.FunctionResult.ToString());

                // 1) JSON 문자열로 변환
                string json = result.FunctionResult.ToString();

                // 2) JsonUtility 파싱
                var wrapper = JsonUtility.FromJson<UpdateItemExpResult>(json);
                if (wrapper == null || wrapper.updatedItem == null)
                {
                    Debug.LogError("Unexpected CloudScript result format.");
                    return;
                }

                wrapper.updatedItem.rarity = ParseRarity(json);

                // 더미 데이터를 실 데이터에 반영
                userFlowerBouquet = wrapper.updatedItem;
                UserFlowerBouquetData.userFlowerBouquetDictionary[userFlowerBouquet.id] = userFlowerBouquet;

                // 강화 이펙트 실행하고, 끝나면 아이템 UI 갱신
                EffectManager.Instance.ActivateEffect("Effect", "Item_Enhance",
                    flowerItemSpecificUI.GetItemSlotRectTransform(), onEffectCompleted: () => flowerItemSpecificUI.SetItemSpecificUI(userFlowerBouquet.id));

                // 유저가 소모한 재화의 변동사항을 클라이언트 데이터에 반영
                UserCurrencyUpdate(addedExp);

                // 유저가 소모한 재화의 변동사항에 따라 UI도 함께 갱신
                UserCurrencyUIUpdate();

                // 막았던 UI 다시 풀어줌
                UIManager.Instance.EnableCanvasGroup();
            },
            error => {
                Debug.LogError($"CloudScript error: {error.GenerateErrorReport()}");
                // 막았던 UI 일단 다시 풀어줌
                UIManager.Instance.EnableCanvasGroup();
            });
        }

        public void UserCurrencyUpdate(int currency)
        {
            UserParameterData.SubstractUserParameterData("itemexperience", currency);
        }
        public void UserCurrencyUIUpdate()
        {
            UIManager.Instance.ResetNavigatorUI();
        }

        public void SetUserFlowerBouquet(UserFlowerBouquet userFlowerBouquet)
        {
            this.userFlowerBouquet = userFlowerBouquet;
        }

        /// <summary>
        /// Json 파싱용 클래스
        /// </summary>
        [Serializable]
        class UpdateItemExpResult
        {
            public UserFlowerBouquet updatedItem;
            public int remainingExp;
        }

        private FlowerBouquetRarity ParseRarity(string json)
        {
            // 원본 JSON에서 rarity 문자열 추출
            string rarityStr = null;
            int idx = json.IndexOf("\"rarity\"");
            if (idx >= 0)
            {
                int colon = json.IndexOf(':', idx);
                int firstQuote = json.IndexOf('\"', colon + 1);
                int secondQuote = json.IndexOf('\"', firstQuote + 1);
                rarityStr = json.Substring(firstQuote + 1, secondQuote - firstQuote - 1);
            }

            // 문자열 → enum 변환
            if (!string.IsNullOrEmpty(rarityStr)
                && Enum.TryParse<FlowerBouquetRarity>(rarityStr, true, out var parsed))
            {
                return parsed;
            }

            return FlowerBouquetRarity.none;
        }
    }
}