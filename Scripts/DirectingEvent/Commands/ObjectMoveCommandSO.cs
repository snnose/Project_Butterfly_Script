using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace DirectingEventSystem
{
    [CreateAssetMenu(menuName = "DirectingEvent/Commands/ObjectMove")]
    public class ObjectMoveCommandSO : DirectingEvent
    {
        public override IEnumerator Execute()
        {
            yield return MoveObject(objectMoveOptions);
        }

        private IEnumerator MoveObject(List<ObjectMoveOption> objectMoveOptions)
        {
            int index = GetOptionIndex(OptionType.ObjectMove);
            ObjectMoveOption objectMoveOption = objectMoveOptions[index];
            IncreaseOptionIndex(OptionType.ObjectMove);

            int count = objectMoveOption.targetObjects.Count;
            float delay = 0f;

            for (int i = 0; i < count; i++)
            {
                DirectingEventObject eventObject = objectMoveOption.targetObjects[i];
                Transform eventObjectTransform = eventObject.transform;

                Vector3 endPosition;

                if (objectMoveOption.endTransform != null)
                {
                    endPosition = objectMoveOption.endTransform.position;
                }
                else
                {
                    endPosition = objectMoveOption.endPosition;
                }

                if (delay < objectMoveOption.duration)
                {
                    delay = objectMoveOption.duration;
                }

                if (!string.IsNullOrEmpty(objectMoveOption.effectKey))
                {
                    EffectManager.Instance.ActivateDOMoveEffect("Effect", objectMoveOption.effectKey, eventObjectTransform.position, endPosition);
                }

                if (!string.IsNullOrEmpty(objectMoveOption.animationName))
                {
                    eventObject.SetAnimation(objectMoveOption.animationName, true);
                }

                if (objectMoveOption.isCameraFollow)
                {
                    DirectingEventManager.Instance.stageCamera.transform.DOMove(endPosition + DirectingEventManager.Instance.cameraOffset, objectMoveOption.duration).SetEase(objectMoveOption.ease);
                }

                eventObjectTransform.DOMove(endPosition, objectMoveOption.duration).SetEase(objectMoveOption.ease)
                    .OnComplete(() =>
                    {
                        eventObject.SetAnimation("idle", true);
                    });
            }

            if (objectMoveOption.isWaitForCompletion)
            {
                yield return new WaitForSeconds(delay);
            }
        }
    }
}