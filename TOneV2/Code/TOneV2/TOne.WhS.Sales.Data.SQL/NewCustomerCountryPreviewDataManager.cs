using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Data.SQL
{
	public class NewCustomerCountryPreviewDataManager : Vanrise.Data.SQL.BaseSQLDataManager, INewCustomerCountryPreviewDataManager
	{
		#region Fields / Properties

		readonly string[] columns = { "ID", "CustomerID", "ProcessInstanceID", "BED", "EED" };

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

		public NewCustomerCountryPreviewDataManager() :
			base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
		{

		}

		#endregion

		public void ApplyNewCustomerCountryPreviewsToDB(IEnumerable<NewCustomerCountryPreview> newCustomerCountryPreviews)
		{
			object streamForDBApply = InitialiazeStreamForDBApply();

			foreach (NewCustomerCountryPreview newCustomerCountryPreview in newCustomerCountryPreviews)
				WriteRecordToStream(newCustomerCountryPreview, streamForDBApply);

			object bulkInsertInfo = FinishDBApplyStream(streamForDBApply);
			base.InsertBulkToTable(bulkInsertInfo as Vanrise.Data.SQL.StreamBulkInsertInfo);
		}

		public IEnumerable<NewCustomerCountryPreview> GetNewCustomerCountryPreviews(RatePlanPreviewQuery query)
		{
            string strcustomerIds = null;
            if (query.CustomerIds != null && query.CustomerIds.Any())
                strcustomerIds = string.Join(",", query.CustomerIds);
            return GetItemsSP("TOneWhS_Sales.sp_CustomerCountry_GetNewPreviews", NewCustomerCountryPreviewMapper, query.ProcessInstanceId, strcustomerIds);
		}

		#region Bulk Apply Methods

		public object InitialiazeStreamForDBApply()
		{
			return base.InitializeStreamForBulkInsert();
		}

		public void WriteRecordToStream(NewCustomerCountryPreview record, object dbApplyStream)
		{
			var streamForBulkInsert = dbApplyStream as Vanrise.Data.SQL.StreamForBulkInsert;
			streamForBulkInsert.WriteRecord
			(
				"{0}^{1}^{2}^{3}^{4}",
				record.CountryId,
                record.CustomerId,
				_processInstanceId,
				GetDateTimeForBCP(record.BED),
				GetDateTimeForBCP(record.EED)
			);
		}

		public object FinishDBApplyStream(object dbApplyStream)
		{
			var streamForBulkInsert = dbApplyStream as Vanrise.Data.SQL.StreamForBulkInsert;
			streamForBulkInsert.Close();
			return new Vanrise.Data.SQL.StreamBulkInsertInfo
			{
				TableName = "TOneWhS_Sales.RP_CustomerCountry_NewPreview",
				Stream = streamForBulkInsert,
				TabLock = false,
				KeepIdentity = false,
				FieldSeparator = '^',
				ColumnNames = columns
			};
		}

		#endregion

		#region Mappers

		private NewCustomerCountryPreview NewCustomerCountryPreviewMapper(IDataReader reader)
		{
			return new NewCustomerCountryPreview()
			{
				CountryId = (int)reader["ID"],
                CustomerId = (int)reader["CustomerID"],
				BED = (DateTime)reader["BED"],
				EED = base.GetReaderValue<DateTime?>(reader, "EED")
			};
		}

		#endregion
	}
}
