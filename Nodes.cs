using Unity.Entities;
using Unity.Mathematics;

namespace Lomka
{
    public partial struct LomkaNodeData
    {
        public float3 originPosition;
        public quaternion originDirection;
        public int index;
    }

    public partial struct LomkaNodeRuntime : IComponentData
    {
        public LomkaNodeData data;
        public Entity lomka;
    }
   
}