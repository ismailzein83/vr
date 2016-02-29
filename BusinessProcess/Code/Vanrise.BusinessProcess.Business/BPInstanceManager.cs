using System.Collections.Generic;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Business
{
    public class BPInstanceManager
    {
        public List<BPInstanceDetail> GetBeforeId(BPInstanceBeforeIdInput input)
        {

            IBPInstanceDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPInstanceDataManager>();

            List<BPInstance> bpInstances = dataManager.GetBeforeId(input);
            List<BPInstanceDetail> bpInstanceDetails = new List<BPInstanceDetail>();
            foreach (BPInstance bpInstance in bpInstances)
            {
                bpInstanceDetails.Add(BPInstanceDetailMapper(bpInstance));
            }
            return bpInstanceDetails;
        }

        public BPInstanceUpdateOutput GetUpdated(ref byte[] maxTimeStamp, int nbOfRows, List<int> definitionsId)
        {
            BPInstanceUpdateOutput bpInstanceUpdateOutput = new BPInstanceUpdateOutput();

            IBPInstanceDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPInstanceDataManager>();

            List<BPInstance> bpInstances = dataManager.GetUpdated(ref maxTimeStamp, nbOfRows, definitionsId);
            List<BPInstanceDetail> bpInstanceDetails = new List<BPInstanceDetail>();
            foreach (BPInstance bpInstance in bpInstances)
            {
                bpInstanceDetails.Add(BPInstanceDetailMapper(bpInstance));
            }

            bpInstanceUpdateOutput.ListBPInstanceDetails = bpInstanceDetails;
            bpInstanceUpdateOutput.MaxTimeStamp = maxTimeStamp;
            return bpInstanceUpdateOutput;
        }

        private BPInstanceDetail BPInstanceDetailMapper(BPInstance bpInstance)
        {
            if (bpInstance == null)
                return null;
            return new BPInstanceDetail() 
            {
                Entity = bpInstance
            };
        }
    }
}
