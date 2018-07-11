using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    public class ServiceDataManager : BaseSQLDataManager, IServiceDataManager
    {

        #region Constructors
        public ServiceDataManager() :
            base(GetConnectionStringName("DemoProjectCallCenter_DBConnStringKey", "DemoProjectCallCenter_DBConnStringKey"))
        {
        }
        #endregion

        #region Public Methods
        public List<Service> GetServices()
        {
            return GetItemsSP("[CcEntities].[sp_Service_GetServices]", ServiceMapper);
        }
        #endregion  


        #region Mappers
        Service ServiceMapper(IDataReader reader)
        {
            return new Service
            {
                ServiceId = GetReaderValue<long>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name"),
                Status = GetReaderValue<string>(reader, "Status")
            };
        }
        #endregion
    }
}
