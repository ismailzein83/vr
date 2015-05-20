﻿using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.MobileCDRAnalysis
{
    public partial class EmailReceiverType
    {
        public static List<EmailReceiverType> GetEmailReceiverTypes()
        {
            List<EmailReceiverType> EmailReceiverTypesList= new List<EmailReceiverType>();

            try
            {
                using (Entities context = new Entities())
                {

                    EmailReceiverTypesList = context.EmailReceiverTypes
                        .OrderBy(x => x.Name)
                        .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.MobileCDRAnalysis.EmailReceiverType.GetEmailReceiverTypes()", err);
            }


            return EmailReceiverTypesList;
        }
    }
}
