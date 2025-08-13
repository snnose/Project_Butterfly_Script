using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DirectingEventSystem
{
    public class DirectingSpeechBubbleFloat : DirectingEvent
    {
        public SpeechBubbleOption speechBubbleOption;

        public override IEnumerator Execute()
        {
            yield return FloatSpeechBubble(speechBubbleOption);
        }

        private IEnumerator FloatSpeechBubble(SpeechBubbleOption speechBubbleOption)
        {
            // Bubble을 Addressable로 불러와 생성하고 다 쓰면 파괴하는 식으로?? 고민되네
            GameObject speechBubble = GetSpeechBubble();
            SpeechBubbleBehaviour speechBubbleBehaviour;
            speechBubble.TryGetComponent(out speechBubbleBehaviour);

            speechBubbleBehaviour.SetTextOptionList(speechBubbleOption.textOptionList);

            SetCameraPivot(speechBubbleOption);
            // 카메라 위치 세팅 (DOMove로 이동시키는 게 좋을 거 같기도?)
            Transform cameraTargetTransform = GetCameraTargetTransform(speechBubbleOption);
            DirectingEventManager.Instance.stageCamera.transform.position =
                cameraTargetTransform.position + DirectingEventManager.Instance.cameraOffset;

            Transform speechTargetTransform = GetSpeechTargetTransform(speechBubbleOption);

            // 인바운드의 경우 해당 자리에 고정
            if (speechBubbleOption.isInBoundary)
            {
                speechBubbleBehaviour.SetBubblePosition(speechTargetTransform.position + speechBubbleOption.speechBubbleOffset);
            }
            // 아닌 경우 말하는 대상의 위치를 따라간다
            else
            {
                speechBubbleBehaviour.FollowSpeecher(speechTargetTransform.position + speechBubbleOption.speechBubbleOffset, speechBubbleOption.remainingDuration);
            }

            speechBubbleBehaviour.SetRenderer(speechBubbleOption.speechBubbleType);

            if (speechBubbleOption.isBubbleShake)
            {
                speechBubbleBehaviour.SetBubbleShake(speechBubbleOption.speechBubbleShake);
            }

            if (speechBubbleOption.isStringShake)
            {
                speechBubbleBehaviour.SetStringShake(speechBubbleOption.stringShake);
            }

            yield return speechBubbleBehaviour.FloatSpeechBubble(speechBubbleOption.stringKeys, speechBubbleOption.stringPrintType,
                                                                 speechBubbleOption.remainingDuration, speechBubbleOption.printSpeed,
                                                                 speechBubbleOption.isBubbleShake, speechBubbleOption.isStringShake);


            InitCameraPivot();
        }

        private GameObject GetSpeechBubble()
        {
            if (speechBubble == null)
            {
                SpeechBubbleBehaviour speechBubbleBehaviour = Resources.Load<GameObject>("SpeechBubble/SpeechBubble").GetComponent<SpeechBubbleBehaviour>();
                // FIXME: parent 설정 고민해야함
                speechBubble = speechBubbleBehaviour.InstantiateBubble();
            }
            else
            {
                speechBubble.SetActive(true);
            }

            return speechBubble;
        }

        private void InitCameraPivot()
        {
            //DirectingEventManager.Instance.stageCamera.usePhysicalProperties = false;
            DirectingEventManager.Instance.stageCamera.lensShift = Vector2.zero;
        }

        private void SetCameraPivot(SpeechBubbleOption speechBubbleOption)
        {
            Camera stageCamera = DirectingEventManager.Instance.stageCamera;
            stageCamera.usePhysicalProperties = true;
            stageCamera.lensShift = speechBubbleOption.lensShift;
        }

        private Transform GetCameraTargetTransform(SpeechBubbleOption speechBubbleOption)
        {
            Transform targetTransform = speechBubbleOption.cameraTarget.transform;

            if (speechBubbleOption.cameraTarget.tag == "Player"
                || speechBubbleOption.cameraTarget.tag == "Fairy")
            {
                GameObject cameraTarget = GameObject.FindWithTag(speechBubbleOption.cameraTarget.tag);
                targetTransform = cameraTarget.transform;
            }

            return targetTransform;
        }

        private Transform GetSpeechTargetTransform(SpeechBubbleOption speechBubbleOption)
        {
            Transform targetTransform = speechBubbleOption.speechTarget.transform;

            if (speechBubbleOption.cameraTarget.tag == "Player"
                || speechBubbleOption.cameraTarget.tag == "Fairy")
            {
                GameObject speechTarget = GameObject.FindWithTag(speechBubbleOption.speechTarget.tag);
                targetTransform = speechTarget.transform;
            }

            return targetTransform;
        }
    }
}