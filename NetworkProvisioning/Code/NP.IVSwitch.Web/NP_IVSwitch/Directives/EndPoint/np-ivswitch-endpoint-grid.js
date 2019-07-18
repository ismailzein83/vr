'use strict';

app.directive('npIvswitchEndpointGrid', ['NP_IVSwitch_EndPointAPIService', 'NP_IVSwitch_EndPointService', 'VRNotificationService', 'NP_IVSwitch_EndPointEnum', 'UtilsService', 'VRUIUtilsService', 'NP_IVSwitch_EndPointStateEnum', 'WhS_BE_CarrierAccountAPIService', 'WhS_BE_CarrierAccountActivationStatusEnum', 'WhS_BE_RoutingStatusEnum',
	function (NP_IVSwitch_EndPointAPIService, NP_IVSwitch_EndPointService, VRNotificationService, NP_IVSwitch_EndPointEnum, UtilsService, VRUIUtilsService, NP_IVSwitch_EndPointStateEnum, WhS_BE_CarrierAccountAPIService, WhS_BE_CarrierAccountActivationStatusEnum, WhS_BE_RoutingStatusEnum) {
		return {
			restrict: 'E',
			scope: {
				onReady: '=',
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var endPointGrid = new EndPointGrid($scope, ctrl, $attrs);
				endPointGrid.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			templateUrl: '/Client/Modules/NP_IVSwitch/Directives/EndPoint/Templates/EndPointGridTemplate.html'
		};



		function EndPointGrid($scope, ctrl, $attrs) {
			this.initializeController = initializeController;
			var gridAPI;
			var gridDrillDownTabsObj;
			var carrierAccountId;
			var isCarrierAccountInActive;
			var isCarrierAccountBlock;
			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.endPoint = [];
				$scope.scopeModel.menuActions = [];
				$scope.scopeModel.Type = {};


				$scope.scopeModel.addEndPoint = function () {
					var onEndPointAdded = function (addedEndPoint) {
						gridDrillDownTabsObj.setDrillDownExtensionObject(addedEndPoint);
						gridAPI.itemAdded(addedEndPoint);
					};
					NP_IVSwitch_EndPointService.addEndPoint(carrierAccountId, onEndPointAdded);
				};
				$scope.scopeModel.hadAddEndPointPermission = function () {
					return NP_IVSwitch_EndPointAPIService.HasAddEndPointPermission();
				};

				$scope.scopeModel.onGridReady = function (api) {
					gridAPI = api;
					var drillDownDefinitions = NP_IVSwitch_EndPointService.getDrillDownDefinition();
					gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
					defineAPI();
				};
				var editMenuAction = {
					name: 'Edit',
					clicked: editEndPoint,
					haspermission: hasEditEndPointPermission
				};
				var cloneMenuAction = {
					name: 'Clone',
					clicked: cloneEndPoint,
					haspermission: hasCloneEndPointPermission
				};
				var blockMenuAction = {
					name: 'Block',
					clicked: blockEndPoint,
					haspermission: hasBlockEndPointPermission
				};
				var inActiveMenuAction = {
					name: 'InActivate',
					clicked: inActivateEndPoint,
					haspermission: hasInActivateEndPointPermission
				};
				var vieweMenuAction = {
					name: 'View',
					clicked: viewEndPoint,
				};
				var activeMenuAction = {
					name: 'Activate',
					clicked: activateEndPoint,
					haspermission: hasActivateEndPointPermission
				};
				$scope.scopeModel.menuActions = function (dataItem) {
					var menuActions = [];
					if (isCarrierAccountInActive) {
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
							else if (dataItem.Entity.CurrentState == NP_IVSwitch_EndPointStateEnum.Inactive.value) {
								menuActions.push(vieweMenuAction);
								if (!isCarrierAccountInActive)
									menuActions.push(activeMenuAction);
							}
							else if (dataItem.Entity.CurrentState == NP_IVSwitch_EndPointStateEnum.Blocked.value) {
								menuActions.push(vieweMenuAction);
								menuActions.push(inActiveMenuAction);
								if (!isCarrierAccountBlock)
									menuActions.push(activeMenuAction);
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
				$scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
					return NP_IVSwitch_EndPointAPIService.GetFilteredEndPoints(dataRetrievalInput).then(function (response) {
						var EnumArray = UtilsService.getArrayEnum(NP_IVSwitch_EndPointEnum);
						if (response != undefined && response.Data != undefined) {
							for (var i = 0; i < response.Data.length; i++) {
								gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
							}
						}
						onResponseReady(response);
					}).catch(function (error) {
						VRNotificationService.notifyExceptionWithClose(error, $scope);
					});
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
							if (response.CarrierAccountSettings != undefined && response.CarrierAccountSettings.ActivationStatus == WhS_BE_CarrierAccountActivationStatusEnum.Inactive.value) {
								isCarrierAccountInActive = true
								$scope.scopeModel.isCarrierAccountBlockorInActive = true;
							}
							else
								if (response.CustomerSettings != undefined && response.CustomerSettings.RoutingStatus == WhS_BE_RoutingStatusEnum.Blocked.value) {
									$scope.scopeModel.isCarrierAccountBlockorInActive = true;
									isCarrierAccountBlock = true;
								}
						}
					});
					promises.push(isCarrierAccountActiveOrBlockPromise);
					promises.push(gridAPI.retrieveData(query));
					return UtilsService.waitMultiplePromises(promises);

				};

				api.onEndPointAdded = function (addedEndPoint) {
					gridDrillDownTabsObj.setDrillDownExtensionObject(addedEndPoint);
					gridAPI.itemAdded(addedEndPoint);
				};



				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}

			function editEndPoint(EndPointItem) {
				var onEndPointUpdated = function (updatedEndPoint) {
					gridDrillDownTabsObj.setDrillDownExtensionObject(updatedEndPoint);
					gridAPI.itemUpdated(updatedEndPoint);
				};
				NP_IVSwitch_EndPointService.editEndPoint(EndPointItem.Entity.EndPointId, onEndPointUpdated);
			}

			function cloneEndPoint(EndPointItem) {
				var onEndPointAdded = function (addedEndPoint) {
					gridDrillDownTabsObj.setDrillDownExtensionObject(addedEndPoint);
					gridAPI.itemUpdated(addedEndPoint);
				};
				NP_IVSwitch_EndPointService.cloneEndPoint(carrierAccountId, EndPointItem.Entity.EndPointId, onEndPointAdded);
			}

			function deleteEndPoint(EndPointItem) {
				VRNotificationService.showDeleteConfirmation().then(function (response) {
					if (response) {
						NP_IVSwitch_EndPointAPIService.DeleteEndPoint(EndPointItem.Entity.EndPointId).then(function (response) {
							if (VRNotificationService.notifyOnItemDeleted("End Point", response, "Entity.Description")) {
								gridAPI.itemDeleted(EndPointItem);
							}
						});
					}
				});
			}

			function blockEndPoint(endPointItem) {
				VRNotificationService.showConfirmation().then(function (response) {
					if (response) {
						NP_IVSwitch_EndPointAPIService.BlockEndPoint(endPointItem.Entity.EndPointId).then(function (response) {
							if (VRNotificationService.notifyOnItemUpdated('End Point', response, "Entity.Description")) {
								gridDrillDownTabsObj.setDrillDownExtensionObject(response.UpdatedObject);
								gridAPI.itemUpdated(response.UpdatedObject);
							}

						});
					}
				});
			}

			function inActivateEndPoint(endPointItem) {
				VRNotificationService.showConfirmation().then(function (response) {
					if (response) {
						NP_IVSwitch_EndPointAPIService.InActivateEndPoint(endPointItem.Entity.EndPointId).then(function (response) {
							if (VRNotificationService.notifyOnItemUpdated('End Point', response, "Entity.Description")) {
								gridDrillDownTabsObj.setDrillDownExtensionObject(response.UpdatedObject);
								gridAPI.itemUpdated(response.UpdatedObject);
							}
						});
					}
				});
			}

			function activateEndPoint(endPointItem) {
				VRNotificationService.showConfirmation().then(function (response) {
					if (response) {
						NP_IVSwitch_EndPointAPIService.ActivateEndPoint(endPointItem.Entity.EndPointId).then(function (response) {
							if (VRNotificationService.notifyOnItemUpdated('End Point', response, "Entity.Description")) {
								gridDrillDownTabsObj.setDrillDownExtensionObject(response.UpdatedObject);
								gridAPI.itemUpdated(response.UpdatedObject);
							}
						});
					}
				});
			}


			function hasEditEndPointPermission() {
				return NP_IVSwitch_EndPointAPIService.HasEditEndPointPermission();
			}

			function hasCloneEndPointPermission() {
				return NP_IVSwitch_EndPointAPIService.HasAddEndPointPermission();
			}

			function hasBlockEndPointPermission() {
				return NP_IVSwitch_EndPointAPIService.HasBlockEndPointPermission();
			}

			function hasActivateEndPointPermission() {
				return NP_IVSwitch_EndPointAPIService.HasActivateEndPointPermission();
			}

			function hasInActivateEndPointPermission() {
				return NP_IVSwitch_EndPointAPIService.HasInActivateEndPointPermission();
			}

			function hasDeletePermission() {
				return NP_IVSwitch_EndPointAPIService.HasDeletePermission();
			}

			function viewEndPoint(endPointItem) {
				NP_IVSwitch_EndPointService.viewEndPoint(endPointItem.Entity.EndPointId);
			}
		}
	}]);

