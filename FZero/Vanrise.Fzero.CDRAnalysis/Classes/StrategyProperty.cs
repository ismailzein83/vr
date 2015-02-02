using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using Vanrise.Fzero.CDRAnalysis.Providers;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.CDRAnalysis.Providers
{
    public class StrategyProperty
    {
        private int id;
        private int? strategyId;
        private string strategyName;
        private string period;
        private int? periodValue;
        private int? criteriaId;
        private string criteria;
        private decimal? maxValue;
        private int? strategyThresholdId;
        private int? strategyPeriodId;

        public int Id { get { return id; } set { id = value; } }
        public int? StrategyId { get { return strategyId; } set { strategyId = value; } }
        public string StrategyName { get { return strategyName; } set { strategyName = value; } }
        public string Period { get { return period; } set { period = value; } }
        public int? PeriodValue { get { return periodValue; } set { periodValue = value; } }
        public int? CriteriaId { get { return criteriaId; } set { criteriaId = value; } }
        public string Criteria { get { return criteria; } set { criteria = value; } }
        public decimal? MaxValue { get { return maxValue; } set { maxValue = value; } }
        public int? StrategyThresholdId { get { return strategyThresholdId; } set { strategyThresholdId = value; } }
        public int? StrategyPeriodId { get { return strategyPeriodId; } set { strategyPeriodId = value; } }

        //--------------------------------

        public static List<StrategyProperty> GetstrategyProperty(int strategyId, int criteriaId)
        {
            if (strategyId == 0 && criteriaId == 0)
                return GetAll();
            List<StrategyProperty> strategyProperties = new List<StrategyProperty>();
            StrategyProperty strategyProperty = new StrategyProperty();
            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {

                    if (strategyId != 0 && criteriaId == 0)
                    {

                        var data = from st in context.Strategies
                                   join th in context.StrategyThresholds on st.Id equals th.StrategyId
                                   join sp in context.StrategyPeriods on st.Id equals sp.StrategyId
                                   where (st.Id == strategyId && sp.CriteriaID == th.CriteriaID)
                                   select new { st.Id, st.Name, th.CriteriaID, sp.Value, period = sp.Period.Description, criteria = th.Criteria_Profile.Description, th.MaxValue, thid = th.Id, spid = sp.Id };

                        foreach (var a in data)
                        {
                            strategyProperty = new StrategyProperty();
                            strategyProperty.StrategyId = a.Id;
                            strategyProperty.StrategyName = a.Name;
                            //strategyProperty.MaxValue = a.MAxValue;
                            strategyProperty.CriteriaId = a.CriteriaID;
                            strategyProperty.PeriodValue = a.Value;
                            strategyProperty.Period = a.period;
                            strategyProperty.Criteria = a.criteria;
                            strategyProperty.MaxValue = a.MaxValue;
                            strategyProperty.strategyThresholdId = a.thid;
                            strategyProperty.StrategyPeriodId = a.spid;


                            strategyProperties.Add(strategyProperty);

                        }

                    }
                    if (strategyId == 0 && criteriaId != 0)
                    {

                        var data = from st in context.Strategies
                                   join th in context.StrategyThresholds on st.Id equals th.StrategyId
                                   join sp in context.StrategyPeriods on st.Id equals sp.StrategyId
                                   where (th.CriteriaID == criteriaId && sp.CriteriaID == th.CriteriaID)

                                   select new { st.Id, st.Name, th.CriteriaID, sp.Value, period = sp.Period.Description, criteria = th.Criteria_Profile.Description, th.MaxValue, thid = th.Id, spid = sp.Id };

                        foreach (var a in data)
                        {
                            strategyProperty = new StrategyProperty();
                            strategyProperty.StrategyId = a.Id;
                            strategyProperty.StrategyName = a.Name;
                            //strategyProperty.MaxValue = a.MAxValue;
                            strategyProperty.CriteriaId = a.CriteriaID;
                            strategyProperty.PeriodValue = a.Value;
                            strategyProperty.Period = a.period;
                            strategyProperty.Criteria = a.criteria;
                            strategyProperty.MaxValue = a.MaxValue;
                            strategyProperty.strategyThresholdId = a.thid;
                            strategyProperty.StrategyPeriodId = a.spid;

                            strategyProperties.Add(strategyProperty);

                        }

                    }
                    if (strategyId != 0 && criteriaId != 0)
                    {

                        var data = from st in context.Strategies
                                   join th in context.StrategyThresholds on st.Id equals th.StrategyId
                                   join sp in context.StrategyPeriods on st.Id equals sp.StrategyId
                                   where ((st.Id == strategyId && th.CriteriaID == criteriaId && sp.CriteriaID == th.CriteriaID))
                                   select new { st.Id, st.Name, th.CriteriaID, sp.Value, period = sp.Period.Description, criteria = th.Criteria_Profile.Description, th.MaxValue, thid = th.Id, spid = sp.Id };

                        foreach (var a in data)
                        {
                            strategyProperty = new StrategyProperty();
                            strategyProperty.StrategyId = a.Id;
                            strategyProperty.StrategyName = a.Name;
                            //strategyProperty.MaxValue = a.MAxValue;
                            strategyProperty.CriteriaId = a.CriteriaID;
                            strategyProperty.PeriodValue = a.Value;
                            strategyProperty.Period = a.period;
                            strategyProperty.Criteria = a.criteria;
                            strategyProperty.MaxValue = a.MaxValue;
                            strategyProperty.strategyThresholdId = a.thid;
                            strategyProperty.StrategyPeriodId = a.spid;

                            strategyProperties.Add(strategyProperty);

                        }

                    }

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.StrategyProperties.GetstrategyProperty()", err);
            }
            return strategyProperties;
        }

        //--------------------------------


        public static List<StrategyProperty> GetAll()
        {
            List<StrategyProperty> strategyProperties = new List<StrategyProperty>();
            StrategyProperty strategyProperty = new StrategyProperty();
            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    var data = from st in context.Strategies
                               join th in context.StrategyThresholds on st.Id equals th.StrategyId
                               join sp in context.StrategyPeriods on st.Id equals sp.StrategyId
                               where ((sp.CriteriaID == th.CriteriaID))
                               select new { st.Id, st.Name, th.CriteriaID, sp.Value, period = sp.Period.Description, criteria = th.Criteria_Profile.Description, th.MaxValue, thid = th.Id, spid = sp.Id };

                    foreach (var a in data)
                    {
                        strategyProperty = new StrategyProperty();
                        strategyProperty.StrategyId = a.Id;
                        strategyProperty.StrategyName = a.Name;
                        //strategyProperty.MaxValue = a.MAxValue;
                        strategyProperty.CriteriaId = a.CriteriaID;
                        strategyProperty.PeriodValue = a.Value;
                        strategyProperty.Period = a.period;
                        strategyProperty.Criteria = a.criteria;
                        strategyProperty.MaxValue = a.MaxValue;
                        strategyProperty.strategyThresholdId = a.thid;
                        strategyProperty.StrategyPeriodId = a.spid;

                        strategyProperties.Add(strategyProperty);

                    }

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.StrategyProperties.GetAll()", err);
            }
            return strategyProperties;
        }
        //--------------------------------------------

    }
}
