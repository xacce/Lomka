// using Lomka.Static;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Mathematics;
//
// namespace Lomka.Jobs.Geometry
// {
//     [BurstCompile]
//     [WithNone(typeof(LomkaInitialized))]
//     public partial struct BuiltUvSphereGeometryJob : IJobEntity
//     {
//         [BurstCompile]
//         public void Execute(ref LomkaState lomkaState, LomkaUvSphereRuntimeGeometry geometry, DynamicBuffer<LomkaNodeDesc> nodes,
//             DynamicBuffer<LomkaNodeRuntimeNeighbourElement> neighbours)
//         {
//             if (lomkaState.state != LomkaState.Type.Geometry) return;
//             var points = new NativeList<float3>(Allocator.Temp);
//             var neighboursInt = neighbours.Reinterpret<int>();
//             var neighboursRaw = new NativeList<int>(Allocator.Temp);
//             SphereGeometryHelper.BuildSphere(geometry.latMax, geometry.longMax, geometry.radius, ref points);
//             for (int i = 0; i < points.Length; i++)
//             {
//                 neighboursRaw.Clear();
//                 SphereGeometryHelper.GetNeighbours(i, geometry.longMax, geometry.latMax, ref neighboursRaw);
//                 neighboursInt.AddRange(neighboursRaw);
//
//                 nodes.Add(new LomkaNodeDesc()
//                 {
//                     data = new LomkaNodeData()
//                     {
//                         originPosition = points[i],
//                         originDirection = quaternion.identity,
//                         index = i,
//                         templateIndex = -1,
//                         neighboursLength = neighboursRaw.Length,
//                     }
//                 });
//             }
//
//             lomkaState.state = LomkaState.Type.Templates;
//         }
//     }
// }