using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System.Collections.Generic;
using Defective.JSON;

public class FlowerPartyPlayFabUpdate : MonoBehaviour
{
    public void UpdateUserFlowerBouquetListData(int presetNumber = -1)
    {
        JSONObject userFlowerBouquetListData = GenerateUserFlowerBouquetListData(presetNumber);

        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "UpdateUserFlowerBouquetListData",
            FunctionParameter = new { jsonData = userFlowerBouquetListData.ToString() },

        };

        PlayFabClientAPI.ExecuteCloudScript(request, 
            result =>
            {
                Debug.Log("CloudScript 실행 성공: " + result.FunctionResult);
            },
            error =>
            {
                Debug.LogError("CloudScript 실행 실패: " + error.GenerateErrorReport());
            });
    }

    private JSONObject GenerateUserFlowerBouquetListData(int presetNumber)
    {
        JSONObject userFlowerBouquetJSONList = new JSONObject(JSONObject.Type.Array);
        int count = 1;

        if (presetNumber == -1)
        {
            count = UserFlowerBouquetData.userFlowerBouquetListDictionary.Count;
        }

        for (int i = 1; i <= count; i++)
        {
            JSONObject userFlowerBouquetJSON = new JSONObject(JSONObject.Type.Object);
            userFlowerBouquetJSON.SetField("isSelected", UserFlowerBouquetData.userFlowerBouquetSelectedListDictionary[i]);
            userFlowerBouquetJSON.SetField("id", i);
            userFlowerBouquetJSON.AddField("bouquetList", GetMemberList(i));

            userFlowerBouquetJSONList.Add(userFlowerBouquetJSON);
        }

        return userFlowerBouquetJSONList;
    }

    private JSONObject GetMemberList(int presetNum)
    {
        JSONObject memberList = new JSONObject(JSONObject.Type.Array);
        int memberCount = UserFlowerBouquetData.userFlowerBouquetListDictionary[presetNum].Length;

        for (int i = 0; i < memberCount; i++)
        {
            memberList.Add(UserFlowerBouquetData.userFlowerBouquetListDictionary[presetNum][i]);
        }

        return memberList;
    }
}
