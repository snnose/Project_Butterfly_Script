using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace DirectingEventSystem
{
    [CreateAssetMenu(menuName = "DirectingEvent/Commands/CameraShake")]
    public class CameraShakeCommandSO : DirectingEvent
    {
        public CameraShakeOption cameraShakeOption;
        public override IEnumerator Execute()
        {
            yield return StageCameraShake(cameraShakeOption);
        }

        private IEnumerator StageCameraShake(CameraShakeOption cameraShakeOption)
        {
            Vector3 initPosition = DirectingEventManager.Instance.stageCamera.transform.position;

            float deltaTime = 0f;
            while (deltaTime < cameraShakeOption.duration)
            {
                float strength = Mathf.Lerp(cameraShakeOption.initStrength, cameraShakeOption.endStrength, deltaTime) * cameraShakeOption.shakeAnimationCurve.Evaluate(deltaTime);
                Vector3 randomOffset = Random.insideUnitSphere * strength;
                DirectingEventManager.Instance.stageCamera.transform.position = initPosition + randomOffset;

                deltaTime += Time.deltaTime;
                yield return null;
            }

            DirectingEventManager.Instance.stageCamera.transform.position = initPosition;

            yield break;
        }
    }
}