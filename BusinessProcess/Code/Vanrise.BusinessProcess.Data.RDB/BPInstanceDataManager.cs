//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vanrise.BusinessProcess.Entities;

//namespace Vanrise.BusinessProcess.Data.RDB
//{
//    public class BPInstanceDataManager : IBPInstanceDataManager
//    {
//        public Dictionary<Guid, Type> InputArgumentTypeByDefinitionId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

//        public void ArchiveInstances(List<BPInstanceStatus> completedStatuses, DateTime completedBefore, int nbOfInstances)
//        {
//            throw new NotImplementedException();
//        }

//        public List<BPInstance> GetAfterId(long? processInstanceId, Guid bpDefinitionId, bool getFromArchive)
//        {
//            throw new NotImplementedException();
//        }

//        public List<BPInstance> GetBeforeId(BPInstanceBeforeIdInput input, List<int> grantedPermissionSetIds, bool getFromArchive)
//        {
//            throw new NotImplementedException();
//        }

//        public List<BPDefinitionSummary> GetBPDefinitionSummary(IEnumerable<BPInstanceStatus> executionStatus)
//        {
//            throw new NotImplementedException();
//        }

//        public BPInstance GetBPInstance(long bpInstanceId, bool getFromArchive)
//        {
//            throw new NotImplementedException();
//        }

//        public List<BPInstance> GetFilteredBPInstances(BPInstanceQuery query, List<int> grantedPermissionSetIds, bool getFromArchive)
//        {
//            throw new NotImplementedException();
//        }

//        public List<BPInstance> GetFirstPage(out byte[] maxTimeStamp, int nbOfRows, List<Guid> definitionsId, int parentId, List<string> entityIds, List<int> grantedPermissionSetIds, Guid? taskId)
//        {
//            throw new NotImplementedException();
//        }

//        public List<BPInstance> GetFirstPageFromArchive(int nbOfRows, List<Guid> definitionsId, int parentId, List<string> entityIds, List<int> grantedPermissionSetIds, Guid? taskId)
//        {
//            throw new NotImplementedException();
//        }

//        public List<BPInstance> GetPendingInstances(Guid definitionId, IEnumerable<BPInstanceStatus> acceptableBPStatuses, BPInstanceAssignmentStatus assignmentStatus, int maxCounts, Guid serviceInstanceId)
//        {
//            throw new NotImplementedException();
//        }

//        public List<BPInstance> GetPendingInstancesInfo(IEnumerable<BPInstanceStatus> statuses)
//        {
//            throw new NotImplementedException();
//        }

//        public List<BPInstance> GetUpdated(ref byte[] maxTimeStamp, int nbOfRows, List<Guid> definitionsId, int parentId, List<string> entityIds, List<int> grantedPermissionSetIds, Guid? taskId)
//        {
//            throw new NotImplementedException();
//        }

//        public bool HasRunningInstances(Guid definitionId, List<string> entityIds, IEnumerable<BPInstanceStatus> statuses)
//        {
//            throw new NotImplementedException();
//        }

//        public long InsertInstance(BPInstanceToAdd bpInstanceToAdd)
//        {
//            throw new NotImplementedException();
//        }

//        public void SetCancellationRequestUserId(long bpInstanceId, List<BPInstanceStatus> allowedStatuses, int cancelRequestByUserId)
//        {
//            throw new NotImplementedException();
//        }

//        public void UpdateInstanceAssignmentStatus(long processInstanceId, BPInstanceAssignmentStatus assignmentStatus)
//        {
//            throw new NotImplementedException();
//        }

//        public void UpdateInstanceLastMessage(long processInstanceId, string message)
//        {
//            throw new NotImplementedException();
//        }

//        public void UpdateInstanceStatus(long processInstanceId, BPInstanceStatus status, BPInstanceAssignmentStatus assignmentStatus, string message, bool clearServiceInstanceId, Guid? workflowInstanceId)
//        {
//            throw new NotImplementedException();
//        }

//        public void UpdateServiceInstancesAndAssignmentStatus(List<BPInstance> pendingInstancesToUpdate)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
