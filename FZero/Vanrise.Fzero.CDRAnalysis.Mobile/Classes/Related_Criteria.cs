using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using Vanrise.CommonLibrary;
using Vanrise.Fzero.CDRAnalysis.Mobile;

namespace Vanrise.Fzero.CDRAnalysis.Mobile
{
    public partial class Related_Criteria
    {
       
        //-------------------------------------

        public static Related_Criteria Load(int id)
        {
            Related_Criteria related_Criteria = new Related_Criteria();
            try
            {
                using (Entities context = new Entities())
                {
                    related_Criteria = context.Related_Criteria
                        .Include(s => s.Strategy)
                        .Include(s => s.Criteria_Profile)
                        .Include(s => s.Criteria_Profile1)
                        .Where(s => s.Id == id)
                        .OrderBy(s => new { s.StrategyId,s.CriteriaId})
                        .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Related_Criteria.Load(" + id + ")", err);
            }
            return related_Criteria;
        }
         //-------------------------------------

        public static List<Related_Criteria> GetList(int strategyId, int criteriaId)
        {
            if (strategyId == 0 && criteriaId == 0)
                return GetAll();

            List<Related_Criteria> related_Criteria = new List<Related_Criteria>();
            try
            {
                using (Entities context = new Entities())
                {

                    if (strategyId != 0 && criteriaId == 0)
                    {
                        related_Criteria = context.Related_Criteria
                            .Include(s => s.Strategy)
                            .Include(s => s.Criteria_Profile)
                            .Include(s => s.Criteria_Profile1)
                            .Where(
                                s =>
                                (s.StrategyId == strategyId)

                            )
                            .OrderBy(s => new { s.StrategyId, s.CriteriaId })
                            .ToList();
                    }

                    else if (strategyId == 0 && criteriaId != 0)
                    {
                        related_Criteria = context.Related_Criteria
                            .Include(s => s.Strategy)
                            .Include(s => s.Criteria_Profile)
                            .Include(s => s.Criteria_Profile1)
                            .Where(
                                s =>
                                (s.CriteriaId == criteriaId)
                            )
                            .OrderBy(s => new { s.StrategyId, s.CriteriaId })
                            .ToList();
                    }

                    else if (strategyId != 0 && criteriaId != 0)
                    {
                        related_Criteria = context.Related_Criteria
                            .Include(s => s.Strategy)
                            .Include(s => s.Criteria_Profile)
                            .Include(s => s.Criteria_Profile1)
                            .Where(
                                s =>
                                (s.CriteriaId == criteriaId && s.StrategyId == strategyId)
                            )
                            .OrderBy(s => new { s.StrategyId, s.CriteriaId })
                            .ToList();
                    }
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Related_Criteria.GetList()", err);
            }
            return related_Criteria;


        }
 //-------------------------------------

        public static List<Related_Criteria> GetAll()
        {
            List<Related_Criteria> related_Criteria = new List<Related_Criteria>();
            try
            {
                using (Entities context = new Entities())
                {
                    related_Criteria = context.Related_Criteria
                        .Include(s => s.Strategy)
                        .Include(s => s.Criteria_Profile)
                        .Include(s => s.Criteria_Profile1)
                        .OrderBy(s => new { s.StrategyId, s.CriteriaId })
                        .ToList(); 
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Strategy.GetAll()", err);
            }
            return related_Criteria;
        }

 //-------------------------------------
 public static bool Delete(int id)
        {
            Related_Criteria related_Criteria = new Related_Criteria() { Id = id };
            return Delete(related_Criteria);
        }
 //-------------------------------------
 private static bool Delete(Related_Criteria related_Criteria)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {

                    Related_Criteria st = Related_Criteria.Load(related_Criteria.Id);
                    //if (st.NormalizationRules.Count() > 0 || st.SwitchTruncks.Count() > 0)
                    //{
                    //    success=false;
                    //}


                    context.Entry(related_Criteria).State = System.Data.EntityState.Deleted;
                    context.SaveChanges();
                    success = true;
                }
            }
            catch(Exception err)
            {
                FileLogger.Write("DataLayer.Related_Criteria.Delete(Id: " + related_Criteria.Id + ")", err);
            }
            return success;
        }
 //-------------------------------------

 public static bool Save(Related_Criteria related_Criteria)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {

                    if (related_Criteria.Id == 0)
                    {
                        context.Related_Criteria.Add(related_Criteria);
                    }
                    else
                    {
                        context.Entry(related_Criteria).State = System.Data.EntityState.Modified;
                    }
                    context.SaveChanges();
                }
                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Related_Criteria.Save(Id: " + related_Criteria.Id + ")", err);
            }
            return success;
        }
 //-------------------------------------

    }
}
