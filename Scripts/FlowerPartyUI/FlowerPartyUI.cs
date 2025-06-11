using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerPartyUI : MonoBehaviour
{
    [SerializeField] public Transform flowerBouquetUIContainer;
    [SerializeField] public GameObject flowerBouquetUIPrefab;
    [SerializeField] public List<FlowerBouquetUI> flowerBouquetUIList;
    [SerializeField] public GameObject editButton;
    [SerializeField] public GameObject deleteButton;
    [SerializeField] public GameObject selectedMark;

    [SerializeField] private int currentPresetNumber;    
    [SerializeField] private int[] currentBouquetList;

    public void InitializeFlowerBouquetUI(int key, int[] bouquetList)
    {
        currentPresetNumber = key;
        currentBouquetList = bouquetList;

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
        foreach (int bouquetGroupNum in bouquetList)
        {
            GameObject item = Instantiate(flowerBouquetUIPrefab, flowerBouquetUIContainer);
            FlowerBouquetUI flowerBouquetUI = item.GetComponent<FlowerBouquetUI>();
            flowerBouquetUIList.Add(flowerBouquetUI);
            flowerBouquetUI.SetFlowerBlockUI(bouquetGroupNum);
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

    public void OnClickEditButton()
    {
        // LobbyUI를 내린다
        UIManager.Instance.lobbyUI.SetActive(false);
        // 선택한 Party의 구성 요소들을 복사해서 수정 가능한 상태로 띄워준다.
        UIManager.Instance.flowerPartyEditManager.InitliazePartySlots(currentPresetNumber, currentBouquetList);
        // Party 창을 닫고 PartyEdit 창으로 이동한다.
        UIManager.Instance.SetPartyEditUI();
    }
}