using System;
using System.Collections.Generic;

namespace TABS
{
	public class PersistensePriorityList : List<object>, IComparer<object>
	{
		protected static Dictionary<Type, int> _PersistensePriorities;
		
        public static Dictionary<Type, int> PersistensePriorities 
		{
			get
			{
				if (_PersistensePriorities == null)
				{
					_PersistensePriorities = new Dictionary<Type, int>();
					int index = 0;
					_PersistensePriorities[typeof(TABS.Enumerations)] = index++;
					_PersistensePriorities[typeof(TABS.Switch)] = index++;
					_PersistensePriorities[typeof(TABS.Currency)] = index++;
					_PersistensePriorities[typeof(TABS.CurrencyExchangeRate)] = index++;
					_PersistensePriorities[typeof(TABS.CarrierProfile)] = index++;
					_PersistensePriorities[typeof(TABS.CarrierAccount)] = index++;
					_PersistensePriorities[typeof(TABS.CarrierDocument)] = index++;
                    _PersistensePriorities[typeof(TABS.CarrierAccountConnection)] = index++;                    
                    _PersistensePriorities[typeof(TABS.CodeGroup)] = index++;
					_PersistensePriorities[typeof(TABS.Zone)] = index++;
					_PersistensePriorities[typeof(TABS.Code)] = index++;
					_PersistensePriorities[typeof(TABS.PriceList)] = index++;
                    _PersistensePriorities[typeof(TABS.Rate)] = index++;
                    _PersistensePriorities[typeof(TABS.RatePlan)] = index++;
                    _PersistensePriorities[typeof(TABS.PlaningRate)] = index++;
                    _PersistensePriorities[typeof(TABS.SystemParameter)] = index++;
                    _PersistensePriorities[typeof(TABS.ToDConsideration)] = index++;
                    _PersistensePriorities[typeof(TABS.RouteBlock)] = index++;
					_PersistensePriorities[typeof(TABS.Commission)] = index++;
					_PersistensePriorities[typeof(TABS.Tariff)] = index++;
					_PersistensePriorities[typeof(TABS.SpecialRequest)] = index++;
                    _PersistensePriorities[typeof(TABS.PersistedRunnableTask)] = index++; 
                   
                   
                    
                    _PersistensePriorities[typeof(TABS.Billing_CDR_Main)] = index++;
                    _PersistensePriorities[typeof(TABS.Billing_CDR_Cost)] = index++;
                    _PersistensePriorities[typeof(TABS.Billing_CDR_Sale)] = index++;
                    _PersistensePriorities[typeof(TABS.Billing_Invoice)] = index++;
                    _PersistensePriorities[typeof(TABS.Billing_Invoice_Cost)] = index++;
                    _PersistensePriorities[typeof(TABS.Billing_Invoice_Detail)] = index++;
				}
				return _PersistensePriorities;
			}
		}

        public new void Sort()
        {
            base.Sort(this);
        }

        #region IComparer Members

        public int Compare(object x, object y)
        {
            int px = 0, py = 0;
            if (x != null) PersistensePriorities.TryGetValue(x.GetType(), out px);
            if (y != null) PersistensePriorities.TryGetValue(y.GetType(), out py);
            return px.CompareTo(py);
        }

        #endregion
    }
}