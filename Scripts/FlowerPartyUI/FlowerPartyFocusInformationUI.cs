using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlowerPartyFocusInformationUI : MonoBehaviour
{
    public FlowerBouquetPartyUI focusBouquetUI;
    public TextMeshProUGUI bouquetName;

    [Header("Bouquet Level")]
    public TextMeshProUGUI bouquetLevel;
    public TextMeshProUGUI exp;

    [Header("bouquet Options")]
    public TextMeshProUGUI[] optionNameArray = new TextMeshProUGUI[3]; // Option 개수 = 최대 3개

    [Header("Buttons")]
    public Button enhanceButton;
    public Button resetButton;

    public void SetFocusInformation(FlowerBouquetPartyUI focusBouquetUI)
    {
        int bouquetId = focusBouquetUI.GetBouquetId();
        UserFlowerBouquet userFlowerBouquet = UserFlowerBouquetData.GetUserFlowerBouquet(bouquetId);

        SetBouquetImage(bouquetId);

    }

    private void SetBouquetImage(int bouquetId)
    {
        focusBouquetUI.SetFlowerBlockUI(bouquetId);
    }

    private void SetBouquetName()
    {

    }

    private void SetBouquetLevel()
    {

    }

    private void SetBouquetOptions(UserFlowerBouquet userFlowerBouquet)
    {
        int[] optionId = new int[3];
        // 메인 옵션 id 설정
        optionId[0] = userFlowerBouquet.mainoption;
        // 서브 옵션 1, 2 (없으면 optionId에 -1)
        if (userFlowerBouquet.suboption1level > 0)
            optionId[1] = userFlowerBouquet.suboption1;
        else
            optionId[1] = -1;

        if (userFlowerBouquet.suboption2level > 0)
            optionId[2] = userFlowerBouquet.suboption2;
        else
            optionId[2] = -1;

        for (int i = 0; i < 3; i++)
        {
            if (optionId[i] == -1)
                continue;

            // 텍스트에 옵션 이름, 레벨 설정

        }
    }

    public void OnClickEnhanceButton()
    {

    }

    public void OnClickResetButton()
    {

    }
}
