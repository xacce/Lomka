using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Lomka
{
    [Serializable]
    public partial struct LomkaUvSphereRuntimeGeometry : IComponentData
    {
        public static LomkaUvSphereRuntimeGeometry Default => new LomkaUvSphereRuntimeGeometry()
            { radius = 1f, latMax = 8, longMax = 12, neighbourPickerMode = NeighbourPickerMode.Default };

        public enum NeighbourPickerMode
        {
            Nothing,
            Default,
        }

        public float radius;
        public int latMax;
        public int longMax;
        public NeighbourPickerMode neighbourPickerMode;
    }

    public partial struct LomkaBlobGeometry : IComponentData
    {
        public struct Blob
        {
            public struct Node
            {
                public float3 position;

                public quaternion orientation;

                // public int neighbourLength;
                public BlobArray<int> neighbourPair;
            }

            public BlobArray<Node> nodes;
            public BlobArray<int2> pairs;
        }

        public BlobAssetReference<Blob> blob;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 GetPairSeqId(int i1, int i2)
        {
            return math.select(new int2(i2, i1), new int2(i1, i2), i1 > i2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetOtherIndexFromPair(in int2 pair, int i1)
        {
            return math.select(pair.x, pair.y, pair.x == i1);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SendToNeighbours<T>(ref LomkaBlobGeometry.Blob geometryBlob, int myIndex,
            in T input, ref DynamicBuffer<T> queue) where T : unmanaged, ILomkaNodeIndexProvider
        {
            ref var myNeighbourPairs = ref geometryBlob.nodes[myIndex].neighbourPair;
            for (int j = 0; j < myNeighbourPairs.Length; j++)
            {
                var pair = geometryBlob.pairs[myNeighbourPairs[j]];
                var updatedInput = input;
                updatedInput.nodeId = GetOtherIndexFromPair(pair, myIndex);
                queue.Add(updatedInput);
            }
        }
    }


    public partial struct LomkaState : IComponentData
    {
        public enum CoreStateType
        {
            CoreGeometry,
            CoreTemplates,
            CoreDone,
            // Completed,
            // Loose,
        }

        public enum Flashback
        {
            Nothing,
            Destroy,
        }

        [Flags]
        public enum Flags : ulong
        {
            Nothing = 0 << 0, //enable ide auto generation
            Revealed = 1 << 0,
        }

        public CoreStateType state;
        public Flashback flashback;

        public void SetState(CoreStateType state)
        {
            this.state = state;
        }

        public ulong flags;

        public bool HasFlag(Flags flag)
        {
            return (flags & (ulong)flag) != 0;
        }

        public void SetFlag(Flags flag)
        {
            flags |= (ulong)flag;
        }
    }


    [Serializable]
    public partial struct LomkaData : IComponentData
    {
        public bool hierarchySpawn;
        public float rotationSens;
        public bool allowRotation;
    }


    public partial struct LomkaInputable : IComponentData, IEnableableComponent
    {
    }

    public partial struct LomkaInitialize : IComponentData
    {
    }

    public partial struct LomkaInitialized : IComponentData
    {
    }

    public partial struct LomkaNodeElement : IBufferElementData
    {
        public Entity entity;
    }

    public partial struct LomkaNodeDesc : IBufferElementData
    {
        public LomkaNodeData data;
    }
}