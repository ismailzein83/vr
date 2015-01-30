using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CallGeneratorLibrary.Models;
using CallGeneratorLibrary.Interfaces;
using System.Diagnostics;

namespace CallGeneratorLibrary.Repositories
{
    public class LookupRepository
    {
        public static List<Lookup> GetLookups()
        {
            List<Lookup> lookups = new List<Lookup>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    lookups = context.Lookups.ToList<Lookup>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }

            return lookups;
        }

        public static List<Lookup> SearchLookups(string name)
        {
            List<Lookup> lookups = new List<Lookup>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    //lookups = context.SearchLookups(name).ToList();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }

            return lookups;
        }

        public static Lookup Load(int lookupId)
        {
            Lookup lookup = new Lookup();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    lookup = context.Lookups.Where(l => l.Id == lookupId).FirstOrDefault<Lookup>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }

            return lookup;
        }

        public static bool Delete(int lookupId)
        {
            bool success = false;

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    Lookup look = context.Lookups.Where(lk => lk.Id == lookupId).Single<Lookup>();
                    context.Lookups.DeleteOnSubmit(look);
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

        public static bool Save(Lookup lookup)
        {
            bool success = false;
            if (lookup.Id == default(int))
                success = Insert(lookup);
            else
                success = Update(lookup);
            return success;
        }

        private static bool Insert(Lookup lookup)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.Lookups.InsertOnSubmit(lookup);
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

        private static bool Update(Lookup lookup)
        {
            bool success = false;
            Lookup look = new Lookup();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    look = context.Lookups.Single(l => l.Id == lookup.Id);
                    look.Name = lookup.Name;
                    look.Code = lookup.Code;
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
