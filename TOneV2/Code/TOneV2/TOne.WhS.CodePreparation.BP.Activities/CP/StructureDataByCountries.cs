using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.CP.Processing;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public sealed class StructureDataByCountries : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ZoneToProcess>> ZonesToProcess { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<CountryToProcess>> CountriesToProcess { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ZoneToProcess> zonesToProcess = this.ZonesToProcess.Get(context);

            Dictionary<int, CountryToProcess> countriesToProcessByCountryId = new Dictionary<int, CountryToProcess>();
            CountryToProcess countryToProcess;

            foreach (ZoneToProcess zone in zonesToProcess)
            {
                if (!zone.IsExcluded)
                {
                    CodeToAdd includedCodeOneMatch = zone.CodesToAdd.Find(x => !x.IsExcluded);
                    if (includedCodeOneMatch == null)
                        continue;

                    int countryId = includedCodeOneMatch.CodeGroup.CountryId;

                    if (!countriesToProcessByCountryId.TryGetValue(countryId, out countryToProcess))
                    {
                        countryToProcess = new CountryToProcess();
                        countryToProcess.CodesToAdd = new List<CodeToAdd>();
                        countryToProcess.CodesToMove = new List<CodeToMove>();
                        countryToProcess.CodesToClose = new List<CodeToClose>();
                        countriesToProcessByCountryId.Add(countryId, countryToProcess);
                    }

                    IEnumerable<CodeToAdd> codesToAdd = zone.CodesToAdd.Where(x => !x.IsExcluded);
                    foreach (CodeToAdd code in codesToAdd)
                        countryToProcess.CodesToAdd.Add(code);

                    IEnumerable<CodeToMove> codesToMove = zone.CodesToMove.Where(x => !x.IsExcluded);
                    foreach (CodeToMove code in codesToMove)
                        countryToProcess.CodesToMove.Add(code);

                    IEnumerable<CodeToClose> codesToClose = zone.CodesToClose.Where(x => !x.IsExcluded);
                    foreach (CodeToClose code in codesToClose)
                        countryToProcess.CodesToClose.Add(code);

                
                }
            }

            this.CountriesToProcess.Set(context, countriesToProcessByCountryId.Values);
        }
    }
}
