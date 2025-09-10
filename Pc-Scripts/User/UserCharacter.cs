using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;

[System.Serializable]
public class UserCharacter : IDataFromDto<UserCharacterDTO>
{
    [SerializeField] private int _slotid;
    [SerializeField] private int _id;
    [SerializeField] private int _level;

    public void FromDto(UserCharacterDTO dto)
    {
        _slotid = dto.slotid;
        _id = dto.id;
        _level = dto.level;
    }

    public int slotid { get { return _slotid; } set { _slotid = value; } } // Slot id
    public int id { get { return _id; } set { _id = value; } }// 장착한 character id
    public int level { get { return _level; } set { _level = value; } }// 장착한 character의 level
}

[System.Serializable]
public class UserCharacterDTO : IDto
{
    public int slotid;
    public int id;
    public int level;
}
