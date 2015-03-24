using System;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class RecievedEmail
    {
        public static RecievedEmail Save(RecievedEmail RecievedEmail)
        {
            RecievedEmail CurrentRecievedEmail = new RecievedEmail();
            try
            {
                using (Entities context = new Entities())
                {
                    context.RecievedEmails.Add(RecievedEmail);
                    context.SaveChanges();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.RecievedEmail.Save(" + RecievedEmail.ID.ToString() + ")", err);
            }
            return CurrentRecievedEmail;
        }

        public static RecievedEmail GetLast(int SourceID)
        {
            RecievedEmail RecievedEmail = new RecievedEmail();
            try
            {
                using (Entities context = new Entities())
                {
                    RecievedEmail = context.RecievedEmails.Where(x => x.SourceID == SourceID).ToList().LastOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.ApplicationUser.GetLast()", err);
            }
            return RecievedEmail;
        }
    }
}
