﻿using System;
using System.Data;
using System.Collections.Generic;
using Vanrise.Data.SQL;
using TOne.Data.SQL;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Data.SQL
{
	public class SupplierCodePreviewDataManager : BaseTOneDataManager, ISupplierCodePreviewDataManager
	{
		readonly string[] _columns = { "ProcessInstanceID", "Code", "ChangeType", "RecentZoneName", "ZoneName", "BED", "EED","IsExcluded" };

		private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();
		static SupplierCodePreviewDataManager()
		{
			_columnMapper.Add("ChangeTypeDecription", "ChangeType");
		}

		public long ProcessInstanceId
		{
			set
			{
				_processInstanceID = value;
			}
		}

		long _processInstanceID;

		public SupplierCodePreviewDataManager()
			: base(GetConnectionStringName("TOneWhS_SPL_DBConnStringKey", "TOneWhS_SPL_DBConnString"))
		{

		}

		public void ApplyPreviewCodesToDB(object preparedZones)
		{
			InsertBulkToTable(preparedZones as BaseBulkInsertInfo);
		}

		public IEnumerable<CodePreview> GetFilteredCodePreview(SPLPreviewQuery query)
		{
			return GetItemsSP("[TOneWhS_SPL].[sp_SupplierCode_Preview_GetFiltered]", CodePreviewMapper, query.ProcessInstanceId, query.ZoneName, query.CountryId, query.OnlyModified,query.IsExcluded);
		}

		object Vanrise.Data.IBulkApplyDataManager<CodePreview>.InitialiazeStreamForDBApply()
		{
			return base.InitializeStreamForBulkInsert();
		}

		public void WriteRecordToStream(CodePreview record, object dbApplyStream)
		{
			StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
			streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}",
				_processInstanceID,
				record.Code,
				(int)record.ChangeType,
				record.RecentZoneName,
				record.ZoneName,
				GetDateTimeForBCP(record.BED),
				GetDateTimeForBCP(record.EED),
                (record.IsExcluded) ? 1 : 0);
		}

		public object FinishDBApplyStream(object dbApplyStream)
		{
			StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
			streamForBulkInsert.Close();
			return new StreamBulkInsertInfo
			{
				ColumnNames = _columns,
				TableName = "TOneWhS_SPL.SupplierCode_Preview",
				Stream = streamForBulkInsert,
				TabLock = false,
				KeepIdentity = false,
				FieldSeparator = '^',
			};
		}

		private CodePreview CodePreviewMapper(IDataReader reader)
		{
			CodePreview codePreview = new CodePreview
			{
				Code = reader["Code"] as string,
				ChangeType = (CodeChangeType)GetReaderValue<int>(reader, "ChangeType"),
				RecentZoneName = reader["RecentZoneName"] as string,
				ZoneName = reader["ZoneName"] as string,
				BED = GetReaderValue<DateTime>(reader, "BED"),
				EED = GetReaderValue<DateTime?>(reader, "EED")
			};
			return codePreview;
		}
	}
}
