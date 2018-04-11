(function (appControllers) {

	"use strict";

	VRNotificationLogController.$inject = ['$scope', 'VRCommon_MasterLogAPIService', 'UtilsService', 'VRNavigationService', 'VRUIUtilsService', 'VRNotificationService', 'VRValidationService', 'VR_Notification_VRNotificationTypeAPIService'];

	function VRNotificationLogController($scope, VRCommon_MasterLogAPIService, UtilsService, VRNavigationService, VRUIUtilsService, VRNotificationService, VRValidationService, VR_Notification_VRNotificationTypeAPIService) {

		var viewId;
		var notificationTypeId;

		var notificationTypeSettingsSelectorAPI;
		var notificationTypeSettingsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

		var notificationAlertlevelSelectorAPI;
		var notificationAlertlevelSelectorReadyDeferred = UtilsService.createPromiseDeferred();

		var notificationStatusSelectorAPI;
		var notificationStatusSelectorReadyDeferred = UtilsService.createPromiseDeferred();

		var searchDirectiveAPI;
		var searchDirectiveAPIReadyDeferred = UtilsService.createPromiseDeferred();

		var bodyDirectiveAPI;
		var bodyDirectiveAPIReadyDeferred = UtilsService.createPromiseDeferred();

		loadParameters();
		defineScope();
		load();

		function loadParameters() {
			var parameters = VRNavigationService.getParameters($scope);

			if (parameters != null) {
				viewId = parameters.viewId;
			}
		}
		function defineScope() {
			$scope.scopeModel = {};

			$scope.scopeModel.legendHeader = "Legend";

			$scope.scopeModel.isNotificationTypeSettingSelected = false;

			$scope.scopeModel.onVRNotificationTypeSettingsSelectorReady = function (api) {
				notificationTypeSettingsSelectorAPI = api;
				notificationTypeSettingsSelectorReadyDeferred.resolve();
			};
			$scope.scopeModel.onVRNotificationAlertlevelSelectorReady = function (api) {
				notificationAlertlevelSelectorAPI = api;
				notificationAlertlevelSelectorReadyDeferred.resolve();
			};
			$scope.scopeModel.onNotificationStatusSelectorReady = function (api) {
				notificationStatusSelectorAPI = api;
				notificationStatusSelectorReadyDeferred.resolve();
			};
			$scope.scopeModel.onSearchDirectiveReady = function (api) {
				searchDirectiveAPI = api;
				searchDirectiveAPIReadyDeferred.resolve();
			};
			$scope.scopeModel.onBodyDirectiveReady = function (api) {
				bodyDirectiveAPI = api;
				bodyDirectiveAPIReadyDeferred.resolve();
			};

			$scope.scopeModel.onNotificationTypeSelectionChanged = function (selectedItem) {

				if (selectedItem != undefined) {
					setLegendContent(selectedItem.Id);
					$scope.scopeModel.isNotificationTypeSettingSelected = true;
					notificationTypeId = selectedItem.Id;

					loadNotificationAlertlevelSelector();
					loadSearchDirective();
					loadBodyDirective();

					function loadNotificationAlertlevelSelector() {
						notificationAlertlevelSelectorReadyDeferred.promise.then(function () {

							var notificationAlertlevelSelectorPayload = {
								filter: {
									VRNotificationTypeId: notificationTypeId
								}
							};
							var setLoader = function (value) {
								$scope.scopeModel.isAlertLevelSelectorLoading = value;
							};
							VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, notificationAlertlevelSelectorAPI, notificationAlertlevelSelectorPayload, setLoader, undefined);
						});
					}
					function loadSearchDirective() {
						searchDirectiveAPIReadyDeferred.promise.then(function () {
							var searchDirectivePayload = {
								notificationTypeId: notificationTypeId,
								context: buildSearchEditorContext()
							};
							VRUIUtilsService.callDirectiveLoad(searchDirectiveAPI, searchDirectivePayload, undefined);
						});
					}
					function loadBodyDirective() {
						bodyDirectiveAPIReadyDeferred.promise.then(function () {
							var bodyDirectivePayload = {
								notificationTypeId: notificationTypeId,
								query: buildBodyDirectiveQuery()
							};
							VRUIUtilsService.callDirectiveLoad(bodyDirectiveAPI, bodyDirectivePayload, undefined);
						});
					}

					function setLegendContent(notificationTypeId) {
						$scope.scopeModel.isLegendLoading = true;
						$scope.scopeModel.showLegend = false;
						VR_Notification_VRNotificationTypeAPIService.GetVRNotificationTypeLegendData(notificationTypeId).then(function (response) {
							if (response != null) {
								var legendContent = '<div style="font-size:12px; margin:10px">'
								angular.forEach(response, function (item) {
									legendContent = legendContent +
										'<div><div  class ="' + item.AlertLevelStyle.ClassName + '" style="display: inline-block; width: 20px; height: 10px; margin: 0px 3px"></div>' + item.Entity.Name + '</div>';
								});

								legendContent = legendContent + '</div>';
								$scope.scopeModel.legendContent = legendContent;
								$scope.scopeModel.showLegend = true;
							}
							else {
								$scope.scopeModel.legendContent = null;
							}
							$scope.scopeModel.isLegendLoading = false;
						});
					}
				}
			};

			$scope.scopeModel.validateDates = function () {
				return VRValidationService.validateTimeRange($scope.scopeModel.from, $scope.scopeModel.to);
			};

			$scope.scopeModel.searchClicked = function () {
				var bodyDirectivePayload = {
					notificationTypeId: notificationTypeId,
					query: buildBodyDirectiveQuery(),
					extendedQuery: searchDirectiveAPI.getData()
				};
				bodyDirectiveAPI.load(bodyDirectivePayload);
			};
		}
		function load() {
			$scope.scopeModel.isLoading = true;
			loadAllControls();
		}

		function loadAllControls() {
			return UtilsService.waitMultipleAsyncOperations([loadNotificationTypeSelector, loadNotificationStatusSelector])
				.catch(function (error) {
					VRNotificationService.notifyExceptionWithClose(error, $scope);
				})
				.finally(function () {
					$scope.scopeModel.isLoading = false;
				});
		}
		function loadNotificationTypeSelector() {
			var notificationTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

			notificationTypeSettingsSelectorReadyDeferred.promise.then(function () {

				var notificationTypeSelectorPayload = {
					filter: {
						Filters: [{
							$type: "Vanrise.Notification.Business.VRNotificationTypeViewFilter, Vanrise.Notification.Business",
							ViewId: viewId
						}]
					},
					selectIfSingleItem: true
				};
				VRUIUtilsService.callDirectiveLoad(notificationTypeSettingsSelectorAPI, notificationTypeSelectorPayload, notificationTypeSelectorLoadDeferred);
			});

			return notificationTypeSelectorLoadDeferred.promise;
		}
		function loadNotificationStatusSelector() {
			var notificationStatusSelectorLoadDeferred = UtilsService.createPromiseDeferred();

			notificationStatusSelectorReadyDeferred.promise.then(function () {

				var notificationStatusSelectorPayload;
				VRUIUtilsService.callDirectiveLoad(notificationStatusSelectorAPI, notificationStatusSelectorPayload, notificationStatusSelectorLoadDeferred);
			});

			return notificationStatusSelectorLoadDeferred.promise;
		}

		function buildSearchEditorContext() {
			var context = {
				isNotificationTypeSettingSelected: function () {
					return $scope.scopeModel.isNotificationTypeSettingSelected;
				},
				isAdvancedTabSelected: function () {
					return $scope.advancedSelected;
				}
			};
			return context;
		}

		function buildBodyDirectiveQuery() {
			return {
				AlertLevelIds: notificationAlertlevelSelectorAPI.getSelectedIds(),
				//Description: $scope.scopeModel.description,
				StatusIds: notificationStatusSelectorAPI.getSelectedIds(),
				From: $scope.scopeModel.from,
				To: $scope.scopeModel.to
			};
		}
	}

	appControllers.controller('VR_Notification_VRNotificationLogController', VRNotificationLogController);

})(appControllers);