using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class BusinessObjectDataProviderSettings
    {
        public BusinessObjectDataProviderExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class BusinessObjectDataProviderExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract void LoadRecords(IBusinessObjectDataProviderLoadRecordsContext context);

        public abstract bool DoesSupportFilterOnAllFields { get; }
    }

    public interface IBusinessObjectDataProviderLoadRecordsContext
    {
        DateTime? FromTime { get; }

        DateTime? ToTime { get; }

        RecordFilterGroup FilterGroup { get; }

        int? LimitResult { get; }

        OrderDirection? OrderDirection { get; }

        bool IsLoadStopped { get; }

        List<string> Fields { get; }

        void OnRecordLoaded(DataRecordObject dataRecordObject, DateTime recordTime);
    }
}
