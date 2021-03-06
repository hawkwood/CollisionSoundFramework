using UnityEngine;
using System.Collections.Generic;
using StringyDropdown;

namespace CollisionSoundFramework
{
    public class CollisionSoundController : MonoBehaviour
    {
        public static CollisionSoundController Instance { get; private set; }

        [Tooltip("The max number of sounds that can possibly be playing at once.")]
        public int SoundPoolSize = 100;

        [Tooltip("Turns on or off randomizing the pitch of the collision sounds")]
        public bool PitchModulationEnabled = true;

        [Range(0f, 2f)]
        public float PitchModulationExtent = 0.5f;

        [Tooltip("Don't play collision sounds that will produce an impact with a volume lower than this number")]
        public float MinCollisionVolume = 0.1f;
        public float MaxCollisionVelocity = 5;

        [SerializeField]
        private StringyDropdownStringsModel _MaterialsList;
        public string[] Materials
        {
            get
            {
                return _MaterialsList.strings;
            }
        }

        [HideInInspector]
        public CollisionSoundProviderType SoundEngine = CollisionSoundProviderType.UnityWithList;

        [SerializeField]
        private CollisionSoundProvider _Provider;
        public CollisionSoundProvider Provider
        {
            get
            {
                if (_Provider == null)
                {
                    _Provider = GetComponent<CollisionSoundProvider>();
                    if (_Provider == null)
                    {
#if CSF_FMOD
                        SoundEngine = CollisionSoundProviderType.FMOD;
                        _Provider = this.gameObject.AddComponent<CollisionSoundProviderFMOD>();
#else
                        SoundEngine = CollisionSoundProviderType.Unity;
                        _Provider = this.gameObject.AddComponent<CollisionSoundProviderUnity>();
#endif
                    }
                }
                return _Provider;
            }
        }

        struct CollisionSoundSet
        {
            public string material0;
            public string material1;
            public Vector3 position;
            public float impactVolume;
        }

        private List<CollisionSoundSet> soundSetsThisFrame = new List<CollisionSoundSet>();

        private void Awake()
        {
            Instance = this;
            Provider.Setup();
        }

        public static void Play(CollisionSoundObject soundObjectA, CollisionSoundObject soundObjectB, Vector3 position, float impactVolume)
        {
            if (Instance.Provider == null) return;

            if (string.CompareOrdinal(soundObjectA.Material, soundObjectB.Material) > 0)
            {
                CollisionSoundObject t = soundObjectA;
                soundObjectA = soundObjectB;
                soundObjectB = t;
            }

            if (soundObjectA.BlockPlayback || soundObjectB.BlockPlayback)
            {
                // Debug.Log("BLOCKED playing sound by soundObject");
                return;
            }

            CollisionSoundSet soundSet = new CollisionSoundSet { material0 = soundObjectA.Material, material1 = soundObjectB.Material, position = position, impactVolume = impactVolume };
            if (Instance.soundSetsThisFrame.IndexOf(soundSet) >= 0)
            {
                // Debug.Log("BLOCKED playing sound, already playing");
                return;
            }

            Instance.soundSetsThisFrame.Add(soundSet);

            Instance.Provider.Play(soundObjectA, soundObjectB, position, impactVolume);
        }

        private void FixedUpdate()
        {
            soundSetsThisFrame.Clear();
        }
    }

    public enum CollisionSoundProviderType
    {
        None,
        Unity,
        FMOD,
        UnityWithList,
    }
}