using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Lomka.Jobs
{
    [BurstCompile]
    [WithAll(typeof(LomkaInitialize))]
    public partial struct LomkaCreateNodes : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        public uint seed;

        [NativeSetThreadIndex] private int index;

        [BurstCompile]
        public void Execute(
            ref LomkaState state,
            in LomkaData data,
            in LocalTransform transform,
            DynamicBuffer<LomkaNodeDesc> descs,
            DynamicBuffer<LomkaNodeTemplate> templates,
            Entity entity)
        {
            if (state.state != LomkaState.CoreStateType.CoreTemplates) return;
            var rnd = Random.CreateFromIndex((uint)(seed + index));

            for (int i = 0; i < descs.Length; i++)
            {
                ref var desc = ref descs.ElementAt(i);
                var tmplIndex = rnd.NextInt(0, templates.Length);
                var template = templates[tmplIndex];
                var nodeEntity = ecb.Instantiate(index, template.template); //todo move to main thread
                LocalTransform localTransform;
                if (data.hierarchySpawn)
                {
                    localTransform = LocalTransform.FromPosition(descs[i].data.originPosition);
                    ecb.AddComponent(index, nodeEntity, new Parent() { Value = entity });
                }
                else
                {
                    localTransform = transform.TransformTransform(LocalTransform.FromPosition(descs[i].data.originPosition));
                }

                ecb.SetComponent(index, nodeEntity, localTransform);
                ecb.AppendToBuffer(index, entity, new LomkaNodeElement() { entity = nodeEntity });
                ecb.AppendToBuffer(index, entity, new LinkedEntityGroup() { Value = nodeEntity });
                ecb.AddComponent(index, nodeEntity, new LomkaNodeRuntime()
                {
                    lomka = entity,
                    data = desc.data,
                });
            }

            state.SetState(LomkaState.CoreStateType.CoreDone);
        }
    }
}