(function (appControllers) {

	"use strict";

	factoryEditorController.$inject = ['$scope', 'Demo_Module_CityAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

	function factoryEditorController($scope, Demo_Module_CityAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

		var isEditMode;
		var factoryEntity;

		var factoryTypeAPI;
		var factoryTypeReadypromiseDeferred = UtilsService.createPromiseDeferred();

		loadParameters();
		defineScope();
		load();

		function loadParameters() {
			var parameters = VRNavigationService.getParameters($scope);

			if (parameters != undefined && parameters != null) {
				factoryEntity = parameters.factoryEntity;
			}

			isEditMode = (factoryEntity != undefined);
		};

		function defineScope() {
			$scope.scopeModel = {};

			$scope.scopeModel.onFactoryTypeReady = function (api) {
				factoryTypeAPI = api;
				factoryTypeReadypromiseDeferred.resolve();
			};

			$scope.scopeModel.saveFactory = function () {
				if (isEditMode)
					return updateFactory();
				else
					return insertFactory();
			};

			$scope.scopeModel.close = function () {
				$scope.modalContext.closeModal();
			};
		}

		function load() {
			$scope.scopeModel.isLoading = true;

			if (isEditMode) {
				loadAllControls().finally(function () {
					factoryEntity = undefined;
				});
			}
			else {
				loadAllControls();
			}
		}

		function loadAllControls() {

			function setTitle() {
				if (isEditMode) {
					if (factoryEntity != undefined)
						$scope.title = UtilsService.buildTitleForUpdateEditor(factoryEntity.ConfigId, "Factory");
				}
				else {
					$scope.title = UtilsService.buildTitleForAddEditor("Factory");
				}
			}

			function loadStaticData() {
				if (factoryEntity != undefined) {
					$scope.scopeModel.employeesNumber = factoryEntity.EmployeesNumber;
				}
			}

			function loadFactoryTypeDirective() {
				var factoryTypeLoadPromiseDeferred = UtilsService.createPromiseDeferred();

				factoryTypeReadypromiseDeferred.promise.then(function () {

					var factoryTypePayload = {};
					if (factoryEntity != undefined) {
						factoryTypePayload = { factoryType: factoryEntity };
					}
					VRUIUtilsService.callDirectiveLoad(factoryTypeAPI, factoryTypePayload, factoryTypeLoadPromiseDeferred);
				});

				return factoryTypeLoadPromiseDeferred.promise;
			}

			return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadFactoryTypeDirective]).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}

		function buildFactroyObjectFromScope() {
			var entity = factoryTypeAPI.getData();
			entity.EmployeesNumber = $scope.scopeModel.employeesNumber;

			var object = { Entity: entity };
			return object;
		}

		function insertFactory() {
			if ($scope.onFactoryAdded != undefined) {
				var factoryObject = buildFactroyObjectFromScope();
				$scope.onFactoryAdded(factoryObject);
			}

			$scope.modalContext.closeModal();
		}

		function updateFactory() {

			if ($scope.onFactoryUpdated != undefined) {
				var factoryObject = buildFactroyObjectFromScope();
				$scope.onFactoryUpdated(factoryObject);
			}
			$scope.modalContext.closeModal();
		}
	}

	appControllers.controller("Demo_Module_FactoryEditorController", factoryEditorController);
})(appControllers);