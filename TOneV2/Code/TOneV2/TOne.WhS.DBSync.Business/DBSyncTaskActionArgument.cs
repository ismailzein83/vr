﻿using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class DBSyncTaskActionArgument : Vanrise.Runtime.Entities.BaseTaskActionArgument
    {
        public string ConnectionString { get; set; }
        public bool UseTempTables { get; set; }
        public int DefaultSellingNumberPlanId { get; set; }
        public int SellingProductId { get; set; }
        public int OffPeakRateTypeId { get; set; }
        public int WeekendRateTypeId { get; set; }
        public int HolidayRateTypeId { get; set; }
        public bool MigratePriceListData { get; set; }
        public decimal DefaultRate { get; set; }
        public bool OnlyEffective { get; set; }
        public bool IsCustomerCommissionNegative { get; set; }
        public List<DBTableName> MigrationRequestedTables { get; set; }
        public Dictionary<string, ParameterValue> ParameterDefinitions { get; set; }
    }
}
