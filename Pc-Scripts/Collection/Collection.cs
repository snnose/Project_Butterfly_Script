using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Collection : IDataFromDto<CollectionDTO>
{
    [SerializeField] private int _id;
    [SerializeField] private string _key;
    [SerializeField] private int _optionid;

    public Collection() { }
    public void FromDto(CollectionDTO dto)
    {
        _id = dto.id;
        _key = dto.key;
        _optionid = dto.option;
    }

    /// <summary>
    /// Collection의 고유 id
    /// </summary>
    public int id { get { return _id; } set { _id = value; } }
    /// <summary>
    /// Collection의 고유 key
    /// </summary>
    public string key { get { return _key; } set { _key = value; } }
    /// <summary>
    /// Collection의 세트 옵션 id
    /// </summary>
    public int optionid { get { return _optionid; } set { _optionid = value; } }
    
    protected CollectionBehaviour m_collectionBehaviour;

    public CollectionBehaviour collectionBehaviour
    {
        get { return m_collectionBehaviour; }
        set
        {
            m_collectionBehaviour = value;
            m_collectionBehaviour.SetCollection(this);
        }
    }

    internal Collection InstantiateCharacterObj(GameObject collectionPrefab, Transform containerObj, Vector3 position)
    {
        //1. Collection 오브젝트를 생성
        GameObject collection = Object.Instantiate(collectionPrefab, position, Quaternion.identity);
        
        //2. 컨테이너에 Character를 포함시킨다.
        collection.transform.parent = containerObj;

        //3. Character 오브젝트에 적용된 CharacterBehaviour 컴포너트를 보관한다.
        this.collectionBehaviour = collection.transform.GetComponent<CollectionBehaviour>();

        return this;
    }
}

[System.Serializable]
public class CollectionDTO : IDto
{
    public int id;
    public string key;
    public int option;
}