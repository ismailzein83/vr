//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vanrise.Data.SQL;
//using Vanrise.Entities;

//namespace Vanrise.Common.Data.SQL
//{
//    public class ErlangBCalculatorDataManager : BaseSQLDataManager, IErlangBCalculatorDataManager
//    {
//        #region ctor/Local Variables
//        public ErlangBCalculatorDataManager()
//             : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
//        {

//        }

//        #endregion

//        public bool AreGetErlangBValuesUpdated(ref object updateHandle)
//        {
//            return base.IsDataUpdated("common.ErlangB", ref updateHandle);
//        }

//        public IEnumerable<ErlangBEntity> GetErlangBValues()
//        {
//            return GetItemsSP("Common.sp_ErlangB_GetAll", ErlangBEntityMapper);
//        }

//        public void Insert(ErlangBEntity erlangBValueEntity)
//        {
//            ExecuteNonQuerySP("Common.sp_ErlangB_InsertIfNotExist", erlangBValueEntity.NumberOfDevices, erlangBValueEntity.BlockingPercentage, erlangBValueEntity.BHT);
//        }
//        ErlangBEntity ErlangBEntityMapper(IDataReader reader)
//        {
//            return new ErlangBEntity
//            {

//                NumberOfDevices = GetReaderValue<int>(reader, "NumberOfDevices"),
//                BlockingPercentage =(float)Convert.ToDecimal(reader["BlockingPercentage"]),
//                BHT = (double)reader["BHT"]
//            };
//        }
//    }
//}
