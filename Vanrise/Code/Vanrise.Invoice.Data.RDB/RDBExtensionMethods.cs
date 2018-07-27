using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;

namespace Vanrise.Invoice.Data.RDB
{
    public static class RDBExtensionMethods
    {
        public static T InsertOrUpdateBillingPeriodInfo<T>(this RDBQueryContext<T> queryContext, Entities.BillingPeriodInfo billingPeriodInfo)
        {
            return queryContext
                    .If()
                        .IfCondition().ExistsCondition()
                                        .From(BillingPeriodInfoDataManager.TABLE_NAME, "billPer")
                                        .Where()
                                            .And()
                                                .EqualsCondition("InvoiceTypeID", billingPeriodInfo.InvoiceTypeId)
                                                .EqualsCondition("PartnerID", billingPeriodInfo.PartnerId)
                                            .EndAnd()
                                        .SelectColumns().FixedValue(1, "dum").EndColumns()
                                        .EndSelect()
                        .ThenQuery().Update()
                                    .FromTable(BillingPeriodInfoDataManager.TABLE_NAME)
                                    .Where().And()
                                                .EqualsCondition("InvoiceTypeID", billingPeriodInfo.InvoiceTypeId)
                                                .EqualsCondition("PartnerID", billingPeriodInfo.PartnerId)
                                            .EndAnd()
                                    .ColumnValue("NextPeriodStart", billingPeriodInfo.NextPeriodStart)
                                    .EndUpdate()
                        .ElseQuery().Insert()
                                    .IntoTable(BillingPeriodInfoDataManager.TABLE_NAME)
                                    .ColumnValue("InvoiceTypeID", billingPeriodInfo.InvoiceTypeId)
                                    .ColumnValue("PartnerID", billingPeriodInfo.PartnerId)
                                    .ColumnValue("NextPeriodStart", billingPeriodInfo.NextPeriodStart)
                                    .EndInsert()
                        .EndIf();


        }
    }
}
