using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AdSystem
{
    public class AdColliderBehaviour : MonoBehaviour
    {
        [SerializeField]
        private string placementId = "4744EA287BDB564C";    // PlayFab 테스트 PlacementId

        private void OnTriggerEnter(Collider other)
        {
            // 접촉하는 충돌체가 Player일 때만 작동한다
            // FIXME : 만약 캐릭터와 애벌레의 조작 스위치를 구현한다면 수정 필요
            if (other.TryGetComponent<PlayerController>(out PlayerController playerController))
            {
                Utils.Utility.DebugLog("광고 큐브에 접근");
                AdMobManager.Instance.LoadRewardedAd(placementId);
                UIManager.Instance.ShowAdChatBoxUI();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // 접촉하는 충돌체가 Player일 때만 작동한다
            // FIXME : 만약 캐릭터와 애벌레의 조작 스위치를 구현한다면 수정 필요
            if (other.TryGetComponent<PlayerController>(out PlayerController playerController))
            {
                UIManager.Instance.HideAdChatBoxUI();
            }
        }
    }
}