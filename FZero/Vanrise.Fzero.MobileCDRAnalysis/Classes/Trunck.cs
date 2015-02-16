using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Vanrise.CommonLibrary;





namespace Vanrise.Fzero.MobileCDRAnalysis
{
    public partial class Trunck
    {

        #region Properties

        public SwitchTrunck FirstSwitch { get { return SwitchTruncks.Count >= 1 ? SwitchTruncks.ElementAt(0) : new SwitchTrunck(); } }
        public SwitchTrunck SecondSwitch { get { return SwitchTruncks.Count >= 2 ? SwitchTruncks.ElementAt(1) : new SwitchTrunck(); } }

        #endregion


        public static Trunck Load(int id)
        {
            Trunck trunck = new Trunck();
            try
            {
                using (MobileEntities context = new MobileEntities())
                {            
                    trunck = context.Truncks
                        .Include("SwitchTruncks")
                        .Where(s => s.Id == id)
                        .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.SwitchTrunck.Load(Id" + id + ")", err);
            }
            return trunck;
        }

        public static bool Save(Trunck trunck)
        {
            bool success = false;
            try
            {
                using (MobileEntities context = new MobileEntities())
                {
                    if (trunck.Id == 0)
                    {
                        context.Truncks.Add(trunck);
                    }
                    else
                    {
                        context.Entry(trunck.FirstSwitch).State = System.Data.EntityState.Modified;
                        context.Entry(trunck.SecondSwitch).State = System.Data.EntityState.Modified;
                    }
                    context.SaveChanges();
                }
                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.SwitchTrunck.Save(Id" + trunck.Id + ")", err);
            }
            return success;
        }

        public static bool Delete(int id)
        {
            Trunck trunck = new Trunck() { Id = id };
            return Delete(trunck);
        }

        private static bool Delete(Trunck trunck)
        {
            bool success = false;
            try
            {
                using (MobileEntities context = new MobileEntities())
                {
                    context.Entry(trunck).State = System.Data.EntityState.Deleted;
                    context.SaveChanges();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.SwitchTrunck.Delete(Id" + trunck.Id + ")", err);
            }
            return success;
        }
    }
}
