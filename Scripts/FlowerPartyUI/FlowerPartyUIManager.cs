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

    /*
    // UI 요소들을 동적으로 생성하는 메서드
    public void SetFlowerPartyUI()
    {
        if(isInitialized)
        {
            return;
        }

        foreach (KeyValuePair<int, int[]> pair in UserFlowerBouquetData.userFlowerBouquetListDictionary)
        {
            int key = pair.Key;
            int[] bouquetList = pair.Value;

            FlowerPartyUI flowerPartyUI = flowerParty.GetComponent<FlowerPartyUI>();
            flowerPartyUI.SetFlowerBouequetUI(key, bouquetList);

            
            if (key < 4) // 아직 빈 파티 슬롯이 있다면, 파티 추가 버튼을 활성화하고
            {
                addButtonPrefab.SetActive(true);
                addButtonPrefab.transform.SetAsLastSibling(); // 파티 추가 버튼은 맨 마지막 정렬한다.
            }
            else // 순회 중 마지막 파티 슬롯까지 전체 사용중이라면, 파티 추가 버튼을 숨김 처리한다.
            {
                addButtonPrefab.SetActive(false);
            }
        }

        isInitialized = true; // 초기 로드 플래그 변경
    }
    */
    public void InitializeFlowerPartyUI()
    {
        if (isInitialized)
            return;
      
        int selectedNumber = FindSelectedNumber();
        InstantiateBackground(selectedNumber);

        DOMoveBackgrounds(selectedNumber, 0f);
        DOScaleBackgrounds(selectedNumber, 0f);
        InstantiateFlowerParty(selectedNumber);

        isInitialized = true;
    }

    public IEnumerator SwitchFlowerPartyUI(int targetNumber, float duration)
    {
        flowerParty.SetActive(false);

        CompleteTweens();
        DOMoveBackgrounds(targetNumber, duration);
        DOScaleBackgrounds(targetNumber, duration);
        ChangeFlowerParty(targetNumber);
        SetIsSelected(targetNumber);

        yield return new WaitForSeconds(duration);
        flowerParty.SetActive(true);
    }
    /*
    public void ResetFlowerPartyUI()
    {
        // 필요한 경우 컨테이너 초기화
        foreach (Transform child in partyContainer)
        {
            Destroy(child.gameObject);
        }
    }
    */
    
    private void InstantiateBackground(int selectedNumber)
    {
        int count = UserFlowerBouquetData.userFlowerBouquetListDictionary.Count;

        for (int i = 0; i < count; i++)
        {
            GameObject pbg = Instantiate(partyBackground, partyBackgrounds);
            pbg.TryGetComponent(out FlowerPartyBackground flowerPartyBackground);
            flowerPartyBackground.SetPresetNumber(i + 1);
            flowerPartyBackground.SetPosition(selectedNumber, -backgroundYSpacing);
            flowerPartyBackgroundList.Add(flowerPartyBackground);
        }
    }

    public int FindSelectedNumber()
    {
        int selectedNumber = -1;

        int count = UserFlowerBouquetData.userFlowerBouquetSelectedListDictionary.Count;

        for (int i = 0; i < count; i++)
        {
            // isSelected
            if (UserFlowerBouquetData.userFlowerBouquetSelectedListDictionary[i + 1])
            {
                selectedNumber = i + 1;
                break;
            }
        }

        return selectedNumber;
    }

    private void SetIsSelected(int selectedNumber)
    {
        int count = UserFlowerBouquetData.userFlowerBouquetSelectedListDictionary.Count;

        for (int i = 0; i < count; i++)
        {
            if (selectedNumber == i + 1)
            {
                UserFlowerBouquetData.userFlowerBouquetSelectedListDictionary[i + 1] = true;
            }
            else
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
        int[] bouquetList = UserFlowerBouquetData.userFlowerBouquetListDictionary[selectedNumber];

        FlowerPartyUI flowerPartyUI = flowerParty.GetComponent<FlowerPartyUI>();
        flowerPartyUI.InitializeFlowerBouquetUI(selectedNumber, bouquetList);
    }

    private void ChangeFlowerParty(int selectedNumber)
    {
        int[] bouquetList = UserFlowerBouquetData.userFlowerBouquetListDictionary[selectedNumber];

        FlowerPartyUI flowerPartyUI = flowerParty.GetComponent<FlowerPartyUI>();
        flowerPartyUI.SetFlowerBouquetUI(selectedNumber, bouquetList);
    }

    public void SetBackgroundActive(bool isActive)
    {
        background.SetActive(isActive);
    }

    public void SetFlowerPartyActive(bool isActive)
    {
        flowerParty.SetActive(isActive);
    }
}
