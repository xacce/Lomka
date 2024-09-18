// using Lomka.Authoring;
// using Lomka.Nodes.RandomBlockedNodeJob;
// using Unity.Entities;
// using UnityEngine;
//
// namespace Lomka.Nodes.Authoring
// {
//     [RequireComponent(typeof(LomkaNodeAuthoring))]
//     [DisallowMultipleComponent]
//     public class RandomBlockedNodeJobAuthoring : MonoBehaviour
//     {
//         private class RandomBlockedNodeJobBaker : Baker<RandomBlockedNodeJobAuthoring>
//         {
//             public override void Bake(RandomBlockedNodeJobAuthoring authoring)
//             {
//                 var e = GetEntity(TransformUsageFlags.None);
//                 AddComponent<RandomBlockedNode>(e);
//             }
//         }
//     }
// }