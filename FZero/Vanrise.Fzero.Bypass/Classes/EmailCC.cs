using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class EmailCC
    {
        public static EmailCC Load(int ID)
        {
            EmailCC EmailCC = new EmailCC();
            try
            {
                using (Entities context = new Entities())
                {
                    EmailCC = context.EmailCCs.Include(u => u.MobileOperator).Include(u => u.MobileOperator.User).Include(u => u.Client)
                     .Where(u => u.Id == ID)
                     .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.EmailCC.Load(" + ID.ToString() + ")", err);
            }
            return EmailCC;
        }
        
        public static EmailCC Save(EmailCC EmailCC)
        {
            EmailCC CurrentEmailCC = new EmailCC();
            try
            {
                using (Entities context = new Entities())
                {
                    if (EmailCC.Id == 0)
                    {
                        context.EmailCCs.Add(EmailCC);
                    }
                    else
                    {
                        EmailCC.Client = null;
                        EmailCC.MobileOperator = null;
                        context.Entry(EmailCC).State = System.Data.EntityState.Modified;
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.EmailCC.Save(" + EmailCC.Id.ToString() + ")", err);
            }
            return CurrentEmailCC;
        }

        public static bool Delete(EmailCC emailCC)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {
                    context.Entry(emailCC).State = System.Data.EntityState.Deleted;
                    context.SaveChanges();
                }
                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.EmailCC.Delete(" + emailCC.Id.ToString() + ")", err);
            }
            return success;
        }

        public static bool Delete(int ID)
        {
            EmailCC emailCC = new EmailCC();
            emailCC.Id = ID;
            return Delete(emailCC);
        }

        public static string GetEmailCCs(int MobileOperatorID, int clientID)
        {
            List<EmailCC> EmailCCsList = new List<EmailCC>();

            string CCs = string.Empty;

           


            try
            {
                using (Entities context = new Entities())
                {
                    EmailCCsList = context.EmailCCs.Include(u => u.MobileOperator).Include(u => u.MobileOperator.User).Include(u => u.Client)
                                       .Where(u => u.Id > 0
                                          && (MobileOperatorID == 0 || u.MobileOperatorID == MobileOperatorID || u.MobileOperatorID ==null )
                                         && (clientID == 0 || u.ClientID == clientID)
                                        )
                                         .OrderBy(u => u.Email)
                                         .ToList();


                    foreach (var i in EmailCCsList)
                    {
                        CCs = CCs + i.Email + ";";
                    }


                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.EmailCC.GetEmailCCs(" + MobileOperatorID.ToString() + ")", err);
            }


            return CCs;
        }

        public static string GetClientEmailCCs(int clientID)
        {
            List<EmailCC> EmailCCsList = new List<EmailCC>();

            string CCs = string.Empty;




            try
            {
                using (Entities context = new Entities())
                {
                    EmailCCsList = context.EmailCCs.Include(u => u.MobileOperator).Include(u => u.MobileOperator.User).Include(u => u.Client)
                                       .Where(u => u.Id > 0
                                          && (u.MobileOperatorID == null)
                                         && (clientID == 0 || u.ClientID == clientID)
                                        )
                                         .OrderBy(u => u.Email)
                                         .ToList();


                    foreach (var i in EmailCCsList)
                    {
                        CCs = CCs + i.Email + ";";
                    }


                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.EmailCC.GetEmailCCs(" + clientID.ToString() + ")", err);
            }


            return CCs;
        }

        public static List<EmailCC> GetEmailCCs(int MobileOperatorID, string email, int clientID)
        {
            List<EmailCC> EmailCCsList = new List<EmailCC>();

            try
            {
                using (Entities context = new Entities())
                {
                    EmailCCsList = context.EmailCCs.Include(u => u.MobileOperator).Include(u => u.MobileOperator.User).Include(u => u.Client)
                                       .Where(u => u.Id > 0
                                         && (u.Email.Contains(email))
                                         && (MobileOperatorID == 0 || u.MobileOperatorID == MobileOperatorID)
                                         && (clientID == 0 || u.ClientID == clientID)
                                        )
                                         .OrderBy(u => u.Email)
                                         .ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.EmailCC.GetEmailCCs(" + MobileOperatorID.ToString() + ", " + email + ")", err);
            }


            return EmailCCsList;
        }
    }
}
