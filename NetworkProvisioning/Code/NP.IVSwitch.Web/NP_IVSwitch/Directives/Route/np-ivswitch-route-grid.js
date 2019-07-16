'use strict';

app.directive('npIvswitchRouteGrid', ['NP_IVSwitch_RouteAPIService', 'NP_IVSwitch_RouteService', 'VRNotificationService', 'VRUIUtilsService', 'NP_IVSwitch_EndPointStateEnum', 'WhS_BE_CarrierAccountAPIService', 'WhS_BE_CarrierAccountActivationStatusEnum', 'WhS_BE_RoutingStatusEnum', 'UtilsService',
	function (NP_IVSwitch_RouteAPIService, NP_IVSwitch_RouteService, VRNotificationService, VRUIUtilsService, NP_IVSwitch_EndPointStateEnum, WhS_BE_CarrierAccountAPIService, WhS_BE_CarrierAccountActivationStatusEnum, WhS_BE_RoutingStatusEnum, UtilsService) {
		return {
			restrict: 'E',
			scope: {
				onReady: '=',
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var routeGrid = new RouteGrid($scope, ctrl, $attrs);
				routeGrid.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			templateUrl: '/Client/Modules/NP_IVSwitch/Directives/Route/Templates/RouteGridTemplate.html'
		};

		function RouteGrid($scope, ctrl, $attrs) {
			this.initializeController = initializeController;
			var gridAPI;
			var gridDrillDownTabsObj;
			var carrierAccountId;
			var isInActiveCarrierAccount;

			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.route = [];
				$scope.scopeModel.menuActions = [];

				$scope.scopeModel.addRoute = function () {

					var onRouteAdded = function (addedRoute) {
						gridDrillDownTabsObj.setDrillDownExtensionObject(addedRoute);
						gridAPI.itemAdded(addedRoute);
					};
					NP_IVSwitch_RouteService.addRoute(carrierAccountId, onRouteAdded);
				};
				$scope.scopeModel.hadAddRoutePermission = function () {
					return NP_IVSwitch_RouteAPIService.HasAddRoutePermission();
				};

				$scope.scopeModel.onGridReady = function (api) {
					gridAPI = api;
					var drillDownDefinitions = NP_IVSwitch_RouteService.getDrillDownDefinition();
					gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
					defineAPI();
				};
				$scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

					return NP_IVSwitch_RouteAPIService.GetFilteredRoutes(dataRetrievalInput).then(function (response) {
						if (response != undefined && response.Data != undefined) {
							for (var i = 0; i < response.Data.length; i++) {
								var item = response.Data[i];
								gridDrillDownTabsObj.setDrillDownExtensionObject(item);
							}
						}
						onResponseReady(response);

					}).catch(function (error) {
						VRNotificationService.notifyExceptionWithClose(error, $scope);
					});
				};
				var editMenuAction = {
					name: 'Edit',
					clicked: editRoute,
					haspermission: hasEditRoutePermission
				};
				var cloneMenuAction = {
					name: 'Clone',
					clicked: cloneRoute,
					haspermission: hasCloneRoutePermission
				};
				var blockMenuAction = {
					name: 'Block',
					clicked: blockRoute,
					haspermission: hasBlockRoutePermission
				};
				var inActiveMenuAction = {
					name: 'InActivate',
					clicked: inActivateRoute,
					haspermission: hasInActivateRoutePermission
				};
				var vieweMenuAction = {
					name: 'View',
					clicked: viewRoute,
				};
				var activeMenuAction = {
					name: 'Activate',
					clicked: activateRoute,
					haspermission: hasAcivateRoutePermission
				};
				$scope.scopeModel.menuActions = function (dataItem) {
					var menuActions = [];
					if (isInActiveCarrierAccount) {
						menuActions.push(vieweMenuAction);
					}
					else {
						if (dataItem != undefined && dataItem.Entity != undefined) {
							if (dataItem.Entity.CurrentState == NP_IVSwitch_EndPointStateEnum.Active.value) {
								menuActions.push(editMenuAction);
								menuActions.push(cloneMenuAction);
								menuActions.push(blockMenuAction);
								menuActions.push(inActiveMenuAction);
							}
							else
								if (dataItem.Entity.CurrentState == NP_IVSwitch_EndPointStateEnum.Inactive.value) {
									menuActions.push(vieweMenuAction);
									menuActions.push(activeMenuAction);
								}
								else if (dataItem.Entity.CurrentState == NP_IVSwitch_EndPointStateEnum.Blocked.value) {
									menuActions.push(vieweMenuAction);
									menuActions.push(activeMenuAction);
									menuActions.push(inActiveMenuAction);
								}
								else {
									menuActions.push(activeMenuAction);
									menuActions.push(editMenuAction);
									menuActions.push(cloneMenuAction);
								}
						}
					}
					if (menuActions.length > 0) {
						return menuActions;
					}
				};
			}

			function defineAPI() {
				var api = {};

				api.load = function (query) {
					carrierAccountId = query != undefined && query.CarrierAccountId != undefined ? query.CarrierAccountId : undefined;
					var promises = [];
					var isCarrierAccountActiveOrBlockPromise = WhS_BE_CarrierAccountAPIService.GetCarrierAccount(carrierAccountId).then(function (response) {
						$scope.scopeModel.isCarrierAccountBlockorInActive = false;
						if (response != undefined) {
							if (response.CarrierAccountSettings != undefined && response.CarrierAccountSettings.ActivationStatus == WhS_BE_CarrierAccountActivationStatusEnum.Inactive.value)
							{
								isInActiveCarrierAccount = true;
								$scope.scopeModel.isCarrierAccountBlockorInActive = true;
							}
							else
								if (response.CustomerSettings != undefined && response.SupplierSettings.RoutingStatus == WhS_BE_RoutingStatusEnum.Blocked.value)
									$scope.scopeModel.isCarrierAccountBlockorInActive = true;
						}
					});
					promises.push(isCarrierAccountActiveOrBlockPromise);
					promises.push(gridAPI.retrieveData(query));
					return UtilsService.waitMultiplePromises(promises);
				};

				api.onRouteAdded = function (addedRoute) {
					gridDrillDownTabsObj.setDrillDownExtensionObject(addedRoute);
					gridAPI.itemAdded(addedRoute);
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}

			function editRoute(RouteItem) {
				var onRouteUpdated = function (updatedRoute) {
					gridDrillDownTabsObj.setDrillDownExtensionObject(updatedRoute);
					gridAPI.itemUpdated(updatedRoute);
				};
				NP_IVSwitch_RouteService.editRoute(RouteItem.Entity.RouteId, carrierAccountId, onRouteUpdated);
			}

			function cloneRoute(RouteItem) {
				var onRouteAdded = function (updatedRoute) {
					gridDrillDownTabsObj.setDrillDownExtensionObject(updatedRoute);
					gridAPI.itemUpdated(updatedRoute);
				};


				NP_IVSwitch_RouteService.cloneRoute(RouteItem.Entity.RouteId, carrierAccountId, onRouteAdded);
			}

			function deleteRoute(routeItem) {
				VRNotificationService.showDeleteConfirmation().then(function (response) {
					if (response) {
						NP_IVSwitch_RouteAPIService.DeleteRoute(routeItem.Entity.RouteId).then(function (response) {
							if (VRNotificationService.notifyOnItemDeleted("Route", response, "Entity.Description")) {
								gridAPI.itemDeleted(routeItem);
							}
						});
					}
				});
			}

			function blockRoute(routeItem) {
				VRNotificationService.showConfirmation().then(function (response) {
					if (response) {
						NP_IVSwitch_RouteAPIService.BlockRoute(routeItem.Entity.RouteId).then(function (response) {
							gridDrillDownTabsObj.setDrillDownExtensionObject(response.UpdatedObject);
							gridAPI.itemUpdated(response.UpdatedObject);
						});
					}
				});
			}

			function inActivateRoute(routeItem) {
				VRNotificationService.showConfirmation().then(function (response) {
					if (response) {
						NP_IVSwitch_RouteAPIService.InActivateRoute(routeItem.Entity.RouteId).then(function (response) {
							gridDrillDownTabsObj.setDrillDownExtensionObject(response.UpdatedObject);
							gridAPI.itemUpdated(response.UpdatedObject);
						});
					}
				});
			}

			function activateRoute(routeItem) {
				VRNotificationService.showConfirmation().then(function (response) {
					if (response) {
						NP_IVSwitch_RouteAPIService.ActivateRoute(routeItem.Entity.RouteId).then(function (response) {
							gridDrillDownTabsObj.setDrillDownExtensionObject(response.UpdatedObject);
							gridAPI.itemUpdated(response.UpdatedObject);
						});
					}
				});
			}

			function hasEditRoutePermission() {
				return NP_IVSwitch_RouteAPIService.HasEditRoutePermission();
			}

			function hasInActivateRoutePermission() {
				return NP_IVSwitch_RouteAPIService.HasInActivateRoutePermission();
			}

			function hasAcivateRoutePermission() {
				return NP_IVSwitch_RouteAPIService.HasActivateRoutePermission();
			}

			function hasBlockRoutePermission() {
				return NP_IVSwitch_RouteAPIService.HasBlockRoutePermission();
			}

			function hasCloneRoutePermission() {
				return NP_IVSwitch_RouteAPIService.HasAddRoutePermission();
			}

			function hasDeleteRoutePermission() {
				return NP_IVSwitch_RouteAPIService.HasDeleteRoutePermission();
			}

			function viewRoute(route) {
				NP_IVSwitch_RouteService.viewRoute(route.Entity.RouteId, carrierAccountId);
			}
		}
	}]);

