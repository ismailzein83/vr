using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public abstract class ExceptionLogger
    {
        public void WriteException(Exception ex)
        {
            OnWriteException(ex);
        }

        protected abstract void OnWriteException(Exception ex);
    }
}
