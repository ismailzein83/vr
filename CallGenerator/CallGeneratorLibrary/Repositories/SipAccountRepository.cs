using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallGeneratorLibrary.Repositories
{
    public class SipAccountRepository
    {
        public static SipAccount Load(int Id)
        {
            SipAccount log = new SipAccount();

            try
            {
                WriteToEventLogEx("sip rep");
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    log = context.SipAccounts.Where(l => l.Id == Id).FirstOrDefault<SipAccount>();
                    WriteToEventLogEx(log.Username);
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }

            return log;
        }

        public static SipAccount LoadbyUser(int UserId)
        {
            SipAccount log = new SipAccount();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    log = context.SipAccounts.Where(l => l.UserId == UserId).FirstOrDefault<SipAccount>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }

            return log;
        }

        public static List<SipAccount> GetSipAccounts()
        {
            List<SipAccount> LstSipAccounts = new List<SipAccount>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<SipAccount>(c => c.User);
                    context.LoadOptions = options;

                    LstSipAccounts = context.SipAccounts.ToList<SipAccount>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return LstSipAccounts;
        }

        public static bool Save(SipAccount sipAccount)
        {
            bool success = false;
            if (sipAccount.Id == default(int))
                success = Insert(sipAccount);
            else
                success = Update(sipAccount);
            return success;
        }

        private static bool Insert(SipAccount sipAccount)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.SipAccounts.InsertOnSubmit(sipAccount);
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

        private static bool Update(SipAccount sipAccount)
        {
            bool success = false;
            SipAccount look = new SipAccount();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    look = context.SipAccounts.Single(l => l.Id == sipAccount.Id);

                    look.Username = sipAccount.Username;
                    look.Login = sipAccount.Login;
                    look.Password = sipAccount.Password;
                    look.Server = sipAccount.Server;
                    look.UseProxy = sipAccount.UseProxy;
                    look.ProxyServer = sipAccount.ProxyServer;
                    look.ProxyUser = sipAccount.ProxyUser;
                    look.ProxyPass = sipAccount.ProxyPass;
                    look.DisplayName = sipAccount.DisplayName;
                    look.TotalLines = sipAccount.TotalLines;
                    look.UseAudio = sipAccount.UseAudio;
                    look.DefaultAudioFileName = sipAccount.DefaultAudioFileName;
                    look.DefaultAudioFile = sipAccount.DefaultAudioFile;
                    look.CreatedBy = sipAccount.CreatedBy;
                    look.CreationDate = sipAccount.CreationDate;
                    look.PlaybackDevice = sipAccount.PlaybackDevice;
                    look.NetworkInterface = sipAccount.NetworkInterface;
                    look.Codecs = sipAccount.Codecs;
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
