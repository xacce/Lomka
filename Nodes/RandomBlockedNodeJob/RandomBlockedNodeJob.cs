// using Lomka.Presentation;
// using Trove.PolymorphicStructs;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Collections.LowLevel.Unsafe;
// using Unity.Entities;
// using UnityEngine;
//
// namespace Lomka.Nodes.RandomBlockedNodeJob
// {
//     public partial struct RandomBlockedNode : IComponentData
//     {
//         public enum State
//         {
//             Nothing = 0,
//             Revealed = 1,
//             Blocked = 2,
//         }
//
//         public State state;
//     }
//
//     [PolymorphicStruct]
//     public partial struct RandomBlockedNodeInputChangeState : ILomkaNodeInput
//     {
//         public int state;
//     }
//
//     [BurstCompile]
//     public partial struct RandomBlockedNodeJob : IJobEntity
//     {
//         [NativeDisableContainerSafetyRestriction]
//         public ComponentLookup<LomkaDefaultVfxPresented> presentationRo;
//         public ComponentLookup<LomkaState> lomkaStateRw;
//
//         [ReadOnly] public BufferLookup<LomkaNodeDesc> descsRo;
//         [ReadOnly] public BufferLookup<LomkaNodeElement> elementsRo;
//         [ReadOnly] public ComponentLookup<LomkaBlobGeometry> blobGeometryRo;
//         public EntityCommandBuffer.ParallelWriter ecb;
//
//
//         [NativeSetThreadIndex] private int index;
//
//         [BurstCompile]
//         public void Execute(ref RandomBlockedNode data, ref LomkaNodeRuntime node, DynamicBuffer<LomkaNodeInput> inputs, Entity entity)
//         {
//             if (inputs.IsEmpty) return;
//             var rnd = Unity.Mathematics.Random.CreateFromIndex((uint)entity.Index);
//             ref var lomkaState = ref lomkaStateRw.GetRefRW(node.lomka).ValueRW;
//             var elements = elementsRo[node.lomka];
//             var geometry = blobGeometryRo[node.lomka];
//             ref var geometryBlob = ref geometry.blob.Value;
//             var presentation = presentationRo[node.lomka];
//             bool lomkaIsRevealed = lomkaState.HasFlag(LomkaState.Flags.Revealed);
//             for (int i = 0; i < inputs.Length; i++)
//             {
//                 var inputRaw = inputs[i];
//                 switch (inputRaw.CurrentTypeId)
//                 {
//                     case LomkaNodeInput.TypeId.LomkaNodeInitialized:
//                         break;
//                     case LomkaNodeInput.TypeId.LomkaNodeSelected when (!lomkaIsRevealed) || (data.state == RandomBlockedNode.State.Revealed):
//                     {
//                         lomkaState.SetFlag(LomkaState.Flags.Revealed);
//                         var input = (LomkaNodeInitialized)inputRaw;
//                         for (int j = 0; j < geometryBlob.nodes[node.data.index].neighbourPair.Length; j++)
//                         {
//                             var pairIndex = geometryBlob.nodes[node.data.index].neighbourPair[j];
//                             var pair = geometryBlob.pairs[pairIndex];
//                             var other = LomkaBlobGeometry.GetOtherIndexFromPair(pair, node.data.index);
//                             if (rnd.NextFloat() < 0.5f)
//                             {
//                                 LomkaNodeElement.SendInput(index, ecb, elements, other, new RandomBlockedNodeInputChangeState { state = (int)RandomBlockedNode.State.Blocked });
//                             }
//                             else
//                             {
//                                 LomkaNodeElement.SendInput(index, ecb, elements, other, new RandomBlockedNodeInputChangeState { state = (int)RandomBlockedNode.State.Revealed });
//                             }
//                         }
//
//                         data.state = RandomBlockedNode.State.Revealed;
//                         LomkaDefaultVfxPresented.UpdateNodeState(ref presentation, node.data.index, (int)data.state);
//                         break;
//                     }
//                     case LomkaNodeInput.TypeId.RandomBlockedNodeInputChangeState when data.state == RandomBlockedNode.State.Nothing:
//                     {
//                         var input = (RandomBlockedNodeInputChangeState)inputRaw;
//                         data.state = (RandomBlockedNode.State)input.state;
//                         LomkaDefaultVfxPresented.UpdateNodeState(ref presentation, node.data.index, input.state);
//                         break;
//                     }
//                 }
//             }
//
//             inputs.Clear();
//         }
//     }
// }