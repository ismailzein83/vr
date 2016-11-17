(function (appControllers) {

    "use strict";

    cityEditorController.$inject = ['$scope', 'VRCommon_CityAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function cityEditorController($scope, VRCommon_CityAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var cityId;
        var countryId;
        var editMode;
        var cityEntity;
        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var disableCountry;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                cityId = parameters.CityId;
                countryId = parameters.CountryId;
                disableCountry = parameters.disableCountry
            }
            editMode = (cityId != undefined);
            $scope.disableCountry = editMode || disableCountry;
           
        }

        function defineScope() {

            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            };

            $scope.saveCity = function () {
                if (editMode)
                    return updateCity();
                else
                    return insertCity();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };


        }

        function load() {

            $scope.isLoading = true;

            if (editMode) {
                getCity().then(function () {
                    loadAllControls()
                        .finally(function () {
                            cityEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getCity() {
            return VRCommon_CityAPIService.GetCity(cityId).then(function (city) {
                cityEntity = city;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCountrySelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle()
        {
            if (editMode && cityEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(cityEntity.Name, "City");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("City");
        }

        function loadStaticData() {

            if (cityEntity == undefined)
                return;

            $scope.name = cityEntity.Name;
        }

        function loadCountrySelector() {
            var countryLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            countryReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectedIds: cityEntity != undefined ? cityEntity.CountryId : (countryId != undefined) ? countryId : undefined
                    };

                    VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, directivePayload, countryLoadPromiseDeferred);
                });
            return countryLoadPromiseDeferred.promise;
        }

        function buildCityObjFromScope() {
            var obj = {
                CityId: (cityId != null) ? cityId : 0,
                Name: $scope.name,
                CountryId: countryDirectiveApi.getSelectedIds()
            };
            return obj;
        }

        
        function insertCity() {
            $scope.isLoading = true;

            var cityObject = buildCityObjFromScope();
            return VRCommon_CityAPIService.AddCity(cityObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("City", response, "Name")) {
                    if ($scope.onCityAdded != undefined)
                        $scope.onCityAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

        }
        function updateCity() {
            $scope.isLoading = true;

            var cityObject = buildCityObjFromScope();
            VRCommon_CityAPIService.UpdateCity(cityObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("City", response, "Name")) {
                    if ($scope.onCityUpdated != undefined)
                        $scope.onCityUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('VRCommon_CityEditorController', cityEditorController);
})(appControllers);
