using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using GoogleMobileAds.Api;
using Defective.JSON;

namespace AdSystem
{
    public class AdMobManager : MonoBehaviour
    {
        // Singleton
        private static AdMobManager instance;
        public static AdMobManager Instance
        {
            get
            {
                if (null == instance)
                    return null;

                return instance;
            }
        }

        //"ca-app-pub-3940256099942544/5224354917"; // 구글 공식 테스트 보상형 광고 Id
        [SerializeField]
        private string appId = "ca-app-pub-4539098097982391~8628198975";
        [SerializeField]
        private string rewardAdId;

        private AdPlacementDetails adPlacementDetails;

        RewardedAd rewardedAd;  // 보상형 광고

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this.gameObject);

            // 애드몹 초기화
            MobileAds.Initialize(InitializationStatus =>
            {
                Utils.Utility.DebugLog("AdMob Initialized!");
            });

#if UNITY_EDITOR
            {
                rewardAdId = "ca-app-pub-3940256099942544/5224354917";
            }
#else
            {
                rewardAdId = "ca-app-pub-4539098097982391/3810561531";
            }
#endif
        }

        public void LoadRewardedAd(string placementId)
        {
            AdRequest adRequest = new AdRequest();

            SetAdPlacementDetails(placementId);

            RewardedAd.Load(rewardAdId, adRequest, (RewardedAd ad, LoadAdError error) =>
            {
                {
                    if (ad == null || error != null)
                    {
                        Debug.LogError("광고 로드 실패!" + error.GetMessage());
                        return;
                    }
                }

                rewardedAd = ad;

                rewardedAd.OnAdFullScreenContentClosed += () => LoadRewardedAd(placementId);
                rewardedAd.OnAdFullScreenContentClosed += () => ReportAdActivity(AdActivity.Closed);
                rewardedAd.OnAdFullScreenContentClosed += () => Game.instance.SetMainCameraActive(true);
            });
        }

        public void ShowRewardedAd()
        {
            if (rewardedAd != null && rewardedAd.CanShowAd())
            {
                Utils.Utility.DebugLog("광고 출력!");

                // FIXME: 아래 둘은 나중에 기능 분리 해야함
                // Opened는 광고 시청 UI가 출력될 때, Start는 광고 시청 시작 시로
                ReportAdActivity(AdActivity.Opened);
                ReportAdActivity(AdActivity.Start);

                rewardedAd.Show(HandleUserRewardedEarned);

                // 메인 카메라 제어
                Game.instance.SetMainCameraActive(false);
            }
        }

        private void HandleUserRewardedEarned(Reward reward)
        {
            Debug.Log($"[광고 보상] Type: {reward.Type}, Amount: {reward.Amount}");

            // PlayFab 서버에 로그
            ReportAdActivity(AdActivity.End);
            // 이하 보상 처리 구문 작성 필요

            GiveReward(reward);

        }

        private void GiveReward(Reward reward)
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "GrantRandomExpReward",
                FunctionParameter = new { playFabId = PlayFabUserData.GetPlayFabId(), rewardType = reward.Type, amount = reward.Amount },
                GeneratePlayStreamEvent = true,
            };

            PlayFabClientAPI.ExecuteCloudScript(request,
                result =>
                {
                    JSONObject funcResult = new JSONObject(result.FunctionResult.ToString());
                    ProcessAdRewards(funcResult);
                    Debug.Log("GrantRandomExpReward - CloudScript 호출 성공! ");
                },
                error =>
                {
                    Debug.LogError("CloudScript 호출 실패!: " + error.GenerateErrorReport());
                });
        }

        private void ProcessAdRewards(JSONObject result)
        {
            string rewardKey = result["rewardKey"].ToString().Trim('"');
            int rewardAmount = result["rewardAmount"].intValue;

            Debug.Log($"rewardKey : " + rewardKey);

            if (rewardKey == "experience")
            {
                UIManager.Instance.SetNavigatorRewards(rewardAmount, 0);
            }
            else if (rewardKey == "itemexperience")
            {
                UIManager.Instance.SetNavigatorRewards(0, rewardAmount);
            }
        }

        private void ReportAdActivity(AdActivity activity)
        {
            var request = new ReportAdActivityRequest
            {
                PlacementId = adPlacementDetails.PlacementId,
                RewardId = adPlacementDetails.RewardId,
                Activity = activity,
            };

            PlayFabClientAPI.ReportAdActivity(request,
                result =>
                {
                    Debug.Log($"Ad activity reported: {activity}");
                },
                error =>
                {
                    Debug.LogError($"ReportAdActivity error: {error.GenerateErrorReport()}");
                });
        }

        /*
        public void SetCurrentRewardId(string rewardId)
        {
            currentRewardId = rewardId;
        }

        public string GetCurrentRewardId(string rewardId)
        {
            return currentRewardId;
        }
        */
        public void SetAdPlacementDetails(string placementId)
        {
            var request = new GetAdPlacementsRequest
            {
                AppId = appId,
                Identifier = new NameIdentifier { Id = placementId }
            };

            PlayFabClientAPI.GetAdPlacements(request,
                result =>
                {
                    adPlacementDetails = result.AdPlacements[0];
                    Debug.Log($"Set AdPlacement, PlacementId : {placementId}");
                    Debug.Log($"result.AdPlacements.Count : {result.AdPlacements.Count}");
                },
                error =>
                {
                    Debug.LogError("GetAdPlacements 실패: " + error.GenerateErrorReport());
                });
        }

        public AdPlacementDetails GetAdPlacementDetails()
        {
            return adPlacementDetails;
        }
    }
}
