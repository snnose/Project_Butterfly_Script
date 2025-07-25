using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace DirectingEventSystem
{
    [CreateAssetMenu(menuName = "DirectingEvent/Commands/ObjectScaleChange")]
    public class ObjectScaleChangeCommandSO : DirectingEvent
    {
        ObjectScaleChangeOption objectScaleChangeOption;

        public override IEnumerator Execute()
        {
            yield return ChangeObjectScale(objectScaleChangeOption);
        }

        private IEnumerator ChangeObjectScale(List<int> partialIndex)
        {
            yield break;
        }

        private IEnumerator ChangeObjectScale(ObjectScaleChangeOption objectScaleChangeOption)
        {
            int count = objectScaleChangeOption.targetObjects.Count;
            float delay = -1f;

            for (int i = 0; i < count; i++)
            {
                DirectingEventObject eventObject = objectScaleChangeOption.targetObjects[i];

                if (delay < objectScaleChangeOption.duration)
                {
                    delay = objectScaleChangeOption.duration;
                }

                if (!string.IsNullOrEmpty(objectScaleChangeOption.effectKey))
                {
                    EffectManager.Instance.ActivateEffect("Effect", objectScaleChangeOption.effectKey, eventObject.transform.position);
                }

                if (!string.IsNullOrEmpty(objectScaleChangeOption.animationName))
                {
                    eventObject.SetAnimation(objectScaleChangeOption.animationName, false);
                }

                eventObject.transform.DOScale(objectScaleChangeOption.scale, objectScaleChangeOption.duration).SetEase(objectScaleChangeOption.ease)
                    .OnComplete(() =>
                    {
                        eventObject.SetAnimation("idle", true);
                    });
            }

            if (objectScaleChangeOption.isWaitForCompletion)
            {
                yield return new WaitForSeconds(delay);
            }
        }
    }
}