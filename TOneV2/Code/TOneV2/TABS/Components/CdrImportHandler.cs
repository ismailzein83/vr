using System;
using System.Collections.Generic;

namespace TABS.Components
{
    /// <summary>
    /// Handles CDR Import from a given switch. Will create a thread that will fetch the CDRs from the switch
    /// and updates the 
    /// </summary>
    internal class CdrImportHandler:IDisposable
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(CdrImportHandler));

        public DateTime? LastImport { get; protected set; } 

        /// <summary>
        /// The Switch for which this handler runs an import
        /// </summary>
        public Switch Switch { get; protected set; }
        
        protected System.Threading.Thread _thread;

        /// <summary>
        /// Gets whether the import is finished or not
        /// </summary>
        public bool Finished { get; protected set; }

        /// <summary>
        /// Indicates whether this 
        /// </summary>
        public bool IsRunning { get { return ! Finished; } }

        /// <summary>
        /// Gets the Imported CDRs from the switch. Will be Empty if the import is not yet finished.
        /// </summary>
        public IList<CDR> CDR { get; protected set; }

        /// <summary>
        /// Create a CDR Import Handler for the given source switch.
        /// </summary>
        /// <param name="sourceSwitch"></param>
        public CdrImportHandler(Switch sourceSwitch)
        {
            CDR = new List<CDR>();
            this.Switch = sourceSwitch;
            System.Threading.ThreadStart starter = new System.Threading.ThreadStart(this.Run);
            _thread = new System.Threading.Thread(starter);
            _thread.Start();
        }
        ~CdrImportHandler()
        {
            if (_thread != null)
            {
                _thread.Join();
                _thread.Abort();
            }
            _thread = null;
            log.Info("handler is disposed in ~");
        }
        public void Dispose()
        {
            if(_thread!=null)
            {
                _thread.Join();
                _thread.Abort();
            }
            _thread = null;
            log.Info("handler is disposed");
        }
        /// <summary>
        /// Forcibly stop the CDR Import
        /// </summary>
        public void Abort()
        {
            if (IsRunning)
            {
                log.ErrorFormat("Forcing Stop Import for Switch: {0}. Aborting Thread.", this.Switch);
                _thread.Abort();
                this.CDR = new List<CDR>();
                Finished = true;
            }
        }

        /// <summary>
        /// Run the import procedure in the created thread.
        /// </summary>
        protected void Run()
        {
            try
            {
                var rawCDRs = this.Switch.SwitchManager.GetCDR(this.Switch);
                
                LastImport = DateTime.Now;
                
                // create CDRs from Standard CDRs
                foreach (TABS.Addons.Utilities.Extensibility.CDR rawCDR in rawCDRs)
                    CDR.Add(new CDR(this.Switch, rawCDR));
                rawCDRs = null;
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Error Handling Import for Switch: {0}", this.Switch), ex);
            }
            finally
            {
                Finished = true;
            }
           
        }
    }
}
