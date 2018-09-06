using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.NumberingPlan.Business;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.BP.Activities
{

	public sealed class GetDataFromZoneChanges : CodeActivity
	{


		[RequiredArgument]
		public InArgument<Changes> Changes { get; set; }

		[RequiredArgument]
		public InArgument<DateTime> MinimumDate { get; set; }

		[RequiredArgument]
		public InArgument<Dictionary<long, IEnumerable<SaleCode>>> SaleCodesByZoneId { get; set; }

		[RequiredArgument]
		public InOutArgument<IEnumerable<CodeToMove>> CodesToMove { get; set; }

		[RequiredArgument]
		public InOutArgument<IEnumerable<CodeToClose>> CodesToClose { get; set; }



		protected override void Execute(CodeActivityContext context)
		{
			Changes changes = Changes.Get(context);
			Dictionary<long, IEnumerable<SaleCode>> saleCodesByZoneId = SaleCodesByZoneId.Get(context);
			List<CodeToMove> codesToMove = CodesToMove.Get(context).ToList();
			List<CodeToClose> codesToClose = CodesToClose.Get(context).ToList();
			DateTime minimumDate = MinimumDate.Get(context);

			SaleZoneManager saleZoneManager = new SaleZoneManager();
			CodeGroupManager codeGroupManager = new CodeGroupManager();


			foreach (RenamedZone renamedZone in changes.RenamedZones)
			{
				string oldZoneName = saleZoneManager.GetSaleZoneName(renamedZone.ZoneId);
				IEnumerable<SaleCode> saleCodes = saleCodesByZoneId.GetRecord(renamedZone.ZoneId);

				foreach (SaleCode saleCode in saleCodes)
				{
					CodeToMove codeToMove = new CodeToMove();
					CodeGroup codeGroup = codeGroupManager.GetMatchCodeGroup(saleCode.Code);
					if (!CodeIsExcludedFromRenamedZoneAction(saleCode, changes))
					{
						//if(Vanrise.Common.ExtensionMethods.VRLessThan(saleCode.EED,minimumDate))
						//    continue;

						codesToMove.Add(new CodeToMove
						{
							Code = saleCode.Code,
							CodeGroup = codeGroup,
							BED = Utilities.Max(saleCode.BED, minimumDate),
							EED = saleCode.EED.HasValue ? saleCode.EED : null,
							ZoneName = renamedZone.NewZoneName,
							OldZoneName = oldZoneName
							});
					}
				}
			}

			foreach (DeletedZone deletedZone in changes.DeletedZones)
			{
				IEnumerable<SaleCode> saleCodes = saleCodesByZoneId.GetRecord(deletedZone.ZoneId);
				foreach (SaleCode saleCode in saleCodes)
				{
					if (!CodeIsExcludedFromCloseZoneAction(saleCode, changes))
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


		private bool CodeIsExcludedFromRenamedZoneAction(SaleCode saleCode, Changes changes)
		{
			return (changes.NewCodes.Any(x => x.Code == saleCode.Code) || changes.DeletedCodes.Any(x => x.Code == saleCode.Code));
		}

		private bool CodeIsExcludedFromCloseZoneAction(SaleCode saleCode, Changes changes)
		{
			return (changes.NewCodes.Any(x => x.Code == saleCode.Code && x.ZoneId.HasValue && x.ZoneId.Value == saleCode.ZoneId) || changes.DeletedCodes.Any(x => x.Code == saleCode.Code));
		}


	}
}
