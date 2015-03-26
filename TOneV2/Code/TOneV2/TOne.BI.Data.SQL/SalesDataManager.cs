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
        public IEnumerable<ProfitInfo> GetProfit(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate)
        {
            List<ProfitInfo> rslt = new List<ProfitInfo>();
            string columnsPart = BuildQueryColumnsPart(MeasureColumns.COST, MeasureColumns.SALE);
            string rowsPart = BuildQueryDateRowColumns(timeDimensionType);
            string filtersPart = GetDateFilter(fromDate, toDate);
            string query = BuildQuery(columnsPart, rowsPart, filtersPart);

            ExecuteReaderMDX(query, (reader) =>
                {
                    while(reader.Read())
                    {
                        ProfitInfo profitInfo = new ProfitInfo
                        {
                            Cost = Convert.ToDecimal(reader[MeasureColumns.COST]),
                            Sale = Convert.ToDecimal(reader[MeasureColumns.SALE])
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
