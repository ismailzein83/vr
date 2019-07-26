using NP.IVSwitch.Entities;
using System;
using System.Linq;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.RouteSync.IVSwitch
{
	public abstract class BaseIVSwitchSWSync : SwitchRouteSynchronizer
	{
		#region properties
		public string OwnerName { get; set; }
		public string MasterConnectionString { get; set; }
		public string RouteConnectionString { get; set; }
		public string TariffConnectionString { get; set; }
		public int NumberOfOptions { get; set; }
		public string BlockedAccountMapping { get; set; }
		public Guid Uid { get; set; }
		private static Guid s_businessEntityDefinitionId { get { return new Guid("fdd8a75c-5c6c-46d6-a00a-b99086a01194"); } }
		private Guid s_activeId { get { return new Guid("03aa4967-66ea-4992-b9c9-3f014eff79f2"); } }
		private Guid s_blockId { get { return new Guid("736137e5-6e5a-4722-ba79-aaef0cc5b07f"); } }
		private Guid s_inActiveId { get { return new Guid("b61884de-da86-4de6-8630-dd67a1146b78"); } }
		BusinessEntityStatusHistoryManager _businessEntityStatusHistoryManager = new BusinessEntityStatusHistoryManager();
		BusinessEntityHistoryStackManager _businessEntityHistoryStackManager = new BusinessEntityHistoryStackManager();

		#endregion
		public override void Initialize(ISwitchRouteSynchronizerInitializeContext context)
		{
			PreparedConfiguration preparedData = GetPreparedConfiguration();
			BuildTempTables(preparedData);
		}
		public override void ConvertRoutes(ISwitchRouteSynchronizerConvertRoutesContext context)
		{
			if (context.Routes == null)
				return;

			PreparedConfiguration preparedData = GetPreparedConfiguration();
			var routes = new List<ConvertedRoute>();

			foreach (var route in context.Routes)
			{
				CustomerDefinition customerDefiniton;
				if (preparedData.CustomerDefinitions.TryGetValue(route.CustomerId, out customerDefiniton))
				{
					foreach (var elt in customerDefiniton.EndPoints)
					{
						routes.Add(BuildRouteAndRouteOptions(route, elt, preparedData));
					}
				}
			}
			context.ConvertedRoutes = routes;
		}
		public override object PrepareDataForApply(ISwitchRouteSynchronizerPrepareDataForApplyContext context)
		{
			Dictionary<string, PreparedRoute> customerRoutes = new Dictionary<string, PreparedRoute>();
			foreach (var convertedRoute in context.ConvertedRoutes)
			{
				IVSwitchConvertedRoute ivSwitchConvertedRoute = (IVSwitchConvertedRoute)convertedRoute;

				PreparedRoute preparedRoute;
				if (!customerRoutes.TryGetValue(ivSwitchConvertedRoute.RouteTableName, out preparedRoute))
				{
					preparedRoute = new PreparedRoute
					{
						RouteTableName = ivSwitchConvertedRoute.RouteTableName,
						Routes = new List<IVSwitchRoute>()
					};
					customerRoutes.Add(ivSwitchConvertedRoute.RouteTableName, preparedRoute);
				}
				preparedRoute.Routes.AddRange(ivSwitchConvertedRoute.Routes);

			}
			return customerRoutes;
		}
		public override void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context)
		{
			Dictionary<string, PreparedRoute> routes = (Dictionary<string, PreparedRoute>)context.PreparedItemsForApply;
			IVSwitchRouteDataManager routeDataManager = new IVSwitchRouteDataManager(RouteConnectionString, OwnerName);
			foreach (var item in routes.Values)
			{
				if (item.Routes != null && item.Routes.Any())
					routeDataManager.Bulk(item.Routes, string.Format("{0}_temp", item.RouteTableName));
			}
		}
		public override void Finalize(ISwitchRouteSynchronizerFinalizeContext context)
		{
			PreparedConfiguration preparedData = GetPreparedConfiguration();
			IVSwitchRouteDataManager routeDataManager = new IVSwitchRouteDataManager(RouteConnectionString, OwnerName);
			foreach (var routeTableId in preparedData.RouteTableIdsHashSet)
			{
				routeDataManager.CreatePrimaryKey(string.Format("rt{0}_temp", routeTableId));
				routeDataManager.Swap(GetRouteTableName(routeTableId));
				context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Table rt'{0}' for switch '{1}' is finalized", new object[] { routeTableId, context.SwitchName });
			}
		}
		public abstract PreparedConfiguration GetPreparedConfiguration();
		public abstract List<EndPointStatus> PrepareEndPointStatus(string carrierId, List<int> endPointstatuses);
		public abstract List<RouteStatus> PrepareRouteStatus(string carrierId, List<int> routestatuses);

		public override bool DontCloseMappingRules { get { return true; } }
		public override bool TryBlockCustomer(ITryBlockCustomerContext context)
		{
			IEndPointManager endPointManager = NPManagerFactory.GetManager<IEndPointManager>();
			IVSwitchMasterDataManager masterDataManager = new IVSwitchMasterDataManager(MasterConnectionString);
			List<int> endPointStatus = new List<int> { (int)State.Active, (int)State.Dormant};
			List<EndPointStatus> endPointStatuses = PrepareEndPointStatus(context.CustomerId, endPointStatus);
			if (endPointStatuses == null || !endPointStatuses.Any()) return false;
			context.SwitchBlockingInfo = endPointStatuses;
			var selectedEndPointIds = endPointStatuses.Select(it => it.EndPointId);
			var endPointIds = selectedEndPointIds != null ? selectedEndPointIds.ToList() : null;
			if (endPointIds != null)
			{

				if (masterDataManager.BlockEndPoints(endPointIds, (int)State.Block))
				{
					endPointManager.SetCacheExpired();
					NP.IVSwitch.Entities.RouteEndPointHistoryInfo routeEndPointHistoryInfo = new RouteEndPointHistoryInfo
					{
						Source = NP.IVSwitch.Entities.SourceInfo.Automatically
					};
					string moreInfo = Vanrise.Common.Serializer.Serialize(routeEndPointHistoryInfo);
					InsertEndPointRoutesStatusHistories(endPointIds, null,State.Block, moreInfo);
					return true;
				};
			}
			return false;
		}

		public override bool TryUnBlockCustomer(ITryUnBlockCustomerContext context)
		{
			IEndPointManager endPointManager = NPManagerFactory.GetManager<IEndPointManager>();
			int customerId;
			List<int> endPointIds = new List<int>();
			if (Int32.TryParse(context.CustomerId, out customerId))
			{
				endPointIds = endPointManager.GetCarrierAccountEndPointIds(customerId);
			}
			List<EndPointStatus> endPointStatuses = new List<EndPointStatus>();

			if (endPointIds != null && endPointIds.Any())
			{
				foreach (var endPointId in endPointIds)
				{
					var lastBEStatusHistory = _businessEntityHistoryStackManager.GetLastBusinessEntityHistoryStack(s_businessEntityDefinitionId, endPointId.ToString(), "Status");
                    if(lastBEStatusHistory != null)
                    {
                        RouteEndPointHistoryInfo routeEndPointHistoryInfo = Vanrise.Common.Serializer.Deserialize<RouteEndPointHistoryInfo>(lastBEStatusHistory.MoreInfo);
                        if (routeEndPointHistoryInfo.Source == SourceInfo.Automatically)
                        {
                            _businessEntityHistoryStackManager.DeleteHistoryStack(lastBEStatusHistory.BusinessEntityHistoryStackId);
                            Guid statusId = lastBEStatusHistory.PreviousStatusId.HasValue ? lastBEStatusHistory.PreviousStatusId.Value : lastBEStatusHistory.StatusId;

                            State state = State.InActive;
                            if (statusId == s_activeId)
                                state = State.Active;

                            if (statusId == s_blockId)
                                state = State.Block;

                            if (statusId == s_inActiveId)
                                state = State.InActive;
                            endPointStatuses.Add(new EndPointStatus { EndPointId = endPointId, Status = state });
                        }
                    }
                    else
                    {
                        endPointStatuses.Add(new EndPointStatus { EndPointId = endPointId, Status = State.Active });
                    }
                }
			}


			

			IVSwitchMasterDataManager masterDataManager = new IVSwitchMasterDataManager(MasterConnectionString);
			if (masterDataManager.UpdateEndPointState(endPointStatuses))
			{
				endPointManager.SetCacheExpired();
				NP.IVSwitch.Entities.RouteEndPointHistoryInfo routeEndPointHistory = new RouteEndPointHistoryInfo
				{
					Source = NP.IVSwitch.Entities.SourceInfo.Automatically
				};
				string moreInfo = Vanrise.Common.Serializer.Serialize(routeEndPointHistory);
				InsertEndPointRoutesStatusHistories(endPointStatuses, null, moreInfo);

				return true;
			};
			return false;
		}

		public override bool TryBlockSupplier(ITryBlockSupplierContext context)
		{
			IVSwitchMasterDataManager masterDataManager = new IVSwitchMasterDataManager(MasterConnectionString);
			IRouteManager routeManager = NPManagerFactory.GetManager<IRouteManager>();
			List<int> routeStatus = new List<int> { (int)State.Active, (int)State.Dormant};
			List<RouteStatus> routeStatuses = PrepareRouteStatus(context.SupplierId, routeStatus);
			if (routeStatuses == null || !routeStatuses.Any()) return false;
			var selectedRouteIds = routeStatuses.Select(it => it.RouteId);
			var routeIds = selectedRouteIds != null ? selectedRouteIds.ToList() : null;
			if (routeIds != null)
			{

				if (masterDataManager.BlockRoutes(routeIds, (int)State.Block))
				{
					routeManager.SetCacheExpired();
					NP.IVSwitch.Entities.RouteEndPointHistoryInfo routeEndPointHistoryInfo = new RouteEndPointHistoryInfo
					{
						Source = NP.IVSwitch.Entities.SourceInfo.Automatically
					};
					string moreInfo = Vanrise.Common.Serializer.Serialize(routeEndPointHistoryInfo);
					InsertEndPointRoutesStatusHistories(null, routeIds,State.Block, moreInfo);
					return true;
				};
			}
			return false;
		}


		public override bool TryUnBlockSupplier(ITryUnBlockSupplierContext context)
		{
			IRouteManager routeManager = NPManagerFactory.GetManager<IRouteManager>();
			IVSwitchMasterDataManager masterDataManager = new IVSwitchMasterDataManager(MasterConnectionString);
			int supplierId;
			List<int> routeIds = new List<int>();
			if (Int32.TryParse(context.SupplierId, out supplierId))
			{
				routeIds = routeManager.GetCarrierAccountRouteIds(supplierId);
			}
			List<RouteStatus> routeStatuses = new List<RouteStatus>();

			if (routeIds != null && routeIds.Any())
			{
                foreach (var routeId in routeIds)
                {
                    var lastBEStatusHistory = _businessEntityHistoryStackManager.GetLastBusinessEntityHistoryStack(s_businessEntityDefinitionId, routeId.ToString(), "Status");
                    if (lastBEStatusHistory != null)
                    {
                        RouteEndPointHistoryInfo routeEndPointHistoryInfo = Vanrise.Common.Serializer.Deserialize<RouteEndPointHistoryInfo>(lastBEStatusHistory.MoreInfo);
                        if (routeEndPointHistoryInfo.Source == SourceInfo.Automatically)
                        {
                            _businessEntityHistoryStackManager.DeleteHistoryStack(lastBEStatusHistory.BusinessEntityHistoryStackId);
                            Guid statusId = lastBEStatusHistory.PreviousStatusId.HasValue ? lastBEStatusHistory.PreviousStatusId.Value : lastBEStatusHistory.StatusId;

                            State state = State.InActive;
                            if (statusId == s_activeId)
                                state = State.Active;

                            if (statusId == s_blockId)
                                state = State.Block;

                            if (statusId == s_inActiveId)
                                state = State.InActive;

                            routeStatuses.Add(new RouteStatus { RouteId = routeId, Status = state });
                        }
                    }
                    else
                    {
                        routeStatuses.Add(new RouteStatus { RouteId = routeId, Status = State.Active });
                    }
                }
				if (masterDataManager.UpdateRoutesStates(routeStatuses))
				{
					routeManager.SetCacheExpired();
					NP.IVSwitch.Entities.RouteEndPointHistoryInfo routeEndPointHistory = new RouteEndPointHistoryInfo
					{
						Source = NP.IVSwitch.Entities.SourceInfo.Automatically
					};
					string moreInfo = Vanrise.Common.Serializer.Serialize(routeEndPointHistory);
					InsertEndPointRoutesStatusHistories(null, routeStatuses, moreInfo);

					return true;
				};
			}
			return false;
		}

		public override bool TryDeactivate(ITryDeactivateContext context)
		{
			IVSwitchMasterDataManager masterDataManager = new IVSwitchMasterDataManager(MasterConnectionString);
			IRouteManager routeManager = NPManagerFactory.GetManager<IRouteManager>();
			IEndPointManager endPointManager = NPManagerFactory.GetManager<IEndPointManager>();

			List<int> routeIds;
			List<int> endPointIds;
			int carrierAccountId;
			if (Int32.TryParse(context.CarrierAccountId, out carrierAccountId))
			{
				var allEndPoints = endPointManager.GetCarrierAccountEndPoints(carrierAccountId);
				var endPointsExceptInActive = allEndPoints != null ? allEndPoints.Where(x => x.CurrentState != State.InActive) : null;
				var selectedEndPoints = endPointsExceptInActive != null ? endPointsExceptInActive.Select(x => x.EndPointId) : null;
				endPointIds = selectedEndPoints != null ? selectedEndPoints.ToList() : null;


				var allRoutes = routeManager.GetCarrierAccountRoutes(carrierAccountId);
				var routesExceptInActive = allRoutes != null ? allRoutes.Where(x => x.CurrentState != State.InActive) : null;
				var selectedRoutes = routesExceptInActive != null ? routesExceptInActive.Select(x => x.RouteId) : null;
				routeIds = selectedRoutes != null ? selectedRoutes.ToList() : null;

				if (masterDataManager.UpdateActivationStates(routeIds, endPointIds, (int)State.InActive))
				{
					endPointManager.SetCacheExpired();
					routeManager.SetCacheExpired();
					NP.IVSwitch.Entities.RouteEndPointHistoryInfo routeEndPointHistoryInfo = new RouteEndPointHistoryInfo
					{
						Source = NP.IVSwitch.Entities.SourceInfo.Automatically
					};
					string moreInfo = Vanrise.Common.Serializer.Serialize(routeEndPointHistoryInfo);
					InsertEndPointRoutesStatusHistories(endPointIds, routeIds,State.InActive, moreInfo);
					return true;
				};
			}
			return false;
		}

		public override bool TryReactivate(ITryReactivateContext context)
		{
			IVSwitchMasterDataManager masterDataManager = new IVSwitchMasterDataManager(MasterConnectionString);
			IRouteManager routeManager = NPManagerFactory.GetManager<IRouteManager>();
			IEndPointManager endPointManager = NPManagerFactory.GetManager<IEndPointManager>();
			BusinessEntityStatusHistoryManager businessEntityStatusHistoryManager = new BusinessEntityStatusHistoryManager();
			List<int> routeIds = new List<int>();
			List<int> endPointIds = new List<int>();

			List<RouteStatus> routeStatuses = new List<RouteStatus>();
			List<EndPointStatus> endPointStatuses = new List<EndPointStatus>();
			int carrierAccountId;
			if (Int32.TryParse(context.CarrierAccountId, out carrierAccountId))
			{

				var allEndPoints = endPointManager.GetCarrierAccountEndPoints(carrierAccountId);
				var endPointsExceptInActive = allEndPoints != null ? allEndPoints.Where(x => x.CurrentState == State.InActive) : null;
				var selectedEndPoints = endPointsExceptInActive != null ? endPointsExceptInActive.Select(x => x.EndPointId) : null;
				endPointIds = selectedEndPoints != null ? selectedEndPoints.ToList() : null;

				var allRoutes = routeManager.GetCarrierAccountRoutes(carrierAccountId);
				var routesExceptInActive = allRoutes != null ? allRoutes.Where(x => x.CurrentState == State.InActive) : null;
				var selectedRoutes = routesExceptInActive != null ? routesExceptInActive.Select(x => x.RouteId) : null;
				routeIds = selectedRoutes != null ? selectedRoutes.ToList() : null;

				if (endPointIds != null)
				{
					foreach (var endPointId in endPointIds)
					{
						var lastBEStatusHistory = _businessEntityHistoryStackManager.GetLastBusinessEntityHistoryStack(s_businessEntityDefinitionId, endPointId.ToString(), "Status");
                        if (lastBEStatusHistory != null)
                        {
                            RouteEndPointHistoryInfo routeEndPointHistoryInfo = Vanrise.Common.Serializer.Deserialize<RouteEndPointHistoryInfo>(lastBEStatusHistory.MoreInfo);
                            if (routeEndPointHistoryInfo.Source == SourceInfo.Automatically)
                            {
                                _businessEntityHistoryStackManager.DeleteHistoryStack(lastBEStatusHistory.BusinessEntityHistoryStackId);
                                Guid statusId = lastBEStatusHistory.PreviousStatusId.HasValue ? lastBEStatusHistory.PreviousStatusId.Value : lastBEStatusHistory.StatusId;

                                State state = State.InActive;
                                if (statusId == s_activeId)
                                    state = State.Active;

                                if (statusId == s_blockId)
                                    state = State.Block;

                                if (statusId == s_inActiveId)
                                    state = State.InActive;

                                endPointStatuses.Add(new EndPointStatus { EndPointId = endPointId, Status = state });
                            }
                        }
                        else
                        {
                            endPointStatuses.Add(new EndPointStatus { EndPointId = endPointId, Status =  State.Active });
                        }
                    };
				}

				if (routeIds != null)
				{
					foreach (var routeId in routeIds)
					{
						var lastBEStatusHistory = _businessEntityHistoryStackManager.GetLastBusinessEntityHistoryStack(s_businessEntityDefinitionId, routeId.ToString(), "Status");
						RouteEndPointHistoryInfo routeEndPointHistoryInfo = Vanrise.Common.Serializer.Deserialize<RouteEndPointHistoryInfo>(lastBEStatusHistory.MoreInfo);
						if (routeEndPointHistoryInfo.Source == SourceInfo.Automatically)
						{
							_businessEntityHistoryStackManager.DeleteHistoryStack(lastBEStatusHistory.BusinessEntityHistoryStackId);
							Guid statusId = lastBEStatusHistory.PreviousStatusId.HasValue ? lastBEStatusHistory.PreviousStatusId.Value : lastBEStatusHistory.StatusId;

							State state = State.InActive;
							if (statusId == s_activeId)
								state = State.Active;

							if (statusId == s_blockId)
								state = State.Block;

							if (statusId == s_inActiveId)
								state = State.InActive;

							routeStatuses.Add(new RouteStatus { RouteId = routeId, Status = state });
						}
					}
				}
				if (masterDataManager.ReActivateCarrier(endPointStatuses, routeStatuses))
				{
					endPointManager.SetCacheExpired();
					routeManager.SetCacheExpired();
					NP.IVSwitch.Entities.RouteEndPointHistoryInfo routeEndPointHistory = new RouteEndPointHistoryInfo
					{
						Source = NP.IVSwitch.Entities.SourceInfo.Automatically
					};
					string moreInfo = Vanrise.Common.Serializer.Serialize(routeEndPointHistory);
					ReactivateEndPointRoutesStatusHistories(endPointStatuses, routeStatuses);
					return true;
				};
			}
			return false;
		}
		public override void RemoveConnection(ISwitchRouteSynchronizerRemoveConnectionContext context)
		{
			this.MasterConnectionString = null;
			this.RouteConnectionString = null;
			this.TariffConnectionString = null;
		}

		private string GetRouteTableName(int routeTableId)
		{
			return string.Format("rt{0}", routeTableId);
		}
		private string GetTariffTableName(int tariffTableId)
		{
			return string.Format("trf{0}", tariffTableId);
		}

		#region private functions

		private List<EndPointStatus> GetSuspendedEndPoints(List<EndPointStatus> endPointBlockingInfo, List<EndPointStatus> switchEndPointstatuses)
		{
			List<EndPointStatus> suspendedList = new List<EndPointStatus>();
			Dictionary<int, EndPointStatus> endPointStatuseByEndPointId = switchEndPointstatuses.ToDictionary(item => item.EndPointId, item => item);
			foreach (var currentEndPointStatus in endPointBlockingInfo)
			{
				EndPointStatus switchEndPointStatus;
				if (endPointStatuseByEndPointId.TryGetValue(currentEndPointStatus.EndPointId, out switchEndPointStatus))
				{
					if (switchEndPointStatus.Status == State.Block)
						suspendedList.Add(currentEndPointStatus);
				}
			}
			return suspendedList;
		}
		private void BuildTempTables(PreparedConfiguration preparedData)
		{
			IVSwitchRouteDataManager routeDataManager = new IVSwitchRouteDataManager(RouteConnectionString, OwnerName);
			foreach (var routeTableId in preparedData.RouteTableIdsHashSet)
			{
				routeDataManager.BuildRouteTable(GetRouteTableName(routeTableId));
			}
		}
		private IVSwitchTariff BuildTariff(Entities.Route route)
		{
			IVSwitchTariff tariff = new IVSwitchTariff
			{
				DestinationCode = route.Code,
				TimeFrame = "* * * * *"
			};
			if (route.SaleRate.HasValue)
				tariff.InitCharge = Math.Round(route.SaleRate.Value, 4);

			if (route.SaleZoneId.HasValue)
				tariff.DestinationName = new SaleZoneManager().GetSaleZoneName(route.SaleZoneId.Value);

			return tariff;
		}
		private IVSwitchConvertedRoute BuildRouteAndRouteOptions(Entities.Route route, EndPoint endPoint, PreparedConfiguration preparedData)
		{
			if (route == null)
				return null;

			if (preparedData.SupplierDefinitions == null)
				return null;

			var ivSwitch = new IVSwitchConvertedRoute
			{
				CustomerID = route.CustomerId,
				Routes = new List<IVSwitchRoute>(),
				Tariffs = new List<IVSwitchTariff>(),
				RouteTableName = GetRouteTableName(endPoint.RouteTableId),
				TariffTableName = GetTariffTableName(endPoint.TariffTableId)
			};

			int priority = 0;
			var routes = new List<IVSwitchRoute>();
			if (route.Options != null && route.Options.Count > 0)
				routes = BuildIVSwitchRoutes(route, ref priority, preparedData);

			if (routes.Count == 0)
				routes.Add(BuildBlockedRoute(preparedData.BlockRouteId, preparedData._switchTime, route.Code, ref priority));

			ivSwitch.Routes.AddRange(routes);
			return ivSwitch;
		}
		private List<IVSwitchRoute> BuildIVSwitchRoutes(Entities.Route route, ref int priority, PreparedConfiguration preparedData)
		{
			var routes = new List<IVSwitchRoute>();
			var nonBlockedOptions = route.Options.Where(r => !r.IsBlocked);

			decimal? optionsPercenatgeSum = 0;
			decimal? maxPercentage = 0;
			priority = NumberOfOptions;

			foreach (var option in nonBlockedOptions)
			{
				optionsPercenatgeSum += option.Percentage;
				if (option.Percentage.HasValue && option.Percentage.Value > maxPercentage)
					maxPercentage = option.Percentage;
			}
			int totalBkt = 0;
			int serial = 1;
			foreach (var option in nonBlockedOptions)
			{
				if (!preparedData.SupplierDefinitions.TryGetValue(option.SupplierId, out var supplier) || supplier.Gateways == null)
					continue;

				if (option.Percentage != null && option.Percentage != 0)
				{
					var supplierRoutes = new List<IVSwitchRoute>();
					foreach (var supplierGateWay in supplier.Gateways)
					{
						if (priority == 0)
							break;

						var ivOption = new IVSwitchRoute
						{
							Destination = route.Code,
							RouteId = supplierGateWay.RouteId,
							Preference = priority--,
							StateId = 1,
							HuntStop = 0,
							WakeUpTime = preparedData._switchTime,
							Description = new CarrierAccountManager().GetCarrierAccountName(int.Parse(option.SupplierId)),
							RoutingMode = 8,
							Flag1 = BuildPercentage(supplierGateWay.Percentage, option.Percentage, optionsPercenatgeSum, supplier.Gateways.Count),
							BktSerial = serial++
						};
						int bktCapacity = (int)Math.Round(ivOption.Flag1.Value / 10);
						ivOption.BktCapacity = bktCapacity <= 0 ? 1 : bktCapacity;
						ivOption.BktToken = ivOption.BktCapacity;
						supplierRoutes.Add(ivOption);
					}
					if (option.Backups != null && option.Backups.Count > 0)
					{
						supplierRoutes.AddRange(GetBackupRoutes(route.Code, preparedData._switchTime,
							preparedData.BlockRouteId, option.Backups, preparedData.SupplierDefinitions, ref priority, ref serial));
					}
					if (supplierRoutes.Any())
						supplierRoutes.Last().HuntStop = 1;

					routes.AddRange(supplierRoutes);
				}
				else
				{
					foreach (var supplierGateWay in supplier.Gateways)
					{
						if (priority == 0)
							break;

						routes.Add(new IVSwitchRoute
						{
							Destination = route.Code,
							RouteId = supplierGateWay.RouteId,
							Preference = priority--,
							StateId = 1,
							HuntStop = 0,
							WakeUpTime = preparedData._switchTime,
							Description = new CarrierAccountManager().GetCarrierAccountName(int.Parse(option.SupplierId))
						});
					}
				}
				if (routes.Any())
					routes.ForEach(r => r.TotalBkts = routes.Count);
			}
			return routes;
		}

		private List<IVSwitchRoute> GetBackupRoutes(string code, DateTime wakeUpTime, int blockedRouteId, List<BackupRouteOption> backups, Dictionary<string, SupplierDefinition> suppliersDefinition, ref int priority, ref int serial)
		{
			var backupRoutes = new List<IVSwitchRoute>();

			foreach (var backupRouteOption in backups)
			{
				if (priority == 0)
					break;

				if (backupRouteOption.IsBlocked)
					backupRoutes.Add(BuildBlockedRoute(blockedRouteId, wakeUpTime, code, ref priority));

				else
				{
					SupplierDefinition supplier;
					if (suppliersDefinition.TryGetValue(backupRouteOption.SupplierId, out supplier) && supplier.Gateways != null)
					{
						foreach (var supplierGateWay in supplier.Gateways)
						{
							if (priority == 0)
								break;

							backupRoutes.Add(new IVSwitchRoute
							{
								RoutingMode = 1,
								Destination = code,
								RouteId = supplierGateWay.RouteId,
								TimeFrame = "* * * * *",
								Preference = priority--,
								BktSerial = serial++,
								StateId = 1,
								HuntStop = 0,
								WakeUpTime = wakeUpTime,
								Description = new CarrierAccountManager().GetCarrierAccountName(int.Parse(backupRouteOption.SupplierId))
							});
						}
					}
				}
			}
			return backupRoutes;
		}

		private IVSwitchRoute BuildBlockedRoute(int blockedRouteId, DateTime switchDate, string code, ref int priority)
		{
			return new IVSwitchRoute
			{
				Description = "BLK",
				RouteId = blockedRouteId,
				WakeUpTime = switchDate,
				Destination = code,
				Preference = priority--,
				StateId = 1
			};
		}

		private void InsertEndPointRoutesStatusHistories(List<EndPointStatus> endPointStatuses, List<RouteStatus> routeStatuses,string moreInfo)
		{
			if (endPointStatuses != null)
			{
				foreach (var endPointStatus in endPointStatuses)
				{
					var lastBEStatusHistory = _businessEntityStatusHistoryManager.GetLastBusinessEntityStatusHistory(s_businessEntityDefinitionId, endPointStatus.EndPointId.ToString(), "Status");
                    string previousMoreInfo = string.Empty;
                    if (lastBEStatusHistory != null)
                    {
                        previousMoreInfo = lastBEStatusHistory.PreviousMoreInfo;
                    }
                    else
                    {
                        NP.IVSwitch.Entities.RouteEndPointHistoryInfo routeEndPointHistory = new RouteEndPointHistoryInfo
                        {
                            Source = NP.IVSwitch.Entities.SourceInfo.Automatically
                        };
                         previousMoreInfo = Vanrise.Common.Serializer.Serialize(routeEndPointHistory);

                    }
                    switch (endPointStatus.Status)
					{
						case State.Active:
							_businessEntityStatusHistoryManager.InsertStatusHistory(s_businessEntityDefinitionId, endPointStatus.EndPointId.ToString(), "Status", s_activeId, previousMoreInfo);
							break;
						case State.InActive:
							_businessEntityStatusHistoryManager.InsertStatusHistory(s_businessEntityDefinitionId, endPointStatus.EndPointId.ToString(), "Status", s_inActiveId, previousMoreInfo);
							break;
						case State.Block:
							_businessEntityStatusHistoryManager.InsertStatusHistory(s_businessEntityDefinitionId, endPointStatus.EndPointId.ToString(), "Status", s_blockId, previousMoreInfo);
							break;
					}
				}
			}
			if (routeStatuses != null)
			{
				foreach (var routeStatus in routeStatuses)
				{
					var lastBEStatusHistory = _businessEntityStatusHistoryManager.GetLastBusinessEntityStatusHistory(s_businessEntityDefinitionId, routeStatus.RouteId.ToString(), "Status");
                    string previousMoreInfo = string.Empty;
                    if (lastBEStatusHistory != null)
                    {
                        previousMoreInfo = lastBEStatusHistory.PreviousMoreInfo;
                    }
                    else
                    {
                        NP.IVSwitch.Entities.RouteEndPointHistoryInfo routeEndPointHistory = new RouteEndPointHistoryInfo
                        {
                            Source = NP.IVSwitch.Entities.SourceInfo.Automatically
                        };
                        previousMoreInfo = Vanrise.Common.Serializer.Serialize(routeEndPointHistory);

                    }
                    switch (routeStatus.Status)
					{
						case State.Active:
							_businessEntityStatusHistoryManager.InsertStatusHistory(s_businessEntityDefinitionId, routeStatus.RouteId.ToString(), "Status", s_activeId, previousMoreInfo);
							break;
						case State.InActive:
							_businessEntityStatusHistoryManager.InsertStatusHistory(s_businessEntityDefinitionId, routeStatus.RouteId.ToString(), "Status", s_inActiveId, previousMoreInfo);
							break;
						case State.Block:
							_businessEntityStatusHistoryManager.InsertStatusHistory(s_businessEntityDefinitionId, routeStatus.RouteId.ToString(), "Status", s_blockId, previousMoreInfo);
							break;
					}
				}
			}
		}

		private void InsertEndPointRoutesStatusHistories(List<int> endPointIds, List<int> routeIds,State currentState, string moreInfo)
		{
			if (endPointIds != null)
			{
				foreach (var endPointId in endPointIds)
				{
					switch (currentState)
					{
						case State.InActive:
							_businessEntityStatusHistoryManager.InsertStatusHistory(s_businessEntityDefinitionId, endPointId.ToString(), "Status", s_inActiveId, moreInfo);
							_businessEntityHistoryStackManager.InsertHistoryStack(s_businessEntityDefinitionId, endPointId.ToString(), "Status", s_inActiveId, moreInfo);
							break;
						case State.Block:
							_businessEntityStatusHistoryManager.InsertStatusHistory(s_businessEntityDefinitionId, endPointId.ToString(), "Status", s_blockId, moreInfo);
							_businessEntityHistoryStackManager.InsertHistoryStack(s_businessEntityDefinitionId, endPointId.ToString(), "Status", s_blockId, moreInfo);
							break;
					}
				}
			}
			if (routeIds != null)
			{
				foreach (var routeId in routeIds)
				{
					switch (currentState)
					{
						case State.InActive:
							_businessEntityStatusHistoryManager.InsertStatusHistory(s_businessEntityDefinitionId, routeId.ToString(), "Status", s_inActiveId, moreInfo);
							_businessEntityHistoryStackManager.InsertHistoryStack(s_businessEntityDefinitionId, routeId.ToString(), "Status", s_inActiveId, moreInfo);
							break;
						case State.Block:
							_businessEntityStatusHistoryManager.InsertStatusHistory(s_businessEntityDefinitionId, routeId.ToString(), "Status", s_blockId, moreInfo);
							_businessEntityHistoryStackManager.InsertHistoryStack(s_businessEntityDefinitionId, routeId.ToString(), "Status", s_blockId, moreInfo);
							break;
					}
				}
			}
		}

		private void ReactivateEndPointRoutesStatusHistories(List<EndPointStatus> endPointStatuses, List<RouteStatus> routeStatuses)
		{
			if (endPointStatuses != null)
			{
				foreach (var endPointStatus in endPointStatuses)
				{
					var businessEntityStatusHistory = _businessEntityStatusHistoryManager.GetLastBusinessEntityStatusHistory(s_businessEntityDefinitionId, endPointStatus.EndPointId.ToString(), "Status");
                    string previousMoreInfo = string.Empty;
                    if (businessEntityStatusHistory != null)
                    {
                        previousMoreInfo = businessEntityStatusHistory.PreviousMoreInfo;
                    }
                    else
                    {
                        NP.IVSwitch.Entities.RouteEndPointHistoryInfo routeEndPointHistory = new RouteEndPointHistoryInfo
                        {
                            Source = NP.IVSwitch.Entities.SourceInfo.Automatically
                        };
                        previousMoreInfo = Vanrise.Common.Serializer.Serialize(routeEndPointHistory);

                    }
                    switch (endPointStatus.Status)
					{
						case State.Active:
							_businessEntityStatusHistoryManager.InsertStatusHistory(s_businessEntityDefinitionId, endPointStatus.EndPointId.ToString(), "Status", s_activeId, previousMoreInfo);
							break;
						case State.InActive:
							_businessEntityStatusHistoryManager.InsertStatusHistory(s_businessEntityDefinitionId, endPointStatus.EndPointId.ToString(), "Status", s_inActiveId, previousMoreInfo);
							break;
						case State.Block:
							_businessEntityStatusHistoryManager.InsertStatusHistory(s_businessEntityDefinitionId, endPointStatus.EndPointId.ToString(), "Status", s_blockId, previousMoreInfo);
							break;
					}
				}
			}
			if (routeStatuses != null)
			{
				foreach (var routeStatus in routeStatuses)
				{
					var businessEntityStatusHistory = _businessEntityStatusHistoryManager.GetLastBusinessEntityStatusHistory(s_businessEntityDefinitionId, routeStatus.RouteId.ToString(), "Status");
                    string previousMoreInfo = string.Empty;
                    if (businessEntityStatusHistory != null)
                    {
                        previousMoreInfo = businessEntityStatusHistory.PreviousMoreInfo;
                    }
                    else
                    {
                        NP.IVSwitch.Entities.RouteEndPointHistoryInfo routeEndPointHistory = new RouteEndPointHistoryInfo
                        {
                            Source = NP.IVSwitch.Entities.SourceInfo.Automatically
                        };
                        previousMoreInfo = Vanrise.Common.Serializer.Serialize(routeEndPointHistory);
                    }
                    switch (routeStatus.Status)
					{
						case State.Active:
							_businessEntityStatusHistoryManager.InsertStatusHistory(s_businessEntityDefinitionId, routeStatus.RouteId.ToString(), "Status", s_activeId, previousMoreInfo);
							break;
						case State.InActive:
							_businessEntityStatusHistoryManager.InsertStatusHistory(s_businessEntityDefinitionId, routeStatus.RouteId.ToString(), "Status", s_inActiveId, previousMoreInfo);
							break;
						case State.Block:
							_businessEntityStatusHistoryManager.InsertStatusHistory(s_businessEntityDefinitionId, routeStatus.RouteId.ToString(), "Status", s_blockId, previousMoreInfo);
							break;
					}
				}
			}
		}

		#endregion

		#region Percentage Routing
		private int BuildScaledDownPercentage(decimal? initialPercentage, decimal z1, decimal? maxPercentage, decimal y1, decimal? optionsPercenatgeSum)
		{
			decimal optionsPercenatgeSumTemp = optionsPercenatgeSum ?? 0;
			decimal maxPercentageTemp = maxPercentage ?? 0;
			decimal initialPercentageTemp = initialPercentage ?? 0;
			var scaledDownPercentage = Math.Ceiling(z1 * (1 - ((initialPercentageTemp - y1) / (optionsPercenatgeSumTemp - y1))) + (maxPercentageTemp * ((initialPercentageTemp - y1) / (optionsPercenatgeSumTemp - y1))));
			return decimal.ToInt32(scaledDownPercentage);
		}
		private decimal? BuildPercentagePerGateway(decimal gatewayPercentage, decimal? optionPercentage, decimal? optionsPercenatgeSum)
		{
			if (optionPercentage.HasValue)
				return ((gatewayPercentage * optionPercentage.Value) / optionsPercenatgeSum);
			return 0;
		}

		private decimal? BuildOptionPercentage(decimal? optionPercentage, decimal? optionsPercentageSum, int gatewayCount)
		{
			if (optionPercentage.HasValue && optionsPercentageSum.HasValue)
				return ((optionPercentage.Value * 100) / optionsPercentageSum.Value) / gatewayCount;
			return 0;
		}
		private decimal? BuildPercentage(decimal gatewayPercentage, decimal? optionPercentage, decimal? optionsPercenatgeSum, int gatewayCount)
		{
			decimal? percentage = gatewayPercentage > 0
				? BuildPercentagePerGateway(gatewayPercentage, optionPercentage, optionsPercenatgeSum)
				: BuildOptionPercentage(optionPercentage, optionsPercenatgeSum, gatewayCount);

			if (percentage.HasValue)
			{
				int roundedPercentage = (int)Math.Round(percentage.Value);
				return roundedPercentage <= 0 ? 1 : roundedPercentage;
			}

			return percentage;
		}
		#endregion

		public class PreparedRoute
		{
			public string RouteTableName { get; set; }
			public string TariffTableName { get; set; }
			public List<IVSwitchRoute> Routes { get; set; }
			public List<IVSwitchTariff> Tariffs { get; set; }
		}
	}
}
