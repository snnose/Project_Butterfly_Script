using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DirectingEventSystem
{
    public class DirectingObjectDestroy : DirectingEvent
    {
        public ObjectDestroyOption objectDestroyOption;

        public override IEnumerator Execute()
        {
            yield return DestroyObject(objectDestroyOption);
        }

        private IEnumerator DestroyObject(ObjectDestroyOption objectDestroyOption)
        {
            // 애니메이션 모두 재생 -> Object FadeOut -> Destroy 하는 순서로?

            int count = objectDestroyOption.targetObjects.Count;
            float delay = -1f;

            for (int i = 0; i < count; i++)
            {
                DirectingEventObject eventObject = objectDestroyOption.targetObjects[i];

                string effectKey = objectDestroyOption.effectKey;
                string animationName = objectDestroyOption.animationName;

                if (!string.IsNullOrEmpty(effectKey))
                {
                    EffectManager.Instance.ActivateEffect("Effect", effectKey, eventObject.transform.position);
                }

                if (!string.IsNullOrEmpty(animationName))
                {
                    if (delay < eventObject.GetAnimationDuration(animationName))
                    {
                        delay = eventObject.GetAnimationDuration(animationName);
                    }

                    if (delay < 0f)
                    {
                        Debug.LogError($"{eventObject.name}에 {animationName} 애니메이션이 없습니다!");
                        yield break;
                    }

                    eventObject.SetAnimation(animationName, false);
                }
            }

            if (objectDestroyOption.isWaitForCompletion)
            {
                yield return new WaitForSeconds(delay);
            }
            //yield return fadeOutObject(eventObjectList, 1f);

            for (int i = 0; i < count; i++)
            {
                objectDestroyOption.targetObjects[i].DestroyObject();
                objectDestroyOption.targetObjects[i] = null;
            }

            yield break;
        }
    }
}