﻿using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Retail.BusinessEntity.Data.SQL
{
    public class PackageDataManager :BaseSQLDataManager, IPackageDataManager
    {
           
        #region ctor/Local Variables
        public PackageDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "Retail_BE_DBConnString"))
        {

        }
        #endregion

        #region Public Methods
        public bool Insert(Package package, out int insertedId)
        {
            object packageId;

            int recordsEffected = ExecuteNonQuerySP("Retail.sp_Package_Insert", out packageId, package.Name, package.Description, Vanrise.Common.Serializer.Serialize(package.Settings));
            bool insertedSuccesfully = (recordsEffected > 0);
            if (insertedSuccesfully)
                insertedId = (int)packageId;
            else
                insertedId = 0;
            return insertedSuccesfully;
        }
        public bool Update(Package package)
        {
            int recordsEffected = ExecuteNonQuerySP("Retail.sp_Package_Update", package.PackageId, package.Name, package.Description, Vanrise.Common.Serializer.Serialize(package.Settings));
            return (recordsEffected > 0);
        }
        public bool ArePackagesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Retail.Package", ref updateHandle);
        }
        public List<Package> GetPackages()
        {
            return GetItemsSP("Retail.sp_Package_GetAll", PackageMapper);
        }
        #endregion

        #region Private Methods

        #endregion

        #region  Mappers
        private Package PackageMapper(IDataReader reader)
        {
            Package package = new Package
            {
                PackageId = (int)reader["ID"],
                Description = reader["Description"] as string,
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<PackageSettings>(reader["Settings"] as string)
            };
            return package;
        }

        #endregion
    }
}
