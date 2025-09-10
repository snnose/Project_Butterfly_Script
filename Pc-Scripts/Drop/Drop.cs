using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Drop : IDataFromDto<DropDTO>
{
    [SerializeField] private int _id;
    [SerializeField] private string _key;
    [SerializeField] private DropShape _dropShape; 
    [SerializeField] private DropColor _dropColor; 
    [SerializeField] private DropType _dropType; 
    [SerializeField] private int _dropWeight; 
    [SerializeField] private int _dropScore;
    //[SerializeField] private int _dropColorBonus;
    //[SerializeField] private float _criticalrate;
    //[SerializeField] private int _criticalbonus; 
    [SerializeField] private DropState _dropState; 
    [SerializeField] protected DropBehaviour m_dropBehaviour;

    public Drop() { }
    public void FromDto(DropDTO dto)
    {
        _id = dto.id;
        _key = dto.key;
        _dropShape = (DropShape)Enum.Parse(typeof(DropShape), dto.shape);
        _dropColor = (DropColor)Enum.Parse(typeof(DropColor), dto.color);
        _dropType = (DropType)Enum.Parse(typeof(DropType), dto.type);
        _dropWeight = dto.dropweight;
        _dropScore = dto.dropscore;
        //_criticalrate = dto.criticalrate;
        //_criticalbonus = dto.criticalbonus;
    }

    /// <summary>
    /// Drop의 id
    /// </summary>
    public int id { get { return _id; } set { _id = value; } }
    /// <summary>
    /// Drop의 key
    /// </summary>
    public string key { get { return _key; } set { _key = value; } }
    /// <summary>
    /// Drop의 모양
    /// </summary>
    public DropShape dropShape { get { return _dropShape; } set { _dropShape = value; } }
    /// <summary>
    /// Drop의 색상
    /// </summary>
    public DropColor dropColor { get { return _dropColor; } set { _dropColor = value; } }
    /// <summary>
    /// Drop의 종류
    /// </summary>
    public DropType dropType { get { return _dropType; } set { _dropType = value; } }
    /// <summary>
    /// Drop의 등장할 수 있는 가중치
    /// </summary>
    public int dropWeight { get { return _dropWeight; } set { _dropWeight = value; } }
    /// <summary>
    /// Drop의 기본 점수
    /// </summary>
    public int dropScore { get { return _dropScore; } set { _dropScore = value; } }
    /// <summary>
    /// Drop의 색깔 추가 점수
    /// </summary>
    //public int dropColorBonus { get { return _dropColorBonus; } set { _dropColorBonus = value; } }
    /// <summary>
    /// Drop의 치명타 확률
    /// </summary>
    //public float criticalrate { get { return _criticalrate; } set { _criticalrate = value; } }
    /// <summary>
    /// Drop의 치명타 시 추가 점수 수치
    /// </summary>
    //public int criticalbonus { get { return _criticalbonus; } set { _criticalbonus = value; } }
    /// <summary>
    /// 현재 Drop의 상태
    /// </summary>
    public DropState dropState { get { return _dropState; } set { _dropState = value; } }

    public DropBehaviour dropBehaviour
    {
        get { return m_dropBehaviour; }
        set
        {
            m_dropBehaviour = value;
            m_dropBehaviour.SetDrop(this);
        }
    }
    
    internal Drop InstantiateDropObj(GameObject dropPrefab, Transform containerObj, Vector3 position)
    {
        //1. Drop 오브젝트를 생성
        GameObject drop = UnityEngine.Object.Instantiate(dropPrefab, position, Quaternion.identity);
        
        //2. 컨테이너에 Drop을 포함시킨다.
        drop.transform.parent = containerObj;

        //3. Drop 오브젝트에 적용된 DropBehaviour 컴포너트를 보관한다.
        this.dropBehaviour = drop.transform.GetComponent<DropBehaviour>();

        return this;
    }
}

[System.Serializable]
public class DropDTO : IDto
{
    public int id;
    public string key;
    public string shape;
    public string color;
    public string type;
    public int dropweight;
    public int dropscore;
    //public int colorbonus;
    //public float criticalrate;
    //public int criticalbonus;
}