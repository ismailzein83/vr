using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class DeportedNumberRequestInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public string OldLinePath { get; set; }
        public string NewLinePath { get; set; }

    }
}
