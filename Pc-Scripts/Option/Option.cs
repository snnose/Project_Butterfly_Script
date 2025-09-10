using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Option : IDataFromDto<OptionDTO>
{
    [SerializeField] private int _id;
    [SerializeField] private string _key;
    [SerializeField] private float _basevalue;
    [SerializeField] private float _addvalue;

    public Option() { }
    public void FromDto(OptionDTO dto)
    {
        _id = dto.id;
        _key = dto.key;
        _basevalue = dto.basevalue;
        _addvalue = dto.addvalue;
    }

    /// <summary>
    /// Option의 id
    /// </summary>
    public int id { get { return _id; } set { _id = value; } }
    /// <summary>
    /// Option의 key
    /// </summary>
    public string key { get { return _key; } set { _key = value; } }
    /// <summary>
    /// 해당 Option의 1레벨 수치
    /// </summary>
    public float basevalue { get { return _basevalue; } set { _basevalue = value; } }
    /// <summary>
    /// 해당 Option의 레벨 당 상승 수치
    /// </summary>
    public float addvalue { get { return _addvalue; } set { _addvalue = value; } } 

    protected OptionBehaviour m_optionBehaviour;

    public OptionBehaviour optionBehaviour
    {
        get { return m_optionBehaviour; }
        set
        {
            m_optionBehaviour = value;
            m_optionBehaviour.SetOption(this);
        }
    }
    

    internal Option InstantiateCharacterObj(GameObject optionPrefab, Transform containerObj, Vector3 position)
    {
        //1. Option 오브젝트를 생성
        GameObject option = Object.Instantiate(optionPrefab, position, Quaternion.identity);
        
        //2. 컨테이너에 Character를 포함시킨다.
        option.transform.parent = containerObj;

        //3. Character 오브젝트에 적용된 CharacterBehaviour 컴포너트를 보관한다.
        this.optionBehaviour = option.transform.GetComponent<OptionBehaviour>();

        return this;
    }
}

[System.Serializable]
public class OptionDTO : IDto
{
    public int id;
    public string key;
    public float basevalue;
    public float addvalue;
}