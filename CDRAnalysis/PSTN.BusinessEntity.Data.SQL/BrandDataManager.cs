﻿using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Data;

namespace PSTN.BusinessEntity.Data.SQL
{
    public class BrandDataManager : Vanrise.Data.SQL.BaseSQLDataManager, IBrandDataManager
    {
        public BrandDataManager() : base("CDRDBConnectionString") { }

        public List<SwitchBrand> GetBrands()
        {
            return GetItemsSP("PSTN_BE.sp_SwitchType_GetAll", BrandMapper);
        }

        public Vanrise.Entities.BigResult<SwitchBrand> GetFilteredBrands(Vanrise.Entities.DataRetrievalInput<SwitchBrandQuery> input)
        {
            return RetrieveData(input, (tempTableName) =>
            {
                ExecuteNonQuerySP("PSTN_BE.sp_SwitchType_CreateTempByName", tempTableName, input.Query.Name);
            }, (reader) => BrandMapper(reader));
        }

        public SwitchBrand GetBrandById(int brandId)
        {
            return GetItemSP("PSTN_BE.sp_SwitchType_GetByID", BrandMapper, brandId);
        }

        public bool AddBrand(SwitchBrand brandObj, out int insertedId)
        {
            object brandId;

            int recordsAffected = ExecuteNonQuerySP("PSTN_BE.sp_SwitchType_Insert", out brandId, brandObj.Name);

            insertedId = (recordsAffected > 0) ? (int)brandId : -1;
            return (recordsAffected > 0);
        }

        public bool UpdateBrand(SwitchBrand brandObj)
        {
            int recordsAffected = ExecuteNonQuerySP("PSTN_BE.sp_SwitchType_Update", brandObj.BrandId, brandObj.Name);
            return (recordsAffected > 0);
        }

        public bool DeleteBrand(int brandId)
        {
            int recordsEffected = ExecuteNonQuerySP("PSTN_BE.sp_SwitchType_Delete", brandId);
            return (recordsEffected > 0);
        }

        #region Mappers

        SwitchBrand BrandMapper(IDataReader reader)
        {
            SwitchBrand type = new SwitchBrand();

            type.BrandId = (int)reader["ID"];
            type.Name = reader["Name"] as string;

            return type;
        }

        #endregion
    }
}
