using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public List<User> user;
    public List<UserCharacter> userCharacter;
    public List<UserFlowerBouquet> userFlowerBouquet;
    public List<UserFlowerBouquetList> userFlowerBouquetList;
    public List<UserParameter> userParameter;
    public List<UserStage> userStage;
}