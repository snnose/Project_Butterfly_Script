using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using DG.Tweening;

namespace EffectSystem
{
    public class EffectBehaviour : MonoBehaviour
    {
        public IObjectPool<EffectBehaviour> pool { get; set; }

        public EffectInformation effectInformation;

        private new ParticleSystem particleSystem;
        private ParticleSystemRenderer particleSystemRenderer; // 이펙트의 Material 접근 가능

        private IEnumerator activateEffect = null;
        // 이펙트가 출력 중인 동안, UI 조작을 막을지에 대한 플래그
        private bool uiFlag = false;
        private MaterialEffectOptions materialOptions;

        public void SetMaterialOptions(MaterialEffectOptions options)
        {
            this.materialOptions = options;
        }
        public void SetMaterialOptionsNull()
        {
            this.materialOptions = null;
        }

        private void Awake()
        {
            if (particleSystem == null)
            {
                particleSystem = this.GetComponent<ParticleSystem>();
            }

            if (particleSystemRenderer == null)
            {
                particleSystemRenderer = this.GetComponent<ParticleSystemRenderer>();
            }
        }

        public void Get()
        {
            activateEffect = EffectCoroutine();
        }

        // 24.12.02) 루프 이펙트 고민
        // 루프하는 이펙트들은 외부에서 꺼줘야하는데 문제는 그 이펙트를 어떻게 기억하고 꺼주느냐인데
        // 예시로 AchivementCorrect 같은 경우 CellBehaivour에서 PointerUP, PointerEndDrag가 실행될 때 반환돼야함
        // => 루프 이펙트는 가져올 때 따로 기억해두는 리스트를 둔다? 사용하는 곳에서?

        // 외부에서 해당 오브젝트를 반환할 때 사용
        public void Release()
        {
            // 재생 중인 코루틴 해제
            if (activateEffect != null)
            {
                StopCoroutine(activateEffect);
                activateEffect = null;
            }

            if (this.gameObject.activeSelf)
                pool.Release(this);
        }

        public void ActivateEffect(Action directionCallBack = null)
        {
            if (activateEffect != null && directionCallBack == null)
            {
                StartCoroutine(activateEffect);
            }
            else if (activateEffect != null && directionCallBack != null)
            {
                activateEffect = null;
                StartCoroutine(EffectCoroutine(directionCallBack));
            }
            else
            {
                //Debug.Log("activateEffect is null!);
            }
        }

        private IEnumerator EffectCoroutine(Action directionCallBack = null)
        {
            // 이펙트 출력 시작 시, uiFlag가 TRUE라면 UI 조작도 막아준다.
            if (uiFlag)
            {
                UIManager.Instance.DisableCanvasGroup();
            }
            // 파티클 시스템의 속성값을 다루는 main
            ParticleSystem.MainModule particleSystemMain = particleSystem.main;
            float duration;

            if (materialOptions != null)
            {
                float time = 0f;
                Debug.Log("materialOptions");

                while (time < materialOptions.duration)
                {
                    time += Time.deltaTime;
                    float progress = Mathf.Lerp(materialOptions.startValue, materialOptions.endValue, Mathf.Clamp01(time / materialOptions.duration)); // Lerp로 서서히 변화
                    particleSystemRenderer.material.SetFloat(materialOptions.propertyName, progress); // _FriezeProgress 값 설정
                    yield return null;  // 1프레임 대기
                }

                // 마지막에 정확히 FriezeProgress 값이 end로 설정되도록 보장
                particleSystemRenderer.material.SetFloat(materialOptions.propertyName, materialOptions.endValue);
            }


            // 이펙트 루프인 경우 effectInfo.duration 시간 만큼만 재생
            if (particleSystemMain.loop)
                duration = effectInformation.duration;
            // 루프가 없으면 파티클 시스템에 지정된 duration 사용
            else
                duration = particleSystemMain.duration;

            // 이펙트 재생 속도 변경
            if (effectInformation.speed != 0f)
                particleSystemMain.simulationSpeed = effectInformation.speed;
            // 크기 변경
            if (effectInformation.scale != Vector3.zero)
                this.transform.localScale = effectInformation.scale;

            MoveEffect(duration);

            // 콜백 함수가 있다면 실행. 콜백 함수가 있다면 항상 실행하도록 보장 처리
            if (directionCallBack != null)
            {
                if (effectInformation.callBackTimer > 0f)
                {
                    yield return new WaitForSeconds(effectInformation.callBackTimer);
                    duration -= effectInformation.callBackTimer;
                }

                directionCallBack?.Invoke();
            }

            if (duration > 0f)
            {
                yield return new WaitForSeconds(duration);
            }

            // 이펙트가 루프하고 effectInfo.duration이 -1이면 외부에서 해제하기 전까지 반복
            if (particleSystemMain.loop
              && effectInformation.duration == -1f)
                yield return new WaitForSeconds(999f);

            pool.Release(this);

            // 이펙트 출력 시작 시, uiFlag가 TRUE였다면 이펙트 해제 시 UI 조작을 다시 해제해준다.
            if (uiFlag)
            {
                UIManager.Instance.EnableCanvasGroup();
            }

            yield break;
        }

        private void MoveEffect(float duration)
        {
            switch (effectInformation.moveType)
            {
                case EffectMoveType.None:
                    break;
                case EffectMoveType.DOMove:
                    this.transform.DOMove(effectInformation.endPosition, duration);
                    break;
                case EffectMoveType.DOPath:
                    this.transform.DOPath(effectInformation.path.ToArray(), duration);
                    break;
                case EffectMoveType.CustomDOPath:
                    CustomDOPath(duration);
                    break;
                default:
                    break;
            }
        }

        private void CustomDOPath(float duration)
        {
            Vector3 startPos = effectInformation.initPosition;
            Vector3 endPos = effectInformation.endPosition;
            int stopoverCount = effectInformation.stopoverCount;

            Vector3[] path = GetCustomPath(startPos, endPos, stopoverCount);
            this.transform.DOPath(path, duration, PathType.CatmullRom);
        }

        private Vector3[] GetCustomPath(Vector3 startPos, Vector3 endPos, int stopoverCount)
        {
            List<Vector3> path = new List<Vector3>();
            Vector3 range = new Vector3(effectInformation.randomCoefficient.x * Math.Abs(startPos.x - endPos.x),
                                        effectInformation.randomCoefficient.y * Math.Abs(startPos.y - endPos.y),
                                        effectInformation.randomCoefficient.z * Math.Abs(startPos.z - endPos.z)) / (stopoverCount + 2);
            path.Add(startPos);
           
            
            for (int i = 1; i <= stopoverCount; i++)
            {
                float t = (float)i / (float)(stopoverCount + 1);
                Vector3 stopover = Vector3.Lerp(startPos, endPos, t);
                stopover += new Vector3(UnityEngine.Random.Range(-range.x, range.x),
                                        UnityEngine.Random.Range(-range.y, range.y),
                                        UnityEngine.Random.Range(-range.z, range.z));
                Debug.Log($"EffectBehaviour / startPos : {startPos}, endPos : {endPos}, stopover : {stopover}");
                path.Add(stopover);
            }

            path.Add(endPos);

            return path.ToArray();
        }

        public void SetUIFlag(bool flag)
        {
            uiFlag = flag;
        }

        public void SetDOMove(Vector3 initPosition, Vector3 endPosition)
        {
            effectInformation.moveType = EffectMoveType.DOMove;

            SetInitPosition(initPosition);
            effectInformation.endPosition = endPosition;
        }

        public void SetCustomDOPath(Vector3 initPosition, Vector3 endPosition)
        {
            effectInformation.moveType = EffectMoveType.CustomDOPath;

            SetInitPosition(initPosition);
            effectInformation.endPosition = endPosition;
        }

        public void SetDOPath(List<Vector3> path)
        {
            effectInformation.moveType = EffectMoveType.DOPath;

            SetInitPosition(path[0]);
            effectInformation.path = path;
        }

        public void SetInitPosition(Vector3 initPosition)
        {
            effectInformation.initPosition = initPosition;
            this.transform.position = effectInformation.initPosition;
        }

        public void SetMovePosition(Vector3 endPosition)
        {
            effectInformation.endPosition = endPosition;
        }
    }
}
