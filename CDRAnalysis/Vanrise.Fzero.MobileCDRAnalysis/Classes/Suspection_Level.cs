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
     public partial class Suspicion_Level
    {

        public static Suspicion_Level Load(int id)
        {
            Suspicion_Level Suspicion_Level = new Suspicion_Level();
            try
            {
                using (Entities context = new Entities())
                {
                    Suspicion_Level = context.Suspicion_Level
                        .Where(s => s.Id == id)
                        .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Suspicion_Level.Load(" + id + ")", err);
            }
            return Suspicion_Level;
        }

        //----------------------------------------
        public static List<Suspicion_Level> GetAll()
        {
            List<Suspicion_Level> suspection_levels = new List<Suspicion_Level>();
            try
            {
                using (Entities context = new Entities())
                {
                    suspection_levels = context.Suspicion_Level
                       .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Suspicion_Level.GetAll()", err);
            }
            return suspection_levels;
        }

        //----------------------------------------
        public static bool Save(Suspicion_Level Suspicion_Level)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {
                    if (Suspicion_Level.Id == 0)
                    {
                        context.Suspicion_Level.Add(Suspicion_Level);
                    }
                    else
                    {
                        context.Entry(Suspicion_Level).State = System.Data.EntityState.Modified;

                    }
                    context.SaveChanges();
                }
                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Suspicion_Level.Save(Id: " + Suspicion_Level.Id + ")", err);
            }
            return success;
        }

        //--------------------------------------------


    }
}
