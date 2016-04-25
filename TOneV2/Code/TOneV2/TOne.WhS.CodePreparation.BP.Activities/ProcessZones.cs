using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Business;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;
using Vanrise.BusinessProcess;
using Vanrise.Entities;

namespace TOne.WhS.CodePreparation.BP.Activities
{

    public sealed class ProcessZones : CodeActivity
    {


        [RequiredArgument]
        public InArgument<Changes> Changes { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<int, IEnumerable<SaleCode>>> ZoneBySaleCodes { get; set; }

        [RequiredArgument]
        public InOutArgument<IEnumerable<CodeToMove>> CodesToMove { get; set; }

        [RequiredArgument]
        public InOutArgument<IEnumerable<CodeToClose>> CodesToClose { get; set; }



        protected override void Execute(CodeActivityContext context)
        {
            Changes changes = Changes.Get(context);
            Dictionary<int, IEnumerable<SaleCode>> zoneBySaleCodes = ZoneBySaleCodes.Get(context);
            List<CodeToMove> codesToMove = CodesToMove.Get(context).ToList();
            List<CodeToClose> codesToClose = CodesToClose.Get(context).ToList();
            DateTime minimumDate = MinimumDate.Get(context);

            SaleZoneManager saleZoneManager = new SaleZoneManager();
            CodeGroupManager codeGroupManager = new CodeGroupManager();
            string oldZoneName;

            foreach (RenamedZone renamedZone in changes.RenamedZones)
            {
                oldZoneName = saleZoneManager.GetSaleZoneName((long)renamedZone.ZoneId);
                IEnumerable<SaleCode> saleCodes = zoneBySaleCodes.GetRecord(renamedZone.ZoneId.Value);

                foreach (SaleCode saleCode in saleCodes)
                {
                    CodeToMove codeToMove = new CodeToMove();
                    CodeGroup codeGroup = codeGroupManager.GetMatchCodeGroup(saleCode.Code);
                    if (ValidateSaleCodeInRenamedZone(saleCode, changes))
                    {
                        codesToMove.Add(new CodeToMove
                        {
                            Code = saleCode.Code,
                            CodeGroup = codeGroup,
                            BED = minimumDate,
                            EED = null,
                            ZoneName = renamedZone.NewZoneName,
                            OldZoneName = oldZoneName
                        });
                    }
                }
            }

            foreach (DeletedZone deletedZone in changes.DeletedZones)
            {
                IEnumerable<SaleCode> saleCodes = zoneBySaleCodes.GetRecord(deletedZone.ZoneId);
                foreach (SaleCode saleCode in saleCodes)
                {
                    if (ValidateSaleCodeInDeletedZone(saleCode, changes))
                    {
                        CodeGroup codeGroup = codeGroupManager.GetMatchCodeGroup(saleCode.Code);
                        codesToClose.Add(new CodeToClose
                        {
                            Code = saleCode.Code,
                            CodeGroup = codeGroup,
                            CloseEffectiveDate = minimumDate,
                            ZoneName = deletedZone.ZoneName,
                        });
                    }
                }
            }

                CodesToMove.Set(context, codesToMove);

                CodesToClose.Set(context, codesToClose);

        }


        private bool ValidateSaleCodeInRenamedZone(SaleCode saleCode, Changes changes)
        {
            if (!changes.NewCodes.Any(x => x.Code == saleCode.Code) && !changes.DeletedCodes.Any(x => x.Code == saleCode.Code))
                return true;
            return false;
        }

        private bool ValidateSaleCodeInDeletedZone(SaleCode saleCode, Changes changes)
        {
            if (!changes.NewCodes.Any(x => x.Code == saleCode.Code && x.ZoneId.HasValue && x.ZoneId.Value == saleCode.ZoneId) && !changes.DeletedCodes.Any(x => x.Code == saleCode.Code))
                return true;
            return false;
        }

    }
}
