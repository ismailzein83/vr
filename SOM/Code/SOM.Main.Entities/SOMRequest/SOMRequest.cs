using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM.Main.Entities
{
    public enum SOMRequestStatus { New = 0, Running = 20, Waiting = 30, Completed = 50, Aborted = 60 }

    public class SOMRequest
    {
        public long SOMRequestId { get; set; }

        public string EntityId { get; set; }

        public string Title { get; set; }

        public long? ProcessInstanceId { get; set; }

        public SOMRequestStatus Status { get; set; }

        public string WaitingInfo { get; set; }

        public SOMRequestSettings Settings { get; set; }

        public DateTime CreatedTime { get; set; }
    }

    public class SOMRequestSettings
    {
        public SOMRequestExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class SOMRequestExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract BaseSOMRequestBPInputArg ConvertToBPInputArgument(ISOMRequestConvertToBPInputArgumentContext context);
    }

    public abstract class BaseSOMRequestBPInputArg : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public Guid SOMRequestTypeId { get; set; }

        public long SOMRequestId { get; set; }
    }

    public interface ISOMRequestConvertToBPInputArgumentContext
    {

    }

}
