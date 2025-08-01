﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace DirectingEventSystem
{
    [CreateAssetMenu(menuName = "DirectingEvent/Commands/CameraMove")]
    public class CameraMoveCommandSO : DirectingEvent
    {
        public CameraMoveOption cameraMoveOption;

        public override IEnumerator Execute()
        {
            yield return MoveCamera(cameraMoveOption);
        }

        private IEnumerator MoveCamera(CameraMoveOption cameraMoveOption)
        {
            // 카메라 이동 시에는 무조건 UI를 꺼주는 거로 하는 것이 어떨까
            DirectingEventManager.Instance.SetUIActive(false);

            Vector3 offset = DirectingEventManager.Instance.cameraOffset;

            Transform targetTransform = cameraMoveOption.targetTransform;

            if (cameraMoveOption.initTransform == null)
            {

            }
            else
            {
                DirectingEventManager.Instance.stageCamera.transform
                    .DOMove(cameraMoveOption.initTransform.position + offset, 0f);
            }

            if (cameraMoveOption.targetTransform == null)
            {
                targetTransform = GameObject.FindWithTag("Player").transform;
            }

            if (cameraMoveOption.hideJoyStick)
            {
                //UIManager.Instance.stageUIManager.SetJoyStickActive(false);
            }

            if (cameraMoveOption.hideDialogue)
            {

            }

            DirectingEventManager.Instance.stageCamera.transform
                                .DOMove(targetTransform.position + offset, cameraMoveOption.duration).SetEase(cameraMoveOption.ease);

            if (cameraMoveOption.isWaitForCompletion)
            {
                yield return new WaitForSeconds(cameraMoveOption.duration);
            }
        }
    }
}