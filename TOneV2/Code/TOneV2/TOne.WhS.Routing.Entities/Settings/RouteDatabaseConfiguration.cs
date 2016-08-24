
namespace TOne.WhS.Routing.Entities
{
    public enum TimeUnit
    {
        Minutes = 0,
        Hours = 1
    }

    public class RouteDatabaseConfiguration
    {
        //public int SpecificDBToKeep { get; set; }

        public int CurrentDBToKeep { get; set; }

        public int FutureDBToKeep { get; set; }

        public int MaximumEstimatedExecutionTime { get; set; }

        public TimeUnit TimeUnit { get; set; }
    }
}
