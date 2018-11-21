(function (appControllers) {
	"use strict";

	cityEditorController.$inject = ['$scope', 'Demo_Module_CityAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

	function cityEditorController($scope, Demo_Module_CityAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

		var isEditMode;
		var cityId;
		var cityEntity;
		var countryIdItem;

		var countryDirectiveAPI;
		var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

		var citySettingsDirectiveAPI;
		var citySettingsReadypromiseDeferred = UtilsService.createPromiseDeferred();


		loadParameters();
		defineScope();
		load();

		function loadParameters() {

			var parameters = VRNavigationService.getParameters($scope);

			if (parameters != undefined && parameters != null) {
				cityId = parameters.cityId;
				countryIdItem = parameters.countryIdItem;
			}

			isEditMode = (cityId != undefined);
		};

		function defineScope() {
			$scope.scopeModel = {};
			$scope.scopeModel.disableCountry = countryIdItem != undefined;

			$scope.scopeModel.onCountrySelectorReady = function (api) {
				countryDirectiveAPI = api;
				countryReadyPromiseDeferred.resolve();
			};

			$scope.scopeModel.onCitySettingsDirectiveReady = function (api) {
				citySettingsDirectiveAPI = api;
				citySettingsReadypromiseDeferred.resolve();
			};

			$scope.scopeModel.onCityDistrictGridReady = function (api) {
				districtGridAPI = api;
				districtGridReadypromiseDeferred.resolve();
			};

			$scope.scopeModel.saveCity = function () {
				if (isEditMode)
					return updateCity();
				else
					return insertCity();
			};

			$scope.scopeModel.close = function () {
				$scope.modalContext.closeModal();
			};
		}

		function load() {
			$scope.scopeModel.isLoading = true;
			if (isEditMode) {
				getCity().then(function () {
					loadAllControls().finally(function () {
						cityEntity = undefined;
					});
				}).catch(function (error) {
					$scope.scopeModel.isLoading = false;
					VRNotificationService.notifyExceptionWithClose(error, $scope);
				});
			}
			else {
				loadAllControls();
			}
		}

		function getCity() {
			return Demo_Module_CityAPIService.GetCityById(cityId).then(function (response) {
				cityEntity = response;
			});
		}

		function loadAllControls() {

			function setTitle() {
				if (isEditMode && cityEntity != undefined)
					$scope.title = UtilsService.buildTitleForUpdateEditor(cityEntity.Name, "City");
				else
					$scope.title = UtilsService.buildTitleForAddEditor("City");
			}

			function loadStaticData() {
				if (cityEntity != undefined) {
					$scope.scopeModel.name = cityEntity.Name;
				}
			}

			function loadCountrySelector() {
				var countryLoadPromiseDeferred = UtilsService.createPromiseDeferred();

				countryReadyPromiseDeferred.promise.then(function () {
					var countryPayload = {};
					if (countryIdItem != undefined)
						countryPayload.selectedIds = countryIdItem.CountryId;
					if (cityEntity != undefined)
						countryPayload.selectedIds = cityEntity.CountryId;
					VRUIUtilsService.callDirectiveLoad(countryDirectiveAPI, countryPayload, countryLoadPromiseDeferred);
				});

				return countryLoadPromiseDeferred.promise;
			}

			function loadCitySettingsDirective() {
				var citySettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();

				citySettingsReadypromiseDeferred.promise.then(function () {

					var citySettingsPayload = {};
					if (cityEntity != undefined) {
						citySettingsPayload = {
							citySettings: cityEntity.Settings
						};
					}
					VRUIUtilsService.callDirectiveLoad(citySettingsDirectiveAPI, citySettingsPayload, citySettingsLoadPromiseDeferred);
				});

				return citySettingsLoadPromiseDeferred.promise;
			}

			return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCountrySelector, loadCitySettingsDirective]).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}

		function buildCityObjectFromScope() {
			var object = {
				CityId: (cityId != undefined) ? cityId : undefined,
				Name: $scope.scopeModel.name,
				CountryId: countryDirectiveAPI.getSelectedIds(),
				Settings: citySettingsDirectiveAPI.getData()
			};

			return object;
		}

		function insertCity() {
			$scope.scopeModel.isLoading = true;
			var cityObject = buildCityObjectFromScope();
			return Demo_Module_CityAPIService.AddCity(cityObject).then(function (response) {
				if (VRNotificationService.notifyOnItemAdded("City", response, "Name")) {
					if ($scope.onCityAdded != undefined) {
						$scope.onCityAdded(response.InsertedObject);
					}
					$scope.modalContext.closeModal();
				}
			}).catch(function (error) {
				$scope.scopeModel.isLoading = false;
				VRNotificationService.notifyException(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}

		function updateCity() {
			$scope.scopeModel.isLoading = true;
			var cityObject = buildCityObjectFromScope();
			Demo_Module_CityAPIService.UpdateCity(cityObject).then(function (response) {
				if (VRNotificationService.notifyOnItemUpdated("City", response, "Name")) {
					if ($scope.onCityUpdated != undefined) {
						$scope.onCityUpdated(response.UpdatedObject);
					}
					$scope.modalContext.closeModal();
				}
			}).catch(function (error) {
				$scope.scopeModel.isLoading = false;
				VRNotificationService.notifyException(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}
	}

	appControllers.controller("Demo_Module_CityEditorController", cityEditorController);
})(appControllers);