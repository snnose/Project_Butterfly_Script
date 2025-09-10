using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;


public static class GimmickData
{
    private static Dictionary<int, Gimmick> gimmickDictionary = new Dictionary<int, Gimmick>();

    public static void SetGimmickDictionary(List<Gimmick> gimmickList)
    {
        foreach (Gimmick gimmick in gimmickList)
        {
            if (!gimmickDictionary.ContainsKey(gimmick.id))
            {
                gimmickDictionary[gimmick.id] = gimmick;
            }
        }
    }

    public static void SetGimmickData(JSONObject jsonObject)
    {
        Gimmick gimmick = new Gimmick();

        gimmick.id = jsonObject["id"].intValue;
        gimmick.type = (GimmickType)Enum.Parse(typeof(GimmickType), jsonObject["type"].stringValue);
        gimmick.level = jsonObject["level"].intValue;
        gimmick.isOccupied = jsonObject["isOccupied"].boolValue;
        gimmick.activeTurn = jsonObject["ActiveTurn"].intValue;
        gimmick.durationTurn = jsonObject["DurationTrun"].intValue;
        gimmick.amount = jsonObject["amount"].intValue;
        gimmick.drop = jsonObject["drop"].intValue;
        gimmick.isRandomPosition = jsonObject["isRandomPosition"].boolValue;
        gimmick.isInteractable = jsonObject["isInteractable"].boolValue;
        gimmick.linePlacement = (LinePlacement)Enum.Parse(typeof(LinePlacement), jsonObject["linePlacement"].stringValue);
        gimmick.linePlacementDirection = (LinePlacementDirection)Enum.Parse(typeof(LinePlacementDirection), jsonObject["linePlacementDirection"].stringValue);

        // JSON 객체에서 "position" 키에 해당하는 배열 가져오기
        JSONObject positionJsonObject = jsonObject.GetField("position");
        // JSONObject를 int[]로 변환
        int[] positionRaitioArray = new int[positionJsonObject.count];
        for (int i = 0; i < positionJsonObject.count; i++)
        {
            positionRaitioArray[i] = positionJsonObject[i].intValue;
        }
        // 변환된 int[]를 gimmick.positionList에 할당
        gimmick.positionList = positionRaitioArray;

        if (!gimmickDictionary.ContainsKey(gimmick.id))
        {
            gimmickDictionary.Add(gimmick.id, gimmick);
        }
    }
    public static Dictionary<int, Gimmick> GetGimmickData()
    {
        return gimmickDictionary;
    }
    public static int GetStageDataCount()
    {
        return gimmickDictionary.Count;
    }
    public static int GetGimmickDropId(int id)
    {
        if (gimmickDictionary.ContainsKey(id))
        {
            return gimmickDictionary[id].drop;
        }

        return 0;
    }
    public static int GetGimmickAmount(int id)
    {
        if (gimmickDictionary.ContainsKey(id))
        {
            return gimmickDictionary[id].amount;
        }

        return 0;
    }
    public static Gimmick GetGimmick(int id)
    {
        if (gimmickDictionary.ContainsKey(id))
        {
            return gimmickDictionary[id];
        }

        return null;
    }
    public static bool IsGimmickDataExist(int id)
    {
        if (gimmickDictionary.ContainsKey(id))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
