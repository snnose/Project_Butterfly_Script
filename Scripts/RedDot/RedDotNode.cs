using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RedDot
{
    public class RedDotNode : MonoBehaviour
    {
        public List<string> Subscribe;

        public string itemType;

        public GameObject redDot;  

        private void Awake()
        {
            RedDotSystem.OnRedDotChange += OnRedDotChange;
        }

        private void OnEnable()
        {
            RefreshRedDot();
        }

        private void OnDisable()
        {
            
        }

        bool IsContainItem(string eRedDotItem)
        {
            return Subscribe.Contains(eRedDotItem);
        }

        private int GetTotalRedDotCount()
        {
            int totalCount = 0;

            for (int i = 0; i < Subscribe.Count; i++)
            {
                totalCount += RedDotSystem.GetRedDotCount(Subscribe[i]);
            }

            //Debug.Log("itemType : " + itemType + " / totalCount : " + totalCount);

            return totalCount;
        }

        public void RefreshRedDot()
        {
            int totalRedDotCount = GetTotalRedDotCount();

            // itemType이 null이 아니면 진입 (itemType이 null이면 잎 노드)
            if (this.itemType != null)
            {
                // totalRedDotCount 개수로 현재 노드를 활성화 여부를 판단 (Count가 0이면 변화 없음)
                RedDotSystem.SetRedDot(this.itemType, totalRedDotCount);
            }

            //Debug.Log("RefreshRedDot()");

            if (totalRedDotCount > 0)
            {
                redDot.SetActive(true);
            }
            else
            {
                redDot.SetActive(false);
            }
        }

        private void OnRedDotChange(string eRedDotItem)
        {
            if (IsContainItem(eRedDotItem))
            {
                RefreshRedDot();
            }
        }
    }
}
