using System;
using System.Collections.Generic;

namespace TABS
{
    public class CurrencyExchangeRate : Components.BaseEntity, Interfaces.ICachedCollectionContainer
    {
        public override string Identifier { get { return "CurrencyExchangeRate:" + CurrencyExchangeRateID; } }

        private int _CurrencyExchangeRateID;
        private Currency _Currency;
        private float _Rate;
        private DateTime _ExchangeDate;


        public virtual int CurrencyExchangeRateID
        {
            get { return _CurrencyExchangeRateID; }
            set { _CurrencyExchangeRateID = value; }
        }

        public virtual Currency Currency
        {
            get { return _Currency; }
            set { _Currency = value; }
        }

        public virtual float Rate
        {
            get { return _Rate; }
            set { _Rate = value; }
        }

        public virtual float RateInverse
        {
            get { return 1 / Rate; }
            set { Rate = 1 / value; }
        }

        public virtual DateTime ExchangeDate
        {
            get { return _ExchangeDate; }
            set { _ExchangeDate = value; }
        }

        public static Dictionary<Currency, List<CurrencyExchangeRate>> _ExchangeRates;
        public static Dictionary<Currency, List<CurrencyExchangeRate>> ExchangeRates
        {
            get
            {
                if (_ExchangeRates != null) return _ExchangeRates;
                try
                {
                    _ExchangeRates = new Dictionary<Currency, List<CurrencyExchangeRate>>();

                    //                    IList<CurrencyExchangeRate> currencyExchangeRates = TABS.DataConfiguration.CurrentSession
                    //                         .CreateQuery(@"SELECT CE FROM CurrencyExchangeRate CE 
                    //                            WHERE CE.ExchangeDate >= :from AND CE.ExchangeDate <= :to")
                    //                          .SetParameter("from", new DateTime(1995, 1, 1))
                    //                          .SetParameter("to", new DateTime(2100, 1, 1))
                    //                          .List<CurrencyExchangeRate>(); 


                    //Added For bug 2774
                    List<CurrencyExchangeRate> currencyExchangeRates = DataHelper.GetExchangeRates();

                    foreach (TABS.CurrencyExchangeRate currencyExchangerate in currencyExchangeRates)
                    {
                        TABS.Currency curr = currencyExchangerate.Currency;
                        if (!_ExchangeRates.ContainsKey(curr)) _ExchangeRates[curr] = new List<CurrencyExchangeRate>();
                        _ExchangeRates[curr].Add(currencyExchangerate);
                    }

                    return _ExchangeRates;
                }
                catch (Exception EX)
                {
                    //throw (EX);
                }
                return _ExchangeRates;
            }
        }

        /// <summary>
        /// Needed for marker interface to be called by reflection
        /// </summary>
        public static void ClearCachedCollections()
        {
            _ExchangeRates = null;
            TABS.Components.CacheProvider.Clear(typeof(CurrencyExchangeRate).FullName);
        }


    }
}