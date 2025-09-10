using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;


public static class UserStageData
{
    private static Dictionary<int, UserStage> userstageDictionary = new Dictionary<int, UserStage>();

    public static void SetUserStageDataDictionary(List<UserStage> userStageList)
    {
        foreach (UserStage userStage in userStageList)
        {
            if (!userstageDictionary.ContainsKey(userStage.id))
            {
                userstageDictionary[userStage.id] = userStage;
            }
        }
    }

    public static void SetUserStageData(JSONObject jsonObject)
    {
        UserStage userstage = new UserStage();

        userstage.id = jsonObject["id"].intValue;
        userstage.isCleared = jsonObject["isCleared"].boolValue;
        userstage.highscore = jsonObject["highscore"].intValue;
        userstage.highrank = jsonObject["highrank"].intValue;

        if (!userstageDictionary.ContainsKey(userstage.id))
        {
            userstageDictionary.Add(userstage.id, userstage);
        }
    }

    public static List<UserStage> UpdateUserStageData()
    {
        List<UserStage> userStageList = new List<UserStage>();

        foreach (KeyValuePair<int, UserStage> pair in userstageDictionary)
        {
            UserStage userStage = pair.Value;
            userStageList.Add(userStage);
        }

        return userStageList;
    }

    // 해당 User정보를 id를 기준으로 반환
    public static UserStage GetUserStage(int id)
    {
        if(userstageDictionary.ContainsKey(id))
        {
            UserStage userstage = userstageDictionary[id];
            return userstage;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return null;
        }
    }

    public static void SetUserStageDataCleared(int id, int score, int highrank, bool isCleared)
    {
        if (userstageDictionary.ContainsKey(id))
        {
            userstageDictionary[id].highrank = highrank;
            userstageDictionary[id].isCleared = isCleared;
            if(userstageDictionary[id].highscore < score)
            {
                userstageDictionary[id].highscore = score;
            }
        }
    }
    
    // 스테이지 전체 클리어 여부를 반환
    public static List<bool> GetStageClearedList()
    {
        List<bool> stageClearedList = new List<bool>();
        
        // userstageDictionary의 키를 정렬한 후, 각 UserStage의 isCleared 값을 리스트에 추가
        foreach (int stageId in userstageDictionary.Keys.OrderBy(id => id))
        {
            stageClearedList.Add(userstageDictionary[stageId].isCleared);
        }
        
        return stageClearedList;
    }

}
