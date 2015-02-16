using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.CDRAnalysis.Mobile
{
    public partial class EmailReceiver
    {
        public static EmailReceiver Load(int ID)
        {
            EmailReceiver EmailReceiver = new EmailReceiver();
            try
            {
                using (Entities context = new Entities())
                {
                    EmailReceiver = context.EmailReceivers
                     .Where(u => u.Id == ID)
                     .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.CDRAnalysis.Mobile.EmailReceiver.Load(" + ID.ToString() + ")", err);
            }
            return EmailReceiver;
        }

        public static EmailReceiver Save(EmailReceiver EmailReceiver)
        {
            EmailReceiver CurrentEmailReceiver = new EmailReceiver();
            try
            {
                using (Entities context = new Entities())
                {
                    if (EmailReceiver.Id == 0)
                    {
                        context.EmailReceivers.Add(EmailReceiver);
                    }
                    else
                    {
                        context.Entry(EmailReceiver).State = System.Data.EntityState.Modified;
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.CDRAnalysis.Mobile.EmailReceiver.Save(" + EmailReceiver.Id.ToString() + ")", err);
            }
            return CurrentEmailReceiver;
        }

        public static bool Delete(EmailReceiver emailReceiver)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {
                    context.Entry(emailReceiver).State = System.Data.EntityState.Deleted;
                    context.SaveChanges();
                }
                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.CDRAnalysis.Mobile.EmailReceiver.Delete(" + emailReceiver.Id.ToString() + ")", err);
            }
            return success;
        }

        public static bool Delete(int ID)
        {
            EmailReceiver emailReceiver = new EmailReceiver();
            emailReceiver.Id = ID;
            return Delete(emailReceiver);
        }

        public static List<EmailReceiver> GetEmailReceivers(string email, int EmailTemplateID, int EmailReceiverTypeID)
        {
            List<EmailReceiver> EmailCCsList = new List<EmailReceiver>();

            try
            {
                using (Entities context = new Entities())
                {
                    EmailCCsList = context.EmailReceivers.Include(u => u.EmailReceiverType).Include(u => u.EmailTemplate)
                                       .Where(u => u.Id > 0
                                         && (u.Email.Contains(email))
                                         && (EmailTemplateID == 0 || u.EmailTemplateID == EmailTemplateID)
                                         && (EmailReceiverTypeID == 0 || u.EmailReceiverTypeID == EmailReceiverTypeID)
                                        )
                                         .OrderBy(u => u.Email)
                                         .ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.CDRAnalysis.Mobile.EmailReceiver.GetEmailReceivers(" + email + ")", err);
            }


            return EmailCCsList;
        }
    }
}
