using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
using DG.Tweening;
using TMPro;
using Defective.JSON;

public class FairyEvolutionPopup : MonoBehaviour
{
    [SerializeField] private FairyChangeUI fairyChangeUI;

    [SerializeField] private int selectedFairyId;
    [SerializeField] private int selectedFairyLevel;
    public TextMeshProUGUI explainText;
    private int userCurrency;
    public GameObject currencyTextObject;
    public TextMeshProUGUI currencyText;
    private int maxCurrency;
    public GameObject maxTextObject;
    public TextMeshProUGUI maxText;
    public GameObject cancelButtonObject;
    public GameObject confirmButtonObject;
    public Button confirmButton;
    public GameObject fairyEvolutionPopupUI;

    public void SetFairyEvolutionPopup()
    {
        userCurrency = UserParameterData.GetUserParameter("experience");
        currencyText.text = userCurrency.ToString();
        int levelupcostperlevel = CharacterData.GetCharacterLevelupCostPerLevel(selectedFairyId);
        maxCurrency = levelupcostperlevel * (1 + selectedFairyLevel); // FIXME:: 레벨에 따른 진화 비용 공식
        maxText.text = maxCurrency.ToString();

        // 만약 현재 유저의 FairyLevel이 최대라면, 버튼 비활성화 및 설명 text 변경
        if (UserData.GetUserFairyLevelData(selectedFairyId) == CharacterData.GetCharacterMaximumLevel(selectedFairyId))
        {
            ObjectPoolManager.Instance.RequestObject("ShadowPopup", "Shadow_Popup", "정령의 레벨이 최대치입니다!!");
            return;
            /*
            explainText.text = "정령의 레벨이 최대치입니다!!"; // FIXME :: 임시 텍스트. 키값으로 변경 필요
            currencyTextObject.SetActive(false);
            maxTextObject.SetActive(false);

            confirmButton.interactable = false; // 버튼 비활성화
            confirmButtonObject.SetActive(false);
            */
        }
        else
        {
            currencyTextObject.SetActive(true);
            maxTextObject.SetActive(true);
            // 확인 버튼 기능 동적 할당
            // 1. 우선 버튼 리스너 초기화부터
            confirmButton.onClick.RemoveAllListeners();
            // 2. FairyLevelup 기능을 동적 할당
            confirmButton.onClick.AddListener(() =>
            {
                // 우선 UI 조작 막기
                UIManager.Instance.DisableCanvasGroup();
                FairyLevelup(selectedFairyId, maxCurrency);
            });
        }

        // 현재 수량과 필요 수량을 비교 후, text 컬러 변경 및 버튼 활성화 처리
        if (userCurrency >= maxCurrency)
        {
            UIManager.Instance.ActiveGlobalVoume();
            explainText.text = "{재화}를 소모하여 정령을 진화시키겠습니까?"; // FIXME :: 임시 텍스트. 키값으로 변경 필요
            currencyText.color = Color.green;
            confirmButton.interactable = true; // 버튼 정상화
            confirmButtonObject.SetActive(true);
            this.gameObject.SetActive(true);
        }
        else if (userCurrency < maxCurrency)
        {
            ObjectPoolManager.Instance.RequestObject("ShadowPopup", "Shadow_Popup", "{재화}가 부족하여 정령을 진화시킬 수 없습니다");
            UIManager.Instance.DeactiveSetGlobalVoume();
            return;
            /*
            explainText.text = "{재화}가 부족하여 정령을 진화시킬 수 없습니다"; // FIXME :: 임시 텍스트. 키값으로 변경 필요
            currencyText.color = Color.red;
            confirmButton.interactable = false; // 버튼 비활성화
            confirmButtonObject.SetActive(false);
            */
        }
    }

    public void FairyLevelup(int characterid, int maxCurrency)
    {
        // 요청 전 우선 UI 조작 부터 막기
        UIManager.Instance.DisableCanvasGroup();
        // 유저 재화 소모 및 레벨업 요청
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest()
        {
            FunctionName = "updateUserFairyLevel", // 호출할 CloudScript 함수의 이름
            FunctionParameter = new { playFabId = PlayFabUserData.GetPlayFabId(), characterId = characterid, currency = maxCurrency }, // 함수에 전달할 매개변수
            GeneratePlayStreamEvent = true // 선택 사항 - 이 이벤트를 PlayStream에서 보여줍니다.
        };
        PlayFabClientAPI.ExecuteCloudScript(request,
        // Success 콜백 함수
        (result) =>
        {
            OnCloudScriptSuccessUpdateUserFairyLevel(result);
        },
        // Failure 콜백 함수
        OnCloudScriptFailure
        );
    }

    private void OnCloudScriptSuccessUpdateUserFairyLevel(ExecuteCloudScriptResult result)
    {
        // CloudScript 결과에서 FunctionResult를 가져옴
        JSONObject functionResult = new JSONObject(result.FunctionResult.ToString());
        Debug.Log(functionResult["success"]);

        // Title exceeded limit (15초 동안 10회만 데이터 변경 가능)발생 시, success이나 result는 false로 처리됨
        if (functionResult["success"].boolValue == false)
        {
            Debug.LogWarning("May be [Title ExceededLimit] occured. (Max 10 changes within 15sec)");
            // 혹은 캐릭터 프리셋 변경에는 쿨타임을 걸어놔야 할 것 같음
        }
        else
        {
            SkeletonGraphic fairySkeletonGraphic = fairyChangeUI.GetFairySkeletonGraphic();

            Debug.Log("SuccessToChangeUserFairyLevelup!");
            // 연출 이펙트 시작 시, 카메라를 흔들면서 틴트를 하얀색으로 바꿔준다. 연출 종료 후 다시 검정색으로 바꿔준다.
            fairySkeletonGraphic.rectTransform.DOShakePosition(2.5f, 50f, 100, 90, true, true);
            FairySkeletonTintControl("_Black", Color.white);
            // 캐릭터 교체 성공 이펙트 출력. 이펙트 출력이 종료된 이후에 UI 조작 가능
            EffectManager.Instance.ActivateEffect("UI", "Character_Evolution", new Vector3(0f, 0f, 100f), true);
            Utils.Utility.RunAfterDelay(2.35f, () =>
            {
                FairySkeletonTintControl("_Black", Color.black);
                Debug.Log("스파인 색 변경 완료!");
            });

            /*
            EffectManager.Instance.ActivateEffect("UI", "Character_Evolution", characterUIController.mainfairy, true, 
            ()=>{
                    characterUIController.FairySkeletonTintControl("_Black", Color.black);
                });
            */
            // 정상 동작 시, 클라이언트 데이터를 갱신
            // 0. "현재" 슬롯의 fairy id 값 및 레벨 확인
            int fairyId = functionResult["characterId"].intValue;
            int fairyLevel = functionResult["characterLevel"].intValue;
            // 1. fairy의 레벨 정보 갱신
            UserCharacterData.UpdateUserCharacterFairyLevelData(fairyId);
            UserData.UpdateUserFairyLevelData(fairyId);
            // 2. fairyUI의 애벌레 스파인 갱신
            fairyChangeUI.SetSkeletonGraphic(fairyId, fairyLevel);
            fairyChangeUI.UpdateFairyList();
            // 3. collection에 fairy의 레벨업을 반영하여 스파인 갱신
            // => collectionUI 진입 시 갱신해주는 것이 좋을 것 같음
            //collectionUIController.UpdateFairySpine(fairyId);
            // 4. exp 갱신
            UIManager.Instance.SetNavigatorExperienceRewards(functionResult["remainingExperience"].intValue);
            fairyChangeUI.SetUserExperienceText(functionResult["remainingExperience"].intValue);
            // 5. fairy 교체 버튼의 AddListener 갱신
            //characterUIController.UpdateCharacterOrFairyButtonAddListener(fairyId, fairyLevel + 1);
            // 6. ADDME :: 레벨 확장된 만큼 선택할 수 있도록 갱신 필요
            // 팝업 창 종료
            fairyEvolutionPopupUI.SetActive(false);
        }
    }
    private void OnCloudScriptFailure(PlayFabError error)
    {
        // 오류 로그를 출력합니다.
        Debug.LogError("CloudScript 호출 실패: " + error.GenerateErrorReport());
    }

    // option : "_Color" = Tint Color 변경, "_Black" = Dark Color 변경
    private void FairySkeletonTintControl(string option, Color color)
    {
        SkeletonGraphic fairySkeletonGraphic = fairyChangeUI.GetFairySkeletonGraphic();
        MeshRenderer fairyMeshRenderer = fairySkeletonGraphic.GetComponent<MeshRenderer>();
        MaterialPropertyBlock materialProperty = new MaterialPropertyBlock();

        fairyMeshRenderer.SetPropertyBlock(materialProperty);

        int id = Shader.PropertyToID(option);
        materialProperty.SetColor(id, color);
        fairyMeshRenderer.SetPropertyBlock(materialProperty);
    }

    public void SetSelectedFairyId(int fairyId)
    {
        selectedFairyId = fairyId;
    }

    public void SetSelectedFairyLevel(int fairyLevel)
    {
        selectedFairyLevel = fairyLevel;
    }
}
