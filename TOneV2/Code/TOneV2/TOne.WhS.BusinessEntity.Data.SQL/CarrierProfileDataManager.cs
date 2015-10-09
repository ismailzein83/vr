using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class CarrierProfileDataManager : BaseSQLDataManager, ICarrierProfileDataManager
    {
        
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();

        static CarrierProfileDataManager()
        {
            _columnMapper.Add("CarrierProfileId", "ID");
        }
        public CarrierProfileDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        public Vanrise.Entities.BigResult<Entities.CarrierProfile> GetFilteredCarrierProfiles(Vanrise.Entities.DataRetrievalInput<Entities.CarrierProfileQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                //string saleZonePackageIdsParam = null;
                //if (input.Query.SaleZonePackageIds != null)
                //    saleZonePackageIdsParam = string.Join(",", input.Query.SaleZonePackageIds);

                ExecuteNonQuerySP("TOneWhS_BE.sp_CarrierProfile_CreateTempByFiltered", tempTableName, input.Query.CarrierProfileId, input.Query.Name);
            };

            return RetrieveData(input, createTempTableAction, CarrierProfileMapper, _columnMapper);
        }

        public CarrierProfile GetCarrierProfile(int carrierProfileId)
        {
            return GetItemSP("TOneWhS_BE.sp_CarrierProfile_Get", CarrierProfileMapper, carrierProfileId);
        }

        public List<CarrierProfileInfo> GetAllCarrierProfiles()
        {
            return GetItemsSP("TOneWhS_BE.sp_CarrierProfile_GetAll", CarrierProfileInfoMapper);
        }
        private CarrierProfileInfo CarrierProfileInfoMapper(IDataReader reader)
        {
            CarrierProfileInfo carrierProfileInfo = new CarrierProfileInfo
            {
                CarrierProfileId = (int)reader["ID"],
                Name = reader["Name"] as string
            };
            return carrierProfileInfo;
        }

        private CarrierProfile CarrierProfileMapper(IDataReader reader)
        {
            CarrierProfile carrierProfile = new CarrierProfile
            {
                CarrierProfileId = (int)reader["ID"],
                Name = reader["Name"] as string,
              
            };
            return carrierProfile;
        }
    }
}
