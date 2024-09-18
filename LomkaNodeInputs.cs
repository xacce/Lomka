using Trove.PolymorphicStructs;
using Unity.Entities;

namespace Lomka
{
    public interface ILomkaNodeIndexProvider
    {
        public int nodeId { get; set; }
    }
}