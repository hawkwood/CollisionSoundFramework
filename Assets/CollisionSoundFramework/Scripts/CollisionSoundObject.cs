using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace CollisionSoundFramework
{
    public class CollisionSoundObject : MonoBehaviour
    {
        private static Dictionary<Collider, CollisionSoundObject> SoundObjects = new Dictionary<Collider, CollisionSoundObject>();

        [StringyDropdown.StringyDropdown(FileName = "MaterialStrings")]
        public string Material;

        [SerializeField]
        private bool _BlockPlayback = false;
        public bool BlockPlayback
        {
            get
            {
                return _BlockPlayback;
            }
            set
            {
                _BlockPlayback = value;
            }
        }

        [Space(5)]

        public bool OverridePitchModulationSettings = false;
        public bool PitchModulationEnabled = true;

        [Range(0f, 2f)]
        public float PitchModulationExtent = 0.5f;

        [Space(5)]

        public bool OverrideMaxCollisionVelocity = false;
        public float MaxCollisionVelocity = 5;

        public UnityEvent OnDidPlay;

        private Collider[] Colliders;

        protected virtual void Awake()
        {
            Colliders = this.GetComponentsInChildren<Collider>(true);

            for (int index = 0; index < Colliders.Length; index++)
            {
                SoundObjects[Colliders[index]] = this;
            }
        }

        protected virtual void OnDestroy()
        {
            Colliders = this.GetComponentsInChildren<Collider>(true);

            for (int index = 0; index < Colliders.Length; index++)
            {
                SoundObjects.Remove(Colliders[index]);
            }
        }

        public void DidPlaySound()
        {
            OnDidPlay?.Invoke();
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            Collider collider = collision.collider;
            if (SoundObjects.ContainsKey(collider))
            {
                CollisionSoundObject collisionSoundObject = SoundObjects[collider];

                float volume = CalculateImpactVolume(collision);
                if (volume < CollisionSoundController.Instance.MinCollisionVolume)
                {
                    //Debug.Log("Volume too low to play: " + Volume);
                    return;
                }

                CollisionSoundController.Play(this, collisionSoundObject, collision.contacts[0].point, volume);
            }
        }

        // TODO: track collision points to fire additional sounds when objects move, or attenuate sound if collision persists

        private float CalculateImpactVolume(Collision collision)
        {
            float Volume;
            float vel = collision.relativeVelocity.magnitude;
            float normVel = NormalizedValue(vel, 0, OverrideMaxCollisionVelocity ? MaxCollisionVelocity : CollisionSoundController.Instance.MaxCollisionVelocity);
            Volume = EaseOutCubic(normVel);
            if (normVel > 0)
            {
                // Debug.Log("Velocity: " + vel + " -> " + normVel + " -> " + Volume);
            }

            return Volume;
        }

        public static float NormalizedValue(float value, float min = 0, float max = 1)
        {
            float retVal = Mathf.Clamp(value, min, max);
            return (retVal / max);
        }

        public static float EaseOutCubic(float value) // for values 0 to 1
        {
            return 1 - Mathf.Pow(1 - value, 3);
        }

        public static float EaseInCubic(float value) // for values 0 to 1
        {
            return value * value * value;
        }
    }
}