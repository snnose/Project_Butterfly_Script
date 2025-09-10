using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;

[System.Serializable]
public class User : IDataFromDto<UserDTO>
{
    [SerializeField] private UserCharacterOwned[] _userCharacterOwned;
    [SerializeField] private UserFairyOwned[] _userFairyOwned;
    [SerializeField] private UserDropOwned[] _userDropOwned;

    public void FromDto(UserDTO dto)
    {
        int len = dto.characters.Length;
        _userCharacterOwned = new UserCharacterOwned[len];
        for (int i = 0; i < len; i++)
        {
            _userCharacterOwned[i] = dto.characters[i];
        }

        len = dto.fairies.Length;
        _userFairyOwned = new UserFairyOwned[len];
        for (int i = 0; i < len; i++)
        {
            _userFairyOwned[i] = dto.fairies[i];
        }

        len = dto.drops.Length;
        _userDropOwned = new UserDropOwned[len];
        for (int i = 0; i < len; i++)
        {
            _userDropOwned[i] = dto.drops[i];
        }
    }

    public UserCharacterOwned[] userCharacterOwned { get{ return _userCharacterOwned; } set { _userCharacterOwned = value; } }
    public UserFairyOwned[] userFairyOwned { get { return _userFairyOwned; } set { _userFairyOwned = value; } }
    public UserDropOwned[] userDropOwned { get { return _userDropOwned; } set { _userDropOwned = value; } }
}

[System.Serializable]
public class UserDTO : IDto
{
    public UserCharacterOwned[] characters;
    public UserFairyOwned[] fairies;
    public UserDropOwned[] drops;
}

[System.Serializable]
public class UserCharacterOwned
{
    public int id;
    public bool isOwned;
    public int level;
}

[System.Serializable]
public class UserFairyOwned
{
    public int id;
    public bool isOwned;
}

[System.Serializable]
public class UserDropOwned
{
    public int id;
    public bool isOwned;
}