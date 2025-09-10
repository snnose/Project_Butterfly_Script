using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;


public static class DropData
{
    private static Dictionary<int, Drop> dropDictionary = new Dictionary<int, Drop>();

    public static void SetDropDictionary(List<Drop> dropList)
    {
        foreach (Drop drop in dropList)
        {
            if (!dropDictionary.ContainsKey(drop.id))
            {
                dropDictionary[drop.id] = drop;
            }
        }
    }

    public static void SetDropData(JSONObject jsonObject)
    {
        Drop drop = new Drop();

        drop.id = jsonObject["id"].intValue;
        drop.key = jsonObject["key"].stringValue;
        drop.dropColor = (DropColor)Enum.Parse(typeof(DropColor), jsonObject["color"].stringValue);
        drop.dropShape = (DropShape)Enum.Parse(typeof(DropShape), jsonObject["shape"].stringValue);
        drop.dropType = (DropType)Enum.Parse(typeof(DropType), jsonObject["type"].stringValue);
        drop.dropWeight = jsonObject["dropweight"].intValue;
        drop.dropScore = jsonObject["dropscore"].intValue;
        //drop.dropColorBonus = jsonObject["colorbonus"].intValue;
        //drop.criticalrate = jsonObject["criticalrate"].floatValue;
        //drop.criticalbonus = jsonObject["criticalbonus"].intValue;

        if (!dropDictionary.ContainsKey(drop.id))
        {
            dropDictionary.Add(drop.id, drop);
        }
    }
    public static int GetDropDataCount()
    {
        return dropDictionary.Count;
    }
    public static int GetNormalDropDataCount()
    {
        // dropType이 NORMAL이 아닌 항목의 개수 계산
        int normalCount = dropDictionary.Values.Count(drop => drop.dropType == DropType.NORMAL);

        Debug.Log($"dropType이 NORMAL이 아닌 항목의 개수: {normalCount}");

        return normalCount;
    }
    public static int GetGimmickDropDataCount()
    {
        // dropType이 NORMAL이 아닌 항목의 개수 계산
        int gimmickCount = dropDictionary.Values.Count(drop => drop.dropType != DropType.NORMAL);

        Debug.Log($"dropType이 GIMMICK인 항목의 개수: {gimmickCount}");

        return gimmickCount;
    }
    public static bool IsDropDataExist(int id)
    {
        if(dropDictionary.ContainsKey(id))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 해당 Drop의 Key를 id를 기준으로 반환
    public static string GetDropKey(int id)
    {
        if(dropDictionary.ContainsKey(id))
        {
            Drop drop = dropDictionary[id];
            return drop.key;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return null;
        }
    }
    // 해당 Drop의 DropScore를 id를 기준으로 반환
    public static int GetDropScore(int id)
    {
        if(dropDictionary.ContainsKey(id))
        {
            Drop drop = dropDictionary[id];
            return drop.dropScore;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return 0;
        }
    }
    // 해당 Drop을 id를 기준으로 반환
    public static Drop GetDrop(int id)
    {
        if(dropDictionary.ContainsKey(id))
        {
            Drop drop = dropDictionary[id];
            return drop;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return null;
        }
    }
    // 해당 Drop을 shape를 기준으로 반환
    public static DropShape GetDropShape(int id)
    {
        if(dropDictionary.ContainsKey(id))
        {
            Drop drop = dropDictionary[id];
            return drop.dropShape;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return DropShape.none;
        }
    }
    // 해당 Drop을 color를 기준으로 반환
    public static DropColor GetDropColor(int id)
    {
        if(dropDictionary.ContainsKey(id))
        {
            Drop drop = dropDictionary[id];
            return drop.dropColor;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return DropColor.none;
        }
    }
    public static Dictionary<int, Drop> GetDropDictionary()
    {
        return dropDictionary;
    }
}
