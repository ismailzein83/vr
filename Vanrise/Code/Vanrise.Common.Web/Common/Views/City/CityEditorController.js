(function (appControllers) {

    "use strict";

    cityEditorController.$inject = ['$scope', 'VRCommon_CityAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function cityEditorController($scope, VRCommon_CityAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var cityId;
        var countryId;
        var regionId;
        var editMode;
        var cityEntity;
        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var countrySelectedPromiseDeferred;

        var regionDirectiveApi;
        var regionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var disableCountry;
        var context;
        var isViewHistoryMode;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                cityId = parameters.CityId;
                countryId = parameters.CountryId;
                regionId = parameters.RegionId;
                disableCountry = parameters.disableCountry;
                context = parameters.context;
            }
            editMode = (cityId != undefined);
            isViewHistoryMode = (context != undefined && context.historyId != undefined);
            $scope.disableCountry = editMode || disableCountry;

        }

        function defineScope() {

            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            };
            $scope.onRegionDirectiveReady = function (api) {
                regionDirectiveApi = api;
                regionReadyPromiseDeferred.resolve();
            };
            $scope.onCountrySelectionChanged = function () {
                var selectedCountryId = countryDirectiveApi.getSelectedIds();
                if (selectedCountryId != undefined) {
                    var setLoader = function (value) { $scope.isLoadingRegions = value };
                    var payload = {
                        filter: {
                            CountryId: selectedCountryId
                        }
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, regionDirectiveApi, payload, setLoader, countrySelectedPromiseDeferred);
                }
                else if (regionDirectiveApi != undefined) {
                    regionDirectiveApi.clearDataSource();
                    regionDirectiveApi.clearFilter();
                }
                    
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
            else if (isViewHistoryMode) {
                getCityHistory().then(function () {
                    loadAllControls().finally(function () {
                        cityEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });

            }

            else {
                loadAllControls();
            }
        }
        function getCityHistory() {
            return VRCommon_CityAPIService.GetCityHistoryDetailbyHistoryId(context.historyId).then(function (response) {
                cityEntity = response;

            });
        }
        function getCity() {
            return VRCommon_CityAPIService.GetCity(cityId).then(function (city) {
                cityEntity = city;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCountryRegionSection])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            if (editMode && cityEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(cityEntity.Name, "City");
            else if (isViewHistoryMode && cityEntity != undefined)
                $scope.title = "View City: " + cityEntity.Name;
            else
                $scope.title = UtilsService.buildTitleForAddEditor("City");
        }

        function loadStaticData() {

            if (cityEntity == undefined)
                return;

            $scope.name = cityEntity.Name;
        }



        function loadCountryRegionSection() {
            var loadCountryPromiseDeferred = UtilsService.createPromiseDeferred();

            var promises = [];
            promises.push(loadCountryPromiseDeferred.promise);

            var payload;

            if (cityEntity != undefined && cityEntity.CountryId != undefined || countryId !=undefined ) {
                payload = {};
                payload.selectedIds = cityEntity && cityEntity.CountryId || countryId || undefined;
                countrySelectedPromiseDeferred = UtilsService.createPromiseDeferred();
            }

            countryReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, payload, loadCountryPromiseDeferred);
            });

            if (cityEntity != undefined && cityEntity.CountryId != undefined || countryId != undefined) {
                var loadRegionPromiseDeferred = UtilsService.createPromiseDeferred();

                promises.push(loadRegionPromiseDeferred.promise);

                UtilsService.waitMultiplePromises([regionReadyPromiseDeferred.promise, countrySelectedPromiseDeferred.promise]).then(function () {
                    var regionPayload = {
                        filter: {
                            CountryId: cityEntity != undefined && cityEntity.CountryId  || countryId || undefined
                        },
                        selectedIds: cityEntity && cityEntity.Settings != undefined && cityEntity.Settings.RegionId || regionId || undefined
                    };

                    VRUIUtilsService.callDirectiveLoad(regionDirectiveApi, regionPayload, loadRegionPromiseDeferred);
                    countrySelectedPromiseDeferred = undefined;
                });
            }

            return UtilsService.waitMultiplePromises(promises);
        }

        function buildCityObjFromScope() {
            var regionId = regionDirectiveApi.getSelectedIds();
            var obj = {
                CityId: (cityId != null) ? cityId : 0,
                Name: $scope.name,
                CountryId: countryDirectiveApi.getSelectedIds(),
                Settings: regionId != undefined ? { RegionId: regionId } : null
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
