using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using NP.IVSwitch.Data;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace NP.IVSwitch.Business
{
	public class FirewallManager
	{
		#region public
		public IDataRetrievalResult<FirewallDetail> GetFilteredFirewalls(DataRetrievalInput<FirewallQuery> input)
		{
			var allFirewalls = this.GetCachedFirewalls();
			Func<Firewall, bool> filterExpression =
				x =>
					((input.Query.Host == null || x.Host.Contains(input.Query.Host)) &&
					 (input.Query.Description == null ||
					  x.Description.Contains(input.Query.Description)));
			VRActionLogger.Current.LogGetFilteredAction(FireWallLoggableEntity.Instance, input);

			var resultProcessingHandler = new ResultProcessingHandler<FirewallDetail>()
			{
				ExportExcelHandler = new FirewallsDetailExportExcelHandler()
			};
			return DataRetrievalManager.Instance.ProcessResult(input, allFirewalls.ToBigResult(input, filterExpression, FirewallDetailMapper), resultProcessingHandler);
		}

		public string GetFirewallDescription(int Id)
		{
			var fireWall = this.GetFirewall(Id);
			return fireWall != null ? fireWall.Description : null;
		}
		public Firewall GetFirewallHistoryDetailbyHistoryId(int firewallHistoryId)
		{
			VRObjectTrackingManager s_vrObjectTrackingManager = new VRObjectTrackingManager();
			var fireWall = s_vrObjectTrackingManager.GetObjectDetailById(firewallHistoryId);
			return fireWall.CastWithValidate<Firewall>("Firewall : historyId ", firewallHistoryId);
		}

		public Firewall GetFirewall(int firewallId, bool isViewedFromUI)
		{
			Dictionary<int, Firewall> cachedFirewalls = GetCachedFirewalls();
			var fireWall = cachedFirewalls.GetRecord(firewallId);
			if (fireWall != null && isViewedFromUI)
				VRActionLogger.Current.LogObjectViewed(FireWallLoggableEntity.Instance, fireWall);
			return fireWall;
		}

		public Firewall GetFirewall(int firewallId)
		{
			return GetFirewall(firewallId, false);
		}
		public InsertOperationOutput<FirewallDetail> AddFirewall(Firewall firewall)
		{
			var insertOperationOutput = new InsertOperationOutput<FirewallDetail>
			{
				Result = InsertOperationResult.Failed,
				InsertedObject = null
			};
			Dictionary<int, Firewall> firewalls = GetCachedFirewalls();
			if (firewalls != null)
			{
				IpAddressHelper helper = new IpAddressHelper();
				List<string> allHosts = new List<string>();
				allHosts = firewalls.Values.FindAllRecords(x => x.Id != firewall.Id).Select(x => x.Host).ToList();
				string message = "";
				if (helper.IsNotValidSubnetORInSameSubnet(allHosts, firewall.Host, out message))
				{
					insertOperationOutput.ShowExactMessage = true;
					insertOperationOutput.Message = message;
					return insertOperationOutput;
				}
			}
			IFirewallDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IFirewallDataManager>();
			Helper.SetSwitchConfig(dataManager);
			int firewallId;

			if (dataManager.Insert(firewall, out firewallId))
			{
				firewall.Id = firewallId;
				Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
				VRActionLogger.Current.TrackAndLogObjectAdded(FireWallLoggableEntity.Instance, firewall);
				insertOperationOutput.Result = InsertOperationResult.Succeeded;
				insertOperationOutput.InsertedObject = FirewallDetailMapper(GetFirewall(firewallId));
			}
			else
			{
				insertOperationOutput.ShowExactMessage = true;
				insertOperationOutput.Message = "IP address already exist";
			}
			return insertOperationOutput;
		}
		public UpdateOperationOutput<FirewallDetail> UpdateFirewall(Firewall firewallItem)
		{
			var updateOperationOutput = new UpdateOperationOutput<FirewallDetail>
			{
				Result = UpdateOperationResult.Failed,
				UpdatedObject = null
			};
			Dictionary<int, Firewall> firewalls = GetCachedFirewalls();

			if (firewalls != null)
			{
				IpAddressHelper helper = new IpAddressHelper();
				List<string> allHosts = new List<string>();
				allHosts = firewalls.Values.FindAllRecords(x => x.Id != firewallItem.Id).Select(x => x.Host).ToList();
				string message = "";
				if (helper.IsNotValidSubnetORInSameSubnet(allHosts, firewallItem.Host, out message))
				{
					updateOperationOutput.ShowExactMessage = true;
					updateOperationOutput.Message = message;
					return updateOperationOutput;
				}
			}
			IFirewallDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IFirewallDataManager>();
			Helper.SetSwitchConfig(dataManager);
			if (dataManager.Update(firewallItem))
			{
				Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
				VRActionLogger.Current.TrackAndLogObjectUpdated(FireWallLoggableEntity.Instance, firewallItem);
				updateOperationOutput.Result = UpdateOperationResult.Succeeded;
				updateOperationOutput.UpdatedObject = FirewallDetailMapper(GetFirewall(firewallItem.Id));
			}
			else
			{
				updateOperationOutput.ShowExactMessage = true;
				updateOperationOutput.Message = "IP address already exist";
			}
			return updateOperationOutput;
		}
		#endregion


		#region Private Classes
		private class FirewallsDetailExportExcelHandler : ExcelExportHandler<FirewallDetail>
		{
			public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<FirewallDetail> context)
			{
				var sheet = new ExportExcelSheet()
				{
					SheetName = "Firewalls",
					Header = new ExportExcelHeader() { Cells = new List<ExportExcelHeaderCell>() }
				};

				sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "ID" });
				sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Host" });
				sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Description" });
				sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Creation Date", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });

				sheet.Rows = new List<ExportExcelRow>();
				if (context.BigResult != null && context.BigResult.Data != null)
				{
					foreach (var record in context.BigResult.Data)
					{
						if (record != null)
						{
							var row = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
							row.Cells.Add(new ExportExcelCell() { Value = record.Entity.Id });
							row.Cells.Add(new ExportExcelCell() { Value = record.Entity.Host });
							row.Cells.Add(new ExportExcelCell() { Value = record.Entity.Description });
							row.Cells.Add(new ExportExcelCell() { Value = record.Entity.CreationDate });
							sheet.Rows.Add(row);
						}
					}
				}

				context.MainSheet = sheet;
			}
		}
		private class CacheManager : Vanrise.Caching.BaseCacheManager
		{
			IFirewallDataManager _dataManager = IVSwitchDataManagerFactory.GetDataManager<IFirewallDataManager>();
			public DateTime lastCheckTime { get; set; }
			protected override bool IsTimeExpirable { get { return true; } }
		}

		private class FireWallLoggableEntity : VRLoggableEntityBase
		{
			public static FireWallLoggableEntity Instance = new FireWallLoggableEntity();

			private FireWallLoggableEntity()
			{

			}

			static FirewallManager fireWallManager = new FirewallManager();

			public override string EntityUniqueName
			{
				get { return "NP_IVSwitch_Firewall"; }
			}

			public override string ModuleName
			{
				get { return "IVSwitch"; }
			}

			public override string EntityDisplayName
			{
				get { return "Firewall"; }
			}

			public override string ViewHistoryItemClientActionName
			{
				get { return "NP_IVSwitch_Firewall_ViewHistoryItem"; }
			}

			public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
			{
				Firewall firewall = context.Object.CastWithValidate<Firewall>("context.Object");
				return firewall.Id;
			}

			public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
			{
				Firewall firewall = context.Object.CastWithValidate<Firewall>("context.Object");
				return fireWallManager.GetFirewallDescription(firewall.Id);
			}
		}

		#endregion

		#region Private Methods

		Dictionary<int, Firewall> GetCachedFirewalls()
		{
			return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedFirewalls",
			   () =>
			   {
				   IFirewallDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IFirewallDataManager>();
				   Helper.SetSwitchConfig(dataManager);
				   return dataManager.GetFirewalls().ToDictionary(x => x.Id, x => x);
			   });
		}

		#endregion
		#region mapper

		FirewallDetail FirewallDetailMapper(Firewall firewall)
		{
			return new FirewallDetail
			{
				Entity = firewall
			};
		}
		#endregion
	}
}
