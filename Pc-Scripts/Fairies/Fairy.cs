using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Fairy : IDataFromDto<FairyDTO>
{
    [SerializeField] private int _id;
    //[SerializeField] private int _rarity;
    [SerializeField] private string _key;
    [SerializeField] private FairyType _fairyType;
    //[SerializeField] private DropColor _fairyColor;
    //[SerializeField] private int _fairyColorBonus;
    //[SerializeField] private FairyClass _fairyClass;
    [SerializeField] private int _dropId;
    [SerializeField] private int _collection;
    [SerializeField] private int _mainoption;
    //[SerializeField] private int _expbase;
    //[SerializeField] private int _expadd;
    //[SerializeField] private int _linkcount;
    [SerializeField] private int _growthRequirement;
    //[SerializeField] private string[] _abilityList;
    protected FairyBehaviour m_fairyBehaviour;

    public Fairy() { }
    public void FromDto(FairyDTO dto)
    {
        _id = dto.id;
        //_rarity = dto.rarity;
        _key = dto.key;
        _fairyType = (FairyType)System.Enum.Parse(typeof(FairyType), dto.type);
        //_fairyColor = (DropColor)System.Enum.Parse(typeof(DropColor), dto.color);
        //_fairyColorBonus = dto.colorbonus;
        //_fairyClass = (FairyClass)System.Enum.Parse(typeof(FairyClass), dto.fairyclass); 얘는 json 파일 내의 이름 (class)를 변경해야될듯
        _dropId = dto.drop;
        _collection = dto.collection;
        _mainoption = dto.mainoption;
        //_expbase = dto.expbase;
        //_expadd = dto.expadd;
        //_linkcount = dto.linkcount;
        _growthRequirement = dto.growthRequirement;
        //_abilityList = dto.abilityList;
    }

    /// <summary>
    /// Fairy의 id
    /// </summary>
    public int id { get { return _id; } set { _id = value; } }
    /// <summary>
    /// Fairy의 레어도
    /// </summary>
    //public int rarity { get { return _rarity; } set { _rarity = value; } }
    /// <summary>
    /// Fairy의 key
    /// </summary>
    public string key { get { return _key; } set { _key = value; } }
    /// <summary>
    /// Fairy의 종류
    /// </summary>
    public FairyType fairyType { get { return _fairyType; } set { _fairyType = value; } }
    /// <summary>
    /// Fairy의 색상
    /// </summary>
    //public DropColor fairyColor { get { return _fairyColor; } set { _fairyColor = value; } }
    /// <summary>
    /// Fairy의 컬러 보너스
    /// </summary>
    //public int fairyColorBonus { get { return _fairyColorBonus; } set { _fairyColorBonus = value; } }
    /// <summary>
    /// Fairy의 클래스
    /// </summary>
    //public FairyClass fairyClass { get { return _fairyClass; } set { _fairyClass = value; } }
    /// <summary>
    /// 해당 Fairy가 보유한 Drop의 id값
    /// </summary>
    public int dropId { get { return _dropId; } set { _dropId = value; } }
    /// <summary>
    /// 해당 Fairy가 포함되는 세트 종류
    /// </summary>
    public int collection { get { return _collection; } set { _collection = value; } }
    /// <summary>
    /// 해당 Fairy가 보유한 메인 옵션
    /// </summary>
    public int mainoption { get { return _mainoption; } set { _mainoption = value; } }
    /// <summary>
    /// 해당 Fairy의 1레벨 경험치 요구량
    /// </summary>
    //public int expbase { get { return _expbase; } set { _expbase = value; } }
    /// <summary>
    /// 해당 Fairy의 레벨 당 상승하는 경험치 요구량
    /// </summary>
    //public int expadd { get { return _expadd; } set { _expadd = value; } }
    /// <summary>
    /// 해당 Fairy가 보유한 linkcount 수치
    /// </summary>
    //public int linkcount { get { return _linkcount; } set { _linkcount = value; } }
    /// <summary>
    /// 해당 Fairy의 growthRequirement
    /// </summary>
    public int growthRequirement { get { return _growthRequirement; } set { _growthRequirement = value; } }
    /// <summary>
    /// 해당 Fairy가 보유한 abilityList
    /// </summary>
    //public string[] abilityList { get { return _abilityList; } set { _abilityList = value; } }

    public FairyBehaviour fairyBehaviour
    {
        get { return m_fairyBehaviour; }
        set
        {
            m_fairyBehaviour = value;
            m_fairyBehaviour.SetFairy(this);
        }
    }
    

    internal Fairy InstantiateCharacterObj(GameObject fairyPrefab, Transform containerObj, Vector3 position)
    {
        //1. Fairy 오브젝트를 생성
        GameObject fairy = Object.Instantiate(fairyPrefab, position, Quaternion.identity);
        
        //2. 컨테이너에 Character를 포함시킨다.
        fairy.transform.parent = containerObj;

        //3. Character 오브젝트에 적용된 CharacterBehaviour 컴포너트를 보관한다.
        this.fairyBehaviour = fairy.transform.GetComponent<FairyBehaviour>();

        return this;
    }
}

[System.Serializable]
public class FairyDTO : IDto
{
    [SerializeField] public int id;
    //[SerializeField] public int rarity;
    [SerializeField] public string key;
    [SerializeField] public string type;
    //[SerializeField] public string color;
    //[SerializeField] public int colorbonus;
    //[SerializeField] public string fairyclass;
    [SerializeField] public int drop;
    [SerializeField] public int collection;
    [SerializeField] public int mainoption;
    //[SerializeField] public int expbase;
    //[SerializeField] public int expadd;
    //[SerializeField] public int linkcount;
    [SerializeField] public int growthRequirement;
    //[SerializeField] public string[] abilityList;
}