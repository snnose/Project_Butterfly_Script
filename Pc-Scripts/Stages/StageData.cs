using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;


public static class StageData
{
    private static Dictionary<int, Stage> stageDictionary = new Dictionary<int, Stage>();

    public static void SetStageDictionary(List<Stage> stageList)
    {
        foreach (Stage stage in stageList)
        {
            if (!stageDictionary.ContainsKey(stage.id))
            {
                stageDictionary[stage.id] = stage;
            }
        }
    }

    public static void SetStageData(JSONObject jsonObject)
    { 
        Stage stage = new Stage();

        stage.id = jsonObject["id"].intValue;
        stage.key = jsonObject["key"].stringValue;
        stage.mode = (StageMode)Enum.Parse(typeof(StageMode), jsonObject["mode"].stringValue);
        stage.difficulty = (StageDifficulty)Enum.Parse(typeof(StageDifficulty), jsonObject["difficulty"].stringValue);

        // JSON 객체에서 "firstgimmick" 키에 해당하는 배열 가져오기
        JSONObject firstgimmickJsonObject = jsonObject.GetField("firstgimmick");
        // JSONObject를 int[]로 변환
        int[] firstgimmickRaitioArray = new int[firstgimmickJsonObject.count];
        for (int i = 0; i < firstgimmickJsonObject.count; i++)
        {
            firstgimmickRaitioArray[i] = firstgimmickJsonObject[i].intValue;
        }
        // 변환된 int[]를 stage.firstGimmick 할당
        stage.firstGimmick = firstgimmickRaitioArray;

        // JSON 객체에서 "repeatgimmick" 키에 해당하는 배열 가져오기
        JSONObject repeatgimmickJsonObject = jsonObject.GetField("repeatgimmick");
        // JSONObject를 int[]로 변환
        int[] repeatgimmickRaitioArray = new int[repeatgimmickJsonObject.count];
        for (int i = 0; i < repeatgimmickJsonObject.count; i++)
        {
            repeatgimmickRaitioArray[i] = repeatgimmickJsonObject[i].intValue;
        }
        // 변환된 int[]를 stage.repeatGimmickList에 할당
        stage.repeatGimmickList = repeatgimmickRaitioArray;
        
        stage.world = jsonObject["world"].stringValue;
        stage.targetScore = jsonObject["TargetScore"].intValue;
        //stage.limitScore = jsonObject["LimitScore"].intValue;

        // JSON 객체에서 "TargetScoreRaitio" 키에 해당하는 배열 가져오기
        JSONObject targetScoreRaitioJsonObject = jsonObject.GetField("TargetScoreRaitio");
        // JSONObject를 float[]로 변환
        float[] targetScoreRaitioArray = new float[targetScoreRaitioJsonObject.count];
        for (int i = 0; i < targetScoreRaitioJsonObject.count; i++)
        {
            targetScoreRaitioArray[i] = targetScoreRaitioJsonObject[i].floatValue;
        }
        // 변환된 float[]를 stage.targetScoreRaitio에 할당
        stage.targetScoreRatio = targetScoreRaitioArray;

        //stage.limitTurnCount = jsonObject["LimitTurnCount"].intValue;
        //stage.limitTime = jsonObject["LimitTime"].floatValue;
        //stage.perfectCount = jsonObject["PerfectCount"].intValue;
        stage.requiredProgress = jsonObject["RequiredProgress"].intValue;
        //stage.bonusColor = (DropColor)Enum.Parse(typeof(DropColor), jsonObject["BonusColor"].stringValue);
        //stage.bonusColorScore = jsonObject["BonusColorScore"].intValue;
        stage.boardShapeId = jsonObject["boardShape"].intValue;
        stage.bosskey = jsonObject["boss"].stringValue;

        // JSON 객체에서 "AchievementDropType" 키에 해당하는 배열 가져오기
        JSONObject achievementDropTypeJsonObject = jsonObject.GetField("AchievementDropType");
        // JSONObject를 float[]로 변환
        AchievementDropType[] achievementDropTypeArray = new AchievementDropType[achievementDropTypeJsonObject.count];
        for (int i = 0; i < achievementDropTypeJsonObject.count; i++)
        {
            achievementDropTypeArray[i] = (AchievementDropType)Enum.Parse(typeof(AchievementDropType), achievementDropTypeJsonObject[i].stringValue);
        }
        // 변환된 string[]를 stage.achievementDropType 할당
        stage.achievementDropType = achievementDropTypeArray;

        if (!stageDictionary.ContainsKey(stage.id))
        {
            stageDictionary.Add(stage.id, stage);
        }
    }
    public static Dictionary<int, Stage> GetStageData()
    {
        return stageDictionary;
    }
    public static int GetStageDataCount()
    {
        return stageDictionary.Count;
    }
    public static bool IsStageDataExist(int id)
    {
        if(stageDictionary.ContainsKey(id))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    // 해당 Stage Key를 id를 기준으로 반환
    public static string GetStageKey(int id)
    {
        if(stageDictionary.ContainsKey(id))
        {
            Stage stage = stageDictionary[id];
            return stage.key;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return null;
        }
    }
    // 해당 Stage를 id를 기준으로 반환
    public static Stage GetStage(int id)
    {
        if(stageDictionary.ContainsKey(id))
        {
            Stage stage = stageDictionary[id];
            return stage;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return null;
        }
    }
    // 해당 Stage Key를 id를 기준으로 반환
    public static int GetStageTargetScore(int id)
    {
        if(stageDictionary.ContainsKey(id))
        {
            Stage stage = stageDictionary[id];
            return stage.targetScore;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return 0;
        }
    }
    // 해당 Stage id를 기준으로 LimitTime을 반환
    /*
    public static float GetStageLimitTime(int id)
    {
        if(stageDictionary.ContainsKey(id))
        {
            Stage stage = stageDictionary[id];
            return stage.limitTime;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return 0;
        }
    }
    */
    // 해당 Stage id를 기준으로 stage에서 사용하는 achievementdroptype 전체 갯수 로드
    public static AchievementDropType[] GetStageAchievementDropType(int id)
    {
        if(stageDictionary.ContainsKey(id))
        {
            return stageDictionary[id].achievementDropType;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return null;
        }
    }
    // 해당 Stage id를 기준으로 stage에서 사용하는 초기 gimmick 로드
    public static int[] GetStageFirstGimmick(int id)
    {
        if(stageDictionary.ContainsKey(id))
        {
            return stageDictionary[id].firstGimmick;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return null;
        }
    }
    // 해당 Stage id를 기준으로 stage에서 사용하는 반복 gimmick 로드
    public static int[] GetStageRepeatGimmick(int id)
    {
        if(stageDictionary.ContainsKey(id))
        {
            return stageDictionary[id].repeatGimmickList;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return null;
        }
    }
}
