using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DirectingEventSystem
{
    public class DirectingEventZone : MonoBehaviour
    {
        [Header("Path")]
        [Tooltip("Scriptable Object가 저장되는 위치입니다.")]
        public DefaultAsset targetFolder;
        public bool isActivated = false;

        [Header("Proper Progress")]
        [SerializeField] private int mainProgress;
        [SerializeField] private int subProgress;

        [Header("Collider")]
        public BoxCollider zoneCollider;
        [SerializeField]
        private Vector3 zoneSize;

        [Header("Directing Events")]
        [SerializeReference]
        public List<DirectingEvent> directingEventList;

        private void OnValidate()
        {
            if (zoneCollider != null)
            {
                zoneCollider.size = zoneSize;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // properProgress가 현재 유저의 progress와 같고
            // 해당 연출 이벤트를 본 적이 없으면 연출
            if ((mainProgress, subProgress) == UserParameterData.GetUserProgress("user_progress")
                && !isActivated 
                && other.TryGetComponent(out PlayerController playerController))
            {
                Debug.Log("EventZone 입장!");
                isActivated = true;
                UserParameterData.IncreaseUserProgressData(false, true);
                EnqueueEvents();
                DirectingEventManager.Instance.ExecuteDirectingEvents();
            }
        }

        public void EnqueueEvents()
        {
            int count = directingEventList.Count;
            for (int i = 0; i < count; i++)
            {
                DirectingEventManager.Instance.EnqueueEvent(directingEventList[i].Execute());
            }
        }

        /// <summary>
        /// Scriptable Object가 생성되는 경로를 반환합니다.
        /// </summary>
        public string GetPath()
        {
            return AssetDatabase.GetAssetPath(targetFolder);
        }
    }
}