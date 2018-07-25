(function (appControllers) {

	"use strict";

	switchEditorController.$inject = ["$scope", "WhS_BE_SwitchAPIService", "UtilsService", "VRNotificationService", "VRNavigationService", "VRUIUtilsService", "UpdateOperationResultEnum"];

	function switchEditorController($scope, WhS_BE_SwitchAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, UpdateOperationResultEnum) {

		var isEditMode;
		var switchId;
		var switchEditorDirectiveAPI;
		var switchEditorReadyDeferred = UtilsService.createPromiseDeferred();

		loadParameters();
		defineScope();
		load();

		function loadParameters() {
			var parameters = VRNavigationService.getParameters($scope);

			if (parameters != undefined && parameters != null) {
				switchId = parameters.switchId;
			}

			isEditMode = (switchId != undefined);
		}
		function defineScope() {
			$scope.scopeModel = {};
			$scope.scopeModel.hasSaveSwitchPermission = function () {
				if (isEditMode)
					return WhS_BE_SwitchAPIService.HasUpdateSwitchPermission();
				else
					return WhS_BE_SwitchAPIService.HasAddSwitchPermission();
			};

			$scope.scopeModel.saveSwitch = function () {
				$scope.isLoading = true;
				var savePromiseResponse;
				var savePromise = switchEditorDirectiveAPI.save().then(function (response) { savePromiseResponse = response });
				var rootPromiseNode = {
					promises: [savePromise]
				}

				if (isEditMode) {
					rootPromiseNode.getChildNode = function () {
						var resetSwitchSyncDataPromise = UtilsService.createPromiseDeferred();
						if (savePromiseResponse != undefined && savePromiseResponse.Result != undefined && savePromiseResponse.Result == UpdateOperationResultEnum.Succeeded.value) {
							VRNotificationService.showConfirmation("Do you want to reset the switch info in order to trigger the full sync with the next route sync process?").then(function (result) {
								if (result)
									WhS_BE_SwitchAPIService.ResetSwitchSyncData(switchId).then(function () { resetSwitchSyncDataPromise.resolve(); });
								else resetSwitchSyncDataPromise.resolve();
							});
						}
						else resetSwitchSyncDataPromise.resolve();

						return {
							promises: [resetSwitchSyncDataPromise.promise]
						};
					};
				}

				UtilsService.waitPromiseNode(rootPromiseNode).then(function () {
					$scope.modalContext.closeModal();
				}).finally(function () {
					$scope.isLoading = false;
				})
			};

			$scope.onSwitchEditorReady = function (api) {
				switchEditorDirectiveAPI = api;
				switchEditorReadyDeferred.resolve();
			};

			$scope.scopeModel.close = function () {
				$scope.modalContext.closeModal();
			};
		}
		function load() {
			$scope.isLoading = true;
			loadAllControls();
		}
		function loadAllControls() {
			return UtilsService.waitMultipleAsyncOperations([loadSwitchEditorDirective]).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			}).finally(function () {
				$scope.isLoading = false;
			});
		}
		function loadSwitchEditorDirective() {
			var switchEditorLoadDeferred = UtilsService.createPromiseDeferred();
			switchEditorReadyDeferred.promise.then(function () {
				VRUIUtilsService.callDirectiveLoad(switchEditorDirectiveAPI, { switchId: switchId }, switchEditorLoadDeferred);
			});
			return switchEditorLoadDeferred.promise.then(function () {
				setTitle();
			});
		}
		function setTitle() {
			if (isEditMode) {
				$scope.title = UtilsService.buildTitleForUpdateEditor(switchEditorDirectiveAPI.getTitle(), 'SwitchName');
			}
			else {
				$scope.title = UtilsService.buildTitleForAddEditor('SwitchName');
			}
		}

	}

	appControllers.controller("WhS_BE_SwitchEditorController", switchEditorController);
})(appControllers);
