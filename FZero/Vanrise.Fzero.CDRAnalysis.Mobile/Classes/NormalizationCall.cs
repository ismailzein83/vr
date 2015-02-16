
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using Vanrise.CommonLibrary;




namespace Vanrise.Fzero.CDRAnalysis.Mobile
{
    public partial class NormalizationRule
    {
        public string UpdateStatement
        {
            get
            {
                string updateStatement = string.Empty;
                if (!string.IsNullOrWhiteSpace(PrefixToAdd))
                {
                    updateStatement = "'" + PrefixToAdd + "' + ";
                }
                if (SubstringStartIndex.HasValue)
                {
                    //if (updateStatement.Length > 0)
                    //    updateStatement += " + ";
                    updateStatement += " Substring([" + Party + "], " + SubstringStartIndex + ", " + SubstringLength + ") ";
                }
                else
                {
                    updateStatement += "[" + Party + "]";
                }
                if (!string.IsNullOrWhiteSpace(SuffixToAdd))
                {
                    if (updateStatement.Length > 0)
                        updateStatement += " + ";
                    updateStatement += "'" + SuffixToAdd + "'";

                }

                return updateStatement;
            }
        }

        public static List<NormalizationRule> GetList(int switchId, int TrunckID, string party, int? length, string prefix, 
            int pageSize, int pageNumber, string orderedColumn, out int rowsCount)
        {
            List<NormalizationRule> rules = new List<NormalizationRule>();
            rowsCount = 0;
            try
            {
                using (Entities context = new Entities())
                {
                    var query = context.NormalizationRules
                        .Include(r=>r.SwitchTrunck.Trunck)
                        .Include(r => r.SwitchTrunck1.Trunck)
                        .Include(r=>r.SwitchProfile)
                        .Where(r => 
                            (switchId <= 0 || r.SwitchId == switchId)
                            &&
                            (party == string.Empty || r.Party == party)
                            &&
                            (TrunckID == 0 || r.In_TrunckId == TrunckID || r.Out_TrunckId == TrunckID)
                            &&
                            (!length.HasValue || r.CallLength == length)
                            &&
                            (prefix == string.Empty || r.Prefix == prefix)
                            );
                    rowsCount = query.Count();

                    if (rowsCount <= (pageNumber - 1) * pageSize)
                    {
                        pageNumber = rowsCount / pageSize;
                        pageNumber += rowsCount % pageSize == 0 ? 0 : 1;
                    }

                    var orderedQuery = query.OrderBy(r => r.Id);
                    switch (orderedColumn)
                    {
                        case "SwitchId":
                            orderedQuery = query.OrderBy(r => r.SwitchId);
                            break;
                    }
                    rules = orderedQuery
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.NormalizationRule.GetList()", err);
            }
            return rules;
        }

        public static bool Save(NormalizationRule rule)
        {
            bool success = false;
            try
            {
                using(Entities context = new Entities())
                {
                    if (rule.Id == 0)
                    {
                        context.NormalizationRules.Add(rule);
                    }
                    else
                    {
                        context.Entry(rule).State = System.Data.EntityState.Modified;
                    } 
                    context.SaveChanges();
                    success = true;
                }
            }
            catch (Exception err)
            {
                //FileLogger.Write("DataLayer.NormalizationRule.Save(Id: " + rule.Id + ")", err);
            }
            return success;
        }

        public static bool Delete(int id)
        {
            NormalizationRule rule = new NormalizationRule() { Id = id };
            return Delete(rule);
        }

        private static bool Delete(NormalizationRule rule)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {
                    context.Entry(rule).State = System.Data.EntityState.Deleted;
                    context.SaveChanges();
                }
            }
            catch (Exception err)
            {
                //FileLogger.Write("DataLayer.NormalizationRule.Delete(Id: " + rule.Id + ")", err);
            }
            return success;
        }

        public static NormalizationRule Load(int id)
        {
            NormalizationRule rule = new NormalizationRule();
            try
            {
                using (Entities context = new Entities())
                {
                    rule = context.NormalizationRules
                        .Where(r => r.Id == id)
                        .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.NormalizationRule.Load(" + id + ")", err);
            }
            return rule;
        }

        public static bool CheckIfExists(NormalizationRule rule)
        {
            int Count = 0;
            try
            {
                
                using (Entities context = new Entities())
                {
                    Count = context.NormalizationRules
                        .Where(r => r.Party == rule.Party).Where(r => r.SwitchId == rule.SwitchId).Where(r => r.In_TrunckId == rule.In_TrunckId).Where(r => r.CallLength == rule.CallLength).Where(r => r.Prefix == rule.Prefix).Where(r => (r.In_TrunckId.HasValue || r.In_TrunckId == r.In_TrunckId)).Where(r => r.Id != rule.Id)
                        .Count();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.NormalizationRule.CheckIfExists(" + rule.Id + ")", err);
            }
            return ( Count==0?  true:false);
        }
    }
}
