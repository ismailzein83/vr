
namespace TABS.Addons.Runnables
{
    [NamedAddon("T.One - Job Queue", "Manages Jobs to be executed on switches.")]
    class SwitchJobRunner : RunnableBase
    {        
        public override void Run()
        {
            foreach (Utilities.SwitchJob job in Utilities.SwitchJobQueueHandler.Jobs)
                job.Run();            
        }
        public override string Status
        {
            get { return string.Empty; }
        }
    }
}
