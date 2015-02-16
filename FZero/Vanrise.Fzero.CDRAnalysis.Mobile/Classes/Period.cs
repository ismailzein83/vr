using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.CDRAnalysis.Mobile
{
    public partial class Period
    {

        public static Period Load(int id)
        {
            Period period = new Period();
            try
            {
                using (Entities context = new Entities())
                {
                    period = context.Periods
                        .Where(s => s.Id == id)
                        .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Criteria_profile.Load(" + id + ")", err);
            }
            return period;
        }

        //----------------------------------------
        public static List<Period> GetAll()
        {
            List<Period> periods = new List<Period>();
            try
            {
                using (Entities context = new Entities())
                {
                    periods = context.Periods
                       .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Criteria_profiles.GetAll()", err);
            }
            return periods;
        }

        //----------------------------------------
        public static bool Save(Period period)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {
                    if (period.Id == 0)
                    {
                        context.Periods.Add(period);
                    }
                    else
                    {
                        context.Entry(period).State = System.Data.EntityState.Modified;

                    }
                    context.SaveChanges();
                }
                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Period.Save(Id: " + period.Id + ")", err);
            }
            return success;
        }

        //--------------------------------------------


    }
}
