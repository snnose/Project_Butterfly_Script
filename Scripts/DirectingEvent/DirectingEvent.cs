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

        protected GameObject speechBubble;
    }
}