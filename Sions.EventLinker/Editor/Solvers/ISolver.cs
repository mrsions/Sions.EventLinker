#nullable enable

namespace Sions.EventLinker
{
    public interface ISolver
    {
        public EventGenerator Generator { get; }
    }
}