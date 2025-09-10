using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ActiveSkill : IDataFromDto<ActiveSkillDTO>
{
    [SerializeField] private int _id;
    [SerializeField] private ActiveSkillType _activeSkillType;
    [SerializeField] private string _key;
    [SerializeField] private int[] _skillParameter;
    [SerializeField] private int _skillCoolTime;
    [SerializeField] private int _skillLimitCount;
    [SerializeField] private bool _isActiveSkill;
    [SerializeField] private int _gimmickId;
    [SerializeField] private int _maxSkillCounter;

    public ActiveSkill() { }

    public void FromDto(ActiveSkillDTO dto)
    {
        _id = dto.id;
        _activeSkillType = (ActiveSkillType)Enum.Parse(typeof(ActiveSkillType), dto.skillType);
        _key = dto.key;
        _skillParameter = dto.skillParameter;
        _skillCoolTime = dto.coolTime;
        _skillLimitCount = dto.limitCount;
        _isActiveSkill = dto.isActiveSkill;
        _gimmickId = dto.gimmickId;
        _maxSkillCounter = dto.maxSkillCounter;
    }

    /// <summary>
    /// ActiveSkill 고유 id
    /// </summary>
    public int id { get { return _id; } set { _id = value; } }
    /// <summary>
    /// ActiveSkill의 Type
    /// </summary>
    public ActiveSkillType activeSkillType { get { return _activeSkillType; } set { _activeSkillType = value; } }
    /// <summary>
    /// ActiveSkill key
    /// </summary>
    public string key { get { return _key; } set { _key = value; } }
    /// <summary>
    /// 각 스킬 고유의 파라미터 값
    /// </summary>
    public int[] skillParameter { get { return _skillParameter; } set { _skillParameter = value; } }
    /// <summary>
    /// 스킬 재사용 대기 시간
    /// </summary>
    public int skillCoolTime { get { return _skillCoolTime; } set { _skillCoolTime = value; } }
    /// <summary>
    /// 한 게임에서 사용 가능한 스킬 제한 횟수
    /// </summary>
    public int skillLimitCount { get { return _skillLimitCount; } set { _skillLimitCount = value; } }
    /// <summary>
    /// 액티브 스킬인지 여부에 대한 플래그
    /// </summary>
    public bool isActiveSkill { get { return _isActiveSkill; } set { _isActiveSkill = value; } }
    /// <summary>
    /// 액티브 스킬에서 사용하는 기믹 id
    /// </summary>
    public int gimmickId { get { return _gimmickId; } set { _gimmickId = value; } }
    /// <summary>
    /// 패시브 - 차지형 스킬에서 사용하는 스킬 최대 카운터
    /// </summary>
    public int maxSkillCounter { get { return _maxSkillCounter; } set { _maxSkillCounter = value; } }
}

[Serializable]
public class ActiveSkillDTO : IDto
{
    public int id;
    public string skillType;
    public string key;
    public int[] skillParameter;
    public int coolTime;
    public int limitCount;
    public bool isActiveSkill;
    public int gimmickId;
    public int maxSkillCounter;
}