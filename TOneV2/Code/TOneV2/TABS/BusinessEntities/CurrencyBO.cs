using System;
using System.Collections.Generic;

namespace TABS.BusinessEntities
{
    public class CurrencyBO
    {
        public static IList<TABS.CurrencyExchangeRate> GetCurrencyExchange(TABS.Currency currency, DateTime rateUpdateDate)
        {
            NHibernate.IQuery query = TABS.ObjectAssembler.CurrentSession.CreateQuery("FROM TABS.CurrencyExchangeRate x WHERE x.ExchangeDate = :date and x.Currency = :currency");
            query.SetEntity("currency", currency);
            query.SetDateTime("date", rateUpdateDate);
            return query.List<TABS.CurrencyExchangeRate>();
        }

        public static IList<TABS.CurrencyExchangeRate> GetCurrencyExchangeHistory(string symbol)
        {
            TABS.Currency currency = TABS.Currency.All[symbol];
            return TABS.ObjectAssembler.CurrentSession
                .CreateQuery("SELECT R FROM CurrencyExchangeRate R WHERE R.Currency = :currency ORDER BY ExchangeDate DESC")
                .SetParameter("currency", currency)
                .List<TABS.CurrencyExchangeRate>();
        }
    }
}
