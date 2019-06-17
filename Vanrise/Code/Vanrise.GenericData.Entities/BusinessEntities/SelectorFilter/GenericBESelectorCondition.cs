using System;

namespace Vanrise.GenericData.Entities
{
    public abstract class GenericBESelectorCondition
    {
        public abstract Guid ConfigId { get; }
        public abstract RecordFilterGroup GetFilterGroup(IGenericBESelectorConditionGetFilterGroupContext context);
    }

    public interface IGenericBESelectorConditionGetFilterGroupContext
    {

    }

    public class GenericBESelectorConditionGetFilterGroupContext : IGenericBESelectorConditionGetFilterGroupContext
    {

    }
}