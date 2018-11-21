(function (appControllers) {

	'use strict';

	countryEditorController.$inject = ['$scope', 'Demo_Module_CountryAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

	function countryEditorController($scope, Demo_Module_CountryAPIService, VRNotificationService, VRNavigationService, UtilsService) {

		var isEditMode;
		var countryId;
		var countryEntity;

		loadParameters();
		defineScope();
		load();

		function loadParameters() {
			var parameters = VRNavigationService.getParameters($scope);

			if (parameters != undefined && parameters != null) {
				countryId = parameters.countryId;
			}

			isEditMode = (countryId != undefined);
		}

		function defineScope() {
			$scope.scopeModel = {};

			$scope.scopeModel.saveCountry = function () {
				if (isEditMode)
					return updateCountry();
				else
					return insertCountry();
			};

			$scope.scopeModel.close = function () {
				$scope.modalContext.closeModal();
			};
		}

		function load() {
			$scope.scopeModel.isLoading = true;
			if (isEditMode) {
				getCountry().then(function () {
					loadAllControls().finally(function () {
						countryEntity = undefined;
					});
				}).catch(function (error) {
					$scope.scopeModel.isLoading = false;
					VRNotificationService.notifyExceptionWithCloseWithClose(error, $scope);
				});
			}
			else {
				loadAllControls();
			}
		}

		function getCountry() {
			return Demo_Module_CountryAPIService.GetCountryById(countryId).then(function (response) {
				countryEntity = response;
			});
		}

		function loadAllControls() {

			function setTitle() {
				if (isEditMode && countryEntity != undefined) {
					$scope.title = UtilsService.buildTitleForUpdateEditor(countryEntity.Name, "Country");
				}
				else {
					$scope.title = UtilsService.buildTitleForAddEditor("Country");
				}
			}

			function loadStaticData() {
				if (countryEntity != undefined) {
					$scope.scopeModel.name = countryEntity.Name;
					$scope.scopeModel.population = countryEntity.Settings.Population;
					$scope.scopeModel.capital = countryEntity.Settings.Capital;
				}
			}

			return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).catch(function (error) {
				VRNotificationService.notifyExceptionWithCloseWithClose(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}

		function buildCountryObjectFromScope() {
			var object = {
				CountryId: countryId != undefined ? countryId : undefined,
				Name: $scope.scopeModel.name,
				Settings: {
					Capital: $scope.scopeModel.capital,
					Population: $scope.scopeModel.population
				}
			};
			return object;
		}

		function insertCountry() {
			$scope.scopeModel.isLoading = true;
			var country = buildCountryObjectFromScope();

			return Demo_Module_CountryAPIService.AddCountry(country).then(function (response) {
				if (VRNotificationService.notifyOnItemAdded("Country", response, "Name")) {
					if ($scope.onCountryAdded != undefined)
						$scope.onCountryAdded(response.InsertedObject);
					$scope.modalContext.closeModal();
				}
			}).catch(function (error) {
				$scope.scopeModel.isLoading = false;
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}

		function updateCountry() {
			$scope.scopeModel.isLoading = true;
			var country = buildCountryObjectFromScope();

			return Demo_Module_CountryAPIService.UpdateCountry(country).then(function (response) {
				if (VRNotificationService.notifyOnItemUpdated("Country", response, "Name")) {
					if ($scope.onCountryUpdated != undefined)
						$scope.onCountryUpdated(response.UpdatedObject);
					$scope.modalContext.closeModal();
				}
			}).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
				$scope.scopeModel.isLoading = false;
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}
	}

	appControllers.controller("Demo_Module_CountryEditorController", countryEditorController);
})(appControllers);