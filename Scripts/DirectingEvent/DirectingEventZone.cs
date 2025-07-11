using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DirectingEventSystem
{
    public class DirectingEventZone : MonoBehaviour
    {
        public bool isActivated = false;

        [Header("Proper Progress")]
        [SerializeField] private (int main, int sub) properProgress;

        [Header("Collider")]
        public BoxCollider zoneCollider;
        [SerializeField]
        private Vector3 zoneSize;

        [Header("Move Option")]
        public List<ObjectMoveOption> moveOptionList;
        [Header("Instansiate Option")]
        public List<ObjectInstantiateOption> instantiateOptionList;
        [Header("Destroy Option")]
        public List<ObjectDestroyOption> destroyOptionList;
        [Header("Scale Change Option")]
        public List<ObjectScaleChangeOption> scaleChangeOptionList;
        [Header("Camera Move Option")]
        public List<CameraMoveOption> cameraMoveOptionList;
        [Header("Camera Rotate Option")]
        public List<CameraRotateOption> cameraRotateOptionList;
        [Header("Camera Shake Option")]
        public List<CameraShakeOption> cameraShakeOptionList;
        [Header("Dialogue Option")]
        public List<DialogueOption> dialogueOptionList;
        [Header("SpeechBubble Option")]
        public List<SpeechBubbleOption> speechBubbleOptionList;
        [Header("FadeOut-In Option")]
        public List<FadeOutInOption> fadeOutInOptionList;
        [Header("Directing Events")]
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
            if (properProgress == UserParameterData.GetUserProgress("user_progress")
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
                SetEventOptions(directingEventList[i]);
                DirectingEventManager.Instance.EnqueueEvent(directingEventList[i].Execute());
            }
        }

        private void SetEventOptions(DirectingEvent directingEvent)
        {
            directingEvent.objectMoveOptions = moveOptionList;
            directingEvent.objectInstantiateOptions = instantiateOptionList;
            directingEvent.objectDestroyOptions = destroyOptionList;
            directingEvent.objectScaleChangeOptions = scaleChangeOptionList;

            directingEvent.cameraMoveOptions = cameraMoveOptionList;
            directingEvent.cameraRotateOptions = cameraRotateOptionList;
            directingEvent.cameraShakeOptions = cameraShakeOptionList;

            directingEvent.dialogueOptions = dialogueOptionList;

            directingEvent.speechBubbleOptions = speechBubbleOptionList;

            directingEvent.fadeOutInOptions = fadeOutInOptionList;
        }
    }
}