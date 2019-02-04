﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Data.SQL
{
	public class ChangedCustomerCountryPreviewDataManager : Vanrise.Data.SQL.BaseSQLDataManager, IChangedCustomerCountryPreviewDataManager
	{
		#region Fields / Properties

		readonly string[] columns = { "ID", "CustomerID" ,"ProcessInstanceID", "EED" };

		private long _processInstanceId;

		public long ProcessInstanceId
		{
			set
			{
				_processInstanceId = value;
			}
		}

		#endregion

		#region Constructors

		public ChangedCustomerCountryPreviewDataManager() :
			base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
		{

		}

		#endregion

		public void ApplyChangedCustomerCountryPreviewsToDB(IEnumerable<ChangedCustomerCountryPreview> changedCustomerCountryPreviews)
		{
			object streamForDBApply = InitialiazeStreamForDBApply();

			foreach (ChangedCustomerCountryPreview changedCountryPreview in changedCustomerCountryPreviews)
				WriteRecordToStream(changedCountryPreview, streamForDBApply);

			object bulkInsertInfo = FinishDBApplyStream(streamForDBApply);
			base.InsertBulkToTable(bulkInsertInfo as Vanrise.Data.SQL.StreamBulkInsertInfo);
		}

		public IEnumerable<ChangedCustomerCountryPreview> GetChangedCustomerCountryPreviews(RatePlanPreviewQuery query)
		{
            string strcustomerIds = null;
            if (query.CustomerIds != null && query.CustomerIds.Any())
                strcustomerIds = string.Join(",", query.CustomerIds);
            return GetItemsSP("TOneWhS_Sales.sp_CustomerCountry_GetChangedPreviews", ChangedCustomerCountryPreviewMapper, query.ProcessInstanceId, strcustomerIds);
		}
	    public IEnumerable<int> GetAffectedCustomerIds(long processInstanceId)
	    {
	        List<int> listAffectedCustomerIds = new List<int>();
	        ExecuteReaderSP("TOneWhS_BE.SP_SalePricelistChangedCountryChanges_GetAffectedCustomerIds", (reader) =>
	        {
	            while (reader.Read())
	            {
	                listAffectedCustomerIds.Add(GetReaderValue<int>(reader, "CustomerId"));
	            }
	        }, processInstanceId);
	        return listAffectedCustomerIds;
	    }
        #region Bulk Apply Methods

        public object InitialiazeStreamForDBApply()
		{
			return base.InitializeStreamForBulkInsert();
		}

		public void WriteRecordToStream(ChangedCustomerCountryPreview record, object dbApplyStream)
		{
			var streamForBulkInsert = dbApplyStream as Vanrise.Data.SQL.StreamForBulkInsert;
			streamForBulkInsert.WriteRecord
			(
				"{0}^{1}^{2}^{3}",
				record.CountryId,
                record.CustomerId,
				_processInstanceId,
				GetDateTimeForBCP(record.EED)
			);
		}

		public object FinishDBApplyStream(object dbApplyStream)
		{
			var streamForBulkInsert = dbApplyStream as Vanrise.Data.SQL.StreamForBulkInsert;
			streamForBulkInsert.Close();
			return new Vanrise.Data.SQL.StreamBulkInsertInfo
			{
				TableName = "TOneWhS_Sales.RP_CustomerCountry_ChangedPreview",
				Stream = streamForBulkInsert,
				TabLock = false,
				KeepIdentity = false,
				FieldSeparator = '^',
				ColumnNames = columns
			};
		}

		#endregion

		#region Mappers

		private ChangedCustomerCountryPreview ChangedCustomerCountryPreviewMapper(IDataReader reader)
		{
			return new ChangedCustomerCountryPreview()
			{
				CountryId = (int)reader["ID"],
                CustomerId=(int)reader["CustomerID"],
				EED = (DateTime)reader["EED"]
			};
		}

		#endregion
	}
}
