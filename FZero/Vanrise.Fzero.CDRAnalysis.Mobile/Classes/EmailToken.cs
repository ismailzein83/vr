using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;


namespace Vanrise.Fzero.CDRAnalysis.Mobile
{
    public partial class EmailToken
    {
        public static List<EmailToken> GetAllEmailTokens()
        {
            List<EmailToken> EmailTokensList = new List<EmailToken>() ;

            try
            {
                using (Entities context = new Entities())
                {
                    EmailTokensList = context.EmailTokens
                        .OrderBy(x => x.Token) .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.CDRAnalysis.Mobile.EmailToken.GetAllEmailTokens()", err);
            }


            return EmailTokensList;
        }
    }
}
