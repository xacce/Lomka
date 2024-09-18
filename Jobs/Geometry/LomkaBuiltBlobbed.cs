using Unity.Burst;
using Unity.Entities;

namespace Lomka.Jobs.Geometry
{
    [BurstCompile]
    [WithAll(typeof(LomkaInitialize))]
    public partial struct LomkaBuiltBlobbed : IJobEntity
    {
        [BurstCompile]
        public void Execute(ref LomkaState lomkaState, LomkaBlobGeometry blobGeometry, DynamicBuffer<LomkaNodeDesc> nodes)
        {
            if (lomkaState.state != LomkaState.CoreStateType.CoreGeometry) return;
            ref var blob = ref blobGeometry.blob.Value;

            for (int i = 0; i < blob.nodes.Length; i++)
            {
                ref var node = ref blob.nodes[i];
                nodes.Add(new LomkaNodeDesc()
                {
                    data = new LomkaNodeData()
                    {
                        originPosition = node.position,
                        originDirection = node.orientation,
                        index = i,
                    }
                });
            }

            lomkaState.SetState(LomkaState.CoreStateType.CoreTemplates);
        }
    }
}