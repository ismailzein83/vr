using System.Collections.Generic;

namespace TABS
{
    public class RatePlan : PriceListBase
    {
        public override string Identifier { get { return "RatePlan:" + ID; } }

		public override Dictionary<Zone, RateBase> Rates
		{
			get
			{
				if (_Rates == null)
				{
					_Rates = ObjectAssembler.GetRates(this);
				}
				return _Rates;
			}
			set
			{
				_Rates = value;
			}
		}

        public RatePlan()
        {
        }
    }
}