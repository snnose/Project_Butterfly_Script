using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character : IDataFromDto<CharacterDTO>
{
    [SerializeField] private int _id;
    [SerializeField] private string _key;
    //[SerializeField] private CharacterColor _characterColor;
    [SerializeField] private CharacterType _characterType;
    [SerializeField] private int _characterBoardShape;
    //[SerializeField] private int _dropBonus;
    //[SerializeField] private int _linkcount;
    [SerializeField] private int[] _activeSkillList;
    [SerializeField] private int _flowerSkill;
    //[SerializeField] private int _maximumlinkcount;
    [SerializeField] private int _maximumlevel;
    [SerializeField] private int _levelupcostperlevel;

    public Character() { }
    public void FromDto(CharacterDTO dto)
    {
        _id = dto.id;
        _key = dto.key;
        //_characterColor = (CharacterColor)Enum.Parse(typeof(CharacterColor), dto.color);
        _characterType = (CharacterType)Enum.Parse(typeof(CharacterType), dto.type);
        _characterBoardShape = dto.boardShape;
        //_dropBonus = dto.dropbonus;
        //_linkcount = dto.linkcount;
        _activeSkillList = dto.skills;
        _flowerSkill = dto.flowerSkill;
        //_maximumlinkcount = dto.maximumlinkcount;
        _maximumlevel = dto.maximumlevel;
        _levelupcostperlevel = dto.levelupcostperlevel;
    }

    /// <summary>
    /// Character의 id
    /// </summary>
    public int id{ get { return _id; } set { _id = value; } }
    /// <summary>
    /// Character의 key
    /// </summary>
    public string key{ get { return _key; } set { _key = value; } }
    /// <summary>
    /// Character의 색상
    /// </summary>
    //public CharacterColor characterColor{ get { return _characterColor; } set { _characterColor = value; } }
    /// <summary>
    /// Character의 종류
    /// </summary>
    public CharacterType characterType{ get { return _characterType; } set { _characterType = value; } }
    /// <summary>
    /// Character가 가지는 보드 모양 : Cell id 참조
    /// </summary>
    public int characterBoardShape{ get { return _characterBoardShape; } set { _characterBoardShape = value; } }
    /// <summary>
    /// Character가 같은 color의 드롭 보너스를 갖는 수치
    /// </summary>
    //public int dropBonus{ get { return _dropBonus; } set { _dropBonus = value; } }
    /// <summary>
    /// Character가 가지는 기본 linkcount 수치
    /// </summary>
    //public int linkcount{ get { return _linkcount; } set { _linkcount = value; } }
    /// <summary>
    /// Character가 가질 수 있는 active skill id 전체 목록
    /// </summary>
    public int[] activeSkillList{ get { return _activeSkillList; } set { _activeSkillList = value; } }
    /// <summary>
    /// Character가 가지는 FlowerGame에서만 사용 가능한 Skill
    /// </summary>
    public int flowerSkill{ get { return _flowerSkill; } set { _flowerSkill = value; } }
    /// <summary>
    /// Character가 가질 수 있는 최대 linkcount 수치: 인게임에서도 이 최대치를 넘을 수 없음
    /// </summary>
    //public int maximumlinkcount{ get { return _maximumlinkcount; } set { _maximumlinkcount = value; } }
    /// <summary>
    /// Character가 가질 수 있는 최대 level의 수치 : 추후 확장될 여지가 있도록 구현 필요
    /// </summary>
    public int maximumlevel{ get { return _maximumlevel; } set { _maximumlevel = value; } }
    /// <summary>
    /// Character의 level을 상승시키는 데 필요한 비용. (레벨 비례 상승 분량)
    /// </summary>
    public int levelupcostperlevel{ get { return _levelupcostperlevel; } set { _levelupcostperlevel = value; } }
    
    protected CharacterBehaviour m_characterBehaviour;

    public CharacterBehaviour characterBehaviour
    {
        get { return m_characterBehaviour; }
        set
        {
            m_characterBehaviour = value;
            m_characterBehaviour.SetCharacter(this);
        }
    }
    

    internal Character InstantiateCharacterObj(GameObject characterPrefab, Transform containerObj, Vector3 position)
    {
        //1. Character 오브젝트를 생성
        GameObject character = UnityEngine.Object.Instantiate(characterPrefab, position, Quaternion.identity);
        
        //2. 컨테이너에 Character를 포함시킨다.
        character.transform.parent = containerObj;

        //3. Character 오브젝트에 적용된 CharacterBehaviour 컴포너트를 보관한다.
        this.characterBehaviour = character.transform.GetComponent<CharacterBehaviour>();

        return this;
    }
}

[System.Serializable]
public class CharacterDTO : IDto
{
    public int id;
    public string key;
    //public string color;
    public string type;
    public int boardShape;
    //public int dropbonus;
    //public int linkcount;
    public int[] skills;
    public int flowerSkill;
    //public int maximumlinkcount;
    public int maximumlevel;
    public int levelupcostperlevel;
}