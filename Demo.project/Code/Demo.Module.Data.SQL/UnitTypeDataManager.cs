using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Demo.Module.Data.SQL
{
    public class UnitTypeDataManager : BaseSQLDataManager, IUnitTypeDataManager
    {

        #region ctor/Local Variables

        private static Dictionary<string, string> _mapper = new Dictionary<string, string>();

        public UnitTypeDataManager()
            : base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoDBConnectionString"))
        {

        }
        #endregion

        #region Public Methods
      
        public IEnumerable<UnitType> GetAllUnitTypes()
        {
            return GetItemsSP("[dbo].[sp_UnitType_GetAll]", UnitTypeMapper);
        }

        public bool AreUnitTypeUpdated(ref object lastReceivedDataInfo)
        {
            return IsDataUpdated("dbo.UnitType", ref lastReceivedDataInfo);
        }
        #endregion

        #region Private Methods

        #endregion

        #region Mappers
        UnitType UnitTypeMapper(IDataReader reader)
        {
            UnitType unitType = new UnitType
            {
                UnitTypeId = (int)reader["ID"],
                Description = reader["Description"] as string
            };
            return unitType;
        }        
        #endregion
    }
}
