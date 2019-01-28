using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class ProcessStateDataManager : BaseSQLDataManager, IProcessStateDataManager
    {
        #region ctor/Local Variables
        public ProcessStateDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }

        #endregion

        #region Public Methods

        public List<ProcessState> GetProcessStates()
        {
            return GetItemsSP("[Common].[sp_ProcessState_GetAll]", ProcessStateEntityMapper);
        }

        public bool InsertOrUpdate(string processStateUniqueName, ProcessStateSettings settings)
        {
            return ExecuteNonQuerySP("[Common].[sp_ProcessState_InsertOrUpdate]", processStateUniqueName, Vanrise.Common.Serializer.Serialize(settings)) > 0;
        }

        #endregion

        #region Private Methods
        private ProcessState ProcessStateEntityMapper(IDataReader reader)
        {
            return new ProcessState()
            {
                UniqueName = reader["UniqueName"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<ProcessStateSettings>(reader["Settings"] as string)
            };
        }
        #endregion
    }
}