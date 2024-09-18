#if UNITY_EDITOR
using Lomka.Authoring.So;
using Unity.Entities;
using UnityEngine;

namespace Lomka.Authoring
{
    public class LomkaBlobGeometryAuthoring : MonoBehaviour
    {
        [SerializeField] private AbstractLomkaBlobGeometrySo data;

        private class LomkaBlobGeometryBa : Baker<LomkaBlobGeometryAuthoring>
        {
            public override void Bake(LomkaBlobGeometryAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.None);
                AddComponent(e, new LomkaBlobGeometry() { blob = authoring.data.GetBlob(this) });
            }
        }
    }
}
#endif