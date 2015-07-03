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
        public static SipAccount GetTop()
        {
            SipAccount sipAccount = new SipAccount();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    sipAccount = context.SipAccounts.FirstOrDefault<SipAccount>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return sipAccount;
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
                Logger.LogException(ex);
            }
            return success;
        }

        private static bool Update(SipAccount sipAccount)
        {
            bool success = false;
            SipAccount sipAccountObj = new SipAccount();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    sipAccountObj = context.SipAccounts.Single(l => l.Id == sipAccount.Id);

                    sipAccountObj.Username = sipAccount.Username;
                    sipAccountObj.Login = sipAccount.Login;
                    sipAccountObj.Password = sipAccount.Password;
                    sipAccountObj.Server = sipAccount.Server;
                    sipAccountObj.UseProxy = sipAccount.UseProxy;
                    sipAccountObj.ProxyServer = sipAccount.ProxyServer;
                    sipAccountObj.ProxyUser = sipAccount.ProxyUser;
                    sipAccountObj.ProxyPass = sipAccount.ProxyPass;
                    sipAccountObj.DisplayName = sipAccount.DisplayName;
                    sipAccountObj.TotalLines = sipAccount.TotalLines;
                    sipAccountObj.UseAudio = sipAccount.UseAudio;
                    sipAccountObj.DefaultAudioFileName = sipAccount.DefaultAudioFileName;
                    sipAccountObj.DefaultAudioFile = sipAccount.DefaultAudioFile;
                    sipAccountObj.CreatedBy = sipAccount.CreatedBy;
                    sipAccountObj.CreationDate = sipAccount.CreationDate;
                    sipAccountObj.PlaybackDevice = sipAccount.PlaybackDevice;
                    sipAccountObj.NetworkInterface = sipAccount.NetworkInterface;
                    sipAccountObj.Codecs = sipAccount.Codecs;
                    sipAccountObj.IsChangedCallerId = sipAccount.IsChangedCallerId;
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
    }
}
