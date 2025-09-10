using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserParameter : IDataFromDto<UserParameterDTO>
{
    [SerializeField] private int _experience;
    [SerializeField] private int _itemexperience;
    [SerializeField] private int _mainprogress;
    [SerializeField] private int _subprogress;

    public void FromDto(UserParameterDTO dto)
    {
        _experience = dto.experience;
        _itemexperience = dto.itemexperience;
        _mainprogress = dto.mainprogress;
        _subprogress = dto.subprogress;
    }

    public int experience { get { return _experience; } set { _experience = value; } }
    public int itemexperience { get { return _itemexperience; } set { _itemexperience = value; } }
    public int mainprogress { get { return _mainprogress; } set { _mainprogress = value; } }
    public int subprogress { get { return _subprogress; } set { _subprogress = value; } }
}

[System.Serializable]
public class UserParameterDTO : IDto
{
    public int experience;
    public int itemexperience;
    public int mainprogress;
    public int subprogress;
}