using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace DirectingEventSystem
{
   public abstract class DirectingEvent : ScriptableObject
    {
        public abstract IEnumerator Execute();

        public List<ObjectMoveOption> objectMoveOptions { get; set; }
        public List<ObjectInstantiateOption> objectInstantiateOptions { get; set; }
        public List<ObjectDestroyOption> objectDestroyOptions { get; set; }
        public List<ObjectScaleChangeOption> objectScaleChangeOptions { get; set; }

        public List<CameraMoveOption> cameraMoveOptions { get; set; }
        public List<CameraRotateOption> cameraRotateOptions { get; set; }
        public List<CameraShakeOption> cameraShakeOptions { get; set; }

        public List<DialogueOption> dialogueOptions { get; set; }
        public List<SpeechBubbleOption> speechBubbleOptions { get; set; }
        public List<FadeOutInOption> fadeOutInOptions { get; set; }

        protected GameObject speechBubble;

        protected int GetOptionIndex(OptionType optionType)
        {
            int index = DirectingEventManager.Instance.GetOptionIndex()[(int)optionType];
            return index;
        }

        protected void IncreaseOptionIndex(OptionType optionType)
        {
            int index = GetOptionIndex(optionType);
            DirectingEventManager.Instance.SetOptionIndexValue((int)optionType, ++index);
        }
    }
}