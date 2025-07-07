using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class FlowerBouquetUI : MonoBehaviour
{
    [SerializeField] public List<GameObject> flowerBlockList;
    [SerializeField] public List<FlowerBlockUI> flowerBlockUIList;
    public RectTransform rectTransform;
    [SerializeField] public Button button;
    public Outline outline;
    public Color outlineColor;

    [SerializeField] protected int bouquetId;
    //[SerializeField] private int minBlockQuantity = 3;
    [SerializeField] protected int maxBlockQuantity = 5;
    [SerializeField] protected int blockQuantity = 0;

    public abstract void SetFlowerBlockUI(int bouquetId, int slotNumber = -1);
    public abstract void ClearFlowerBlockUI();
    public bool CheckBlockQuantityEmpty()
    {
        if(blockQuantity >= maxBlockQuantity)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public void IncreaseBlockQuantity(int num)
    {
        blockQuantity = blockQuantity + num;
    }

    public abstract void OnClickBouquetSlotButton();
    public void SetHighlightActive(bool isActive)
    {
        if (isActive)
        {
            outlineColor.a = 1;
        }
        else
        {
            outlineColor.a = 0;
        }

        outline.effectColor = outlineColor;
    }
    public int GetBouquetId()
    {
        return bouquetId;
    }
}