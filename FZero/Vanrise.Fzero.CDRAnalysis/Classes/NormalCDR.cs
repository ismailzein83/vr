using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using Vanrise.CommonLibrary;
using Vanrise.Fzero.CDRAnalysis.Providers;
namespace Vanrise.Fzero.CDRAnalysis
{
    public partial  class NormalCDR
    {

        public static List<NormalCDR> GetList(string A_Temp)
        {
            //if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(Description))
            //    return GetAll();

            List<NormalCDR> normalCDRs = new List<NormalCDR>();
            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    normalCDRs = context.NormalCDRs
                       .Where(s =>
                            (s.A_Temp == A_Temp)
                        ).Take(1000)
                        .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.NormalCDRs.GetList()", err);
            }
            return normalCDRs;
        }

        public static IEnumerable<string> GetRelatedList(string A_Temp)
        {
            A_Temp = (A_Temp == "" ? "0" : A_Temp);
            Int64 a_TempInt = Int64.Parse(A_Temp);

            StringBuilder s = new StringBuilder();
            for (int i = 0; i < A_Temp.Length - a_TempInt.ToString().Length; i++)
            {
                s.Append("0");
            }

            List<string> matchList = new List<string>();
            for (Int64 i = a_TempInt - 4; i <= a_TempInt + 4; i++)
            {
                string ns = s + i.ToString();
                //if (ns != A_Temp)
                matchList.Add(ns);
            }
            matchList.Add(A_Temp);
                    

            IEnumerable<string> relatedList = new List<string>();

            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {

                    relatedList = (from  c in context.NormalCDRs
                                   join th in matchList on c.A_Temp equals th
                                   orderby c.A_Temp
                                   select c.A_Temp).ToList().Distinct();
                   
                 
                }

            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.NormalCDRs.GetList()", err);
            }
            return relatedList;
        }



    }
}
