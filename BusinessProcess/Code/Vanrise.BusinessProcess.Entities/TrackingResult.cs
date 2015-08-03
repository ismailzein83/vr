
namespace Vanrise.BusinessProcess.Entities
{
    public class TrackingResult : Vanrise.Entities.BigResult<BPTrackingMessage>
    {
        public BPInstanceStatus InstanceStatus { get; set; }
    }
}
