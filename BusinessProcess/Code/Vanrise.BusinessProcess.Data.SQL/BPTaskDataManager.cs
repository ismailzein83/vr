using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;
using System.Linq;

namespace Vanrise.BusinessProcess.Data.SQL
{
    public class BPTaskDataManager : BaseSQLDataManager, IBPTaskDataManager
    {
        public BPTaskDataManager()
            : base(GetConnectionStringName("BusinessProcessDBConnStringKey", "BusinessProcessDBConnString"))
        {

        }
        #region public methods

        public bool InsertTask(string title, long processInstanceId, int typeId, IEnumerable<int> assignedUserIds, BPTaskStatus bpTaskStatus, BPTaskInformation taskInformation, out long taskId)
        {
            string assignedUsers = null;
            if (assignedUserIds != null && assignedUserIds.Count() > 0)
                assignedUsers = string.Join<int>(",", assignedUserIds);

            object obj;
            taskId = default(long);

            if (ExecuteNonQuerySP("[bp].[sp_BPTask_Insert]", out obj, title, processInstanceId, typeId, taskInformation != null ? Serializer.Serialize(taskInformation) : null, (int)bpTaskStatus, assignedUsers) > 0)
            {
                taskId = (long)obj;
                return true;
            }
            else
                return false;
        }

        public BPTask GetTask(long taskId)
        {
            return GetItemSP("[bp].[sp_BPTask_GetByID]", BPTaskMapper, taskId);
        }

        public void UpdateTaskExecution(long taskId, int executedByUserId, BPTaskStatus bpTaskStatus, BPTaskExecutionInformation bPTaskExecutionInformation)
        {
            ExecuteNonQuerySP("[bp].[sp_BPTask_UpdateTaskExecution]", taskId, executedByUserId, (int)bpTaskStatus, bPTaskExecutionInformation != null ? Serializer.Serialize(bPTaskExecutionInformation) : null);
        }

        public List<BPTask> GetUpdated(ref byte[] maxTimeStamp, int nbOfRows, int? processInstanceId, int? userId)
        {
            List<BPTask> bpTasks = new List<BPTask>();
            byte[] timestamp = null;

            ExecuteReaderSP("[bp].[sp_BPTask_GetUpdated]", (reader) =>
            {
                while (reader.Read())
                    bpTasks.Add(BPTaskMapper(reader));
                if (reader.NextResult())
                    while (reader.Read())
                        timestamp = GetReaderValue<byte[]>(reader, "MaxTimestamp");
            },
               maxTimeStamp, nbOfRows, processInstanceId, userId);
            maxTimeStamp = timestamp;
            return bpTasks;
        }

        public List<BPTask> GetBeforeId(long lessThanID, int nbOfRows, int? processInstanceId, int? userId)
        {
            return GetItemsSP("[bp].[sp_BPTask_GetBeforeID]", BPTaskMapper, lessThanID, nbOfRows, processInstanceId, userId);
        }

        public IEnumerable<BPTaskType> GetBPTaskTypes()
        {
            return GetItemsSP("bp.sp_BPTaskType_GetAll", BPTaskTypeMapper);
        }

        public bool AreBPTaskTypesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[bp].[BPTask]", ref updateHandle);
        }
        #endregion

        #region Mappers

        BPTask BPTaskMapper(IDataReader reader)
        {
            var bpTask = new BPTask
            {
                BPTaskId = (long)reader["ID"],
                ProcessInstanceId = (long)reader["ProcessInstanceID"],
                TypeId = (int)reader["TypeID"],
                Title = reader["Title"] as string,
                ExecutedById = GetReaderValue<int?>(reader, "ExecutedBy"),
                Status = (BPTaskStatus)reader["Status"],
                CreatedTime = (DateTime)reader["CreatedTime"],
                LastUpdatedTime = GetReaderValue<DateTime>(reader, "LastUpdatedTime")
            };

            string taskInformation = reader["TaskInformation"] as string;
            if (!String.IsNullOrWhiteSpace(taskInformation))
                bpTask.TaskInformation = Serializer.Deserialize<BPTaskInformation>(taskInformation);

            string taskExecutionInformation = reader["TaskExecutionInformation"] as string;
            if (!String.IsNullOrWhiteSpace(taskExecutionInformation))
                bpTask.TaskExecutionInformation = Serializer.Deserialize<BPTaskExecutionInformation>(taskExecutionInformation);

            string assignedUsers = reader["AssignedUsers"] as string;
            if (!String.IsNullOrWhiteSpace(assignedUsers))
                bpTask.AssignedUsers = assignedUsers.Split(',').Select(itm => int.Parse(itm)).ToList();

            return bpTask;
        }

        BPTaskDetail BPTaskDetailMapper(IDataReader reader)
        {
            return new BPTaskDetail()
            {
                Entity = BPTaskMapper(reader)
            };
        }

        BPTaskType BPTaskTypeMapper(IDataReader reader)
        {
            var bpTaskType = new BPTaskType
            {
                BPTaskTypeId = (int)reader["ID"],
                Name = reader["Name"] as string
            };
            string settings = reader["Settings"] as string;
            if (!String.IsNullOrWhiteSpace(settings))
                bpTaskType.Settings = Serializer.Deserialize<BPTaskTypeSettings>(settings);
                        
            return bpTaskType;
        }
        #endregion
    }
}
