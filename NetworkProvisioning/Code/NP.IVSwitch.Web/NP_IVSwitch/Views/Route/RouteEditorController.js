﻿(function (appControllers) {

	"use strict";

	RouteEditorController.$inject = ['$scope', 'NP_IVSwitch_RouteAPIService', 'VRNotificationService', 'UtilsService', 'VRNavigationService', 'VRUIUtilsService', 'NP_IVSwitch_EndPointStateEnum', 'NP_IVSwitch_TransportModeEnum'];

	function RouteEditorController($scope, npIvSwitchRouteApiService, VRNotificationService, UtilsService, VRNavigationService, VRUIUtilsService, NP_IVSwitch_EndPointStateEnum, NP_IVSwitch_TransportModeEnum) {

		var isEditMode;
		var isCloneMode;

		var routeId;
		var carrierAccountId;

		var routeEntity;

		var selectorCodecProfileAPI;
		var selectorCodecProfileReadyDeferred = UtilsService.createPromiseDeferred();

		var selectorTranslationRuleAPI;
		var selectorTranslationRuleReadyDeferred = UtilsService.createPromiseDeferred();

		var selectorSIPProfileAPI;
		var selectorSIPProfileReadyDeferred = UtilsService.createPromiseDeferred();

		var context;
		var isViewHistoryMode;

		loadParameters();

		defineScope();

		load();


		function loadParameters() {
			var parameters = VRNavigationService.getParameters($scope);
			isCloneMode = parameters.IsClone;
			if (parameters != undefined && parameters != null) {
				routeId = parameters.RouteId;
				carrierAccountId = parameters.CarrierAccountId;
				context = parameters.context;
			}

			isEditMode = (routeId != undefined);
			isViewHistoryMode = (context != undefined && context.historyId != undefined);
		}

		function defineScope() {
			$scope.scopeModel = {};
			$scope.scopeModel.isEditMode = isEditMode;
			$scope.scopeModel.isCloneMode = isCloneMode;
			$scope.scopeModel.port = "5060";
			$scope.scopeModel.connectiontimeout = "60";
			$scope.scopeModel.transportmodeid = { value: 1, description: "UDP" };

			$scope.scopeModel.save = function () {
				if (isCloneMode) {
					return insert();
				}
				else
					if (isEditMode) {
						return update();
					}
					else {
						return insert();
					}
			};

			$scope.scopeModel.close = function () {
				$scope.modalContext.closeModal();
			};

			$scope.scopeModel.onSelectorCodecProfileReady = function (api) {
				selectorCodecProfileAPI = api;
				selectorCodecProfileReadyDeferred.resolve();
			};

			$scope.scopeModel.onSelectorTranslationRuleReady = function (api) {
				selectorTranslationRuleAPI = api;
				selectorTranslationRuleReadyDeferred.resolve();
			};

			$scope.scopeModel.onSelectionChanged = function (SelectedItem) {

				if (SelectedItem != undefined) {
					$scope.scopeModel.TypeId = SelectedItem.value;
				}
			};

			$scope.scopeModel.onSelectionCodecProfileChanged = function (SelectedItem) {

				if (SelectedItem != undefined) {
					$scope.scopeModel.codecprofileid = SelectedItem.CodecProfileId;
				}
				else {
					$scope.scopeModel.codecprofileid = undefined;
				}
			};

			$scope.scopeModel.onSelectionTranslationRuleChanged = function (SelectedItem) {

				if (SelectedItem != undefined) {
					$scope.scopeModel.translationruleid = SelectedItem.TranslationRuleId;
				}
				else {
					$scope.scopeModel.translationruleid = undefined;
				}
			};

			$scope.scopeModel.onSelectionChangedState = function (SelectedItem) {
				if (SelectedItem != undefined) {
					$scope.scopeModel.currentstate = SelectedItem;
				}
			};

			$scope.scopeModel.onSelectionChangedTransportMode = function (SelectedItem) {
				if (SelectedItem != undefined) {
					$scope.scopeModel.transportmodeid = SelectedItem;
				}
			};

			$scope.scopeModel.onSelectorSIPProfileReady = function (api) {
				selectorSIPProfileAPI = api;
				selectorSIPProfileReadyDeferred.resolve();
			};

		}
		function load() {
			$scope.scopeModel.isLoading = true;

			if (isEditMode) {
				getRoute().then(function () {
					loadAllControls();
				}).catch(function (error) {
					VRNotificationService.notifyExceptionWithClose(error, $scope);
					$scope.scopeModel.isLoading = false;
				});
			}

			else if (isViewHistoryMode) {
				getRouteHistory().then(function () {
					loadAllControls();
				}).catch(function (error) {
					VRNotificationService.notifyExceptionWithClose(error, $scope);
					$scope.isLoading = false;
				});

			}

			else {
				loadAllControls();
			}
		}

		function getRouteHistory() {
			return npIvSwitchRouteApiService.GetRouteHistoryDetailbyHistoryId(context.historyId).then(function (response) {
				routeEntity = response;

			});
		}
		function getRoute() {
			return npIvSwitchRouteApiService.GetRoute(routeId).then(function (response) {
				routeEntity = response;
			});
		}

		function loadAllControls() {
			return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSelectorCodecProfile, loadSelectorTranslationRule, loadWakeUpTime, loadSIPProfileSelector]).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}
		function setTitle() {
			if (isEditMode) {
				var routeName = (routeEntity != undefined) ? routeEntity.Description : null;
				$scope.title = UtilsService.buildTitleForUpdateEditor(routeName, 'Route');
			}
			else if (isViewHistoryMode && routeEntity != undefined)
				$scope.title = "View Route: " + routeEntity.Description;
			else {
				$scope.title = UtilsService.buildTitleForAddEditor('Route');
			}
		}
		function loadStaticData() {

			$scope.scopeModel.states = UtilsService.getArrayEnum(NP_IVSwitch_EndPointStateEnum);
			$scope.scopeModel.transportmode = UtilsService.getArrayEnum(NP_IVSwitch_TransportModeEnum);
			if (routeEntity == undefined) {
				$scope.scopeModel.currentstate = $scope.scopeModel.states[0];
				return;
			}
			$scope.scopeModel.description = routeEntity.Description;
			$scope.scopeModel.logalias = routeEntity.LogAlias;

			$scope.scopeModel.channelslimit = routeEntity.ChannelsLimit;
			$scope.scopeModel.host = routeEntity.Host;
			$scope.scopeModel.port = routeEntity.Port;
			$scope.scopeModel.connectiontimeout = routeEntity.ConnectionTimeOut;
			$scope.scopeModel.percentage = routeEntity.Percentage;

			if (routeEntity.TransportModeId != undefined)
				$scope.scopeModel.transportmodeid = $scope.scopeModel.transportmode[routeEntity.TransportModeId - 1];
			if (routeEntity.CurrentState != undefined)
				$scope.scopeModel.currentstate = $scope.scopeModel.states[routeEntity.CurrentState - 1];


		}
		function loadSelectorCodecProfile() {
			var selectorCodecProfileLoadDeferred = UtilsService.createPromiseDeferred();

			selectorCodecProfileReadyDeferred.promise.then(function () {
				var selectorCodecProfilePayload = {};

				if (routeEntity != undefined && routeEntity.CodecProfileId != 0)
					selectorCodecProfilePayload.selectedIds = routeEntity.CodecProfileId;

				VRUIUtilsService.callDirectiveLoad(selectorCodecProfileAPI, selectorCodecProfilePayload, selectorCodecProfileLoadDeferred);
			});

			return selectorCodecProfileLoadDeferred.promise;
		}
		function loadSelectorTranslationRule() {
			var selectorTranslationRuleLoadDeferred = UtilsService.createPromiseDeferred();

			selectorTranslationRuleReadyDeferred.promise.then(function () {
				var selectorTranslationRulePayload = {};
				if (routeEntity != undefined && routeEntity.TransRuleId != 0 && !isCloneMode )
					selectorTranslationRulePayload.selectedIds = routeEntity.TransRuleId;

				VRUIUtilsService.callDirectiveLoad(selectorTranslationRuleAPI, selectorTranslationRulePayload, selectorTranslationRuleLoadDeferred);
			});

			return selectorTranslationRuleLoadDeferred.promise;
		}

		function loadWakeUpTime() {
			if (routeEntity != undefined && routeEntity.WakeUpTime != undefined)
				$scope.scopeModel.wakeuptime = routeEntity.WakeUpTime;
			else
				npIvSwitchRouteApiService.GetSwitchDateTime().then(function (response) {
					$scope.scopeModel.wakeuptime = response;
				});
		}

		function loadSIPProfileSelector() {
			var selectorSIPProfileLoadDeferred = UtilsService.createPromiseDeferred();
	
			selectorSIPProfileReadyDeferred.promise.then(function () {
				var selectorSIPProfilePayload = {};
				if (routeEntity != undefined && routeEntity.SIPProfile != undefined)
					selectorSIPProfilePayload.selectedIds = routeEntity.SIPProfile;
				else
					selectorSIPProfilePayload.selectedIds = "internal";
				VRUIUtilsService.callDirectiveLoad(selectorSIPProfileAPI, selectorSIPProfilePayload, selectorSIPProfileLoadDeferred);
			});
			return selectorSIPProfileLoadDeferred.promise;
		}

		function insert() {
			$scope.scopeModel.isLoading = true;

			return npIvSwitchRouteApiService.AddRoute(buildRouteObjFromScopeForAdd()).then(function (response) {
				if (VRNotificationService.notifyOnItemAdded('Route', response, 'Name')) {

					if ($scope.onRouteAdded != undefined) {
						var insertedObjects = response.InsertedObject;
						if (insertedObjects != undefined) {
							for (var i = 0; i < insertedObjects.length; i++) {
								$scope.onRouteAdded(insertedObjects[i]);
							}
						}
					}
					$scope.modalContext.closeModal();
				}
			}).catch(function (error) {
				VRNotificationService.notifyException(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}
		function update() {
			$scope.scopeModel.isLoading = true;

			return npIvSwitchRouteApiService.UpdateRoute(buildRouteObjFromScopeForUpdate()).then(function (response) {


				if (VRNotificationService.notifyOnItemUpdated('Route', response, 'Name')) {

					if ($scope.onRouteUpdated != undefined) {
						$scope.onRouteUpdated(response.UpdatedObject);
					}
					$scope.modalContext.closeModal();
				}
			}).catch(function (error) {
				VRNotificationService.notifyException(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
				routeEntity = undefined;
			});
		}

		function buildRouteObjFromScopeForAdd() {
			var routesToAdd = [];
			var allHosts = $scope.scopeModel.hosts;
			if (allHosts != undefined) {
				for (var i = 0; i < allHosts.length; i++) {
					routesToAdd.push({
						Entity: {
							RouteId: routeEntity != undefined ? routeEntity.RouteId : undefined,
							Description: $scope.scopeModel.description,
							ChannelsLimit: $scope.scopeModel.channelslimit,
							LogAlias: $scope.scopeModel.logalias,
							Host: allHosts[i],
							Port: $scope.scopeModel.port,
							ConnectionTimeOut: $scope.scopeModel.connectiontimeout,
							CurrentState: $scope.scopeModel.currentstate != undefined ? $scope.scopeModel.currentstate.value : undefined,
							CodecProfileId: $scope.scopeModel.codecprofileid,
							TransRuleId: $scope.scopeModel.translationruleid,
							WakeUpTime: $scope.scopeModel.wakeuptime,
							TransportModeId: $scope.scopeModel.transportmodeid != undefined ? $scope.scopeModel.transportmodeid.value : undefined,
							Percentage: $scope.scopeModel.percentage,
							SIPProfile: selectorSIPProfileAPI.getSelectedIds()
						},

						CarrierAccountId: carrierAccountId
					});
				}
			}
			return { RoutesToAdd: routesToAdd };

		}

		function buildRouteObjFromScopeForUpdate() {
			return {

				Entity: {
                    RouteId: routeId,
					Description: $scope.scopeModel.description,
					ChannelsLimit: $scope.scopeModel.channelslimit,
					LogAlias: $scope.scopeModel.logalias,
					Host: $scope.scopeModel.host,
					Port: $scope.scopeModel.port,
					ConnectionTimeOut: $scope.scopeModel.connectiontimeout,
					CurrentState: $scope.scopeModel.currentstate != undefined ? $scope.scopeModel.currentstate.value : undefined,
					CodecProfileId: $scope.scopeModel.codecprofileid,
					TransRuleId: $scope.scopeModel.translationruleid,
					WakeUpTime: $scope.scopeModel.wakeuptime,
					TransportModeId: $scope.scopeModel.transportmodeid != undefined ? $scope.scopeModel.transportmodeid.value : undefined,
					Percentage: $scope.scopeModel.percentage,
					SIPProfile: selectorSIPProfileAPI.getSelectedIds()
				},

				CarrierAccountId: carrierAccountId

			};
		}
	}

	appControllers.controller('NP_IVSwitch_RouteEditorController', RouteEditorController);

})(appControllers);