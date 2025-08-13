using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace DirectingEventSystem
{
    public class DirectingCameraMove : DirectingEvent
    {
        [SerializeField] 
        public CameraMoveEventData dataTemplate;
        public CameraMoveOption optionData;

        public override IEnumerator Execute()
        {
            yield return MoveCamera(optionData);
        }

        private IEnumerator MoveCamera(CameraMoveOption cameraMoveOption)
        {
            DirectingEventManager.Instance.SetUIActive(false);

            Vector3 offset = DirectingEventManager.Instance.cameraOffset;


            if (cameraMoveOption.initObject == null)
            {

            }
            else
            {
                DirectingEventManager.Instance.stageCamera.transform
                    .DOMove(cameraMoveOption.initObject.transform.position + offset, 0f);
            }
            
            // 목표 Transform 설정 (null인 경우 캐릭터를 향한다)
            Transform targetTransform = cameraMoveOption.targetObject != null ? cameraMoveOption.targetObject.transform : targetTransform = GameObject.FindWithTag("Player").transform;

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