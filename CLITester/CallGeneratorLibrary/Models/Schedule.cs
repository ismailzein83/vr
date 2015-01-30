using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallGeneratorLibrary.Repositories;
using System.Diagnostics;

namespace CallGeneratorLibrary
{
    public partial class Schedule
    {
        string operatorprefix = "";
        string operatorprefixId = "";
        public string OperatorPrefixId
        {
            get
            {
                try
                {
                    if (operatorprefixId == "")
                    {
                        List<ScheduleOperator> lstOp = new List<ScheduleOperator>();
                        lstOp = ScheduleOperatorRepository.GetScheduleOperatorsByScheduleId(this.Id);
                        foreach (ScheduleOperator op in lstOp)
                            operatorprefixId += op.OperatorId + "$" + op.Operator.FullName + "$" + op.CarrierId + "$" + op.Carrier.ShortName + " - " + op.Carrier.Prefix + "$";
                    }

                    return operatorprefixId;
                }
                catch (System.Exception ex)
                {
                    WriteToEventLogEx(ex.ToString());
                    return null;
                }
            }
        }

        public string OperatorPrefix
        {
            get
            {
                try
                {
                    if (operatorprefix == "")
                    {
                        List<ScheduleOperator> lstOp = new List<ScheduleOperator>();
                        lstOp = ScheduleOperatorRepository.GetScheduleOperatorsByScheduleId(this.Id);
                        foreach (ScheduleOperator op in lstOp)
                            operatorprefix += op.Operator.FullName + "$" + op.Carrier.ShortName + " - " + op.Carrier.Prefix + "!";
                    }

                    return operatorprefix;
                }
                catch (System.Exception ex)
                {
                    WriteToEventLogEx(ex.ToString());
                    return null;
                }
            }
        }

        private static void WriteToEventLogEx(string message)
        {
            string cs = "Call Generator Lib Excep";
            EventLog elog = new EventLog();
            if (!EventLog.SourceExists(cs))
            {
                EventLog.CreateEventSource(cs, cs);
            }
            elog.Source = cs;
            elog.EnableRaisingEvents = true;
            elog.WriteEntry(message);
        }
    }
}
