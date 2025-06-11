using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using EffectSystem;

namespace DirectingEventSystem
{
    public class DirectingEventManager : MonoBehaviour
    {
        private static DirectingEventManager instance;
        public static DirectingEventManager Instance
        {
            get
            {
                if (null == instance)
                    return null;

                return instance;
            }
        }

        public Camera stageCamera;
        public Vector3 cameraOffset;
        public bool isDirecting = false;
        [SerializeField]
        private bool isFloatingDialogue = false;

        private List<int> optionIndex = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this.gameObject);
        }

        private Queue<IEnumerator> directingEventQueue = new Queue<IEnumerator>();

        public void EnqueueEvent(IEnumerator directingEvent)
        {
            directingEventQueue.Enqueue(directingEvent);
        }

        public void ExecuteDirectingEvents()
        {
            isDirecting = true;
            StartCoroutine(executeDirectingEvents());
        }

        private IEnumerator executeDirectingEvents()
        {
            SetUIActive(false);

            while (directingEventQueue.Count > 0)
            {
                yield return StartCoroutine(directingEventQueue.Dequeue());

                if (isFloatingDialogue)
                {
                    yield return new WaitUntil(() => !isFloatingDialogue);
                }
            }

            // 이하 코드들은 초기화 구문으로 묶어서 관리 필요
            isDirecting = false;
            ResetOptionIndex();
            SetUIActive(true);
        }

        private void ClearDirectingEventQueue()
        {
            directingEventQueue.Clear();
        }

        private void ResetOptionIndex()
        {
            int count = optionIndex.Count;

            for (int i = 0; i < count; i++)
            {
                optionIndex[i] = 0;
            }
        }

        public void SetOptionIndexValue(int index, int value)
        {
            optionIndex[index] = value;
        }

        public List<int> GetOptionIndex()
        {
            return this.optionIndex;
        }

        public void SetIsFloatingDialogue(bool isFloating)
        {
            isFloatingDialogue = isFloating;
        }

        public void SetUIActive(bool isActive)
        {
            UIManager.Instance.stageUIManager.SetJoyStickActive(isActive);
            UIManager.Instance.SetNavigatorUIActive(isActive);
            UIManager.Instance.SetStageUIActive(isActive);
        }

        private IEnumerator fadeOutObject(List<DirectingEventObject> eventObjectList, float duration)
        {
            int count = eventObjectList.Count;

            for (int i = 0; i < count; i++)
            {
                DirectingEventObject eventObject = eventObjectList[i];
                Component renderer = eventObject.GetRenderer();

                if (renderer is SpriteRenderer spriteRenderer)
                {
                    spriteRenderer.DOFade(0f, duration);
                }
                else if (renderer is Spine.Unity.SkeletonAnimation skeletonAnimation)
                {
                    DOTween.To(() => skeletonAnimation.skeleton.A, x => skeletonAnimation.skeleton.A = x, 0f, duration);
                }
            }

            yield return new WaitForSeconds(duration);

            yield break;
        }
    }
}