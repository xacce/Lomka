using Unity.Entities;
using UnityEngine;

namespace Lomka.Authoring.So
{
    public abstract class AbstractLomkaBlobGeometrySo : ScriptableObject
    {
        public abstract BlobAssetReference<LomkaBlobGeometry.Blob> GetBlob(IBaker baker);
    }
}