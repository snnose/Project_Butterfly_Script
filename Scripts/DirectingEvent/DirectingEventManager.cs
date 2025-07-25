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

        public Dictionary<(int main, int sub), DirectingEventZone> eventZoneDictionary = new Dictionary<(int main, int sub), DirectingEventZone>();
        public Dictionary<StageType, DirectingEventLoader> loaderDictionary = new Dictionary<StageType, DirectingEventLoader>();


        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this.gameObject);

            // StageCamera 설정
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

        public void ExecuteDirectingEvents(int mainProgress, int subProgress)
        {
            if (!eventZoneDictionary.ContainsKey((mainProgress, subProgress)))
                return;

            isDirecting = true;

            eventZoneDictionary[(mainProgress, subProgress)].EnqueueEvents();
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
            SetUIActive(true);
        }

        public async void LoadDirectingEventZone(StageType stageType, int mainProgress, int subProgress)
        {
            DirectingEventLoader directingEventLoader = loaderDictionary[stageType];

            await directingEventLoader.LoadDirectingEvent(stageType, mainProgress, subProgress);
        }

        public void AddDirectingEventLoader(StageType stageType, DirectingEventLoader directingEventLoader)
        {
            loaderDictionary[stageType] = directingEventLoader;
        }

        private void ClearDirectingEventQueue()
        {
            directingEventQueue.Clear();
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
    }

    /// <summary>
    /// 섬 타입. FIXME: 추후 섬 이름에 대응하여 바꿔주어야 함.
    /// </summary>
    public enum StageType
    {
        None,
        First,
        Second,
        Third,
    }
}