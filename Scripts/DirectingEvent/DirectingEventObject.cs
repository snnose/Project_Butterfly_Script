using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;

namespace DirectingEventSystem
{
    public class DirectingEventObject : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public SkeletonAnimation skeletonAnimation;

        public void SetAnimation(string animationName, bool isLoop)
        {
            if (skeletonAnimation == null)
            {
                Debug.LogError($"{this.name}에 SkeletonAnimation이 존재하지 않습니다!");
                return;
            }

            skeletonAnimation.AnimationState.SetAnimation(0, animationName, isLoop);
        }

        public float GetAnimationDuration(string animationName)
        {
            if (skeletonAnimation == null)
            {
                Debug.LogError($"{this.name}에 SkeletonAnimation이 존재하지 않습니다!");
                return -1f;
            }

            Spine.Animation animation = skeletonAnimation.Skeleton.Data.FindAnimation(animationName);

            if (animation != null)
            {
                return animation.Duration;
            }

            return -1f;
        }

        public Component GetRenderer()
        {
            if (spriteRenderer != null)
                return spriteRenderer;

            if (skeletonAnimation != null)
                return skeletonAnimation;

            Debug.LogError($"{this.name}에 어떠한 Renderer도 부착되지 않았습니다!");
            return null;
        }
    }
}