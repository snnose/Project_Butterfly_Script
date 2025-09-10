using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;

public static class UserFlowerBouquetData
{
    public static Dictionary<int, int[]> userFlowerBouquetListDictionary = new Dictionary<int, int[]>();
    public static Dictionary<int, bool> userFlowerBouquetSelectedListDictionary = new Dictionary<int, bool>();
    public static Dictionary<int, UserFlowerBouquet> userFlowerBouquetDictionary = new Dictionary<int, UserFlowerBouquet>();

    public static void SetUserFlowerBouquetDataDictionary(List<UserFlowerBouquet> userFlowerBouquetList)
    {
        foreach (UserFlowerBouquet userFlowerBouquet in userFlowerBouquetList)
        {
            if (!userFlowerBouquetDictionary.ContainsKey(userFlowerBouquet.id))
            {
                userFlowerBouquetDictionary.Add(userFlowerBouquet.id, userFlowerBouquet);
            }
        }
    }

    public static void SetUserFlowerBouquetListDataDictionary(List<UserFlowerBouquetList> userFlowerBouquetPartyList)
    {
        foreach (UserFlowerBouquetList userFlowerBouquetParty in userFlowerBouquetPartyList)
        {
            if (!userFlowerBouquetListDictionary.ContainsKey(userFlowerBouquetParty.id))
            {
                userFlowerBouquetListDictionary[userFlowerBouquetParty.id] = userFlowerBouquetParty.bouquetList;
            }

            if (!userFlowerBouquetSelectedListDictionary.ContainsKey(userFlowerBouquetParty.id))
            {
                userFlowerBouquetSelectedListDictionary[userFlowerBouquetParty.id] = userFlowerBouquetParty.isSelected;
            }
        }
    }

    public static List<UserFlowerBouquet> UpdateUserFlowerBouquetData()
    {
        List<UserFlowerBouquet> userFlowerBouquetList = new List<UserFlowerBouquet>();

        foreach (KeyValuePair<int, UserFlowerBouquet> pair in userFlowerBouquetDictionary)
        {
            userFlowerBouquetList.Add(pair.Value);
        }

        return userFlowerBouquetList;
    }

    public static List<UserFlowerBouquetList> UpdateUserFlowerBouquetListData()
    {
        List<UserFlowerBouquetList> userFlowerBouquetPartyList = new List<UserFlowerBouquetList>();

        foreach (KeyValuePair<int, int[]> pair in userFlowerBouquetListDictionary)
        {
            UserFlowerBouquetList userFlowerBouquetList = new UserFlowerBouquetList();

            userFlowerBouquetList.id = pair.Key;
            userFlowerBouquetList.bouquetList = userFlowerBouquetListDictionary[pair.Key];
            userFlowerBouquetList.isSelected = userFlowerBouquetSelectedListDictionary[pair.Key];

            userFlowerBouquetPartyList.Add(userFlowerBouquetList);
        }

        return userFlowerBouquetPartyList;
    }

    /// <summary>
    /// UserFlowerBouquetDictionary에 UserFlowerBouquet을 추가합니다.
    /// </summary>
    public static void AddUserFlowerBouquet(int flowerBouquetId, UserFlowerBouquet userFlowerBouquet)
    {
        if (!userFlowerBouquetDictionary.ContainsKey(flowerBouquetId))
        {
            userFlowerBouquetDictionary[flowerBouquetId] = userFlowerBouquet;
        }
        else
        {
            Debug.LogWarning($"UserFlowerBouquetData - AddUserFlowerBouquet / id : {flowerBouquetId} 키가 이미 존재합니다!");
        }
    }

    public static Dictionary<int, UserFlowerBouquet> GetUserFlowerBouquetDictionary()
    {
        return userFlowerBouquetDictionary;
    }

    /*
    public static void SetUserFlowerBouquetData(JSONObject jsonObject)
    {
        UserFlowerBouquet userFlowerBouquet = new UserFlowerBouquet();

        userFlowerBouquet.id = jsonObject["id"].intValue;
        userFlowerBouquet.key = jsonObject["key"].stringValue;
        userFlowerBouquet.rarity = (FlowerBouquetRarity)Enum.Parse(typeof(FlowerBouquetRarity), jsonObject["rarity"].stringValue);
        userFlowerBouquet.collection = jsonObject["collection"].intValue;
        userFlowerBouquet.level = jsonObject["level"].intValue;
        userFlowerBouquet.maxlevel = jsonObject["maxlevel"].intValue;
        userFlowerBouquet.exp = jsonObject["exp"].intValue;
        userFlowerBouquet.expbase = jsonObject["expbase"].intValue;
        userFlowerBouquet.expadd = jsonObject["expadd"].intValue;
        userFlowerBouquet.mainoption = jsonObject["mainoption"].intValue;
        userFlowerBouquet.mainoptionlevel = jsonObject["mainoptionlevel"].intValue;
        userFlowerBouquet.suboption1 = jsonObject["suboption1"].intValue;
        userFlowerBouquet.suboption1level = jsonObject["suboption1level"].intValue;
        userFlowerBouquet.suboption2 = jsonObject["suboption2"].intValue;
        userFlowerBouquet.suboption2level = jsonObject["suboption2level"].intValue;
        
        // JSON 객체에서 "fairyList" 키에 해당하는 배열 가져오기
        JSONObject fairyListJsonObject = jsonObject.GetField("fairyList");
        // JSONObject를 int[]로 변환
        int[] fairyListArray = new int[fairyListJsonObject.count];
        for (int i = 0; i < fairyListJsonObject.count; i++)
        {
            fairyListArray[i] = fairyListJsonObject[i].intValue;
        }
        // 변환된 int[]를 userFlowerBouquet.fairyList 할당
        userFlowerBouquet.fairyList = fairyListArray;

        userFlowerBouquet.isDecomposable = jsonObject["isDecomposable"].boolValue;
        userFlowerBouquet.decomposeCurrencyType = (CurrencyType)Enum.Parse(typeof(CurrencyType), jsonObject["decomposeCurrencyType"].stringValue);
        userFlowerBouquet.decomposeCurrencyAmount = jsonObject["decomposeCurrencyAmount"].intValue;
        userFlowerBouquet.acquisitionDate = jsonObject["acquisitionDate"].stringValue;

        if (!userFlowerBouquetDictionary.ContainsKey(userFlowerBouquet.id))
        {
            userFlowerBouquetDictionary.Add(userFlowerBouquet.id, userFlowerBouquet);
        }
    }
    public static void SetUserFlowerBouquetListData(JSONObject jsonObject)
    {
        UserFlowerBouquetList userFlowerBouquetList = new UserFlowerBouquetList();

        userFlowerBouquetList.isSelected = jsonObject["isSelected"].boolValue;
        userFlowerBouquetList.id = jsonObject["id"].intValue;
        // JSON 객체에서 "bouquetList" 키에 해당하는 배열 가져오기
        JSONObject bouquetListJsonObject = jsonObject.GetField("bouquetList");
        // JSONObject를 int[]로 변환
        int[] bouquetListArray = new int[bouquetListJsonObject.count];
        for (int i = 0; i < bouquetListJsonObject.count; i++)
        {
            bouquetListArray[i] = bouquetListJsonObject[i].intValue;
        }
        // 변환된 int[]를 userFlowerBouquetList.bouquetList 할당
        userFlowerBouquetList.bouquetList = bouquetListArray;

        if (!userFlowerBouquetListDictionary.ContainsKey(userFlowerBouquetList.id))
        {
            userFlowerBouquetListDictionary.Add(userFlowerBouquetList.id, userFlowerBouquetList.bouquetList);
        }
        if (!userFlowerBouquetSelectedListDictionary.ContainsKey(userFlowerBouquetList.id))
        {
            userFlowerBouquetSelectedListDictionary.Add(userFlowerBouquetList.id, userFlowerBouquetList.isSelected);
        }
    }
    */

    // 해당 UserFlowerBouquet id를 기준으로 반환
    public static UserFlowerBouquet GetUserFlowerBouquet(int id)
    {
        if(userFlowerBouquetDictionary.ContainsKey(id))
        {
            UserFlowerBouquet userFlowerBouquet = userFlowerBouquetDictionary[id];
            return userFlowerBouquet;
        }
        else
        {
            Debug.LogError("UserFlowerBouquet / Id:"+ id +" Cannot Found!");
            return null;
        }
    }

    // 해당 userFlowerBouquetList를 id를 기준으로 반환
    public static int[] GetUserFlowerBouquetListKey(int id)
    {
        if(userFlowerBouquetListDictionary.ContainsKey(id))
        {
            int[] bouquetList = userFlowerBouquetListDictionary[id];
            return bouquetList;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return null;
        }
    }
    // 해당 userFlowerBouquetList를 id를 기준으로 반환. true인 경우, -1은 제외한 리스트만 반환한다. false 이면 전체 리스트를 반환한다.
    public static int[] GetUserFlowerBouquetList(int id, bool excludeEmptyFairy = false)
    {
        List<int> bouquetList = new List<int>();
        if(userFlowerBouquetDictionary.ContainsKey(id))
        {
            foreach(int fairyId in userFlowerBouquetDictionary[id].fairyList)
            {
                if(excludeEmptyFairy && fairyId == -1) continue;
                bouquetList.Add(fairyId);
            }
            return bouquetList.ToArray();
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return null;
        }
    }
    public static int[] GetUserSelectedFlowerBouquetLists() // true : id가 -1인 빈 슬롯은 제외, false : 전체 포함
    {
        int[] list = Array.Empty<int>();
        // 현재 선택중인 파티 탐색
        foreach (var pair in userFlowerBouquetSelectedListDictionary)
        {
            int id = pair.Key;
            bool isSelected = pair.Value;

            // 1) 선택된(id→true) 인 경우만 처리
            if (!isSelected) continue;

            // 2) 해당 id가 꽃다발 리스트에 있으면 가져와서 추가
            if (userFlowerBouquetListDictionary.ContainsKey(pair.Key))
            {
                list = userFlowerBouquetListDictionary[pair.Key];
            }
        }
        return list;
    }
}
