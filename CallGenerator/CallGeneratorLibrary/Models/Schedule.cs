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
        string testgroupId = "";

        public string TestGroupId
        {
            get
            {
                try
                {
                    if (testgroupId == "")
                    {
                        ScheduleGroup GroupId = new ScheduleGroup();
                        GroupId = ScheduleGroupRepository.LoadGroup(this.Id);

                        testgroupId = GroupId.GroupId.ToString();
                    }

                    return testgroupId;
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

                        List<ScheduleNumbers> lstOp = new List<ScheduleNumbers>();
                        lstOp = ScheduleGroupRepository.GetScheduleNumbers(this.Id);
                        foreach (ScheduleNumbers op in lstOp)
                            operatorprefix += op.Number + "!";
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
            string cs = "VanCallGen";
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
