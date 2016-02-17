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
    public class ServiceTypeDataManager : BaseSQLDataManager, IServiceTypeDataManager
    {

        #region ctor/Local Variables

        private static Dictionary<string, string> _mapper = new Dictionary<string, string>();

        public ServiceTypeDataManager()
            : base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoDBConnectionString"))
        {

        }
        #endregion

        #region Public Methods
      
        public IEnumerable<ServiceType> GetAllServiceTypes()
        {
            return GetItemsSP("[dbo].[sp_ServiceType_GetAll]", ServiceTypeMapper);
        }

        public bool AreServiceTypeUpdated(ref object lastReceivedDataInfo)
        {
            return IsDataUpdated("dbo.ServiceType", ref lastReceivedDataInfo);
        }
        #endregion

        #region Private Methods

        #endregion

        #region Mappers
        ServiceType ServiceTypeMapper(IDataReader reader)
        {
            ServiceType serviceType = new ServiceType
            {
                ServiceTypeId = (int)reader["ID"],
                Description = reader["Description"] as string
            };
            return serviceType;
        }        
        #endregion
    }
}
