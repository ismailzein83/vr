using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using Vanrise.CommonLibrary;





namespace Vanrise.Fzero.MobileCDRAnalysis
{
     public partial class Suspection_Level
    {

        public static Suspection_Level Load(int id)
        {
            Suspection_Level suspection_level = new Suspection_Level();
            try
            {
                using (MobileEntities context = new MobileEntities())
                {
                    suspection_level = context.Suspection_Level
                        .Where(s => s.Id == id)
                        .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Suspection_level.Load(" + id + ")", err);
            }
            return suspection_level;
        }

        //----------------------------------------
        public static List<Suspection_Level> GetAll()
        {
            List<Suspection_Level> suspection_levels = new List<Suspection_Level>();
            try
            {
                using (MobileEntities context = new MobileEntities())
                {
                    suspection_levels = context.Suspection_Level
                       .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Suspection_level.GetAll()", err);
            }
            return suspection_levels;
        }

        //----------------------------------------
        public static bool Save(Suspection_Level suspection_level)
        {
            bool success = false;
            try
            {
                using (MobileEntities context = new MobileEntities())
                {
                    if (suspection_level.Id == 0)
                    {
                        context.Suspection_Level.Add(suspection_level);
                    }
                    else
                    {
                        context.Entry(suspection_level).State = System.Data.EntityState.Modified;

                    }
                    context.SaveChanges();
                }
                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Suspection_level.Save(Id: " + suspection_level.Id + ")", err);
            }
            return success;
        }

        //--------------------------------------------


    }
}
