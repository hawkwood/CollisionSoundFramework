using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StringyDropdown;

namespace CollisionSoundFramework
{
    public class CollisionSoundProviderUnity : CollisionSoundProvider
    {
        [SerializeField]
        private GameObject audioSourcePrefab;

        [System.Serializable]
        public struct MaterialSounds
        {
            [StringyDropdown(FileName = "MaterialStrings")]
            public string Material;

            public List<AudioClip> Clips;
        }

        [SerializeField]
        private List<MaterialSounds> materialsSounds = new List<MaterialSounds>();

        [Header("Load From Resources")]

        [SerializeField]
        private bool loadFromResources = true;

        [SerializeField]
        private string audioSourcePrefabPath = "CollisionSoundPrefab";
        [SerializeField]
        private string collisionSoundsPath = "CollisionSounds";

        [Header("Exposed for debug")]

        [SerializeField]
        private List<string> matKeyIndices = new List<string>();

        private AudioSource[] audioPool;
        private int CurrentPoolIndex;

        public override void Setup()
        {
            matKeyIndices.Clear();

            for (int i = 0; i < materialsSounds.Count; i++)
            {
                MaterialSounds materialSounds = materialsSounds[i];
                matKeyIndices.Add(materialSounds.Material);
            }

            audioPool = new AudioSource[CollisionSoundController.Instance.SoundPoolSize];

            if (audioSourcePrefab == null && loadFromResources)
            {
                audioSourcePrefab = Resources.Load<GameObject>(audioSourcePrefabPath);
            }

            for (int index = 0; index < audioPool.Length; index++)
            {
                audioPool[index] = GameObject.Instantiate<GameObject>(audioSourcePrefab).GetComponent<AudioSource>();
                audioPool[index].transform.parent = this.transform;
            }

            if (loadFromResources)
            {
                List<string> materials = new List<string>(CollisionSoundController.Instance.Materials);

                AudioClip[] clips = Resources.LoadAll<AudioClip>(collisionSoundsPath);
                for (int index = 0; index < clips.Length; index++)
                {
                    string name = clips[index].name;
                    int dividerIndex = name.IndexOf("__");
                    if (dividerIndex >= 0)
                        name = name.Substring(0, dividerIndex);

                    MaterialSounds materialSounds;
                    int idx = matKeyIndices.IndexOf(name);
                    if (idx < 0)
                    {
                        if (!materials.Contains(name))
                        {
                            Debug.LogWarning("CollisionSoundFramework: Found clip for material that is not in materials list: " + clips[index].name);
                            continue;
                        }

                        materialSounds = new MaterialSounds();
                        materialSounds.Material = name;
                        materialSounds.Clips = new List<AudioClip>();

                        materialsSounds.Add(materialSounds);
                        matKeyIndices.Add(name);
                    }
                    else
                    {
                        materialSounds = materialsSounds[idx];
                    }

                    if (!materialSounds.Clips.Contains(clips[index]))
                    {
                        materialSounds.Clips.Add(clips[index]);
                    }
                }   
            }
        }

        public override void Play(CollisionSoundObject soundObjectA, CollisionSoundObject soundObjectB, Vector3 position, float impactVolume)
        {
            if (soundObjectA == null || string.IsNullOrEmpty(soundObjectA.Material) || soundObjectB == null || string.IsNullOrEmpty(soundObjectB.Material))
                return;

            Play(soundObjectA.Material, position, impactVolume, Pitch(soundObjectA));
            soundObjectA.DidPlaySound();
            Play(soundObjectB.Material, position, impactVolume, Pitch(soundObjectB));
            soundObjectB.DidPlaySound();
        }

        private float Pitch(CollisionSoundObject soundObject)
        {
            float min = 0;
            float max = 3;
            if (soundObject.OverridePitchModulationSettings)
            {
                if (!soundObject.PitchModulationEnabled) return 1;

                min = Mathf.Max(1 - soundObject.PitchModulationExtent, 0);
                max = 1 + soundObject.PitchModulationExtent;
            }
            else
            {
                if (!CollisionSoundController.Instance.PitchModulationEnabled) return 1;

                min = Mathf.Max(1 - CollisionSoundController.Instance.PitchModulationExtent, 0);
                max = 1 + CollisionSoundController.Instance.PitchModulationExtent;
            }

            float pitch = Random.Range(min, max);

            return pitch;
        }

        private void Play(string material, Vector3 position, float impactVolume, float pitch)
        {
            int matIdx = matKeyIndices.IndexOf(material);
            if (matIdx < 0 || materialsSounds[matIdx].Clips.Count == 0)
            {
                Debug.LogError("CollisionSoundFramework: Trying to play sound for material without a clip. Need to add to MaterialSounds or add a clip at: " + collisionSoundsPath + "/" + material);
                return;
            }

            int clipIdxindex = Random.Range(0, materialsSounds[matIdx].Clips.Count);
            AudioClip clip = materialsSounds[matIdx].Clips[clipIdxindex];

            AudioSource source = audioPool[CurrentPoolIndex];

            source.pitch = pitch;
            source.transform.position = position;
            source.volume = impactVolume;
            source.clip = clip;
            source.Play();

            CurrentPoolIndex++;

            if (CurrentPoolIndex >= audioPool.Length)
            {
                CurrentPoolIndex = 0;
            }

            // Debug.Log("Playing " + material);
        }
    }
}
