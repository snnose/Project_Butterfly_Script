using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerPartyUI : MonoBehaviour
{
    [SerializeField] public Transform flowerBouquetUIContainer;
    [SerializeField] public GameObject flowerBouquetUIPrefab;
    [SerializeField] public List<FlowerBouquetPartyUI> flowerBouquetUIList;
    [SerializeField] public GameObject editButton;
    [SerializeField] public GameObject deleteButton;
    [SerializeField] public GameObject selectedMark;
    public Button addPresetButton;

    [SerializeField] private int currentPresetNumber;    
    [SerializeField] private int[] currentBouquetList;

    public void InitializeFlowerBouquetUI(int key)
    {
        currentPresetNumber = key;

        /*
        if(UserFlowerBouquetData.userFlowerBouquetSelectedListDictionary[key])
        {
            selectedMark.SetActive(true);
        }
        else
        {
            selectedMark.SetActive(false);
        }
        */
        
        // 프리셋 하나에 부케를 5개 편성하므로 i < 5
        for (int i = 0; i < 5; i++)
        {
            GameObject item = Instantiate(flowerBouquetUIPrefab, flowerBouquetUIContainer);
            FlowerBouquetPartyUI flowerBouquetUI = item.GetComponent<FlowerBouquetPartyUI>();
            flowerBouquetUIList.Add(flowerBouquetUI);
        }
    }

    public void SetFlowerBouquetUI(int key, int[] bouquetList)
    {
        currentPresetNumber = key;
        currentBouquetList = bouquetList;
        int count = bouquetList.Length;

        for (int i = 0; i < count; i++)
        {
            flowerBouquetUIList[i].SetFlowerBlockUI(bouquetList[i]);
        }
    }

    private void ClearFlowerBouquetUI()
    {
        foreach (FlowerBouquetPartyUI flowerBouquetUI in flowerBouquetUIList)
        {
            flowerBouquetUI.ClearFlowerBlockUI();
        }
    }

    public void SetAddPresetButtonActive(bool isActive)
    {
        addPresetButton.gameObject.SetActive(isActive);

        flowerBouquetUIContainer.gameObject.SetActive(!isActive);
        editButton.gameObject.SetActive(!isActive);
        deleteButton.gameObject.SetActive(!isActive);
    }

    public void OnClickEditButton()
    {
        // LobbyUI를 내린다
        UIManager.Instance.lobbyUI.SetActive(false);
        // 선택한 Party의 구성 요소들을 복사해서 수정 가능한 상태로 띄워준다.
        UIManager.Instance.flowerPartyEditManager.InitliazePartySlots(currentPresetNumber, currentBouquetList);
        // Party 창을 닫고 PartyEdit 창으로 이동한다.
        UIManager.Instance.SetPartyEditUI();
    }

    public void OnClickDeleteButton()
    {
        int presetNum = UIManager.Instance.flowerPartyUIManager.GetCurrentPresetNumber();

        // 현 preset을 삭제
        UserFlowerBouquetData.userFlowerBouquetListDictionary.Remove(presetNum);

        SetAddPresetButtonActive(true);
    }

    public void OnClickAddPresetButton()
    {
        int presetNum = UIManager.Instance.flowerPartyUIManager.GetCurrentPresetNumber();

        // 현재 preset에 새 프리셋을 생성한다
        int[] bouquetList = new int[] { 0, 0, 0, 0, 0 };
        currentBouquetList = bouquetList;
        UserFlowerBouquetData.userFlowerBouquetListDictionary[presetNum] = bouquetList;

        // 편성되지 않은 FlowerBouquetUI 출력 
        ClearFlowerBouquetUI();

        SetAddPresetButtonActive(false);

        OnClickEditButton();
    }
}