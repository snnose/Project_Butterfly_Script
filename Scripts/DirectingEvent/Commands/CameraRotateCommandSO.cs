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
        public CameraRotateOption cameraRotateOption;

        public override IEnumerator Execute()
        {
            yield return RotateCamera(cameraRotateOption);
        }

        private IEnumerator RotateCamera(CameraRotateOption cameraRotateOption)
        {
            DirectingEventManager.Instance.stageCamera.transform.DORotate(cameraRotateOption.endRotation, cameraRotateOption.duration).SetEase(cameraRotateOption.ease);

            if (cameraRotateOption.isWaitForCompletion)
            {
                yield return new WaitForSeconds(cameraRotateOption.duration);
            }
        }
    }
}