using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Data.SQL
{
	public class NewCustomerCountryDataManager : Vanrise.Data.SQL.BaseSQLDataManager, INewCustomerCountryDataManager
	{
		#region Fields / Properties

		private readonly string[] columns = { "ID", "ProcessInstanceID", "CustomerID", "CountryID", "BED", "EED" };

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

		public NewCustomerCountryDataManager() :
			base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
		{

		}

		#endregion

		#region Public Methods

		public void ApplyNewCustomerCountriesToDB(IEnumerable<NewCustomerCountry> newCustomerCountries)
		{
			object streamForDBApply = InitialiazeStreamForDBApply();

			foreach (NewCustomerCountry newCustomerCountry in newCustomerCountries)
				WriteRecordToStream(newCustomerCountry, streamForDBApply);

			object bulkInsertInfo = FinishDBApplyStream(streamForDBApply);
			base.InsertBulkToTable(bulkInsertInfo as Vanrise.Data.SQL.StreamBulkInsertInfo);
		}

		#endregion

		#region Private Methods

		public object InitialiazeStreamForDBApply()
		{
			return base.InitializeStreamForBulkInsert();
		}

		public void WriteRecordToStream(NewCustomerCountry record, object dbApplyStream)
		{
			var streamForBulkInsert = dbApplyStream as Vanrise.Data.SQL.StreamForBulkInsert;
			streamForBulkInsert.WriteRecord
			(
				"{0}^{1}^{2}^{3}^{4}^{5}",
				record.CustomerCountryId,
				_processInstanceId,
				record.CustomerId,
				record.CountryId,
				record.BED,
				record.EED
			);
		}

		public object FinishDBApplyStream(object dbApplyStream)
		{
			var streamForBulkInsert = dbApplyStream as Vanrise.Data.SQL.StreamForBulkInsert;
			streamForBulkInsert.Close();
			return new Vanrise.Data.SQL.StreamBulkInsertInfo
			{
				TableName = "TOneWhS_Sales.RP_CustomerCountry_New",
				Stream = streamForBulkInsert,
				TabLock = false,
				KeepIdentity = false,
				FieldSeparator = '^',
				ColumnNames = columns
			};
		}

		#endregion
	}
}
