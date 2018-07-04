using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Analytic.Entities
{

    public interface IAutomatedReportEvaluatorContext
    {
        void WriteErrorBusinessTrackingMsg(string messageFormat, params object[] args);
        void WriteWarningBusinessTrackingMsg(string messageFormat, params object[] args);
        void WriteInformationBusinessTrackingMsg(string messageFormat, params object[] args);
    }
}
