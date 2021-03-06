using UnityEngine;
using System.Collections;

namespace CollisionSoundFramework
{
    public abstract class CollisionSoundProvider : MonoBehaviour
    {
        public abstract void Setup();
        public abstract void Play(CollisionSoundObject soundObjectA, CollisionSoundObject soundObjectB, Vector3 position, float impactVolume);
    }
}