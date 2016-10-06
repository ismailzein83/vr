using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Data.SQL;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data.SQL
{
    public class SchedulerTaskActionTypeDataManager : BaseSQLDataManager, ISchedulerTaskActionTypeDataManager 
    {
        public SchedulerTaskActionTypeDataManager()
            : base(GetConnectionStringName("RuntimeConfigDBConnStringKey", "RuntimeConfigDBConnString"))
        {

        }

        public List<Entities.SchedulerTaskActionType> GetAll()
        {
            return GetItemsSP("runtime.sp_SchedulerTaskActionType_GetAll", SchedulerTaskActionTypeMapper);
        }

        #region Private Methods

        private SchedulerTaskActionType SchedulerTaskActionTypeMapper(IDataReader reader)
        {
            return new SchedulerTaskActionType
            {
                ActionTypeId = GetReaderValue<Guid>(reader,"ID"),
                Name = reader["Name"] as string,
                Info = Serializer.Deserialize<ActionTypeInfo>(reader["ActionTypeInfo"] as string)
            };
        }

        #endregion
    }
}
