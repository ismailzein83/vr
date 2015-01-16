using System;
using System.Text;

namespace TABS.Addons.Runnables
{
    [NamedAddon("T.One - CDR Import", "Imports CDR Records from All Switches Connected to the system using their Switch Managers and their configurations if required.")]
    public class CDRImportRunner : RunnableBase
    {
        internal static string status;

        /// <summary>
        /// Start the CDR Import (Get from switches and save to database)
        /// </summary>
        /// <returns></returns>
        public override void Run()
        {
            status = string.Empty;
            TABS.Components.Engine.GetCDRsFromSwitches();
            status = string.Empty;
        }

        /// <summary>
        /// Request a stop for the operation
        /// </summary>
        /// <returns></returns>
        public override bool Stop()
        {
            TABS.Components.Engine.StopCDRImport();
            TABS.Components.Engine.CdrImportCodeMap = null;
            //TABS.CodeMap.ClearCachedCollections();
            GC.Collect();
            return base.Stop();
        }

        public override bool Abort()
        {
            TABS.Components.Engine._IsCDRImportRunning = false;
            return base.Abort();
        }

        public override string Status
        {
            get 
            { 
                if(Components.Engine.IsCDRImportRunning)
                {
                    StringBuilder running = new StringBuilder();
                    foreach (var handler in Components.Engine.CdrImportHandlers)
                        if (handler.IsRunning)
                            running.Append(running.Length > 0 ? ", " : "")
                                .Append(handler.Switch.Name);
                    if (running.Length > 0)
                        return "Importing from: " + running.ToString();

                    return "";
                }
                else
                    return status;                
            }
        }

    }
}