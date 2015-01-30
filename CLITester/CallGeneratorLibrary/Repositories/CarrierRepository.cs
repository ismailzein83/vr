using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallGeneratorLibrary.Repositories
{
    public class CarrierRepository
    {
        public static Carrier Load(int CarrierId)
        {
            Carrier log = new Carrier();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    log = context.Carriers.Where(l => l.Id == CarrierId).FirstOrDefault<Carrier>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }

            return log;
        }
        public static List<Carrier> LoadbyUserID(int UserId)
        {
            List<Carrier> log = new List<Carrier>();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    log = context.Carriers.Where(l => l.CustomerId == UserId).ToList<Carrier>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }

            return log;
        }

        public static bool ExistShortName(string ShortName)
        {
            bool Exist = false;
            List<Carrier> LstCarriers = new List<Carrier>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    LstCarriers = context.Carriers.Where(l => l.ShortName == ShortName).ToList<Carrier>();
                    if (LstCarriers.Count > 0)
                        Exist = true;
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }

            return Exist;
        }


        public static List<Carrier> GetCarriers(int UserId)
        {
            List<Carrier> LstCarriers = new List<Carrier>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    LstCarriers = context.Carriers.Where(x => x.CustomerId ==  UserId).ToList<Carrier>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return LstCarriers;
        }

        public static List<Carrier> GetCarriers(string name, string country)
        {
            List<Carrier> LstCarriers = new List<Carrier>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    LstCarriers = context.Carriers.Where(x => (name == "" || x.Name.Contains(name))).ToList<Carrier>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return LstCarriers;
        }

        public static bool Save(Carrier oper)
        {
            bool success = false;
            if (oper.Id == default(int))
                success = Insert(oper);
            else
                success = Update(oper);
            return success;
        }

        private static bool Insert(Carrier oper)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.Carriers.InsertOnSubmit(oper);
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

        private static bool Update(Carrier oper)
        {
            bool success = false;
            Carrier look = new Carrier();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    look = context.Carriers.Single(l => l.Id == oper.Id);
                    look.CustomerId = oper.CustomerId;
                    look.Name = oper.Name;
                    look.Prefix = oper.Prefix;
                    look.ShortName = oper.ShortName;
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

        public static bool Delete(int userId)
        {
            bool success = false;

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    Carrier carrier = context.Carriers.Where(u => u.Id == userId).Single<Carrier>();
                    context.Carriers.DeleteOnSubmit(carrier);
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
