using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum FlowerBouquetRarity
{
    none,
    story,
    normal,
    rare,
    epic,
    legend
}
public enum CurrencyType
{
    none,
    experience,
    itemexperience
}
/// <summary>
/// FlowerGame용 블록 세트. 
/// 현재 유저가 세팅해둔 블록 세트 리스트이고, 개별 데이터에 해당함.
/// groupNum를 기준으로 FlowerBouquet 묶음 리스트를 처리한다.
/// </summary>
[Serializable]
public class UserFlowerBouquetList : IDataFromDto<UserFlowerBouquetListDTO>
{
    [SerializeField] private bool _isSelected;
    [SerializeField] private int _id;
    [SerializeField] private int[] _bouquetList;

    public void FromDto(UserFlowerBouquetListDTO dto)
    {
        _isSelected = dto.isSelected;
        _id = dto.id;
        _bouquetList = dto.bouquetList;
    }

    public bool isSelected { get { return _isSelected; } set { _isSelected = value; } }
    public int id { get { return _id; } set { _id = value; } }
    public int[] bouquetList { get { return _bouquetList; } set { _bouquetList = value; } }
}

[Serializable]
public class UserFlowerBouquetListDTO : IDto
{
    public bool isSelected;
    public int id;
    public int[] bouquetList;
}

/// <summary>
/// FlowerGame용 
/// 현재 유저가 보유중인 블록 아이템 리스트.
/// 각 FlowerBlock들이 3x3 크기에 어떤 구성인지 등등에 대한 아이템 정보를 보유하고 있다.
/// </summary>
[Serializable]
public class UserFlowerBouquet : IDataFromDto<UserFlowerBouquetDTO>
{
    [SerializeField] private int _id;
    [SerializeField] private string _key;
    [SerializeField] private FlowerBouquetRarity _rarity;
    [SerializeField] private int _collection;
    [SerializeField] private int _level;
    [SerializeField] private int _maxlevel;
    [SerializeField] private int _exp;
    [SerializeField] private int _expbase;
    [SerializeField] private int _expadd;
    [SerializeField] private int _mainoption;
    [SerializeField] private int _mainoptionlevel;
    [SerializeField] private int _suboption1;
    [SerializeField] private int _suboption1level;
    [SerializeField] private int _suboption2;
    [SerializeField] private int _suboption2level;
    [SerializeField] private int[] _fairyList;
    [SerializeField] private bool _isDecomposable;
    [SerializeField] private CurrencyType _decomposeCurrencyType;
    [SerializeField] private int _decomposeCurrencyAmount;
    [SerializeField] private string _acquisitionDate;

    public void FromDto(UserFlowerBouquetDTO dto)
    {
        _id = dto.id;
        _key = dto.key;
        _rarity = (FlowerBouquetRarity)Enum.Parse(typeof(FlowerBouquetRarity), dto.rarity);
        _collection = dto.collection;
        _level = dto.level;
        _maxlevel = dto.maxlevel;
        _exp = dto.exp;
        _expbase = dto.expbase;
        _expadd = dto.expadd;
        _mainoption = dto.mainoption;
        _mainoptionlevel = dto.mainoptionlevel;
        _suboption1 = dto.suboption1;
        _suboption1level = dto.suboption1level;
        _suboption2 = dto.suboption2;
        _suboption2level = dto.suboption2level;
        _fairyList = dto.fairyList;
        _isDecomposable = dto.isDecomposable;
        _decomposeCurrencyType = (CurrencyType)Enum.Parse(typeof(CurrencyType), dto.decomposeCurrencyType);
        _decomposeCurrencyAmount = dto.decomposeCurrencyAmount;
        _acquisitionDate = dto.acquisitionDate;
    }

    /// <summary>
    /// FlowerBouquet의 고유 id
    /// </summary>
    public int id { get { return _id; } set { _id = value; } }
    /// <summary>
    /// FlowerBouquet의 고유 키
    /// </summary>
    public string key { get { return _key; } set { _key = value; } }
    /// <summary>
    /// FlowerBouquet의 희귀도
    /// </summary>
    public FlowerBouquetRarity rarity { get { return _rarity; } set { _rarity = value; } }
    /// <summary>
    /// FlowerBouquet가 속한 세트의 id
    /// </summary>
    public int collection { get { return _collection; } set { _collection = value; } }
    /// <summary>
    /// FlowerBouquet의 현재 레벨 (강화 단계)
    /// </summary>
    public int level { get { return _level; } set { _level = value; } }
    /// <summary>
    /// FlowerBouquet의 최대 레벨 (강화 단계)
    /// </summary>
    public int maxlevel { get { return _maxlevel; } set { _maxlevel = value; } }
    /// <summary>
    /// FlowerBouquet의 현재 경험치
    /// </summary>
    public int exp { get { return _exp; } set { _exp = value; } }
    /// <summary>
    /// FlowerBouquet의 기본 요구 경험치
    /// </summary>
    public int expbase { get { return _expbase; } set { _expbase = value; } }
    /// <summary>
    /// FlowerBouquet의 레벨 비례 추가 요구 경험치
    /// </summary>
    public int expadd { get { return _expadd; } set { _expadd = value; } }
    /// <summary>
    /// FlowerBouquet의 주옵션
    /// </summary>
    public int mainoption { get { return _mainoption; } set { _mainoption = value; } }
    /// <summary>
    /// FlowerBouquet의 주옵션 레벨
    /// </summary>
    public int mainoptionlevel { get { return _mainoptionlevel; } set { _mainoptionlevel = value; } }
    /// <summary>
    /// FlowerBouquet의 첫번째 부옵션
    /// </summary>
    public int suboption1 { get { return _suboption1; } set { _suboption1 = value; } }
    /// <summary>
    /// FlowerBouquet의 첫번째 부옵션 레벨
    /// </summary>
    public int suboption1level { get { return _suboption1level; } set { _suboption1level = value; } }
    /// <summary>
    /// FlowerBouquet의 두번째 부옵션
    /// </summary>
    public int suboption2 { get { return _suboption2; } set { _suboption2 = value; } }
    /// <summary>
    /// FlowerBouquet의 두번째 부옵션 레벨
    /// </summary>
    public int suboption2level { get { return _suboption2level; } set { _suboption2level = value; } }
    /// <summary>
    /// FlowerBouquet의 모양 (-1 = 빈칸 / fairyid = 아이디에 해당하는 꽃이 칸을 채움)
    /// </summary>
    public int[] fairyList { get { return _fairyList; } set { _fairyList = value; } }
    /// <summary>
    /// FlowerBouquet의 분해 여부 결정 (true = 분해 가능)
    /// </summary>
    public bool isDecomposable { get { return _isDecomposable; } set { _isDecomposable = value; } }
    /// <summary>
    /// FlowerBouquet의 분해 결과물 타입
    /// </summary>
    public CurrencyType decomposeCurrencyType { get { return _decomposeCurrencyType; } set { _decomposeCurrencyType = value; } }
    /// <summary>
    /// FlowerBouquet의 분해 결과물 양
    /// </summary>
    public int decomposeCurrencyAmount { get { return _decomposeCurrencyAmount; } set { _decomposeCurrencyAmount = value; } }
    /// <summary>
    /// FlowerBouquet의 획득 날짜
    /// </summary>
    public string acquisitionDate { get { return _acquisitionDate; } set { _acquisitionDate = value; } }

    public int[] GenerateFairyList()
    {
        // 리스트에 -1을 4개 추가
        List<int> randomFairyList = Enumerable.Repeat(-1, 4).ToList();

        // 100001 ~ 100030 사이의 랜덤 정수 5개를 리스트에 추가
        for (int i = 0; i < 5; i++)
        {
            randomFairyList.Add(UnityEngine.Random.Range(100001, 100031));
        }

        // 리스트를 섞는다 (Fisher-Yates Shuffle)
        for (int i = randomFairyList.Count - 1; i > 0; i--)
        {
            // 0부터 i를 포함한 범위에서 랜덤 인덱스 j 선택
            int j = UnityEngine.Random.Range(0, i + 1);

            // a[i]와 a[j]의 위치 변경
            (randomFairyList[i], randomFairyList[j]) = (randomFairyList[j], randomFairyList[i]);
        }

        return randomFairyList.ToArray();
    }
}

[Serializable]
public class UserFlowerBouquetDTO : IDto
{
    public int id;
    public string key;
    public string rarity;
    public int collection;
    public int level;
    public int maxlevel;
    public int exp;
    public int expbase;
    public int expadd;
    public int mainoption;
    public int mainoptionlevel;
    public int suboption1;
    public int suboption1level;
    public int suboption2;
    public int suboption2level;
    public int[] fairyList;
    public bool isDecomposable;
    public string decomposeCurrencyType;
    public int decomposeCurrencyAmount;
    public string acquisitionDate;
}