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
    public partial  class NormalCDR
    {

        public static List<NormalCDR> GetList(string MSISDN)
        {
            //if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(Description))
            //    return GetAll();

            List<NormalCDR> normalCDRs = new List<NormalCDR>();
            try
            {
                using (Entities context = new Entities())
                {
                    normalCDRs = context.NormalCDRs
                       .Where(s =>
                            (s.MSISDN == MSISDN)
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

        public static IEnumerable<string> GetRelatedList(string MSISDN)
        {
            MSISDN = (MSISDN == "" ? "0" : MSISDN);
            Int64 a_TempInt = Int64.Parse(MSISDN);

            StringBuilder s = new StringBuilder();
            for (int i = 0; i < MSISDN.Length - a_TempInt.ToString().Length; i++)
            {
                s.Append("0");
            }

            List<string> matchList = new List<string>();
            for (Int64 i = a_TempInt - 4; i <= a_TempInt + 4; i++)
            {
                string ns = s + i.ToString();
                //if (ns != MSISDN)
                matchList.Add(ns);
            }
            matchList.Add(MSISDN);
                    

            IEnumerable<string> relatedList = new List<string>();

            try
            {
                using (Entities context = new Entities())
                {

                    relatedList = (from  c in context.NormalCDRs
                                   join th in matchList on c.MSISDN equals th
                                   orderby c.MSISDN
                                   select c.MSISDN).ToList().Distinct();
                   
                 
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
