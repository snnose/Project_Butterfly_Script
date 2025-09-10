using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// DTO 클래스임을 나타내는 마커 인터페이스
public interface IDto { }
// DTO로부터 데이터를 받아 초기화하는 기능을 명시하는 인터페이스
public interface IDataFromDto<in TDto> where TDto : IDto
{
    void FromDto(TDto dto);
}

/// <summary>
/// Scriptable Object Data를 일반화 하기 위한 스크립트
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseSOData<T> : ScriptableObject
{
    public List<T> dataList;
}
