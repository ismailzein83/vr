using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace CLINumberLibrary
{
    public class PhoneNumberRepository
    {
        public static List<PhoneNumber> GetPhoneNumbers()
        {
            List<PhoneNumber> LstPhoneNumbers = new List<PhoneNumber>();
            try
            {
                using (CLINumberModelDataContext context = new CLINumberModelDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<PhoneNumber>(c => c.Operator);
                    context.LoadOptions = options;

                    LstPhoneNumbers = context.PhoneNumbers.OrderBy(x => x.CreationDate).ToList<PhoneNumber>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return LstPhoneNumbers;
        }

        public static PhoneNumber Load(int phoneNumberId)
        {
            PhoneNumber log = new PhoneNumber();

            try
            {
                using (CLINumberModelDataContext context = new CLINumberModelDataContext())
                {
                    log = context.PhoneNumbers.Where(l => l.Id == phoneNumberId).FirstOrDefault<PhoneNumber>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return log;
        }

        public static PhoneNumber GetFreePhoneNumber(int operatorId)
        {
            PhoneNumber phoneNumber = new PhoneNumber();
            try
            {
                using (CLINumberModelDataContext context = new CLINumberModelDataContext())
                {
                    phoneNumber = context.PhoneNumbers.Where(l => l.OperatorId == operatorId && (l.Status == (int)CLINumberLibrary.Utilities.Enums.PhoneNumberStatus.Free)).FirstOrDefault<PhoneNumber>();
                    if (phoneNumber != null)
                    {
                        phoneNumber.Status = (int)CLINumberLibrary.Utilities.Enums.PhoneNumberStatus.Busy;
                        phoneNumber.LastCallDate = DateTime.Now;
                        context.SubmitChanges();
                    }
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return phoneNumber;
        }

        public static PhoneNumber FreeThisPhoneNumber(int phoneNumberId)
        {
            PhoneNumber phone = new PhoneNumber();
            try
            {
                using (CLINumberModelDataContext context = new CLINumberModelDataContext())
                {
                    phone = context.PhoneNumbers.Where(l => l.Id == phoneNumberId).FirstOrDefault<PhoneNumber>();
                    if (phone != null)
                    {
                        phone.Status = (int)CLINumberLibrary.Utilities.Enums.PhoneNumberStatus.Free;
                        context.SubmitChanges();
                    }
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return phone;
        }

        public static void FreeAllLockedPhone()
        {
            //Expiry minutes
            int expTime = 0;
            int.TryParse(ConfigurationSettings.AppSettings["ExpiryTime"], out expTime);

            List<PhoneNumber> lstPhones = GetLockedPhoneNumbers();
            for (int i = 0; i < lstPhones.Count(); i++)
            {
                TimeSpan span = new TimeSpan();
                span = DateTime.Now - lstPhones[i].LastCallDate.Value;

                if (span.TotalMinutes > expTime)
                {
                    try
                    {
                        lstPhones[i].Status = (int)CLINumberLibrary.Utilities.Enums.PhoneNumberStatus.Free;
                        Save(lstPhones[i]);
                    }
                    catch (System.Exception ex)
                    {
                        Logger.LogException(ex);
                    }
                }
            }
        }

        public static List<PhoneNumber> GetLockedPhoneNumbers()
        {
            List<PhoneNumber> LstPhoneNumbers = new List<PhoneNumber>();
            try
            {
                using (CLINumberModelDataContext context = new CLINumberModelDataContext())
                {
                    LstPhoneNumbers = context.PhoneNumbers.Where(l => l.Status == (int)CLINumberLibrary.Utilities.Enums.PhoneNumberStatus.Busy).OrderBy(x => x.CreationDate).ToList<PhoneNumber>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return LstPhoneNumbers;
        }

        public static bool Save(PhoneNumber oper)
        {
            bool success = false;
            if (oper.Id == default(int))
                success = Insert(oper);
            else
                success = Update(oper);
            return success;
        }

        private static bool Insert(PhoneNumber oper)
        {
            bool success = false;
            try
            {
                using (CLINumberModelDataContext context = new CLINumberModelDataContext())
                {
                    context.PhoneNumbers.InsertOnSubmit(oper);
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

        private static bool Update(PhoneNumber oper)
        {
            bool success = false;
            PhoneNumber look = new PhoneNumber();

            try
            {
                using (CLINumberModelDataContext context = new CLINumberModelDataContext())
                {
                    look = context.PhoneNumbers.Single(l => l.Id == oper.Id);

                    look.Number = oper.Number;
                    look.OperatorId = oper.OperatorId;
                    look.CreationDate = oper.CreationDate;
                    look.Status = oper.Status;
                    look.Prefix = oper.Prefix;
                    look.LastCallDate = oper.LastCallDate;
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

        public static bool Delete(int phoneNumberId)
        {
            bool success = false;

            try
            {
                using (CLINumberModelDataContext context = new CLINumberModelDataContext())
                {
                    PhoneNumber phoneNumber = context.PhoneNumbers.Where(u => u.Id == phoneNumberId).Single<PhoneNumber>();
                    context.PhoneNumbers.DeleteOnSubmit(phoneNumber);
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
