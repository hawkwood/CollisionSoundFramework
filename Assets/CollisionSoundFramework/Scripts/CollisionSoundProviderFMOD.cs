using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if CSF_FMOD
using FMOD.Studio;
using FMODUnity;

namespace CollisionSoundFramework
{
    public class CollisionSoundProviderFMOD : CollisionSoundProvider
    {
        private static Dictionary<string, string> eventStrings;
        public static Dictionary<string, string> EventStrings
        {
            get
            {
                if (eventStrings == null)
                {
                    eventStrings = new Dictionary<string, string>(new EnumEqualityComparer<CollisionSoundMaterials>());

                    foreach (CollisionSoundMaterials mat in CollisionSoundMaterialsList.List)
                    {
                        if (mat == CollisionSoundMaterials.CustomMaterial)
                        {
                            continue;
                        }

                        eventStrings.Add(mat, string.Format("event:/Collisions/{0}", mat.ToString()));
                    }
                }
                return eventStrings;
            }
        }

        private static Dictionary<CollisionSoundMaterials, System.Guid> eventGuids;
        public static Dictionary<CollisionSoundMaterials, System.Guid> EventGuids
        {
            get
            {
                if (eventGuids == null)
                {
                    eventGuids = new Dictionary<string, System.Guid>(new EnumEqualityComparer<CollisionSoundMaterials>());

                    foreach (var mat in EventStrings)
                    {
                        if (mat.Key == CollisionSoundMaterials.EndNewtoMaterials)
                        {
                            continue;
                        }

                        eventGuids.Add(mat.Key, FMODUnity.RuntimeManager.PathToGUID(mat.Value));
                    }
                }
                return eventGuids;
            }
        }

        public override void Setup()
        {

        }

        public override void Play(string materialA, string materialB, Vector3 position, float impactVolume)
        {
            if (string.IsNullOrEmpty(material))
                return;

            System.Guid playGuid = EventGuids[material];
            
            EventInstance collidingInstance = RuntimeManager.CreateInstance(playGuid);
            collidingInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
            collidingInstance.setVolume(impactVolume);
            collidingInstance.start();
            collidingInstance.release();
        }
    }
}
#else

namespace CollisionSoundFramework
{
    public class CollisionSoundProviderFMOD : CollisionSoundProvider
    {
        public override void Setup()
        {
        }

        public override void Play(CollisionSoundObject soundObjectA, CollisionSoundObject soundObjectB, Vector3 position, float impactVolume)
        {
            throw new System.NotImplementedException();
        }
    }
}
#endif