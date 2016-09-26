using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess
{    
    public interface IBPContext
    {
        void WriteTrackingMessage(LogEntryType severity, string messageFormat, params object[] args);
    }
}
