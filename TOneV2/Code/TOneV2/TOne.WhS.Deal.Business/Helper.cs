using System;
using Vanrise.Common;

namespace TOne.WhS.Deal.Business
{
    public static class Helper
    {
        public static decimal GetDiscountedRateValue(decimal rate, int discount)
        {
            var discountValue = (rate * discount) / 100;
            var discountedRate = rate - discountValue;
            return discountedRate;
        }
    }
}
