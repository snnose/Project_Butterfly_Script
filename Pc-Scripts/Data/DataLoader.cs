using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Defective.JSON;
using System.IO;

public static class DataLoader
{
    private static Dictionary<string, string> jsonDataMap = new Dictionary<string, string>();
    private static Dictionary<int, JSONObject> jsonDataMapById = new Dictionary<int, JSONObject>();
    private static TextAsset[] dataFiles;

    public static int retryCount = 0;
    public static int maxRetryCount = 3;

    /*
    // 모든 User 데이터 캐싱
    public static async Task CallUserData()
    {
        // CloudScript 함수를 호출하기 위한 요청 객체를 생성합니다.
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest()
        {
            FunctionName = "getUserData", // 호출할 CloudScript 함수의 이름
            FunctionParameter = new { PlayFabId = PlayFabUserData.GetPlayFabId(), EntityId = PlayFabUserData.GetEntityTokenId() },
            GeneratePlayStreamEvent = true // PlayStream 이벤트 생성 여부
            
        };
        // CloudScript 함수를 호출합니다.
        PlayFabClientAPI.ExecuteCloudScript(request, OnCloudScriptSuccessUserData, OnCloudScriptUserDataFailure);
    }
    public static async Task CallUserFlowerBouquetData()
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest()
        {
            FunctionName = "getUserFlowerBouquetData",
            FunctionParameter = new { EntityId = PlayFabUserData.GetEntityTokenId() },
            GeneratePlayStreamEvent = true // PlayStream 이벤트 생성 여부
        };

        PlayFabClientAPI.ExecuteCloudScript(request,
            result =>
            {
                Debug.LogWarning(result.FunctionResult.ToString());
                JSONObject userFlowerBouquetDataJSONObject = new JSONObject(result.FunctionResult.ToString());
                ParseUserFlowerBouquetData(userFlowerBouquetDataJSONObject);
            },
            async error =>
            {
                // 오류 로그를 출력합니다.
                Debug.LogError("CloudScript 호출 실패: " + error.GenerateErrorReport());

                if (retryCount < maxRetryCount)
                {
                    retryCount++;
                    Debug.Log($"재시도 중... ({retryCount} / {maxRetryCount})");

                    int delay = (int)Math.Pow(2, retryCount - 1);
                    await Task.Delay(delay * 1000);

                    await CallUserData();
                }
                else
                {
                    Debug.LogError("CloudScript 호출 실패 - 최대 재시도 횟수 도달");
                }
            });
    }
    // 모든 게임 내부 데이터 캐싱
    public static async Task CallTitleInternalData()
    {
        // CloudScript 함수를 호출하기 위한 요청 객체를 생성합니다.
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest()
        {
            FunctionName = "getTitleInternalData", // 호출할 CloudScript 함수의 이름
            GeneratePlayStreamEvent = true // PlayStream 이벤트 생성 여부
        };
        // CloudScript 함수를 호출합니다.
        PlayFabClientAPI.ExecuteCloudScript(request, OnCloudScriptSuccessInternalData, OnCloudScriptInternalDataFailure);
    }
    private static void OnCloudScriptSuccessUserData(ExecuteCloudScriptResult result)
    {
        JSONObject userDataJsonObject = new JSONObject(result.FunctionResult.ToString());
        ParseUserDatas(userDataJsonObject);
    }
    // CloudScript 호출이 성공했을 때 실행될 콜백 함수입니다.
    private static void OnCloudScriptSuccessInternalData(ExecuteCloudScriptResult result)
    {
        JSONObject jsonObject = new JSONObject(result.FunctionResult.ToString());
        ParseInternalDatas(jsonObject);
    }

    // CloudScript 호출이 실패했을 때 실행될 콜백 함수입니다.
    private static async void OnCloudScriptUserDataFailure(PlayFabError error)
    {
        // 오류 로그를 출력합니다.
        Debug.LogError("CloudScript 호출 실패: " + error.GenerateErrorReport());

        if (retryCount < maxRetryCount)
        {
            retryCount++;
            Debug.Log($"재시도 중... ({retryCount} / {maxRetryCount})");

            int delay = (int)Math.Pow(2, retryCount - 1);
            await Task.Delay(delay * 1000);

            await CallUserData();
        }
        else
        {
            Debug.LogError("CloudScript 호출 실패 - 최대 재시도 횟수 도달");
        }
    }

    private static async void OnCloudScriptInternalDataFailure(PlayFabError error)
    {
        // 오류 로그를 출력합니다.
        Debug.LogError("CloudScript 호출 실패: " + error.GenerateErrorReport());

        if (retryCount < maxRetryCount)
        {
            retryCount++;
            Debug.Log($"재시도 중... ({retryCount} / {maxRetryCount})");

            int delay = (int)Math.Pow(2, retryCount - 1);
            await Task.Delay(delay * 1000);

            await CallTitleInternalData();
        }
        else
        {
            Debug.LogError("CloudScript 호출 실패 - 최대 재시도 횟수 도달");
        }
    }
    */

    // 모든 데이터 차례대로 파싱
    // 새로운 데이터 추가할 때 마다 여기에 추가해줘야 함
    private static void ParseUserDatas(JSONObject jsonObject)
    {
        LoadUserData(jsonObject["user"].stringValue);
        LoadPartyData(jsonObject["party"].stringValue);
        LoadUserStageData(jsonObject["user_stage"].stringValue);
        LoadUserCharacterData(jsonObject["character"].stringValue);
        LoadUserItemData(jsonObject["user_item"].stringValue);
        LoadUserFlowerBouquetListData(jsonObject["user_flowerbouquetList"].stringValue);

        LoadUserParameterData(jsonObject);
    }
    private static void ParseUserFlowerBouquetData(JSONObject jsonObject)
    {
        LoadUserFlowerBouquetData(jsonObject["user_flowerbouquet"].stringValue);
    }
    private static void ParseInternalDatas(JSONObject jsonObject)
    {
        LoadDropData(jsonObject["drops"].stringValue);
        LoadBoardData(jsonObject["boards"].stringValue);
        LoadFairyData(jsonObject["fairies"].stringValue);
        LoadCharacterData(jsonObject["characters"].stringValue);
        LoadStageData(jsonObject["stages"].stringValue);
        LoadAchievementData(jsonObject["achievements"].stringValue);
        LoadOptionData(jsonObject["options"].stringValue);
        LoadCollectionData(jsonObject["collections"].stringValue);
        LoadDialogueData(jsonObject["dialogues"].stringValue);
        LoadGimmickData(jsonObject["gimmicks"].stringValue);
        LoadConstantData(jsonObject["constants"].stringValue);
        LoadActiveSkillData(jsonObject["activeskills"].stringValue);
        LoadFlowerBouquetData(jsonObject["flowerbouquets"].stringValue);
    }
    // user 기타 파라미터형 데이터 파싱
    private static void LoadUserParameterData(JSONObject userData)
    {
        /*
        UserParameterData.SetUserProgressData("user_progress", userData["user_progress"].stringValue);
        Debug.Log("user_progress DataLoaded!!" + userData["user_progress"].stringValue);
        UserParameterData.SetUserParameterData("progress", int.Parse(userData["progress"].stringValue));
        Debug.Log("progress DataLoaded!!" + userData["progress"].intValue);
        UserParameterData.SetUserParameterData("itemexperience", int.Parse(userData["itemexperience"].stringValue));
        Debug.Log("itemexperience DataLoaded!!" + userData["itemexperience"].intValue);
        UserParameterData.SetUserParameterData("experience", int.Parse(userData["experience"].stringValue));
        Debug.Log("experience DataLoaded!!" + userData["experience"].intValue);
        */
    }
    // user 데이터 파싱
    private static void LoadUserData(string userData)
    {   
        JSONObject jsonObject = new JSONObject(userData);
        //UserData.SetUserData(jsonObject);

        Debug.Log("DataFileLoaded!!::[users.json]");
    }
    // user_stage 데이터 파싱
    private static void LoadUserStageData(string userstageData)
    {   
        JSONObject jsonObject = new JSONObject(userstageData);
        foreach (JSONObject item in jsonObject)
        {
            UserStageData.SetUserStageData(item);
        }

        Debug.Log("DataFileLoaded!!::[user_stage.json]");
    }
    // user_character 데이터 파싱
    private static void LoadUserCharacterData(string usercharacterData)
    {   
        JSONObject jsonObject = new JSONObject(usercharacterData);
        foreach (JSONObject item in jsonObject)
        {
            //UserCharacterData.SetUserCharacterData(item);
            Debug.Log("usercharacter=>>"+item);
        }

        Debug.Log("DataFileLoaded!!::[user_character.json]");
    }
    // user_item 데이터 파싱
    private static void LoadUserItemData(string useritemData)
    {   
        JSONObject jsonObject = new JSONObject(useritemData);
        foreach (JSONObject item in jsonObject)
        {
            UserItemData.SetUserItemData(item);
            Debug.Log("useritem=>>"+item);
        }

        Debug.Log("DataFileLoaded!!::[user_item.json]");
    }
    // user_item 데이터 파싱
    private static void LoadUserFlowerBouquetData(string useritemData)
    {   
        JSONObject jsonObject = new JSONObject(useritemData);
        foreach (JSONObject item in jsonObject)
        {
            //UserFlowerBouquetData.SetUserFlowerBouquetData(item);
            Debug.Log("useritem=>>"+item);
        }

        Debug.Log("DataFileLoaded!!::[user_item.json]");
    }
    // user_item 데이터 파싱
    private static void LoadUserFlowerBouquetListData(string useritemData)
    {   
        JSONObject jsonObject = new JSONObject(useritemData);
        foreach (JSONObject item in jsonObject)
        {
            //UserFlowerBouquetData.SetUserFlowerBouquetListData(item);
            Debug.Log("useritem=>>"+item);
        }

        Debug.Log("DataFileLoaded!!::[user_item.json]");
    }
    // drops 데이터 파싱
    private static void LoadDropData(string dropData)
    {
        JSONObject dropjson = new JSONObject(dropData);
        
        foreach (JSONObject item in dropjson)
        {
            DropData.SetDropData(item);
            Debug.Log("dropitem=>>"+item);
        }
        Debug.Log("DataFileLoaded!!::[drops.json]");
    }
    // characters 데이터 파싱
    private static void LoadCharacterData(string characterData)
    {
        JSONObject jsonObject = new JSONObject(characterData);

        foreach (JSONObject item in jsonObject)
        {
            CharacterData.SetCharacterData(item);
        }
        Debug.Log("DataFileLoaded!!::[characters.json]");
    }
    // fairies 데이터 파싱
    private static void LoadFairyData(string fairyData)
    {
        JSONObject jsonObject = new JSONObject(fairyData);

        foreach (JSONObject item in jsonObject)
        {
            FairyData.SetFairyData(item);
        }
        Debug.Log("DataFileLoaded!!::[fairies.json]");
    }
    // board 데이터 파싱
    private static void LoadBoardData(string boardData)
    {
        JSONObject jsonObject = new JSONObject(boardData);

        foreach (JSONObject item in jsonObject)
        {
            BoardData.SetBoardData(item);
        }
        Debug.Log("DataFileLoaded!!::[board.json]");
    }
    // party 데이터 파싱
    private static void LoadPartyData(string partyData)
    {
        JSONObject jsonObject = new JSONObject(partyData);
        foreach (JSONObject item in jsonObject)
        {
            PartyData.SetPartyData(item);
            Debug.Log("partyitem=>>"+item);
        }
        Debug.Log("DataFileLoaded!!::[party.json]");
    }
    // stage 데이터 파싱
    private static void LoadStageData(string stageData)
    {
        JSONObject jsonObject = new JSONObject(stageData);
        foreach (JSONObject item in jsonObject)
        {
            StageData.SetStageData(item);
            Debug.Log("stageitem=>>"+item);
        }
        Debug.Log("DataFileLoaded!!::[stage.json]");
    }
    // achievement 데이터 파싱
    private static void LoadAchievementData(string achievementData)
    {
        JSONObject jsonObject = new JSONObject(achievementData);
        foreach (JSONObject item in jsonObject)
        {
            AchievementData.SetAchievementData(item);
            Debug.Log("achievementitem=>>"+item);
        }
        Debug.Log("DataFileLoaded!!::[achievement.json]");
    }
    // option 데이터 파싱
    private static void LoadOptionData(string optionData)
    {
        JSONObject jsonObject = new JSONObject(optionData);
        foreach (JSONObject item in jsonObject)
        {
            OptionData.SetOptionData(item);
            Debug.Log("optionitem=>>"+item);
        }
        Debug.Log("DataFileLoaded!!::[option.json]");
    }
    // collection 데이터 파싱
    private static void LoadCollectionData(string collectionData)
    {
        JSONObject jsonObject = new JSONObject(collectionData);
        foreach (JSONObject item in jsonObject)
        {
            CollectionData.SetCollectionData(item);
            Debug.Log("collectionitem=>>"+item);
        }
        Debug.Log("DataFileLoaded!!::[collection.json]");
    }
    // dialogue 데이터 파싱
    private static void LoadDialogueData(string dialogueData)
    {
        JSONObject jsonObject = new JSONObject(dialogueData);
        foreach (JSONObject item in jsonObject)
        {
            DialogueData.SetDialogueData(item);
            Debug.Log("dialogueitem=>>"+item);
        }
        Debug.Log("DataFileLoaded!!::[dialogue.json]");
    }
    // gimmick 데이터 파싱
    private static void LoadGimmickData(string gimmickData)
    {
        JSONObject jsonObject = new JSONObject(gimmickData);
        foreach (JSONObject item in jsonObject)
        {
            GimmickData.SetGimmickData(item);
            Debug.Log("gimmickitem=>>"+item);
        }
        Debug.Log("DataFileLoaded!!::[gimmick.json]");
    }
    // constants 데이터 파싱
    private static void LoadConstantData(string constantData)
    {
        JSONObject jsonObject = new JSONObject(constantData);
        foreach (JSONObject item in jsonObject)
        {
            ConstantData.SetConstantData(item);
            Debug.Log("constantitem=>>"+item);
        }
        Debug.Log("DataFileLoaded!!::[constant.json]");
    }
    // active skill 데이터 파싱
    private static void LoadActiveSkillData(string skillData)
    {
        JSONObject jsonObject = new JSONObject(skillData);
        foreach (JSONObject item in jsonObject)
        {
            ActiveSkillData.SetActiveSkillData(item);
            Debug.Log("activeskllitem=>>"+item);
        }
        Debug.Log("DataFileLoaded!!::[activeskills.json]");
    }
    // flowerbouquet 데이터 파싱
    private static void LoadFlowerBouquetData(string flowerBouquetData)
    {
        JSONObject jsonObject = new JSONObject(flowerBouquetData);
        foreach (JSONObject item in jsonObject)
        {
            //BouquetData.SetFlowerBouquetData(item);
            Debug.Log("flowerbouquetitem=>>"+item);
        }
        Debug.Log("DataFileLoaded!!::[flowerbouquet.json]");
    }
    // 해당 데이터 파일의 JSON 문자열을 반환
    public static string GetJsonData(string fileName)
    {
        if (jsonDataMap.ContainsKey(fileName))
        {
            return jsonDataMap[fileName];
        }
        else
        {
            Debug.LogWarning("Data file '" + fileName + "' not found!");
            return null;
        }
    }
    
    // 모든 데이터 파일의 맵을 반환
    public static Dictionary<string, string> GetJsonDataMap()
    {
        return jsonDataMap;
    }
    
    // id로 데이터 반환
    public static T GetJSONDataById<T>(int id, string key)
    {
        // id에 해당하는 JSONObject 반환
        JSONObject data = jsonDataMapById[id];

        // 해당 id의 JSONObject에서 key에 해당하는 값 return
        if (data.HasField(key))
        {
            // 요청된 열의 유형을 확인하고 해당 값을 반환합니다.
            if (typeof(T) == typeof(int))
            {
                return (T)(object)data[key].intValue;
            }
            else if (typeof(T) == typeof(float))
            {
                return (T)(object)data[key].floatValue;
            }
            else if (typeof(T) == typeof(string))
            {
                return (T)(object)data[key].stringValue;
            }
            // 필요에 따라 더 많은 유형을 추가할 수 있습니다.

            // 지원되지 않는 유형인 경우 오류를 로그에 기록하고 기본값을 반환합니다.
            Debug.LogError("지원되지 않는 데이터 유형: " + typeof(T).Name);
            return default(T);
        }
        else
        {
            Debug.LogError("key를 찾지 못했습니다: " + key);
            return default(T);
        }
    }
    
}
