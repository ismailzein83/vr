using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.CDRAnalysis.Mobile
{
    public partial class EmailTemplate
    {
        public static EmailTemplate Load(int ID)
        {
            EmailTemplate EmailTemplate = new EmailTemplate();
            try
            {
                using (Entities context = new Entities())
                {
                    EmailTemplate = context.EmailTemplates
                    .Where(u => u.ID == ID)
                    .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.CDRAnalysis.Mobile.EmailTemplate.Load(" + ID.ToString() + ")", err);
            }
            return EmailTemplate;
        }

        public static EmailTemplate Load_With_Receivers(int ID)
        {
            EmailTemplate EmailTemplate = new EmailTemplate();
            try
            {
                using (Entities context = new Entities())
                {
                    EmailTemplate = context.EmailTemplates.Include(u => u.EmailReceivers)
                    .Where(u => u.ID == ID)
                    .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.CDRAnalysis.Mobile.EmailTemplate.Load(" + ID.ToString() + ")", err);
            }
            return EmailTemplate;
        }

        public static List<EmailTemplate> GetEmailTemplates(string name, bool? isActive)
        {
            List<EmailTemplate> EmailTemplatesList= new List<EmailTemplate>();

            try
            {
                using (Entities context = new Entities())
                {

                    EmailTemplatesList = context.EmailTemplates
                        .Where(x => x.Name.Contains(name) && (!isActive.HasValue || x.IsActive == isActive))
                        .OrderBy(x => x.Name)
                        .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.CDRAnalysis.Mobile.EmailTemplate.GetEmailTemplates(" + name + ", " + isActive.ToString() + ")", err);
            }


            return EmailTemplatesList;
        }

        public static List<EmailTemplate> GetEmailTemplates()
        {
            List<EmailTemplate> EmailTemplatesList= new List<EmailTemplate>();

            try
            {
                using (Entities context = new Entities())
                {

                    EmailTemplatesList = context.EmailTemplates
                        .OrderBy(x => x.Name)
                        .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.CDRAnalysis.Mobile.EmailTemplate.GetEmailTemplates()", err);
            }


            return EmailTemplatesList;
        }

        public static bool Save(EmailTemplate EmailTemplate)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {
                    EmailTemplate.LastUpdateDate = DateTime.Now;
                    context.EmailTemplates.Attach(EmailTemplate);
                    context.Entry(EmailTemplate).State = System.Data.EntityState.Modified;
                    context.SaveChanges();
                    success = true;
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.CDRAnalysis.Mobile.EmailTemplate.Save(" + EmailTemplate.ID.ToString() + ")", err);
            }
            return success;
        }
    }
}
