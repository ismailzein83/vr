using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public abstract class ExceptionLogger
    {
        public void WriteException(string eventType, Exception ex)
        {
            OnWriteException(eventType, ex);
        }

        public void WriteException(Exception ex)
        {
            WriteException(null, ex);
        }

        protected abstract void OnWriteException(string eventType, Exception ex);
    }
}
