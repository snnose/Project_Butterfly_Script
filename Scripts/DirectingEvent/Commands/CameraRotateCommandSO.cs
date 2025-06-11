using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DirectingEventSystem;
using DG.Tweening;

namespace DirectingEventSystem
{
    [CreateAssetMenu(menuName = "DirectingEvent/Commands/CameraRotate")]
    public class CameraRotateCommandSO : DirectingEvent
    {
        public override IEnumerator Execute()
        {
            yield return RotateCamera(cameraRotateOptions);
        }

        private IEnumerator RotateCamera(List<CameraRotateOption> cameraRotateOptions)
        {
            int index = GetOptionIndex(OptionType.CameraRotate);
            CameraRotateOption cameraRotateOption = cameraRotateOptions[index];
            IncreaseOptionIndex(OptionType.CameraRotate);

            DirectingEventManager.Instance.stageCamera.transform.DORotate(cameraRotateOption.endRotation, cameraRotateOption.duration).SetEase(cameraRotateOption.ease);

            if (cameraRotateOption.isWaitForCompletion)
            {
                yield return new WaitForSeconds(cameraRotateOption.duration);
            }
        }
    }
}