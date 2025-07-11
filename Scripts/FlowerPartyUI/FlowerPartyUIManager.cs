using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

// FlowerPartyUI는 전체 파티 UI를 관리합니다.
public class FlowerPartyUIManager : MonoBehaviour
{
    [Header("FlowerPartyUIList")]
    [SerializeField] public GameObject partySlots;
    [SerializeField] public GameObject partyEdit;
    // Inspector에서 할당할 UI 관련 참조들
    public Canvas flowerPartyUICanvas;
    [SerializeField] private FlowerPartyPlayFabUpdate flowerPartyPlayFabUpdate; // 플레이팹 서버 업데이트 담당 스크립트
    [SerializeField] private FlowerPartySwipe flowerPartySwipe; // 파티 프리셋 스와이프 기능
    [SerializeField] public GameObject background;            // 배경
    [SerializeField] public RectTransform partyBackgrounds;       // 파티 UI 배경들이 들어갈 부모 컨테이너 (GridLayoutGroup이 붙어있음)
    [SerializeField] public GameObject partyBackground;       // 파티 UI 배경 프리팹
    [SerializeField] private List<FlowerPartyBackground> flowerPartyBackgroundList = new List<FlowerPartyBackground>();
    [SerializeField] public GameObject flowerParty;           // instantiate용 파티 UI 프리팹 (PartyUI 스크립트가 부착된 프리팹)
    [SerializeField] public List<FlowerPartyUI> flowerPartyUIList;  // 파티 UI 조작 용도
    [SerializeField] public GameObject addButtonPrefab;       // '추가하기' 버튼 프리팹
    [SerializeField] public GameObject unsavedChangesPopup;   // 저장 확인 팝업 (UI 패널, Confirm/Cancel 버튼 포함)

    // 초기 로드 플래그
    private bool isInitialized = false;
    [SerializeField] private float backgroundYSpacing = 250f;
    [SerializeField] private int maxPresetCount = 5;

    [SerializeField] private int currentPresetNumber;
    public void InitializeFlowerPartyUI()
    {
        if (isInitialized)
            return;

        int selectedNumber = FindSelectedNumber();
        flowerPartySwipe.InitializePartySwipe(selectedNumber);
        InstantiateFlowerParty(selectedNumber);
        SetFlowerParty(selectedNumber);

        isInitialized = true;
    }

    public IEnumerator SwitchFlowerPartyUI(int targetNumber, float duration)
    {
        flowerParty.SetActive(false);

        CompleteTweens();
        DOMoveBackgrounds(targetNumber, duration);
        DOScaleBackgrounds(targetNumber, duration);
        SetFlowerParty(targetNumber);
        SetIsSelected(targetNumber);

        yield return new WaitForSeconds(duration);
        flowerParty.SetActive(true);
    }
    
    public int FindSelectedNumber()
    {
        int selectedNumber = -1;

        foreach (KeyValuePair<int, bool> isSelected in UserFlowerBouquetData.userFlowerBouquetSelectedListDictionary)
        {
            if (isSelected.Value)
            {
                selectedNumber = isSelected.Key;
            }
        }

        return selectedNumber;
    }

    public void SetIsSelected(int selectedNumber)
    {
        UserFlowerBouquetData.userFlowerBouquetSelectedListDictionary[selectedNumber] = true;

        int count = UserFlowerBouquetData.userFlowerBouquetSelectedListDictionary.Count;

        for (int i = 0; i < count; i++)
        {
            if (selectedNumber != i + 1)
            {
                UserFlowerBouquetData.userFlowerBouquetSelectedListDictionary[i + 1] = false;
            }
        }
    }

    private void CompleteTweens()
    {
        int count = flowerPartyBackgroundList.Count;
        for (int i = 0; i < count; i++)
        {
            flowerPartyBackgroundList[i].CompleteTween();
        }
    }

    private void DOMoveBackgrounds(int targetNumber, float duration)
    {
        Vector3 distance = new Vector3(0, (targetNumber - FindSelectedNumber()) * backgroundYSpacing, 0);

        int count = flowerPartyBackgroundList.Count;
        for (int i = 0; i < count; i++)
        {
            flowerPartyBackgroundList[i].DOMoveBackground(distance, duration);
        }
    }

    private void DOScaleBackgrounds(int selectedNumber, float duration)
    {
        int count = flowerPartyBackgroundList.Count;
        for (int i = 0; i < count; i++)
        {
            flowerPartyBackgroundList[i].DOScaleBackground(selectedNumber, duration);
        }
    }

    private void InstantiateFlowerParty(int selectedNumber)
    {
        FlowerPartyUI flowerPartyUI = flowerParty.GetComponent<FlowerPartyUI>();

        if (UserFlowerBouquetData.userFlowerBouquetListDictionary.ContainsKey(selectedNumber))
        {
            flowerPartyUI.InitializeFlowerBouquetUI(selectedNumber);
        }
        else
        {
            flowerPartyUI.SetAddPresetButtonActive(true);
        }
    }

    public void SetFlowerParty(int selectedNumber)
    {
        FlowerPartyUI flowerPartyUI = flowerParty.GetComponent<FlowerPartyUI>();

        // selectedNumber번 프리셋이 있다면 해당 프리셋을 출력
        if (UserFlowerBouquetData.userFlowerBouquetListDictionary.ContainsKey(selectedNumber))
        {
            int[] bouquetList = UserFlowerBouquetData.userFlowerBouquetListDictionary[selectedNumber];

            flowerPartyUI.SetFlowerBouquetUI(selectedNumber, bouquetList);
            flowerPartyUI.SetAddPresetButtonActive(false);
        }
        // 없다면 AddButton을 보여준다
        else
        {
            flowerPartyUI.SetAddPresetButtonActive(true);
        }
    }

    /// <summary>
    /// PlayFab 서버의 FlowerParty 데이터를 갱신합니다.
    /// </summary>
    public void UpdatePlayFabFlowerParty()
    {
        flowerPartyPlayFabUpdate.UpdateUserFlowerBouquetListData();
    }

    public void SetCurrentPresetNumber(int presetNumber)
    {
        this.currentPresetNumber = presetNumber;
    }

    public void SetBackgroundActive(bool isActive)
    {
        background.SetActive(isActive);
    }

    public void SetFlowerPartyActive(bool isActive)
    {
        flowerParty.SetActive(isActive);
    }

    public int GetCurrentPresetNumber()
    {
        return this.currentPresetNumber;
    }

    public List<FlowerPartyBackground> GetFlowerPartyBackgrounds()
    {
        return this.flowerPartyBackgroundList;
    }
}
