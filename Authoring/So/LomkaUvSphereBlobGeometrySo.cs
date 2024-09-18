#if UNITY_EDITOR
using System.Collections.Generic;
using Lomka.Static;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Lomka.Authoring.So
{
    [CreateAssetMenu(menuName = "Lomka/Uv sphere baked")]
    public class LomkaUvSphereBlobGeometrySo : AbstractLomkaBlobGeometrySo
    {
        [SerializeField] private int latMax;
        [SerializeField] private int longMax;
        [SerializeField] private float radius;

        public override BlobAssetReference<LomkaBlobGeometry.Blob> GetBlob(IBaker baker)
        {
            BlobBuilder builder = new BlobBuilder(Allocator.Temp);
            ref var definition = ref builder.ConstructRoot<LomkaBlobGeometry.Blob>();

            var points = new NativeList<float3>(Allocator.Temp);
            // var neighbourPairIndexes = new NativeList<int>(Allocator.Temp);
            SphereGeometryHelper.BuildSphere(latMax, longMax, radius, ref points);
            var nodesBaked = builder.Allocate(ref definition.nodes, points.Length);
            var pairs = new List<int2>();
            for (int i = 0; i < points.Length; i++)
            {
                var neighboursRaw = new NativeList<int>(Allocator.Temp);
                SphereGeometryHelper.GetNeighbours(i, longMax, latMax, ref neighboursRaw);
                nodesBaked[i] = new LomkaBlobGeometry.Blob.Node()
                {
                    orientation = quaternion.identity,
                    position = points[i],
                    // neighbourLength = neighboursRaw.Length,
                };
                var neihboursBaked = builder.Allocate(ref nodesBaked[i].neighbourPair, neighboursRaw.Length);
                for (int j = 0; j < neighboursRaw.Length; j++)
                {
                    var seqid = LomkaBlobGeometry.GetPairSeqId(i, neighboursRaw[j]);
                    var exi = pairs.IndexOf(seqid);
                    if (exi == -1)
                    {
                        pairs.Add(seqid);
                        neihboursBaked[j] = pairs.Count - 1;
                    }
                    else neihboursBaked[j] = exi;
                }

                neighboursRaw.Dispose();
            }

            var pairsBaked = builder.Allocate(ref definition.pairs, pairs.Count);
            int q = 0;
            foreach (var pair in pairs)
            {
                pairsBaked[q] = pair;
                q++;
            }


            // Debug.Log($"Nodes: {nodesBaked.Length}");
            // Debug.Log($"Pairs: {pairs.Count}");

            points.Dispose();
            // allNeighboursRaw.Dispose();
            BlobAssetReference<LomkaBlobGeometry.Blob> blobReference = builder.CreateBlobAssetReference<LomkaBlobGeometry.Blob>(Allocator.Persistent);
            baker.AddBlobAsset(ref blobReference, out var hash);
            builder.Dispose();
            return blobReference;
        }
    }
}
#endif