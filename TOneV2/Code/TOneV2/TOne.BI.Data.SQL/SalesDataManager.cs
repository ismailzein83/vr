using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BI.Entities;

namespace TOne.BI.Data.SQL
{
    public class SalesDataManager : BaseDataManager, ISalesDataManager
    {
        public DataTable GetProfit(DateTime from, DateTime to)
        {
            string fromdate = from.ToString("yyyyMMdd");
            string todate = to.ToString("yyyyMMdd");
            string mdx = @"select {[Measures].[Cost Net] ,[Measures].[Sale Net]} ON COLUMNS,                                
                                ([Date].[Date Key].&[" + fromdate + @"] : [Date].[Date Key].&[" + todate + @"])
                                ON ROWS
                                FROM [" + this.CubeName + @"]
                                ";

            return GetData(mdx);
        }

        public IEnumerable<ProfitInfo> GetProfit(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate)
        {
            List<ProfitInfo> rslt = new List<ProfitInfo>();
            string query = String.Format(@"select {{[Measures].[Cost Net] ,[Measures].[Sale Net]}} ON COLUMNS,                                
                                            {3}    ON ROWS
                                            FROM [{0}]
                                            WHERE [Date].[Date].&[{1:yyyy-MM-dd}T00:00:00] : [Date].[Date].&[{2:yyyy-MM-dd}T00:00:00]", 
                                            this.CubeName, fromDate, toDate, GetDateColumns(timeDimensionType));
           
            ExecuteReaderMDX(query, (reader) =>
                {
                    while(reader.Read())
                    {
                        ProfitInfo profitInfo = new ProfitInfo
                        {
                            Cost = Convert.ToDecimal(reader["[Measures].[Cost Net]"]),
                            Sale = Convert.ToDecimal(reader["[Measures].[Sale Net]"])
                        };
                        if (profitInfo.Cost > 0 || profitInfo.Sale > 0)
                        {
                            FillTimeCaptions(profitInfo, reader, timeDimensionType);
                            profitInfo.Profit = profitInfo.Sale - profitInfo.Cost;
                            rslt.Add(profitInfo);
                        }
                    }
                });
            return rslt.OrderBy(itm => itm.Time);
        }


    }
}
