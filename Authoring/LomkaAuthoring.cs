#if UNITY_EDITOR
using System;
using DotsInput;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lomka.Authoring
{
    [RequireComponent(typeof(LinkedEntityGroup))]
    [RequireComponent(typeof(DotsInputAuthoring))]
    [DisallowMultipleComponent]
    public class LomkaAuthoring : MonoBehaviour
    {
        [FormerlySerializedAs("settings")] [SerializeField]
        private LomkaData data;

        public bool initializeAtStart = true;
        public bool markAsActive = true;
        public GameObject[] templates = Array.Empty<GameObject>();

        private class LomkaSettingsBaker : Baker<LomkaAuthoring>
        {
            public override void Bake(LomkaAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.Dynamic);
                AddBuffer<LomkaNodeElement>(e);
                AddBuffer<LomkaNodeDesc>(e);
                AddComponent<LomkaState>(e);
                AddComponent<LomkaInputable>(e);
                if (authoring.markAsActive)
                    SetComponentEnabled<LomkaInputable>(e, true);
                AddComponent(e, authoring.data);
                if (authoring.initializeAtStart)
                    AddComponent<LomkaInitialize>(e);


                var templates = AddBuffer<LomkaNodeTemplate>(e);
                foreach (var templateSo in authoring.templates)
                {
                    templates.Add(new LomkaNodeTemplate { template = GetEntity(templateSo, TransformUsageFlags.Dynamic) });
                }
            }
        }
    }
}

#endif