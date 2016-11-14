(function (appControllers) {

	'use strict';

	SalePriceListTemplateEditorController.$inject = ['$scope', 'WhS_BE_SalePriceListTemplateAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

	function SalePriceListTemplateEditorController($scope, WhS_BE_SalePriceListTemplateAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

		var isEditMode;

		var salePriceListTemplateId;
		var salePriceListTemplateEntity;

		var settingsSelectiveAPI;
		var settingsSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

		loadParameters();
		defineScope();
		load();

		function loadParameters() {

			var parameters = VRNavigationService.getParameters($scope);

			if (parameters != undefined && parameters != null) {
				salePriceListTemplateId = parameters.salePriceListTemplateId;
			}

			isEditMode = (salePriceListTemplateId != undefined);
		}
		function defineScope() {

			$scope.scopeModel = {};

			$scope.scopeModel.onSettingsSelectiveReady = function (api) {
				settingsSelectiveAPI = api;
				settingsSelectiveReadyDeferred.resolve();
			};

			$scope.scopeModel.save = function () {
				return (isEditMode) ? updateSalePriceListTemplate() : insertSalePriceListTemplate();
			};

			$scope.scopeModel.hasSavePermission = function () {
				if (isEditMode)
					return WhS_BE_SalePriceListTemplateAPIService.HasEditSalePriceListTemplatePermission();
				return WhS_BE_SalePriceListTemplateAPIService.HasAddSalePriceListTemplatePermission();
			};

			$scope.scopeModel.close = function () {
				$scope.modalContext.closeModal();
			};
		}
		function load() {
			$scope.scopeModel.isLoading = true;

			if (isEditMode) {
				getSalePriceListTemplate().then(function () {
					loadAllControls().finally(function () {
						salePriceListTemplateEntity = undefined;
					});
				}).catch(function (error) {
					VRNotificationService.notifyExceptionWithClose(error, $scope);
					$scope.scopeModel.isLoading = false;
				});
			}
			else {
				loadAllControls();
			}
		}

		function getSalePriceListTemplate() {
			return WhS_BE_SalePriceListTemplateAPIService.GetSalePriceListTemplate(salePriceListTemplateId).then(function (response) {
				salePriceListTemplateEntity = response;
			});
		}
		function loadAllControls() {
			return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSettingsSelective]).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}
		function setTitle() {
			if (isEditMode) {
				if (salePriceListTemplateEntity != undefined)
					$scope.title = UtilsService.buildTitleForUpdateEditor(salePriceListTemplateEntity.Name, 'Sale Pricelist Template');
			}
			else
				$scope.title = UtilsService.buildTitleForAddEditor('Sale Pricelist Template');
		}
		function loadStaticData() {
			if (salePriceListTemplateEntity == undefined)
				return;
			$scope.scopeModel.name = salePriceListTemplateEntity.Name;
		}
		function loadSettingsSelective() {
			var settingsSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

			settingsSelectiveReadyDeferred.promise.then(function () {
				var settingsSelectivePayload;
				if (salePriceListTemplateEntity != undefined) {
					settingsSelectivePayload = {
						settings: salePriceListTemplateEntity.Settings
					};
				}
				VRUIUtilsService.callDirectiveLoad(settingsSelectiveAPI, settingsSelectivePayload, settingsSelectiveLoadDeferred);
			});

			return settingsSelectiveLoadDeferred.promise;
		}

		function insertSalePriceListTemplate() {
			$scope.scopeModel.isLoading = true;
			return WhS_BE_SalePriceListTemplateAPIService.AddSalePriceListTemplate(buildSalePriceListTemplateFromScope()).then(function (response) {
				if (VRNotificationService.notifyOnItemAdded('Sale Pricelist Template', response, 'Name')) {
					if ($scope.onSalePriceListTemplateAdded != undefined)
						$scope.onSalePriceListTemplateAdded(response.InsertedObject);
					$scope.modalContext.closeModal();
				}
			}).catch(function (error) {
				VRNotificationService.notifyException(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}
		function updateSalePriceListTemplate() {
			$scope.scopeModel.isLoading = true;
			return WhS_BE_SalePriceListTemplateAPIService.UpdateSalePriceListTemplate(buildSalePriceListTemplateFromScope()).then(function (response) {
				if (VRNotificationService.notifyOnItemUpdated('Sale Pricelist Template', response, 'Name')) {
					if ($scope.onSalePriceListTemplateUpdated != undefined)
						$scope.onSalePriceListTemplateUpdated(response.UpdatedObject);
					$scope.modalContext.closeModal();
				}
			}).catch(function (error) {
				VRNotificationService.notifyException(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}
		function buildSalePriceListTemplateFromScope() {
			return {
				SalePriceListTemplateId: salePriceListTemplateId,
				Name: $scope.scopeModel.name,
				Settings: settingsSelectiveAPI.getData()
			};
		}
	}

	appControllers.controller('WhS_BE_SalePriceListTemplateEditorController', SalePriceListTemplateEditorController);

})(appControllers);