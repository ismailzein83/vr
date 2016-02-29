using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace Vanrise.BusinessProcess.Data.SQL
{
    public class BPInstanceDataManager : BaseSQLDataManager, IBPInstanceDataManager
    {
        public BPInstanceDataManager()
            : base(GetConnectionStringName("BusinessProcessDBConnStringKey", "BusinessProcessDBConnString"))
        {

        }

        public List<BPInstance> GetUpdated(ref byte[] maxTimeStamp, int nbOfRows, List<int> definitionsId)
        {
            string definitionsIdAsString = null;
            if (definitionsId != null && definitionsId.Count() > 0)
                definitionsIdAsString = string.Join<int>(",", definitionsId);

            List<BPInstance> bpInstances = new List<BPInstance>();
            byte[] timestamp = null;

            ExecuteReaderSP("[bp].[sp_BPInstance_GetUpdated]", (reader) =>
            {
                while (reader.Read())
                    bpInstances.Add(BPInstanceMapper(reader));
                if (reader.NextResult())
                    while (reader.Read())
                        timestamp = GetReaderValue<byte[]>(reader, "MaxTimestamp");
            },
               maxTimeStamp, nbOfRows, definitionsIdAsString);
            maxTimeStamp = timestamp;
            return bpInstances;
        }

        public List<BPInstance> GetBeforeId(BPInstanceBeforeIdInput input)
        {
            string definitionsIdAsString = null;
            if (input.DefinitionsId != null && input.DefinitionsId.Count() > 0)
                definitionsIdAsString = string.Join<int>(",", input.DefinitionsId);

            return GetItemsSP("[BP].[sp_BPInstance_GetBeforeID]", BPInstanceMapper, input.LessThanID, input.NbOfRows, definitionsIdAsString);
        }

        private BPInstance BPInstanceMapper(IDataReader reader)
        {
            BPInstance instance = new BPInstance
            {
                ProcessInstanceID = (long)reader["ID"],
                Title = reader["Title"] as string,
                ParentProcessID = GetReaderValue<long?>(reader, "ParentID"),
                DefinitionID = (int)reader["DefinitionID"],
                WorkflowInstanceID = GetReaderValue<Guid?>(reader, "WorkflowInstanceID"),
                Status = (BPInstanceStatus)reader["ExecutionStatus"],
                RetryCount = GetReaderValue<int>(reader, "RetryCount"),
                LastMessage = reader["LastMessage"] as string,
                CreatedTime = (DateTime)reader["CreatedTime"],
                StatusUpdatedTime = GetReaderValue<DateTime?>(reader, "StatusUpdatedTime")
            };

            string inputArg = reader["InputArgument"] as string;
            if (!String.IsNullOrWhiteSpace(inputArg))
                instance.InputArgument = Serializer.Deserialize(inputArg);

            return instance;
        }
    }
}
