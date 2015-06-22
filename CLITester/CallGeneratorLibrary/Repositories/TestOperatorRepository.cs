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
    public class ChartCall
    {
        public int ChartId { get; set; }
        public int Total { get; set; }
        public string label { get; set; }
    }

    public class TestOperatorRepository
    {
        public static String GetLabel(int status)
        {
            if (status == 1) return "Delivered"; else return "Not Delivered";
        }

        public static String GetLabelUser(int UserId)
        {
            return UserRepository.Load(UserId).UserName;
        }

        public static List<ChartCall> GetChartCalls(int status, int userId)
        {
            List<ChartCall> LstChartCalls = new List<ChartCall>();
            List<ChartCalls> LstOp = new List<ChartCalls>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    LstOp = context.GetChartTotals1(status, userId).GetResult<ChartCalls>().ToList<ChartCalls>();
                    String lbl = GetLabel(status);
                    for (int i = 1; i <= 31; i++)
                    {
                        ChartCall c = new ChartCall();
                        c.ChartId = i;
                        c.Total = 0;
                        c.label = lbl;
                        foreach (ChartCalls cc in LstOp)
                        {
                            if (cc.CreationDate.Value.Day == i)
                            {
                                c.Total = cc.TotalCalls.Value;
                                break;
                            }
                        }
                        LstChartCalls.Add(c);
                    }
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return LstChartCalls;
        }

        public static List<ChartCall> GetChartCallsUser(int userId)
        {
            List<ChartCall> LstChartCalls = new List<ChartCall>();
            List<ChartCalls> LstOp = new List<ChartCalls>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    LstOp = context.GetChartTotals1(null, userId).GetResult<ChartCalls>().ToList<ChartCalls>();
                    String lbl = GetLabelUser(userId);
                    for (int i = 1; i <= 31; i++)
                    {
                        ChartCall c = new ChartCall();
                        c.ChartId = i;
                        c.Total = 0;
                        c.label = lbl;
                        foreach (ChartCalls cc in LstOp)
                        {
                            if (cc.CreationDate.Value.Day == i)
                            {
                                c.Total = cc.TotalCalls.Value;
                                break;
                            }
                        }
                        LstChartCalls.Add(c);
                    }
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return LstChartCalls;
        }

        public static int GetTotalCalls(int userId)
        {
            int Total = 0;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    Total = context.TestOperators.Where(l => l.CreationDate.Value.Month == DateTime.Now.Month && l.UserId == userId).Count();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return Total;
        }

        public static int GetCLIDeliv(int userId)
        {
            int Total = 0;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    Total = context.TestOperators.Where(l => l.CreationDate.Value.Month == DateTime.Now.Month && l.Status == 1 && l.UserId == userId).Count();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return Total;
        }

        public static int GetCLINonDeliv(int userId)
        {
            int Total = 0;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    Total = context.TestOperators.Where(l => l.CreationDate.Value.Month == DateTime.Now.Month && l.Status == 2 && l.UserId == userId).Count();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return Total;
        }

        public static int GetMapCountry(int? status, int userId, int OperatorId)
        {
            int Total = 0;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    if (status == null)
                        Total = context.TestOperators.Where(l => l.CreationDate.Value.Month == DateTime.Now.Month && l.UserId == userId && l.OperatorId == OperatorId).Count();
                    else
                        Total = context.TestOperators.Where(l => l.CreationDate.Value.Month == DateTime.Now.Month && l.Status == status && l.UserId == userId && l.OperatorId == OperatorId).Count();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return Total;
        }

        public static int GetPercentage(string prefix, int? status, int userId)
        {
            int Total = 0;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    if(status == null)
                        Total = context.TestOperators.Where(l => l.CreationDate.Value.Month == DateTime.Now.Month && l.UserId == userId && l.CarrierPrefix == prefix).Count();
                    else
                        Total = context.TestOperators.Where(l => l.CreationDate.Value.Month == DateTime.Now.Month && l.Status == status && l.UserId == userId && l.CarrierPrefix == prefix).Count();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return Total;
        }
        
        public static List<DataCalls> GetDataCalls()
        {
            List<DataCalls> LstOperators = new List<DataCalls>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    LstOperators = context.GetData1().GetResult<DataCalls>().ToList<DataCalls>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return LstOperators;
        }
        
        public static List<TestOperator> GetTestCalls(int userId)
        {
            List<TestOperator> LstOperators = new List<TestOperator>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<TestOperator>(c => c.Operator);
                    context.LoadOptions = options;

                    LstOperators = context.TestOperators.Where(x => (x.UserId == userId && x.EndDate == null && x.ScheduleId == null)).OrderByDescending(l => l.Id).ToList<TestOperator>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return LstOperators;
        }

        public static List<TestOperatorHistory> GetTestOperatorHistory(DateTime? StartDate, DateTime? EndDate,  int? OperatorId, int? DisplayStart, int? DisplayLength)
        {
            List<TestOperatorHistory> LstScheduleNumbers = new List<TestOperatorHistory>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    LstScheduleNumbers = context.GetTestOperators1(StartDate, EndDate, OperatorId, DisplayStart, DisplayLength).GetResult<TestOperatorHistory>().ToList<TestOperatorHistory>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
            return LstScheduleNumbers;
        }


        public static TestOperator Load(int TestOperatorId)
        {
            TestOperator log = new TestOperator();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<TestOperator>(c => c.Operator);
                    context.LoadOptions = options;

                    log = context.TestOperators.Where(l => l.Id == TestOperatorId).FirstOrDefault<TestOperator>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }

            return log;
        }

        public static List<TestOperator> GetTestOperatorsByUserId(int userId)
        {
            List<TestOperator> LstOperators = new List<TestOperator>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<TestOperator>(c => c.Operator);
                    options.LoadWith<TestOperator>(c => c.Schedule);
                    context.LoadOptions = options;

                    LstOperators = context.TestOperators.Where(x => (x.UserId == userId)).OrderByDescending(l => l.Id).ToList<TestOperator>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return LstOperators;
        }

        public static List<TestOperator> GetTestOperatorsByScheduleId(int ScheduleId)
        {
            List<TestOperator> LstOperators = new List<TestOperator>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<TestOperator>(c => c.Operator);
                    options.LoadWith<TestOperator>(c => c.Schedule);
                    context.LoadOptions = options;

                    LstOperators = context.TestOperators.Where(x => (x.ScheduleId == ScheduleId)).OrderByDescending(l => l.Id).Take(100).ToList<TestOperator>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return LstOperators;
        }

        public static List<TestOperator> GetTestOperatorsByScheduleLogId(DateTime d, int ScheduleId)
        {
            List<TestOperator> LstOperators = new List<TestOperator>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<TestOperator>(c => c.Operator);
                    options.LoadWith<TestOperator>(c => c.Schedule);
                    context.LoadOptions = options;

                    LstOperators = context.TestOperators.Where(x => (x.ScheduleId == ScheduleId) && (x.CreationDate >= d)).OrderByDescending(l => l.Id).ToList<TestOperator>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return LstOperators;
        }

        public static List<TestOperator> GetMontyTestOperators()
        {
            List<TestOperator> LstOperators = new List<TestOperator>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<TestOperator>(c => c.Operator);
                    options.LoadWith<TestOperator>(c => c.User);
                    context.LoadOptions = options;

                    LstOperators = context.TestOperators.Where(x =>  x.EndDate == null && x.Requested == null).ToList<TestOperator>();

                    for (int i = 0; i < LstOperators.Count(); i++)
                    {
                        TestOperator op = new TestOperator();
                        op = LstOperators[i];
                        op.Requested = true;

                        DateTime dt = op.CreationDate.Value;
                        TimeSpan span = new TimeSpan();
                        if (dt != null)
                            span = DateTime.Now - dt;
                        double totalSeconds = span.TotalSeconds;

                        int exptime = 180;//seconds

                        if (totalSeconds >= exptime)
                        {
                            op.ErrorMessage = exptime + " sec - EXPIRED";
                            op.EndDate = DateTime.Now;
                            LstOperators.Remove(op);
                        }
                        TestOperatorRepository.Save(op);
                    }
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return LstOperators;
        }

        public static List<TestOperator> GetRequestedTestOperators()
        {
            List<TestOperator> LstOperators = new List<TestOperator>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<TestOperator>(c => c.Operator);
                    options.LoadWith<TestOperator>(c => c.User);
                    context.LoadOptions = options;

                    LstOperators = context.TestOperators.Where(x => x.EndDate == null && x.Requested == true).ToList<TestOperator>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return LstOperators;
        }

        public static int  GetRequestedTestOperatorsByUser(int ParentUserId)
        {
            //List<TestOperator> LstOperators = new List<TestOperator>();
            int count = 0;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    count = context.TestOperators.Where(x => x.EndDate == null && x.ParentUserId == ParentUserId).ToList<TestOperator>().Count;
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return count;
        }


        public static bool Save(TestOperator testOperator)
        {
            bool success = false;
            if (testOperator.Id == default(int))
                success = Insert(testOperator);
            else
                success = Update(testOperator);
            return success;
        }

        private static bool Insert(TestOperator testOperator)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.TestOperators.InsertOnSubmit(testOperator);
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

        private static bool Update(TestOperator testOperator)
        {
            bool success = false;
            TestOperator look = new TestOperator();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    look = context.TestOperators.Single(l => l.Id == testOperator.Id);

                    look.UserId = testOperator.UserId;
                    look.OperatorId = testOperator.OperatorId;
                    look.NumberOfCalls = testOperator.NumberOfCalls;
                    look.CreationDate = testOperator.CreationDate;
                    look.EndDate = testOperator.EndDate;
                    look.TestCli = testOperator.TestCli;
                    look.ReceivedCli = testOperator.ReceivedCli;
                    look.Status = testOperator.Status;
                    look.CarrierPrefix = testOperator.CarrierPrefix;
                    look.CallerId = testOperator.CallerId;
                    look.ErrorMessage = testOperator.ErrorMessage;
                    look.Requested = testOperator.Requested;
                    look.ParentUserId = testOperator.ParentUserId;
                    look.PhonePrefix = testOperator.PhonePrefix;
                    look.PDD = testOperator.PDD;
                    look.Duration = testOperator.Duration;
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
            string cs = "Service CallGen";
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
