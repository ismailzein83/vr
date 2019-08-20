using System;

namespace Vanrise.GenericData.Entities
{
    public interface ISetRecordDescription
    {
        void SetDescription(ISetRecordDescriptionContext context);
    }

    public interface ISetRecordDescriptionContext
    {
        string Description { get; }
    }

    public class SetRecordDescriptionContext : ISetRecordDescriptionContext
    {
        public string Description { get; set; }
    }
}