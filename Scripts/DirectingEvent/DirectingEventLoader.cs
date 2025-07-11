using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace DirectingEventSystem
{
    public class DirectingEventLoader : MonoBehaviour
    {
        [SerializeField] private const int maxProgress = 100;
        [SerializeField] private StageType stageType;

        private async void Awake()
        {
            await InitialLoad();

            DirectingEventManager.Instance.AddDirectingEventLoader(stageType, this);
        }

        private async Task InitialLoad()
        {
            (int main, int sub) progress = UserParameterData.GetUserProgress("user_progress");

            await LoadDirectingEvent(stageType, progress.main, progress.sub);
        }

        public async Task LoadDirectingEvent(StageType stageType, int mainProgress, int subProgress)
        {
            for (int i = subProgress; i < maxProgress; i++)
            {
                string address = $"{stageType}_directingeventzone_{mainProgress}_{i}";
                Debug.Log($"LoadDirectingEvent: {address}");

                // 1) 주소 존재 여부 확인
                var locationHandle = Addressables.LoadResourceLocationsAsync(address, typeof(GameObject));
                await locationHandle.Task;
                if (locationHandle.Status != AsyncOperationStatus.Succeeded || locationHandle.Result.Count == 0)
                {
                    Debug.Log($"Address '{address}' 없음, 로딩 종료");
                    Addressables.Release(locationHandle);
                    break;
                }
                Addressables.Release(locationHandle);

                // 2) 인스턴스 생성
                var directEvent = Addressables.InstantiateAsync(address, this.transform);
                await directEvent.Task;
                if (directEvent.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"Loaded & Instantiated: {address}");
                }
                else
                {
                    Debug.LogError($"Instantiate 실패: {address}");
                    Addressables.Release(directEvent);
                    break;
                }
            }
        }
    }
}