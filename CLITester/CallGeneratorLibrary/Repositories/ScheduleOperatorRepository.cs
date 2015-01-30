using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallGeneratorLibrary.Utilities;
using System.Diagnostics;

namespace CallGeneratorLibrary.Repositories
{
    public class ScheduleOperatorRepository
    {
        public static ScheduleOperator Load(int ScheduleOperatorId)
        {
            ScheduleOperator log = new ScheduleOperator();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<ScheduleOperator>(c => c.Operator);
                    options.LoadWith<ScheduleOperator>(c => c.Carrier);
                    context.LoadOptions = options;

                    log = context.ScheduleOperators.Where(l => l.Id == ScheduleOperatorId).FirstOrDefault<ScheduleOperator>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }

            return log;
        }
        public static List<ScheduleOperator> GetScheduleOperatorsByScheduleId(int ScheduleId)
        {
            List<ScheduleOperator> LstOperators = new List<ScheduleOperator>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<ScheduleOperator>(c => c.Operator);
                    options.LoadWith<ScheduleOperator>(c => c.Carrier);
                    context.LoadOptions = options;

                    LstOperators = context.ScheduleOperators.Where(x => (x.ScheduleId == ScheduleId)).ToList<ScheduleOperator>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return LstOperators;
        }

        public static bool Save(ScheduleOperator ScheduleOperator)
        {
            bool success = false;
            if (ScheduleOperator.Id == default(int))
                success = Insert(ScheduleOperator);
            else
                success = Update(ScheduleOperator);
            return success;
        }

        public static bool DeleteAll(int ScheduleId)
        {
            bool success = false;

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    List<ScheduleOperator> quafunc = context.ScheduleOperators.Where(lk => lk.ScheduleId == ScheduleId).ToList<ScheduleOperator>();
                    context.ScheduleOperators.DeleteAllOnSubmit(quafunc);
                    context.SubmitChanges();
                    success = true;
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
            return success;
        }

        private static bool Insert(ScheduleOperator ScheduleOperator)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.ScheduleOperators.InsertOnSubmit(ScheduleOperator);
                    context.SubmitChanges();
                    success = true;
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return success;
        }

        private static bool Update(ScheduleOperator ScheduleOperator)
        {
            bool success = false;
            ScheduleOperator look = new ScheduleOperator();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    look = context.ScheduleOperators.Single(l => l.Id == ScheduleOperator.Id);

                    look.ScheduleId = ScheduleOperator.ScheduleId;
                    look.OperatorId = ScheduleOperator.OperatorId;
                    look.CarrierId = ScheduleOperator.CarrierId;
                    context.SubmitChanges();
                    success = true;
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return success;
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
