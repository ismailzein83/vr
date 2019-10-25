using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Retail.NIM.Business
{
    public class NodePortInfoFilter : IGenericBusinessEntityFilter
    {
        public long NodeId { get; set; }
        public long? NodePartId { get; set; }

        public bool IsMatch(IGenericBusinessEntityFilterContext context)
        {
            if (context != null)
            {
                if (context.GenericBusinessEntity != null && context.GenericBusinessEntity.FieldValues != null && context.GenericBusinessEntity.FieldValues.Count > 0)
                {
                    long nodeId = (long)context.GenericBusinessEntity.FieldValues.GetRecord("Node");
                    long? nodePartId =(long?) context.GenericBusinessEntity.FieldValues.GetRecord("Part");

                    if (!NodeId.Equals(nodeId))
                    {
                        return false;
                    }

                    if (NodePartId.HasValue && nodePartId.HasValue && !NodePartId.Value.Equals(nodePartId.Value))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}

