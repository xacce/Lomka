#if UNITY_EDITOR
using Unity.Entities;
using UnityEngine;

namespace Lomka.Authoring
{
    [DisallowMultipleComponent]
    public class LomkaNodeAuthoring : MonoBehaviour
    {
        private class LomkaNodeBaker : Baker<LomkaNodeAuthoring>
        {
            public override void Bake(LomkaNodeAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.None);
            }
        }
    }
}
#endif