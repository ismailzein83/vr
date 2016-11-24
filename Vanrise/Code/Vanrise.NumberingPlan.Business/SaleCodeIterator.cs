using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
    public class SaleCodeIterator
    {
        public int SellingNumberPlanId { get; set; }

        public CodeIterator<SaleCode> CodeIterator { get; set; }

        public SaleCodeMatch GetCodeMatch(string number, bool isDistinctFromSaleCodes)
        {
            SaleCodeMatch saleCodeMatch = null;
            SaleCode matchSaleCode = isDistinctFromSaleCodes ? this.CodeIterator.GetExactMatch(number) : this.CodeIterator.GetLongestMatch(number);
            if (matchSaleCode != null)
                saleCodeMatch = new SaleCodeMatch
                {
                    SaleCode = matchSaleCode.Code,
                    SaleZoneId = matchSaleCode.ZoneId,
                    SellingNumberPlanId = this.SellingNumberPlanId,
                };

            return saleCodeMatch;
        }
    }
}
