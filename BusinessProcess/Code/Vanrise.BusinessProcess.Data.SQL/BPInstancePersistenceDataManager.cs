using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.BusinessProcess.Data.SQL
{
    public class BPInstancePersistenceDataManager : BaseSQLDataManager, IBPInstancePersistenceDataManager
    {
        #region Properties/Ctor

       

        public BPInstancePersistenceDataManager()
            : base(GetConnectionStringName("BusinessProcessDBConnStringKey", "BusinessProcessDBConnString"))
        {

        }

        #endregion

        public void InsertOrUpdateInstance(long processInstanceId, string instanceState)
        {
            string query = String.Format(@"IF NOT EXISTS (SELECT TOP 1 NULL FROM [bp].[BPInstancePersistence] WITH (NOLOCK) WHERE [ProcessInstanceID] = @ProcessInstanceID)
                                            INSERT INTO [bp].[BPInstancePersistence] ([ProcessInstanceID], [BPState]) VALUES (@ProcessInstanceID, @InstanceState)
                                            ELSE
                                            UPDATE [bp].[BPInstancePersistence] SET [BPState] = @InstanceState WHERE [ProcessInstanceID] = @ProcessInstanceID
                                            ");
            ExecuteNonQueryText(query, (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@ProcessInstanceID", processInstanceId));
                    cmd.Parameters.Add(new SqlParameter("@InstanceState", instanceState));
                });
        }

        public string GetInstanceState(long processInstanceId)
        {
            return ExecuteScalarText(@"SELECT [BPState] FROM [bp].[BPInstancePersistence] WHERE [ProcessInstanceID] = @ProcessInstanceID", 
                (cmd) => cmd.Parameters.Add(new SqlParameter("@ProcessInstanceID", processInstanceId))) as string;
        }

        public void DeleteState(long processInstanceId)
        {
            ExecuteNonQueryText(@"DELETE [bp].[BPInstancePersistence] WHERE [ProcessInstanceID] = @ProcessInstanceID",
                (cmd) => cmd.Parameters.Add(new SqlParameter("@ProcessInstanceID", processInstanceId)));
        }
    }
}
