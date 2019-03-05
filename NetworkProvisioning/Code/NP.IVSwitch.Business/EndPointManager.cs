using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using NP.IVSwitch.Data;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.GenericData.Transformation;
using Vanrise.GenericData.Transformation.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;
using Vanrise.GenericData.Business;
using Vanrise.Common.Business;
using Vanrise.Caching;
using NP.IVSwitch.Entities.RouteTable;

namespace NP.IVSwitch.Business
{
	public class EndPointManager : IEndPointManager
	{
		#region Public Methods

		public EndPoint GetEndPointHistoryDetailbyHistoryId(int endPointHistoryId)
		{
			VRObjectTrackingManager s_vrObjectTrackingManager = new VRObjectTrackingManager();
			var endPoint = s_vrObjectTrackingManager.GetObjectDetailById(endPointHistoryId);
			return endPoint.CastWithValidate<EndPoint>("endPoint : historyId", endPointHistoryId);
		}

		public EndPoint GetEndPoint(int endPointId, bool isViewedFromUI)
		{
			Dictionary<int, EndPoint> cachedEndPoint = this.GetCachedEndPoint();
			var endPoint = cachedEndPoint.GetRecord(endPointId);
			if (endPoint != null && isViewedFromUI)
				VRActionLogger.Current.LogObjectViewed(EndPointLoggableEntity.Instance, endPoint);
			return endPoint;
		}

		public EndPoint GetEndPoint(int endPointId)
		{
			return GetEndPoint(endPointId, false);
		}

		public string GetEndPointDescription(EndPoint endPoint)
		{
			switch (endPoint.EndPointType)
			{
				case UserType.ACL: return String.Format("ACL - {0}", endPoint.Host);
				case UserType.SIP: return String.Format("SIP - {0}", endPoint.SipLogin);
				case UserType.VendroTermRoute: return string.Format("VendorTermRoute - {0}", endPoint.Description);
				default: throw new NotSupportedException(endPoint.EndPointType.ToString());
			}
		}

		public bool EndPointAclUpdate(IEnumerable<int> endPointIds, int value, RouteTableViewType RouteTableViewType)
		{
			IEndPointDataManager endPointDataManager = IVSwitchDataManagerFactory.GetDataManager<IEndPointDataManager>();
			Helper.SetSwitchConfig(endPointDataManager);
			IEnumerable<EndPoint> endPoints = endPointIds.MapRecords(x => GetEndPoint(x));
			List<int> aclEndPoints = new List<int>();
			List<int> sipEndPoints = new List<int>();
			if (endPoints != null)
			{
				foreach (var endPoint in endPoints)
				{
					if (endPoint.EndPointType == UserType.ACL)
						aclEndPoints.Add(endPoint.EndPointId);
					else
						sipEndPoints.Add(endPoint.EndPointId);
				}
			}

			bool updatedAclResult = false;
			if (aclEndPoints.Count > 0)
				updatedAclResult = endPointDataManager.EndPointAclUpdate(aclEndPoints, value, RouteTableViewType, UserType.ACL);

			bool updateSipResult = false;
			if (sipEndPoints.Count > 0)
				updateSipResult = endPointDataManager.EndPointAclUpdate(sipEndPoints, value, RouteTableViewType, UserType.SIP);

			if (updatedAclResult || updateSipResult)
				CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
			return (updatedAclResult || updateSipResult);
		}

		public string GetEndPointDescription(int endPointId)
		{
			var endPoint = GetEndPoint(endPointId);
			return endPoint != null ? GetEndPointDescription(endPoint) : null;
		}

		public IEnumerable<EndPointEntityInfo> GetEndPointsInfo(EndPointInfoFilter filter)
		{
			Func<EndPoint, bool> filterFunc = null;
			var allEndPoints = this.GetCachedEndPoint();
			if (filter != null)
			{
				int? carrierAccountSWCustomerAccountId = null;
				HashSet<int> assignedEndPointIds = null;
				HashSet<int> alreadyAssignedSWCustomerAccountIds = null;
				HashSet<int> customerIds = null;
				HashSet<int> endPointIds = new HashSet<int>();
				if (filter.CustomerIds != null)
				{
					customerIds = new HashSet<int>(filter.CustomerIds);
					foreach (var Id in customerIds)
					{
						var customerEndpointIds = GetCarrierAccountEndPointIds(Id) ?? null;
						if (customerEndpointIds != null)
						{
							foreach (int endpointId in customerEndpointIds)
							{
								endPointIds.Add(endpointId);
							}
						}

					}
				}
				if (filter.AssignableToCarrierAccountId.HasValue)
				{
					assignedEndPointIds = new HashSet<int>(GetCarrierAccountIdsByEndPointId().Keys);
					var accountManager = new AccountManager();
					carrierAccountSWCustomerAccountId = accountManager.GetCarrierAccountSWCustomerAccountId(filter.AssignableToCarrierAccountId.Value); //Account Id in postgres
					alreadyAssignedSWCustomerAccountIds = new HashSet<int>(accountManager.GetAllAssignedSWCustomerAccountIds());
				}
				filterFunc = (x) =>
				{
					if (filter.AssignableToCarrierAccountId.HasValue)
					{
						if (assignedEndPointIds.Contains(x.EndPointId))
							return false;
						if (carrierAccountSWCustomerAccountId.HasValue && x.AccountId != carrierAccountSWCustomerAccountId.Value)
							return false;
						if (!carrierAccountSWCustomerAccountId.HasValue && alreadyAssignedSWCustomerAccountIds.Contains(x.AccountId))//if end point belongs to Switch Customer that is assigned other Carrier Profile
							return false;
					}

					if (endPointIds != null)
					{
						if (!endPointIds.Contains(x.EndPointId))
						{
							return false;
						}
					}
					if (filter.Filters != null)
					{

						var context = new EndPointFilterContext() { EndPoint = x };
						foreach (var itemFilter in filter.Filters)
						{
							if (!itemFilter.IsMatched(context))
								return false;
						}
					}
					return true;
				};
			}
			return allEndPoints.MapRecords(EndPointEntityInfoMapper, filterFunc);
		}

		public List<int> GetCarrierAccountEndPointIds(CarrierAccount carrierAccount)
		{
			CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
			EndPointCarrierAccountExtension endPointsExtendedSettings =
				carrierAccountManager.GetExtendedSettings<EndPointCarrierAccountExtension>(carrierAccount);
			if (endPointsExtendedSettings == null) return null;
			List<int> endPointIds = new List<int>();
			if (endPointsExtendedSettings.AclEndPointInfo != null)
				endPointIds.AddRange(endPointsExtendedSettings.AclEndPointInfo.Select(itm => itm.EndPointId));
			if (endPointsExtendedSettings.UserEndPointInfo != null)
				endPointIds.AddRange(endPointsExtendedSettings.UserEndPointInfo.Select(itm => itm.EndPointId));
			return endPointIds;
		}

		public List<int> GetCarrierAccountEndPointIds(int carrierAccountId)
		{
			var carrierAccount = new CarrierAccountManager().GetCarrierAccount(carrierAccountId);
			return carrierAccount != null ? GetCarrierAccountEndPointIds(carrierAccount) : null;
		}

		public List<int> GetCarrierAccountsEndPointIds(List<int> carrierAccountIds)
		{
			List<int> carrierEndPointIds = new List<int>();
			foreach (var carrierId in carrierAccountIds)
			{
				List<int> EndPointIds = GetCarrierAccountEndPointIds(carrierId);
				if (EndPointIds != null && EndPointIds.Count() > 0)
					carrierEndPointIds.AddRange(EndPointIds);
			}
			return carrierEndPointIds;
		}

		public IDataRetrievalResult<EndPointDetail> GetFilteredEndPoints(DataRetrievalInput<EndPointQuery> input)
		{
			//Get Carrier by id
			CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
			EndPointCarrierAccountExtension extendedSettingsObject = carrierAccountManager.GetExtendedSettings<EndPointCarrierAccountExtension>(input.Query.CarrierAccountId.Value);

			Dictionary<int, EndPointInfo> aclEndPointDic = new Dictionary<int, EndPointInfo>();
			Dictionary<int, EndPointInfo> userEendPointDic = new Dictionary<int, EndPointInfo>();

			if (extendedSettingsObject != null)
			{
				if (extendedSettingsObject.AclEndPointInfo != null) aclEndPointDic = extendedSettingsObject.AclEndPointInfo.ToDictionary(k => k.EndPointId, v => v);
				if (extendedSettingsObject.UserEndPointInfo != null) userEendPointDic = extendedSettingsObject.UserEndPointInfo.ToDictionary(k => k.EndPointId, v => v);
			}
			var allEndPoints = GetCachedEndPoint();
			Func<EndPoint, bool> filterExpression =
				x => (aclEndPointDic.ContainsKey(x.EndPointId) || (userEendPointDic.ContainsKey(x.EndPointId)));
			VRActionLogger.Current.LogGetFilteredAction(EndPointLoggableEntity.Instance, input);
			return DataRetrievalManager.Instance.ProcessResult(input, allEndPoints.ToBigResult(input, filterExpression, EndPointDetailMapper));
		}
		public List<EndPointInfo> GetAclList(int carrierId)
		{
			CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
			EndPointCarrierAccountExtension endPointsExtendedSettings =
				carrierAccountManager.GetExtendedSettings<EndPointCarrierAccountExtension>(carrierId);
			if (endPointsExtendedSettings == null) return null;
			return endPointsExtendedSettings.AclEndPointInfo;
		}
		public InsertOperationOutput<List<EndPointDetail>> AddEndPoint(MultipleEndPointsToAdd endPointItems)
		{
			InsertOperationOutput<EndPointDetail> insertOperationOutput = new InsertOperationOutput<EndPointDetail>
			{
				Result = InsertOperationResult.Failed,
				InsertedObject = null
			};

			IRouteDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteDataManager>();
			Helper.SetSwitchConfig(dataManager);
			int globalTariffTableId = dataManager.GetGlobalTariffTableId();

			return endPointItems.EndPointType == UserType.ACL
? InsertAcl(endPointItems.EndPointsToAdd, globalTariffTableId) : InsertSip(endPointItems.EndPointsToAdd, globalTariffTableId);
		}
		public UpdateOperationOutput<EndPointDetail> UpdateEndPoint(EndPointToAdd endPointItem)
		{
			var updateOperationOutput = new UpdateOperationOutput<EndPointDetail>
			{
				Result = UpdateOperationResult.Failed,
				UpdatedObject = null
			};
			if (endPointItem.Entity.EndPointType == UserType.ACL)
			{
				var hosts = GetCachedEndPoint();
				var relatedAccount = hosts.Values.FirstOrDefault(r => r.EndPointId == endPointItem.Entity.EndPointId);
				if (relatedAccount != null) endPointItem.Entity.AccountId = relatedAccount.AccountId;
				IpAddressHelper helper = new IpAddressHelper();
				string message;
				if (!ValidateAllAccountsHosts(endPointItem.Entity, out message))
				{
					updateOperationOutput.ShowExactMessage = true;
					updateOperationOutput.Message = message;
					return updateOperationOutput;
				}
				if (helper.ValidateSameAccountHost(hosts, endPointItem.Entity, out message))
				{
					if (!string.IsNullOrEmpty(message))
					{
						updateOperationOutput.ShowExactMessage = true;
						updateOperationOutput.Message = message;
					}
					else
						updateOperationOutput.Result = UpdateOperationResult.SameExists;
					return updateOperationOutput;
				}
			}
			IEndPointDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IEndPointDataManager>();
			Helper.SetSwitchConfig(dataManager);

			if (dataManager.Update(endPointItem.Entity))
			{
				Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
				VRActionLogger.Current.TrackAndLogObjectUpdated(EndPointLoggableEntity.Instance, endPointItem.Entity);
				EndPoint updatedEndPoint = GetEndPoint(endPointItem.Entity.EndPointId);
				updateOperationOutput.Result = UpdateOperationResult.Succeeded;
				updateOperationOutput.UpdatedObject = EndPointDetailMapper(updatedEndPoint);
				AccountManager accountManager = new AccountManager();
				accountManager.UpdateChannelLimit(updatedEndPoint.AccountId);
			}
			else
				updateOperationOutput.Result = UpdateOperationResult.SameExists;
			GetValue(endPointItem);
			return updateOperationOutput;
		}

		public int? GetEndPointCarrierAccountId(int endPointId)
		{
			Dictionary<int, int> carrierAccountIdsByEndPointId = GetCarrierAccountIdsByEndPointId();
			int carrierAccountId;
			if (carrierAccountIdsByEndPointId.TryGetValue(endPointId, out carrierAccountId))
				return carrierAccountId;
			return null;
		}

		public string GetEndPointAccountName(int routeId)
		{
			int? carrierAccountId = GetEndPointCarrierAccountId(routeId);
			return carrierAccountId.HasValue ? new CarrierAccountManager().GetCarrierAccountName(carrierAccountId.Value) : null;
		}

		public void LinkCarrierAccountToEndPoints(int carrierAccountId, List<int> endPointIds)
		{
			if (endPointIds == null || endPointIds.Count == 0)
				throw new ArgumentNullException("endPointIds");
			List<EndPoint> endPoints = new List<EndPoint>();
			int? customerAccountId = null;
			foreach (var endPointId in endPointIds)
			{
				var endPoint = GetEndPoint(endPointId);
				endPoint.ThrowIfNull("endPoint", endPointId);
				endPoints.Add(endPoint);
				if (!customerAccountId.HasValue)
					customerAccountId = endPoint.AccountId;
				else if (customerAccountId.Value != endPoint.AccountId)
					throw new Exception("All endpoints should have same AccountId");
			}
			new AccountManager().TrySetSWCustomerAccountId(carrierAccountId, customerAccountId.Value);
			CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
			EndPointCarrierAccountExtension endPointCarrierAccountExtension = carrierAccountManager.GetExtendedSettings<EndPointCarrierAccountExtension>(carrierAccountId);
			if (endPointCarrierAccountExtension == null)
				endPointCarrierAccountExtension = new EndPointCarrierAccountExtension();
			foreach (var endPoint in endPoints)
			{
				switch (endPoint.EndPointType)
				{
					case UserType.ACL:
						{
							if (endPointCarrierAccountExtension.AclEndPointInfo == null)
								endPointCarrierAccountExtension.AclEndPointInfo = new List<EndPointInfo>();
							endPointCarrierAccountExtension.AclEndPointInfo.Add(new EndPointInfo { EndPointId = endPoint.EndPointId });
						}
						break;
					case UserType.SIP:
						{
							if (endPointCarrierAccountExtension.UserEndPointInfo == null)
								endPointCarrierAccountExtension.UserEndPointInfo = new List<EndPointInfo>();
							endPointCarrierAccountExtension.UserEndPointInfo.Add(new EndPointInfo { EndPointId = endPoint.EndPointId });
						}
						break;
					default: throw new NotSupportedException(String.Format("endPoint.EndPointType '{0}'", endPoint.EndPointType));
				}
			}

			carrierAccountManager.UpdateCarrierAccountExtendedSetting<EndPointCarrierAccountExtension>(
				carrierAccountId, endPointCarrierAccountExtension);
			string carrierAccountName = carrierAccountManager.GetCarrierAccountName(carrierAccountId);
			foreach (var endPointId in endPointIds)
			{
				GenerateRule(carrierAccountId, endPointId, carrierAccountName);
			}
		}
		public Dictionary<int, EndPoint> GetAllEndPoints()
		{
			return GetCachedEndPoint();
		}

		public bool IsCacheExpired(ref DateTime? lastCheckTime)
		{
			return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
				.IsCacheExpired(ref lastCheckTime);
		}
		public DeleteOperationOutput<object> DeleteEndPoint(int endPointId)
		{
			IEndPointDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IEndPointDataManager>();
			Helper.SetSwitchConfig(dataManager);
			DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>
			{
				Result = DeleteOperationResult.Failed
			};
			EndPoint endPointToDelete = GetEndPoint(endPointId);
			var carrierProfileManager = new CarrierProfileManager();
			var carrierAccountManager = new CarrierAccountManager();
			bool doesCarrAccHaveACLExtendedSettings = false;
			bool doesCarrAccHaveSIPExtendedSettings = false;
			bool doesProfileHasExtendedSettings = false;
			int? carrierAccountId = this.GetEndPointCarrierAccountId(endPointId);
			int? profileId = carrierAccountManager.GetCarrierProfileId(carrierAccountId.Value);

			if (!profileId.HasValue)
			{
				return deleteOperationOutput;
			}

			EndPointCarrierAccountExtension endPointsExtendedSettings = null;
			AccountCarrierProfileExtension accountCarrierProfileExtension = carrierProfileManager.GetExtendedSettings<AccountCarrierProfileExtension>(profileId.Value) ?? new AccountCarrierProfileExtension();

			if (carrierAccountId.HasValue)
				endPointsExtendedSettings = carrierAccountManager.GetExtendedSettings<EndPointCarrierAccountExtension>(carrierAccountId.Value);

			if (endPointsExtendedSettings != null && endPointsExtendedSettings.AclEndPointInfo != null && endPointsExtendedSettings.AclEndPointInfo.Count > 0)
				doesCarrAccHaveACLExtendedSettings = true;

			if (endPointsExtendedSettings != null && endPointsExtendedSettings.UserEndPointInfo != null && endPointsExtendedSettings.UserEndPointInfo.Count > 0)
				doesCarrAccHaveSIPExtendedSettings = true;



			switch (endPointToDelete.EndPointType)
			{
				case UserType.ACL:
					if (doesCarrAccHaveACLExtendedSettings)
					{
						endPointsExtendedSettings.AclEndPointInfo.RemoveAll(x => x.EndPointId == endPointId);
						if (endPointsExtendedSettings.AclEndPointInfo.Count == 0)
							endPointsExtendedSettings.AclEndPointInfo = null;

						if (endPointsExtendedSettings.AclEndPointInfo == null && !doesCarrAccHaveSIPExtendedSettings)
						{
							endPointsExtendedSettings = null;
						}
					}

					break;

				case UserType.SIP:

					if (doesCarrAccHaveSIPExtendedSettings)
					{
						endPointsExtendedSettings.UserEndPointInfo.RemoveAll(x => x.EndPointId == endPointId);
						if (endPointsExtendedSettings.UserEndPointInfo.Count == 0)
							endPointsExtendedSettings.UserEndPointInfo = null;

						if (endPointsExtendedSettings.UserEndPointInfo == null && !doesCarrAccHaveACLExtendedSettings)
						{
							endPointsExtendedSettings = null;
						}
					}

					break;
				default:
					deleteOperationOutput.Message = "Carrier Account Have null extended settings";
					deleteOperationOutput.ShowExactMessage = true;
					return deleteOperationOutput;
			}

			carrierAccountManager.UpdateCarrierAccountExtendedSetting<EndPointCarrierAccountExtension>(carrierAccountId.Value, endPointsExtendedSettings);
			if (endPointsExtendedSettings == null && endPointToDelete.RouteTableId.HasValue)
				dataManager.SetRouteTableAsDeleted(endPointToDelete.RouteTableId.Value);

			var profileCarrierAccounts = carrierAccountManager.GetCarriersByProfileId(profileId.Value, true, false);
			if (profileCarrierAccounts != null)
			{
				foreach (var account in profileCarrierAccounts)
				{
					EndPointCarrierAccountExtension endPointCarrierAccountExtension = carrierAccountManager.GetExtendedSettings<EndPointCarrierAccountExtension>(account.CarrierAccountId);
					if (endPointCarrierAccountExtension != null)
					{
						if (endPointCarrierAccountExtension.AclEndPointInfo != null && endPointCarrierAccountExtension.AclEndPointInfo.Count > 0 || endPointCarrierAccountExtension.UserEndPointInfo != null && endPointCarrierAccountExtension.UserEndPointInfo.Count > 0)
						{
							doesProfileHasExtendedSettings = true;
							break;
						}
					}
				}
			}

			if (!doesProfileHasExtendedSettings && accountCarrierProfileExtension != null && !accountCarrierProfileExtension.VendorAccountId.HasValue)
			{
				carrierProfileManager.UpdateCarrierProfileExtendedSetting<AccountCarrierProfileExtension>(profileId.Value, null);
				dataManager.DeleteAccount(endPointToDelete);
			}
			else
				if (!doesProfileHasExtendedSettings && accountCarrierProfileExtension != null)
			{
				accountCarrierProfileExtension.CustomerAccountId = default(int?);
				carrierProfileManager.UpdateCarrierProfileExtendedSetting<AccountCarrierProfileExtension>(profileId.Value, accountCarrierProfileExtension);
			}
			dataManager.DeleteEndPoint(endPointToDelete);
			Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
			deleteOperationOutput.Result = DeleteOperationResult.Succeeded;
			return deleteOperationOutput;
		}


		#endregion

		#region Private Classes

		public class CacheManager : Vanrise.Caching.BaseCacheManager
		{
			protected override bool IsTimeExpirable => true;

			protected override bool UseCentralizedCacheRefresher => true;
			protected override bool ShouldSetCacheExpired(object parameter)
			{
				return base.ShouldSetCacheExpired();
			}

		}


		private class EndPointLoggableEntity : VRLoggableEntityBase
		{
			public static EndPointLoggableEntity Instance = new EndPointLoggableEntity();

			private EndPointLoggableEntity()
			{

			}

			static EndPointManager endPointManager = new EndPointManager();

			public override string EntityUniqueName
			{
				get { return "NP_IVSwitch_EndPoint"; }
			}

			public override string ModuleName
			{
				get { return "IVSwitch"; }
			}

			public override string EntityDisplayName
			{
				get { return "End Point"; }
			}

			public override string ViewHistoryItemClientActionName
			{
				get { return "NP_IVSwitch_EndPoint_ViewHistoryItem"; }
			}

			public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
			{
				EndPoint endPoint = context.Object.CastWithValidate<EndPoint>("context.Object");
				return endPoint.EndPointId;
			}

			public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
			{
				EndPoint endPoint = context.Object.CastWithValidate<EndPoint>("context.Object");
				return endPointManager.GetEndPointDescription(endPoint);
			}
		}

		#endregion

		#region Private Methods
		private void GetValue(EndPointToAdd endPointItem)
		{
			var ids = GetCarrierAccountIdsByEndPointId();
			int carrierId;
			if (ids.TryGetValue(endPointItem.Entity.EndPointId, out carrierId))
			{
				CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
				string carrierAccountName = carrierAccountManager.GetCarrierAccountName(carrierId);
				GenerateRule(carrierId, endPointItem.Entity.EndPointId, carrierAccountName);
			}
		}
		private InsertOperationOutput<List<EndPointDetail>> InsertAcl(List<EndPointToAdd> endPointsToAdd, int globalTariffTableId)
		{
			InsertOperationOutput<List<EndPointDetail>> insertOperationOutput = new InsertOperationOutput<List<EndPointDetail>>
			{
				Result = InsertOperationResult.Failed,
				InsertedObject = new List<EndPointDetail>()
			};
			string mssg;
			var carrierAccountManager = new CarrierAccountManager();
			if (endPointsToAdd != null)
			{
				foreach (var endPointToAdd in endPointsToAdd)
				{
					var recurringHost = endPointsToAdd.Count(item => item.Entity.Host.Equals(endPointToAdd.Entity.Host));
					if (recurringHost > 1)
					{
						var carrierAccountName = carrierAccountManager.GetCarrierAccountName(endPointToAdd.CarrierAccountId);
						var message = string.Format("Subnet address({0}) conflicts with an existing IP address for ({1})", endPointToAdd.Entity.Host, carrierAccountName);
						insertOperationOutput.Message = message;
						insertOperationOutput.ShowExactMessage = true;
						return insertOperationOutput;
					}
					if (!ValidateAllAccountsHosts(endPointToAdd.Entity, out mssg))
					{
						insertOperationOutput.Message = mssg;
						insertOperationOutput.ShowExactMessage = true;
						return insertOperationOutput;
					}
				}
				foreach (var endPointToAdd in endPointsToAdd)
				{

					string carrierAccountName = carrierAccountManager.GetCarrierAccountName(endPointToAdd.CarrierAccountId);
					int? profileId = carrierAccountManager.GetCarrierProfileId(endPointToAdd.CarrierAccountId);

					if (!profileId.HasValue)
					{
						return insertOperationOutput;
					}


					var carrierProfileManager = new CarrierProfileManager();
					AccountCarrierProfileExtension accountExtended = carrierProfileManager.GetExtendedSettings<AccountCarrierProfileExtension>(profileId.Value);

					if (Validatingsubnet(accountExtended, endPointToAdd, profileId.Value, out mssg))
					{
						insertOperationOutput.ShowExactMessage = true;
						insertOperationOutput.Message = mssg;
						return insertOperationOutput;
					}
				}

				IEndPointDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IEndPointDataManager>();
				Helper.SetSwitchConfig(dataManager);
				foreach (var endPointToAdd in endPointsToAdd)
				{
					int endPointId;
					var userEndPointInfoList = new List<EndPointInfo>();
					var aclEndPointInfoList = new List<EndPointInfo>();
					string carrierAccountName = carrierAccountManager.GetCarrierAccountName(endPointToAdd.CarrierAccountId);
					var endPointsExtendedSettings = EndPointCarrierAccountExtension(endPointToAdd, carrierAccountManager, ref aclEndPointInfoList, ref userEndPointInfoList);
					if (!dataManager.Insert(endPointToAdd.Entity, globalTariffTableId, userEndPointInfoList, aclEndPointInfoList, out endPointId, carrierAccountName))
					{
						insertOperationOutput.Result = InsertOperationResult.SameExists;
						return insertOperationOutput;
					}
					var accountManager = new AccountManager();
					accountManager.UpdateChannelLimit(endPointToAdd.Entity.AccountId);
					var endPointInfo = new EndPointInfo
					{
						EndPointId = endPointId

					};
					if (endPointsExtendedSettings.AclEndPointInfo == null)
						endPointsExtendedSettings.AclEndPointInfo = new List<EndPointInfo>();
					endPointsExtendedSettings.AclEndPointInfo.Add(endPointInfo);

					carrierAccountManager.UpdateCarrierAccountExtendedSetting<EndPointCarrierAccountExtension>(endPointToAdd.CarrierAccountId, endPointsExtendedSettings);

					endPointToAdd.Entity.EndPointId = endPointId;
					Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
					VRActionLogger.Current.TrackAndLogObjectAdded(EndPointLoggableEntity.Instance, endPointToAdd.Entity);
					insertOperationOutput.Result = InsertOperationResult.Succeeded;
					insertOperationOutput.InsertedObject.Add(EndPointDetailMapper(GetEndPoint(endPointId)));
					GenerateRule(endPointToAdd.CarrierAccountId, endPointId, carrierAccountName);
				}
			}

			return insertOperationOutput;
		}
		private InsertOperationOutput<List<EndPointDetail>> InsertSip(List<EndPointToAdd> endPointsToAdd, int globalTariffTableId)
		{
			int endPointId;
			InsertOperationOutput<List<EndPointDetail>> insertOperationOutput = new InsertOperationOutput<List<EndPointDetail>>
			{
				Result = InsertOperationResult.Failed,
				InsertedObject = new List<EndPointDetail>()
			};
			var carrierAccountManager = new CarrierAccountManager();
			var userEndPointInfoList = new List<EndPointInfo>();
			var aclEndPointInfoList = new List<EndPointInfo>();
			if (endPointsToAdd != null)
			{
				foreach (var endPointToAdd in endPointsToAdd)
				{
					var recurringSipLogin = endPointsToAdd.Count(x => x.Entity.SipLogin.Equals(endPointToAdd.Entity.SipLogin));
					if (recurringSipLogin > 1)
					{
						insertOperationOutput.Message = "duplicate same SIP Login";
						insertOperationOutput.ShowExactMessage = true;
						return insertOperationOutput;
					}
				};

				foreach (var endPointToAdd in endPointsToAdd)
				{
					string carrierAccountName = carrierAccountManager.GetCarrierAccountName(endPointToAdd.CarrierAccountId);
					int? profileId = carrierAccountManager.GetCarrierProfileId(endPointToAdd.CarrierAccountId);

					if (!profileId.HasValue)
					{
						return insertOperationOutput;
					}
					var carrierProfileManager = new CarrierProfileManager();
					AccountCarrierProfileExtension accountExtended = carrierProfileManager.GetExtendedSettings<AccountCarrierProfileExtension>(profileId.Value);
					if (accountExtended != null && accountExtended.CustomerAccountId.HasValue)
						endPointToAdd.Entity.AccountId = accountExtended.CustomerAccountId.Value;
					else
						endPointToAdd.Entity.AccountId = CreateNewAccount(profileId.Value);
					var endPointsExtendedSettings = EndPointCarrierAccountExtension(endPointToAdd, carrierAccountManager,
						ref aclEndPointInfoList, ref userEndPointInfoList);
					IEndPointDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IEndPointDataManager>();
					Helper.SetSwitchConfig(dataManager);
					bool succInsert = dataManager.Insert(endPointToAdd.Entity, globalTariffTableId, userEndPointInfoList, aclEndPointInfoList, out endPointId, carrierAccountName);
					if (!succInsert)
					{
						insertOperationOutput.Result = InsertOperationResult.SameExists;
						return insertOperationOutput;
					}
					var accountManager = new AccountManager();
					accountManager.UpdateChannelLimit(endPointToAdd.Entity.AccountId);
					var endPointInfo = new EndPointInfo
					{
						EndPointId = endPointId
					};
					if (endPointsExtendedSettings.UserEndPointInfo == null)
						endPointsExtendedSettings.UserEndPointInfo = new List<EndPointInfo>();

					endPointsExtendedSettings.UserEndPointInfo.Add(endPointInfo);
					carrierAccountManager.UpdateCarrierAccountExtendedSetting<EndPointCarrierAccountExtension>(endPointToAdd.CarrierAccountId, endPointsExtendedSettings);
					endPointToAdd.Entity.EndPointId = endPointId;
					Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
					VRActionLogger.Current.TrackAndLogObjectAdded(EndPointLoggableEntity.Instance, endPointToAdd.Entity);
					insertOperationOutput.Result = InsertOperationResult.Succeeded;
					insertOperationOutput.InsertedObject.Add(EndPointDetailMapper(GetEndPoint(endPointId)));
					GenerateRule(endPointToAdd.CarrierAccountId, endPointId, carrierAccountName);
				}
			}
			return insertOperationOutput;
		}
		private static EndPointCarrierAccountExtension EndPointCarrierAccountExtension(EndPointToAdd endPointItem, CarrierAccountManager carrierAccountManager, ref List<EndPointInfo> aclEndPointInfoList, ref List<EndPointInfo> userEndPointInfoList)
		{
			EndPointCarrierAccountExtension endPointsExtendedSettings =
				carrierAccountManager.GetExtendedSettings<EndPointCarrierAccountExtension>(endPointItem.CarrierAccountId) ?? new EndPointCarrierAccountExtension();

			if (endPointsExtendedSettings.AclEndPointInfo != null)
				aclEndPointInfoList = endPointsExtendedSettings.AclEndPointInfo;
			if (endPointsExtendedSettings.UserEndPointInfo != null)
				userEndPointInfoList = endPointsExtendedSettings.UserEndPointInfo;
			return endPointsExtendedSettings;
		}
		private bool ValidateAllAccountsHosts(Entities.EndPoint originalPoint, out string message)
		{
			int carrierId;
			message = "";
			string carrierName = "";

			CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
			EndPointManager endPointManager = new EndPointManager();
			var endPointIds = endPointManager.GetCarrierAccountIdsByEndPointId();
			var allEndPoints = endPointManager.GetCachedEndPoint().Select(x => x.Value).ToList();

			originalPoint.TechPrefix = string.IsNullOrEmpty(originalPoint.TechPrefix) ? "." : originalPoint.TechPrefix;

			if (allEndPoints != null)
			{
				foreach (var item in allEndPoints)
				{
					string[] hostparts = item.Host.Split('/');
					if (hostparts[0].Equals(originalPoint.Host) && item.EndPointId != originalPoint.EndPointId)
					{
						if (item.TechPrefix.Equals(originalPoint.TechPrefix))
						{
							if (endPointIds.TryGetValue(item.EndPointId, out carrierId))
								carrierName = carrierAccountManager.GetCarrierAccountName(carrierId);

							message = string.Format("Same host ({0}) conflicts with an existing IP address for ({1})",
								originalPoint.Host, carrierName);
							return false;
						}
					}
				}
			}
			return true;
		}

		private int CreateNewAccount(int profileId)
		{
			CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
			CarrierProfile carrierProfile = carrierProfileManager.GetCarrierProfile(profileId);
			AccountManager accountManager = new AccountManager();
			Account account = accountManager.GetAccountInfoFromProfile(carrierProfile, true);
			int accountId = accountManager.AddAccount(account);

			AccountCarrierProfileExtension extendedSettings =
				carrierProfileManager.GetExtendedSettings<AccountCarrierProfileExtension>(profileId) ??
				new AccountCarrierProfileExtension();
			extendedSettings.CustomerAccountId = accountId;

			carrierProfileManager.UpdateCarrierProfileExtendedSetting<AccountCarrierProfileExtension>(profileId, extendedSettings);
			return accountId;
		}
		private bool Validatingsubnet(AccountCarrierProfileExtension accountExtended, EndPointToAdd endPointItem, int profileId,
		   out string mssg)
		{
			IpAddressHelper helper = new IpAddressHelper();
			Dictionary<int, Entities.EndPoint> hosts = GetCachedEndPoint();
			if (accountExtended != null && accountExtended.CustomerAccountId.HasValue)
			{
				endPointItem.Entity.AccountId = accountExtended.CustomerAccountId.Value;
				return helper.ValidateSameAccountHost(hosts, endPointItem.Entity, out mssg);
			}
			if (helper.IsInSameSubnet(hosts, endPointItem.Entity.Host, out mssg)) return true;
			endPointItem.Entity.AccountId = CreateNewAccount(profileId);
			return false;
		}

		private void GenerateRule(int carrierId, int endPointId, string carrierName)
		{
			MappingRuleHelper mappingRuleHelper = new MappingRuleHelper(carrierId, endPointId, 1, carrierName);
			mappingRuleHelper.BuildRule();
		}
		Dictionary<int, EndPoint> GetCachedEndPoint()
		{
			return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetEndPoint",
				() =>
				{
					IEndPointDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IEndPointDataManager>();
					Helper.SetSwitchConfig(dataManager);
					return dataManager.IvSwitchSync != null
						? dataManager.GetEndPoints().ToDictionary(x => x.EndPointId, x => x)
						: new Dictionary<int, EndPoint>();
				});
		}

		public Dictionary<int, int> GetCarrierAccountIdsByEndPointId()
		{
			return Vanrise.Caching.CacheManagerFactory.GetCacheManager<TOne.WhS.BusinessEntity.Business.CarrierAccountManager.CacheManager>().GetOrCreateObject("IVSwitch_GetCarrierAccountIdsByEndPointId",
				() =>
				{
					Dictionary<int, int> result = new Dictionary<int, int>();
					CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
					var customers = carrierAccountManager.GetAllCustomers();
					foreach (var customerItem in customers)
					{
						EndPointCarrierAccountExtension extendedSettingsObject =
							carrierAccountManager.GetExtendedSettings<EndPointCarrierAccountExtension>(customerItem);

						if (extendedSettingsObject == null)
							continue;

						if (extendedSettingsObject.AclEndPointInfo != null)
						{
							foreach (var aclInfo in extendedSettingsObject.AclEndPointInfo)
							{
								if (!result.ContainsKey(aclInfo.EndPointId))
									result[aclInfo.EndPointId] = customerItem.CarrierAccountId;
							}
						}

						if (extendedSettingsObject.UserEndPointInfo != null)
						{
							foreach (var userInfo in extendedSettingsObject.UserEndPointInfo)
							{
								if (!result.ContainsKey(userInfo.EndPointId))
									result[userInfo.EndPointId] = customerItem.CarrierAccountId;
							}
						}
					}
					return result;
				});
		}

		#endregion

		#region Mappers

		public EndPointDetail EndPointDetailMapper(EndPoint endPoint)
		{
			EndPointDetail endPointDetail = new EndPointDetail
			{
				Entity = endPoint,
				CurrentStateDescription = Utilities.GetEnumDescription<State>(endPoint.CurrentState),
				CurrentEndPointType = Utilities.GetEnumDescription<UserType>(endPoint.EndPointType)
			};

			return endPointDetail;
		}

		public EndPointEntityInfo EndPointEntityInfoMapper(EndPoint endPoint)
		{
			EndPointEntityInfo endPointEntityInfo = new EndPointEntityInfo
			{
				EndPointId = endPoint.EndPointId,
				Description = string.Format("{0}  ({1})", GetEndPointDescription(endPoint), Utilities.GetEnumDescription(endPoint.CurrentState)),
				AccountId = endPoint.AccountId,
				CliRouting = endPoint.CliRouting,
				DstRouting = endPoint.DstRouting
			};

			return endPointEntityInfo;
		}

		#endregion
	}
}