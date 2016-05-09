﻿using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;

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
                    CodeToAdd includedCodeOneMatchToAdd = zone.CodesToAdd.FirstOrDefault();
                    CodeToMove includedCodeOneMatchToMove = zone.CodesToMove.FirstOrDefault();
                    CodeToClose includedCodeOneMatchToClose = zone.CodesToClose.FirstOrDefault();
                    int countryId;
                    if(includedCodeOneMatchToAdd !=null )
                    {
                        countryId = includedCodeOneMatchToAdd.CodeGroup.CountryId;
                        
                    }
                    else if (includedCodeOneMatchToMove != null)
                    {
                        countryId = includedCodeOneMatchToMove.CodeGroup.CountryId;
                    }
                    else if (includedCodeOneMatchToClose != null)
                    {
                        countryId = includedCodeOneMatchToClose.CodeGroup.CountryId;
                    }
                    else
                    {
                        continue;
                    }

                       

                    if (!countriesToProcessByCountryId.TryGetValue(countryId, out countryToProcess))
                    {                        
                        countryToProcess = new CountryToProcess();
                        countryToProcess.CountryId = countryId;
                        countryToProcess.CodesToAdd = new List<CodeToAdd>();
                        countryToProcess.CodesToMove = new List<CodeToMove>();
                        countryToProcess.CodesToClose = new List<CodeToClose>();
                        countriesToProcessByCountryId.Add(countryId, countryToProcess);
                    }

                    IEnumerable<CodeToAdd> codesToAdd = zone.CodesToAdd;
                    foreach (CodeToAdd code in codesToAdd)
                        countryToProcess.CodesToAdd.Add(code);

                    IEnumerable<CodeToMove> codesToMove = zone.CodesToMove;
                    foreach (CodeToMove code in codesToMove)
                        countryToProcess.CodesToMove.Add(code);

                    IEnumerable<CodeToClose> codesToClose = zone.CodesToClose;
                    foreach (CodeToClose code in codesToClose)
                        countryToProcess.CodesToClose.Add(code);

            }

            this.CountriesToProcess.Set(context, countriesToProcessByCountryId.Values);
        }
    }
}
