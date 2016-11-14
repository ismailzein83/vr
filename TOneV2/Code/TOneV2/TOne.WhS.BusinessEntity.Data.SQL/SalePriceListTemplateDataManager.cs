using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
	public class SalePriceListTemplateDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ISalePriceListTemplateDataManager
	{
		#region Constructors

		public SalePriceListTemplateDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

		#endregion

		#region Public Methods

		public IEnumerable<SalePriceListTemplate> GetAll()
		{
			return GetItemsSP("TOneWhS_BE.sp_SalePriceListTemplate_GetAll", SalePriceListTemplateMapper);
		}

		public bool Insert(SalePriceListTemplate salePriceListTemplate, out int insertedId)
		{
			object salePriceListTemplateId;
			string serializedSettings = GetSerializedObject(salePriceListTemplate.Settings);
			int affectedRows = ExecuteNonQuerySP("TOneWhS_BE.sp_SalePriceListTemplate_Insert", out salePriceListTemplateId, salePriceListTemplate.Name, serializedSettings);
			if (affectedRows > 0)
			{
				insertedId = (int)salePriceListTemplateId;
				return true;
			}
			insertedId = -1;
			return false;
		}

		public bool Update(SalePriceListTemplate salePriceListTemplate)
		{
			string serializedSettings = GetSerializedObject(salePriceListTemplate.Settings);
			int affectedRows = ExecuteNonQuerySP("TOneWhS_BE.sp_SalePriceListTemplate_Update", salePriceListTemplate.SalePriceListTemplateId, salePriceListTemplate.Name, serializedSettings);
			return (affectedRows > 0);
		}

		public bool AreSalePriceListTemplatesUpdated(ref object updateHandle)
		{
			return base.IsDataUpdated("TOneWhS_BE.SalePriceListTemplate", ref updateHandle);
		}

		#endregion

		#region Mappers

		private SalePriceListTemplate SalePriceListTemplateMapper(IDataReader reader)
		{
			return new SalePriceListTemplate()
			{
				SalePriceListTemplateId = (int)reader["ID"],
				Name = reader["Name"] as string,
				Settings = Vanrise.Common.Serializer.Deserialize<SalePriceListTemplateSettings>(reader["Settings"] as string)
			};
		}
		
		#endregion

		#region Private Methods
		
		private string GetSerializedObject(object objectToSerialize)
		{
			if (objectToSerialize != null)
				return Vanrise.Common.Serializer.Serialize(objectToSerialize);
			return null;
		}
		
		#endregion
	}
}
