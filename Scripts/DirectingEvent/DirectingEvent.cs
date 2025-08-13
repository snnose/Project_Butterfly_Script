using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace DirectingEventSystem
{
   [System.Serializable]
   public abstract class DirectingEvent
    {
        protected GameObject speechBubble;

        public abstract IEnumerator Execute();
    }
}