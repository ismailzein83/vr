﻿using System;
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
    public class SchedulerTaskTriggerTypeDataManager : BaseSQLDataManager, ISchedulerTaskTriggerTypeDataManager 
    {
        public SchedulerTaskTriggerTypeDataManager()
            : base(GetConnectionStringName("RuntimeConnStringKey", "RuntimeDBConnString"))
        {

        }

        public List<Entities.SchedulerTaskTriggerType> GetAll()
        {
            return GetItemsSP("runtime.sp_SchedulerTaskTriggerType_GetAll", SchedulerTaskTriggerTypeMapper);
        }

        #region Private Methods

        private SchedulerTaskTriggerType SchedulerTaskTriggerTypeMapper(IDataReader reader)
        {
            return new SchedulerTaskTriggerType
            {
                TriggerTypeId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Info = Serializer.Deserialize<TriggerTypeInfo>(reader["TriggerTypeInfo"] as string)
            };
        }

        #endregion
    }
}
