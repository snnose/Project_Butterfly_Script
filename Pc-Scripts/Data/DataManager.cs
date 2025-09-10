using UnityEngine;using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks; // 비동기 메서드 사용을 위한 네임스페이스
using Utils.SpineUtil;

public class DataManager : MonoBehaviour
{
    private static DataManager _instance;
    public static DataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DataManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(DataManager).Name;
                    _instance = obj.AddComponent<DataManager>();
                }
            }
            return _instance;
        }
    }

    public SaveData saveData;
    private string saveFilePath;

    private void Awake()
    {
        // DataManager는 파괴되지 않는다.
        DontDestroyOnLoad(gameObject);
        saveFilePath = Path.Combine(Application.dataPath, "JSON", "Save", "SaveData.json");
    }

    private void OnApplicationQuit()
    {
        // 게임 종료 시 유저 데이터 저장
        SaveUserData();
    }

    public void SaveUserData()
    {
        // 세이브 파일 갱신 전, SaveData 내 요소를 갱신합니다.
        UpdateUserDatas();

        // json 세이브 파일을 갱신합니다.
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveFilePath, json);

        Debug.Log("Game data saved!");
    }

    private void UpdateUserDatas()
    {
        saveData.user = UserData.UpdateUserData();
        saveData.userCharacter = UserCharacterData.UpdateUserCharacterData();
        saveData.userFlowerBouquet = UserFlowerBouquetData.UpdateUserFlowerBouquetData();
        saveData.userFlowerBouquetList = UserFlowerBouquetData.UpdateUserFlowerBouquetListData();
        saveData.userParameter = UserParameterData.UpdateUserParameterData();
        saveData.userStage = UserStageData.UpdateUserStageData();
    }

    private async Task LoadUserDatas()
    {
        // 세이브 파일이 있다면 읽어온다.
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);

            saveData = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Game data loaded.");
        }
        // 없다면 새 세이브 데이터를 생성한다. (세이브 파일 생성 X)
        else
        {
            Debug.Log("No save file found. creating new data structure.");

            await CreateNewSaveData();
        }

        // 이후 각 ~~Data 클래스의 딕셔너리에 Set
        UserData.SetUserDataDictionary(saveData.user);
        UserCharacterData.SetUserCharacterDataDictionary(saveData.userCharacter);
        UserFlowerBouquetData.SetUserFlowerBouquetDataDictionary(saveData.userFlowerBouquet);
        UserFlowerBouquetData.SetUserFlowerBouquetListDataDictionary(saveData.userFlowerBouquetList);
        UserParameterData.SetUserParameterData(saveData.userParameter);
        UserStageData.SetUserStageDataDictionary(saveData.userStage);
    }

    private async Task LoadInternalDatas()
    {
        var activeSkillTask = LoadDataFromAddress<ActiveSkill>("InternalActiveSkillData");
        var boardTask = LoadDataFromAddress<Board>("InternalBoardData");
        var characterTask = LoadDataFromAddress<Character>("InternalCharacterData");
        var collectionTask = LoadDataFromAddress<Collection>("InternalCollectionData");
        var constantTask = LoadDataFromAddress<Constant>("InternalConstantData");
        var dialogueTask = LoadDataFromAddress<Dialogue>("InternalDialogueData");
        var dropTask = LoadDataFromAddress<Drop>("InternalDropData");
        var fairyTask = LoadDataFromAddress<Fairy>("InternalFairyData");
        var flowerBouquetTask = LoadDataFromAddress<Bouquet>("InternalFlowerBouquetData");
        var gimmickTask = LoadDataFromAddress<Gimmick>("InternalGimmickData");
        var optionTask = LoadDataFromAddress<Option>("InternalOptionData");
        var stageTask = LoadDataFromAddress<Stage>("InternalStageData");
        var stageRewardTask = LoadDataFromAddress<StageReward>("InternalStageRewardData");

        await Task.WhenAll(activeSkillTask, boardTask, characterTask, collectionTask, constantTask,
                           dialogueTask, dropTask, fairyTask, flowerBouquetTask, gimmickTask,
                           optionTask, stageTask, stageRewardTask);

        ActiveSkillData.SetActiveSkillDictionary(await activeSkillTask);        
        BoardData.SetBoardDictionary(await boardTask);
        CharacterData.SetCharacterDictionary(await characterTask);
        CollectionData.SetCollectionDictionary(await collectionTask);
        ConstantData.SetConstantDictionary(await constantTask);
        DialogueData.SetDialogueDictionary(await dialogueTask);
        DropData.SetDropDictionary(await dropTask);
        FairyData.SetFairyDictionary(await fairyTask);
        BouquetData.SetFlowerBouquetDictionary(await flowerBouquetTask);
        GimmickData.SetGimmickDictionary(await gimmickTask);
        OptionData.SetOptionDictionary(await optionTask);
        StageData.SetStageDictionary(await stageTask);
        StageRewardData.SetStageRewardDictionary(await stageRewardTask);
    }

    public async Task CreateNewSaveData()
    {
        saveData = new SaveData();

        var userTask = LoadDataFromAddress<User>("InitialUserData");
        var userCharacterTask = LoadDataFromAddress<UserCharacter>("InitialUserCharacterData");
        var userFlowerBouquetTask = LoadDataFromAddress<UserFlowerBouquet>("InitialUserFlowerBouquetData");
        var userFlowerBouquetListTask = LoadDataFromAddress<UserFlowerBouquetList>("InitialUserFlowerBouquetListData");
        var userParameterTask = LoadDataFromAddress<UserParameter>("InitialUserParameterData");
        var userStageTask = LoadDataFromAddress<UserStage>("InitialUserStageData");

        await Task.WhenAll(userTask, userCharacterTask, userFlowerBouquetTask, userFlowerBouquetListTask, userParameterTask, userStageTask);

        saveData.user = await userTask;
        saveData.userCharacter = await userCharacterTask;
        saveData.userFlowerBouquet = await userFlowerBouquetTask;
        saveData.userFlowerBouquetList = await userFlowerBouquetListTask;
        saveData.userParameter = await userParameterTask;
        saveData.userStage = await userStageTask;

        Debug.Log("Default data initialization complete.");
    }

    private async Task<List<T>> LoadDataFromAddress<T>(string address)
    {
        AsyncOperationHandle<BaseSOData<T>> handle = Addressables.LoadAssetAsync<BaseSOData<T>>(address);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            List<T> defaultItems = new List<T>(handle.Result.dataList);
            Addressables.Release(handle);
            return defaultItems;
        }
        // 실패 시 빈 리스트 반환
        else
        {
            Debug.LogError($"Failed to load default data from address: {address}");
            Addressables.Release(handle);
            return new List<T>();
        }
    }

    public async Task LoadDataAsync()
    {
        // 여기서 데이터를 비동기적으로 로드합니다. 예를 들어 파일이나 웹 서비스로부터 로드합니다.
        // 실제 데이터 로딩 로직으로 교체합니다.

        // Internal Data 로드
        await LoadInternalDatas();
        // 유저 세이브 데이터 로드
        await LoadUserDatas();

        // Spine 로드
        await SpineUtility.PreloadGroupAsync("flowerSpines");
        await SpineUtility.PreloadGroupAsync("flowerGimmicks");
        await SpineUtility.PreloadGroupAsync("characters");

        await Task.Delay(1000);

        // 데이터 로드가 끝났다면 Game.cs에 데이터 로드 완료 신호를 보낸다.
        Game.instance.FinishDataLoad();
    }
}
