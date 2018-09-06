using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.BP.Activities
{
    public sealed class StructureDataByZones : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<CodeToAdd>> CodesToAdd { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<CodeToMove>> CodesToMove { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<CodeToClose>> CodesToClose { get; set; }
      
        [RequiredArgument]
        public OutArgument<IEnumerable<ZoneToProcess>> ZonesToProcess { get; set; }

		[RequiredArgument]
		public OutArgument<AllZonesToProcess> AllZonesToProcess { get; set; }

		protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<CodeToAdd> codesToAdd = this.CodesToAdd.Get(context);
            IEnumerable<CodeToMove> codesToMove = this.CodesToMove.Get(context);
            IEnumerable<CodeToClose> codesToClose = this.CodesToClose.Get(context);

            Dictionary<string, ZoneToProcess> zoneToProcessByZoneName = new Dictionary<string, ZoneToProcess>(StringComparer.InvariantCultureIgnoreCase);
            ZoneToProcess zoneToProcess;
            foreach (CodeToAdd code in codesToAdd)
            {
                if (!zoneToProcessByZoneName.TryGetValue(code.ZoneName, out zoneToProcess))
                {
                    zoneToProcess = new ZoneToProcess();
                    zoneToProcess.ZoneName = code.ZoneName;
                    zoneToProcessByZoneName.Add(code.ZoneName, zoneToProcess);
                }

                zoneToProcess.CodesToAdd.Add(code);
            }

            foreach (CodeToMove code in codesToMove)
            {
                if (!zoneToProcessByZoneName.TryGetValue(code.ZoneName, out zoneToProcess))
                {
                    if (zoneToProcess==null)
                        zoneToProcess = new ZoneToProcess();
                    zoneToProcess.ZoneName = code.ZoneName;
                    zoneToProcessByZoneName.Add(code.ZoneName, zoneToProcess);
                }

                zoneToProcess.CodesToMove.Add(code);
            }

            foreach (CodeToClose code in codesToClose)
            {

                if (!zoneToProcessByZoneName.TryGetValue(code.ZoneName, out zoneToProcess))
                {
                    if (zoneToProcess == null)
                        zoneToProcess = new ZoneToProcess();
                    zoneToProcess.ZoneName = code.ZoneName;
                    zoneToProcessByZoneName.Add(code.ZoneName, zoneToProcess);
                }

                zoneToProcess.CodesToClose.Add(code);
            }

            this.ZonesToProcess.Set(context, zoneToProcessByZoneName.Values);

			AllZonesToProcess.Set(context, new AllZonesToProcess() { Zones = zoneToProcessByZoneName.Values });
		}
	}
}
