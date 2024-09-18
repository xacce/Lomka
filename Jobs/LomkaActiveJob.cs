using DotsInput;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Lomka.Jobs
{
    [BurstCompile]
    [WithAll(typeof(LomkaInputable))]
    [WithAll(typeof(LomkaInitialized))]
    public partial struct LomkaActiveJob : IJobEntity
    {
        public float deltaTime;
        public EntityCommandBuffer.ParallelWriter ecb;
        [NativeSetThreadIndex] private int index;

        [BurstCompile]
        public void Execute(ref LomkaState state, in LomkaData data, DynamicBuffer<DotsInputAxisElement> inputAxes, ref LocalTransform transform, Entity entity)
        {
            if (data.allowRotation)
            {
                var axisRaw = inputAxes[0].GetFloat2();
                var axis = new float3(-axisRaw.y, axisRaw.x, 0f);
                var delta = quaternion.Euler(axis * data.rotationSens * deltaTime);
                transform.Rotation = math.mul(delta, transform.Rotation);
            }

            switch (state.flashback)
            {
                case LomkaState.Flashback.Destroy:
                    ecb.DestroyEntity(index, entity);
                    break;
            }

            state.flashback = LomkaState.Flashback.Nothing;
        }
    }
}