using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public class CustomQueueActivator : QueueActivator
    {
        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            throw new NotImplementedException();
        }

        public override void OnDisposed()
        {
            throw new NotImplementedException();
        }

        public string FQTN { get; set; }

        QueueActivator _activator;
        public override void ProcessItem(IQueueActivatorExecutionContext context)
        {
            if(_activator == null)
            {
                if (this.FQTN == null)
                    throw new NullReferenceException("FQTN");
                Type activatorType = Type.GetType(this.FQTN);
                if (activatorType == null)
                    throw new NullReferenceException(String.Format("activatorType '{0}'", this.FQTN));
                _activator = Activator.CreateInstance(activatorType) as QueueActivator;
                if (_activator == null)
                    throw new Exception(String.Format("'{0}' is not of type QueueActivator", this.FQTN));
            }
            _activator.ProcessItem(context);
        }
    }
}
