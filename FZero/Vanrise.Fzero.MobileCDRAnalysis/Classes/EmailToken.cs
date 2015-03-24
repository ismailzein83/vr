using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;


namespace Vanrise.Fzero.MobileCDRAnalysis
{
    public partial class EmailToken
    {
        public static List<EmailToken> GetAllEmailTokens()
        {
            List<EmailToken> EmailTokensList = new List<EmailToken>() ;

            try
            {
                using (MobileEntities context = new MobileEntities())
                {
                    EmailTokensList = context.EmailTokens
                        .OrderBy(x => x.Token) .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.MobileCDRAnalysis.EmailToken.GetAllEmailTokens()", err);
            }


            return EmailTokensList;
        }
    }
}
