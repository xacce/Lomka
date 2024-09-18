using Lomka.Jobs;
using Lomka.Jobs.Geometry;
using Unity.Collections;
using Unity.Entities;

namespace Lomka
{
    public partial struct LomkaSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndInitializationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
            var uvSphere = new LomkaBuiltBlobbed() { }.ScheduleParallel(state.Dependency);
            var geometryJabs = uvSphere;
            var createNodesJob = new LomkaCreateNodes()
            {
                ecb = ecb.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                seed = 100,
            }.ScheduleParallel(geometryJabs);

            var activeJob = new LomkaActiveJob()
            {
                ecb = ecb.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                deltaTime = SystemAPI.Time.DeltaTime
            }.ScheduleParallel(createNodesJob);
            state.Dependency = activeJob;
        }
    }
}