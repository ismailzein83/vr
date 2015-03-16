﻿using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.MobileCDRAnalysis
{
    public partial class prGetEmails_Result
    {
        public static List<prGetEmails_Result> GetAllEmails(string DestinationEmail, DateTime? FromDate, DateTime? ToDate)
        {
            try
            {
                using (MobileEntities context = new MobileEntities())
                {
                    //return context.prGetEmails( DestinationEmail, FromDate, ToDate).ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.MobileCDRAnalysis.Email.GetAllEmails()", err);
            }

            return null;
        }

        public static bool SendMail(string profile_name, string Subject, string Body, string To, string CC, string BCC)
        {
            try
            {

                using (MobileEntities context = new MobileEntities())
                {
                    //context.prSendMail(profile_name, To, Subject, CC, Body, BCC);
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in prGetEmails_Result.SendMail", err);
                return false;
            }
            return true;
        }

        public static bool SendMailWithAttachement(string profile_name,string To, string AttachmentPath,  string Subject,string CC, string Body, string BCC )
        {
            try
            {

                using (MobileEntities context = new MobileEntities())
                {
                    //context.prSendMailWithAttachment(profile_name, To, Subject, CC, Body, AttachmentPath, BCC);
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in prGetEmails_Result.SendMailWithAttachement", err);
                return false;
            }
            return true;
        }

        public static void DeleteEmail(string Ids)
        {
            try
            {

                using (MobileEntities context = new MobileEntities())
                {
                    //context.prDeleteEmail(Ids);
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in prGetEmails_Result.DeleteEmail()", err);
            }

        }
    }
}

    



