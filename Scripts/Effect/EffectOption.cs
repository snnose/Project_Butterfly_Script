using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectSystem
{
    [System.Serializable]
    public struct EffectInformation
    {
        [Header("[Key]")]
        public string key;

        [Header("[PoolingCount]")]
        public int poolingCount;

        [Header("[Move]")]
        public EffectMoveType moveType;
        public Vector3 initPosition;   // 이펙트가 생성되는 위치
        public Vector3 endPosition;    // 이펙트가 사라지는 위치
        public List<Vector3> path;
        public Vector3 randomCoefficient;
        public int stopoverCount;
        

        [Header("[Custom]")]
        public Vector3 scale;          // 이펙트 크기
        public float duration;         // 이펙트가 유지되는 시간 (파티클과 별개)
        public float speed;            // 이펙트 재생 속도
        public float callBackTimer;    // 콜백 함수 타이머
    }

    [System.Serializable]
    public enum EffectMoveType
    {
        None,
        DOMove,
        DOPath,
        CustomDOPath,
    }

    public class MaterialEffectOptions
    {
        public float startValue;
        public float endValue;
        public float duration;
        public string propertyName;
    }
}