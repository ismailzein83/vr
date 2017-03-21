using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public abstract class ExceptionLogger
    {
        public void WriteException(string eventType, int? viewRequiredPermissionSetId, Exception ex)
        {
            OnWriteException(eventType, viewRequiredPermissionSetId, ex);
        }

        public void WriteException(Exception ex)
        {
            WriteException(null, null, ex);
        }

        protected abstract void OnWriteException(string eventType, int? viewRequiredPermissionSetId, Exception ex);
    }
}
