using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Defective.JSON;
using PlayFab;
using PlayFab.ClientModels;

namespace FlowerItemUI
{
    public class FlowerItemResetController : MonoBehaviour
    {
        [Header("flowerItemSpecific")]
        [SerializeField] private FlowerItemSpecificUI flowerItemSpecificUI;

        [Header("FlowerItemResetControllerUI")]
        [SerializeField] private Button resetButton;
        // FlowerItemSpecific에서 현재 출력 중인 아이템 정보
        [SerializeField] private UserFlowerBouquet userFlowerBouquet;   

        public void OnClickResetButton()
        {            
            ItemReset();
        }

        private void ItemReset()
        {
            // 우선 깊은 복사로 더미 아이템 생성
            UserFlowerBouquet deepCopiedItem = new UserFlowerBouquet
            {
                id = userFlowerBouquet.id,
                level = userFlowerBouquet.level,
                exp = userFlowerBouquet.exp,
                mainoptionlevel = userFlowerBouquet.mainoptionlevel,
                suboption1 = userFlowerBouquet.suboption1,
                suboption1level = userFlowerBouquet.suboption1level,
                suboption2 = userFlowerBouquet.suboption2,
                suboption2level = userFlowerBouquet.suboption2level
            };

            // 현재 아이템 경험치 초기화
            deepCopiedItem.exp = 0;
            // 현재 아이템 레벨 1로 변경
            deepCopiedItem.level = 1;
            // 현재 아이템 메인 옵션 레벨 초기화
            deepCopiedItem.mainoptionlevel = 1;
            // 현재 아이템 보조 옵션1, 2과 옵션 레벨 모두 초기화
            deepCopiedItem.suboption1 = 0;
            deepCopiedItem.suboption1level = 0;
            deepCopiedItem.suboption2 = 0;
            deepCopiedItem.suboption2level = 0;

            // 서버로 데이터 보내서 갱신
            ExecuteResetUserFlowerBouquet(deepCopiedItem);

            // 클라가 들고있는 dictionary 갱신 → 필요 없을듯?
            //UserItemData.UserItemReset(userItemData.id);

            // ItemReset 이펙트 생성
            //ActivateItemResetEffect();
        }

        public void ExecuteResetUserFlowerBouquet(UserFlowerBouquet deepCopiedItem)
        {
            // 우선 UI 입력을 막는다.
            UIManager.Instance.DisableCanvasGroup();
            // 초기화할 아이템 id를 보내고, 초기화를 진행합니다.
            ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest()
            {
                FunctionName = "resetItem", // 호출할 CloudScript 함수의 이름
                FunctionParameter =
                new
                {
                    playFabId = PlayFabUserData.GetPlayFabId(),
                    EntityId = PlayFabUserData.GetEntityTokenId(),
                    itemId = deepCopiedItem.id,
                }, // 함수에 전달할 매개변수

                GeneratePlayStreamEvent = true // 선택 사항 - 이 이벤트를 PlayStream에서 보여줍니다.
            };
            PlayFabClientAPI.ExecuteCloudScript(request,
            // Success 콜백 함수
            (result) =>
            {
                OnCloudScriptSuccessItemReset(result);
            },
            // Failure 콜백 함수
            error =>
            {
                Debug.LogError($"CloudScript error: {error.GenerateErrorReport()}");
            }
        );
        }
        private void OnCloudScriptSuccessItemReset(ExecuteCloudScriptResult result)
        {
            // JSON 래퍼로 파싱
            string json = result.FunctionResult.ToString();
            var wrapper = JsonUtility.FromJson<ResetItemResult>(json);
            if (wrapper == null || wrapper.updatedItem == null)
            {
                Debug.LogError("Invalid ResetItem response.");
                return;
            }

            wrapper.updatedItem.rarity = ParseRarity(json);

            // 더미 데이터를 실 데이터에 반영
            userFlowerBouquet = wrapper.updatedItem;
            UserFlowerBouquetData.userFlowerBouquetDictionary[userFlowerBouquet.id] = userFlowerBouquet;

            // 데이터 업데이트를 성공하면, 화면에 보이는 UI 갱신
            EffectManager.Instance.ActivateEffect("Effect", "Item_Reset"
                , flowerItemSpecificUI.GetItemSlotRectTransform(), onEffectCompleted: () => flowerItemSpecificUI.SetItemSpecificUI(wrapper.updatedItem.id));

            // 막았던 UI 다시 풀어줌
            UIManager.Instance.EnableCanvasGroup();
        }

        public void SetUserFlowerBouquet(UserFlowerBouquet userFlowerBouquet)
        {
            this.userFlowerBouquet = userFlowerBouquet;
        }

        class ResetItemResult
        {
            public UserFlowerBouquet updatedItem;
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