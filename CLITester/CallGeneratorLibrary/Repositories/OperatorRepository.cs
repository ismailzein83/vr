using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallGeneratorLibrary.Repositories
{
    public class OperatorRepository
    {
        public static Operator Load(int OperatorId)
        {
            Operator log = new Operator();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    log = context.Operators.Where(l => l.Id == OperatorId).FirstOrDefault<Operator>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }

            return log;
        }

        public static Operator Load(string mcc, string mnc)
        {
            Operator log = new Operator();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    log = context.Operators.Where(l => l.mcc == mcc && l.mnc == mnc).FirstOrDefault<Operator>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }

            return log;
        }

        public static List<Operator> GetOperators()
        {
            List<Operator> LstOperators = new List<Operator>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    LstOperators = context.Operators.OrderBy(x => x.Country).ToList<Operator>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return LstOperators;
        }

        public static List<Operator> GetOperators(string name, string country)
        {
            List<Operator> LstOperators = new List<Operator>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    LstOperators = context.Operators.Where(x => (name == "" || x.Name.Contains(name)) && ( country == "" || x.Country.Contains(country))).ToList<Operator>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return LstOperators;
        }


        public static List<Operator> GetOperatorMap(string countryCode)
        {
            List<Operator> LstOperators = new List<Operator>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    LstOperators = context.Operators.Where(x => (countryCode == "" || x.CountryPicture == countryCode)).ToList<Operator>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return LstOperators;
        }

        public static bool Delete(int operatorId)
        {
            bool success = false;

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    Operator ooperator = context.Operators.Where(u => u.Id == operatorId).Single<Operator>();
                    context.Operators.DeleteOnSubmit(ooperator);
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

        public static bool Save(Operator oper)
        {
            bool success = false;
            if (oper.Id == default(int))
                success = Insert(oper);
            else
                success = Update(oper);
            return success;
        }

        private static bool Insert(Operator oper)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.Operators.InsertOnSubmit(oper);
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

        private static bool Update(Operator oper)
        {
            bool success = false;
            Operator look = new Operator();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    look = context.Operators.Single(l => l.Id == oper.Id);

                    look.Name = oper.Name;
                    look.mcc = oper.mcc;
                    look.mnc = oper.mnc;
                    look.Country = oper.Country;
                    look.ServiceAndroid = oper.ServiceAndroid;
                    look.ServiceMonty = oper.ServiceMonty;
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

        public static bool InsertLog(OperatorLog oper)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.OperatorLogs.InsertOnSubmit(oper);
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
