using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace DirectingEventSystem
{
    public class DirectingObjectInstantiate : DirectingEvent
    {
        ObjectInstantiateOption objectInstantiateOption;

        public override IEnumerator Execute()
        {
            yield return InstantiateObject(objectInstantiateOption);
        }

        private IEnumerator InstantiateObject(ObjectInstantiateOption instantiateOption)
        {
            // duration 시간 동안 생성돼야하는데, ObjectList 개수 만큼 duration을 쪼개서 일정 시간마다 생성되게 할 지
            // 아니면 각 오브젝트를 생성하면서 DOFade로 투명도를 조절해서 생성되게 할 지 고민
            // => 각 오브젝트가 생성되는데 완전히 걸리는 시간이라 함. Fade In 효과가 어울릴 거 같음
            GameObject eventObjectPrefab;
            if (instantiateOption.gameObject == null)
            {
                eventObjectPrefab = Resources.Load<GameObject>(instantiateOption.path);
            }
            else
            {
                eventObjectPrefab = instantiateOption.gameObject;
            }
            // FIXME: parent를 stage로 설정해야함
            GameObject eventObject = eventObjectPrefab.GetComponent<DirectingEventObject>().InstantiateObject(instantiateOption.spawnPosition, instantiateOption.spawnQuaternion);

            eventObject.transform.DOScale(instantiateOption.scale, instantiateOption.duration);

            if (!string.IsNullOrEmpty(instantiateOption.effectKey))
            {
                EffectManager.Instance.ActivateEffect("Effect", instantiateOption.effectKey, instantiateOption.spawnPosition);
            }

            if (!string.IsNullOrEmpty(instantiateOption.animationName))
            {
                eventObject.GetComponent<DirectingEventObject>().SetAnimation(instantiateOption.animationName, true);
            }

            if (instantiateOption.isWaitForCompletion)
            {
                yield return new WaitForSeconds(instantiateOption.duration);
            }
        }
    }
}