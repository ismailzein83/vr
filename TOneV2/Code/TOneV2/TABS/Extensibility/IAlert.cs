using System;

namespace TABS.Extensibility
{
    public interface IAlert
    {
        DateTime Created { get; set; }
        string Source { get; set; }
        string Description { get; set; }
        string Tag { get; set; }
        AlertLevel Level { get; set; }
        AlertProgress Progress { get; set; }
    }
}
