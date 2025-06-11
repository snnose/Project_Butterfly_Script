using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace DirectingEventSystem
{
    [CreateAssetMenu(menuName = "DirectingEvent/Commands/CameraShake")]
    public class CameraShakeCommandSO : DirectingEvent
    {
        public override IEnumerator Execute()
        {
            yield return StageCameraShake(cameraShakeOptions);
        }

        private IEnumerator StageCameraShake(List<CameraShakeOption> cameraShakeOptions)
        {
            Vector3 initPosition = DirectingEventManager.Instance.stageCamera.transform.position;

            int index = DirectingEventManager.Instance.GetOptionIndex()[(int)OptionType.CameraShake];
            CameraShakeOption cameraShakeOption = cameraShakeOptions[index];
            DirectingEventManager.Instance.SetOptionIndexValue((int)OptionType.CameraShake, ++index);

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