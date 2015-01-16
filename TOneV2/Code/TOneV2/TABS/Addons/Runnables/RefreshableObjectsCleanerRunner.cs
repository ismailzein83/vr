
namespace TABS.Addons.Runnables
{
    [NamedAddon("T.One - Refreshable Objects Cleaner", "Cleans and Refreshes all objects (and collections) that are date/time sensitive (Effective Begin/End Dates and Times) to ensure proper functionality of the system")]
    public class RefreshableObjectsCleanerRunner : RunnableBase
    {
        public override void Run()
        {
            if (IsRunning) return;
            _IsRunning = true;
            
            Components.DateTimeSensitiveMonitor.Refresh();
            
            _IsRunning = false;

            IsLastRunSuccessful = true;
        }

        public override string Status
        {
            get { return string.Empty; }
        }
    }
}
