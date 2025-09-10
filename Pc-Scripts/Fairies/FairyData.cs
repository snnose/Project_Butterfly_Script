using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;


public static class FairyData
{
    private static Dictionary<int, Fairy> fairyDictionary = new Dictionary<int, Fairy>();

    public static void SetFairyDictionary(List<Fairy> fairyList)
    {
        foreach (Fairy fairy in fairyList)
        {
            if (!fairyDictionary.ContainsKey(fairy.id))
            {
                fairyDictionary[fairy.id] = fairy;
            }
        }
    }

    public static void SetFairyData(JSONObject jsonObject)
    {
        Fairy fairy = new Fairy();

        fairy.id = jsonObject["id"].intValue;
        //fairy.rarity = jsonObject["rarity"].intValue;
        fairy.key = jsonObject["key"].stringValue;
        fairy.fairyType = (FairyType)Enum.Parse(typeof(FairyType), jsonObject["type"].stringValue);
        //fairy.fairyColor = (DropColor)Enum.Parse(typeof(DropColor), jsonObject["color"].stringValue);
        //fairy.fairyColorBonus = jsonObject["colorbonus"].intValue;
        //fairy.fairyClass = (FairyClass)Enum.Parse(typeof(FairyClass), jsonObject["class"].stringValue);
        fairy.dropId = jsonObject["drop"].intValue;
        fairy.collection = jsonObject["collection"].intValue;
        fairy.mainoption = jsonObject["mainoption"].intValue;
        //fairy.expbase = jsonObject["expbase"].intValue;
        //fairy.expadd = jsonObject["expadd"].intValue;
        //fairy.linkcount = jsonObject["linkcount"].intValue;
        fairy.growthRequirement = jsonObject["growthRequirement"].intValue;

        // JSON 객체에서 "abilityList" 키에 해당하는 배열 가져오기
        /*
        JSONObject abilityListJsonObject = jsonObject.GetField("abilityList");
        // JSONObject를 int[]로 변환
        string[] abilityListArray = new string[abilityListJsonObject.count];
        for (int i = 0; i < abilityListJsonObject.count; i++)
        {
            abilityListArray[i] = abilityListJsonObject[i].stringValue;
        }
        // 변환된 int[]를 stage.firstGimmick 할당
        fairy.abilityList = abilityListArray;
        */

        if (!fairyDictionary.ContainsKey(fairy.id))
        {
            fairyDictionary.Add(fairy.id, fairy);
        }
    }

    public static int GetFairyDataCount()
    {
        return fairyDictionary.Count;
    }
    public static bool IsFairyDataExist(int id)
    {
        if(fairyDictionary.ContainsKey(id))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    // 해당 Fairy Key를 id를 기준으로 반환
    public static string GetFairyKey(int id)
    {
        if(fairyDictionary.ContainsKey(id))
        {
            Fairy fairy = fairyDictionary[id];
            return fairy.key;
        }
        else
        {
            Debug.LogWarning("Id:"+ id +" Cannot Found!");
            return null;
        }
    }
    // 해당 Fairy를 id를 기준으로 반환
    public static Fairy GetFairy(int id)
    {
        if(fairyDictionary.ContainsKey(id))
        {
            Fairy fairy = fairyDictionary[id];
            return fairy;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return null;
        }
    }
    public static int GetFairyInDrop(int id)
    {
        if(fairyDictionary.ContainsKey(id))
        {
            Fairy fairy = fairyDictionary[id];
            return fairy.dropId;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return 0;
        }
    }
    public static int GetFairyGrowthRequirement(int id)
    {
        if(fairyDictionary.ContainsKey(id))
        {
            Fairy fairy = fairyDictionary[id];
            return fairy.growthRequirement;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return 0;
        }
    }
    public static int GetFairyInMainOption(int id)
    {
        if(fairyDictionary.ContainsKey(id))
        {
            Fairy fairy = fairyDictionary[id];
            return fairy.mainoption;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return 0;
        }
    }
    /*
    public static DropColor GetFairyColor(int id)
    {
        if(fairyDictionary.ContainsKey(id))
        {
            Fairy fairy = fairyDictionary[id];
            return fairy.fairyColor;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return 0;
        }
    }
    */
    /*
    public static string[] GetFairyAbilityList(int id)
    {
        if(fairyDictionary.ContainsKey(id))
        {
            Fairy fairy = fairyDictionary[id];
            return fairy.abilityList;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return null;
        }
    }
    */

    // 드롭 Id로 패어리 Id를 반환
    public static int GetFairyIdByDropId(int dropId)
    {
        var filteredAchievements = fairyDictionary.Values
            .Where(fairy => fairy.dropId == dropId)
            .ToList();

        if (filteredAchievements.Count == 0)
            return -1;

        return filteredAchievements[0].id;
    }
    // 특정 dropId를 가진 Fairy의 key값을 찾는 메서드
    public static string FindFairyKeyByDropId(int dropId)
    {
        // 조건에 맞는 Fairy 찾기
        foreach (var fairy in fairyDictionary.Values)
        {
            if (fairy.dropId == dropId)
            {
                return fairy.key; // 조건에 맞는 Fairy의 key 반환
            }
        }

        // 조건에 맞는 Fairy가 없으면 기본값 반환
        return null;
    }
    // 특정 dropId를 가진 Fairy의 key값을 찾는 메서드
    public static int FindFairyMainOptionByDropId(int dropId)
    {
        // 조건에 맞는 Fairy 찾기
        foreach (var fairy in fairyDictionary.Values)
        {
            if (fairy.dropId == dropId)
            {
                return fairy.mainoption; // 조건에 맞는 Fairy의 mainoption 반환
            }
        }

        // 조건에 맞는 Fairy가 없으면 기본값 반환
        return 0;
    }
}
