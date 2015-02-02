using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class MobileOperatorFeedback
    {
        public static List<MobileOperatorFeedback> GetMobileOperatorFeedbacks()
           
        {
            List<MobileOperatorFeedback> MobileOperatorFeedbacksList = new List<MobileOperatorFeedback>();

            try
            {
                using (Entities context = new Entities())
                {
                    MobileOperatorFeedbacksList = context.MobileOperatorFeedbacks
                                            .OrderByDescending(u => u.Name)
                                            .ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.MobileOperatorFeedback.GetMobileOperatorFeedbackes()", err);
            }

            return MobileOperatorFeedbacksList;
        }
    }
}
