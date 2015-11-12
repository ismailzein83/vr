﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Routing.Data.SQL
{
    public class CodeSaleZoneDataManager : RoutingDataManager, ICodeSaleZoneDataManager
    {
        public void ApplyCodeToCodeSaleZoneTable(object preparedCodeMatches)
        {
            InsertBulkToTable(preparedCodeMatches as BaseBulkInsertInfo);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[CodeSaleZone]",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(Entities.CodeMatches record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            foreach (SaleCodeMatch match in record.SaleCodeMatches)
                streamForBulkInsert.WriteRecord("{0}^{1}", record.Code, match.SaleZoneId);
        }
    }
}
