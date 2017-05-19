(function (appControllers) {

    "use strict";

    regionEditorController.$inject = ['$scope', 'VRCommon_RegionAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function regionEditorController($scope, VRCommon_RegionAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var regionId;
        var countryId;
        var editMode;
        var regionEntity;
        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var disableCountry;
        var context;
        var isViewHistoryMode;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                regionId = parameters.RegionId;
                countryId = parameters.CountryId;
                disableCountry = parameters.disableCountry;
                context = parameters.context;
            }
            editMode = (regionId != undefined);
            isViewHistoryMode = (context != undefined && context.historyId != undefined);
            $scope.disableCountry = editMode || disableCountry;
           
        }

        function defineScope() {

            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            };

            $scope.saveRegion = function () {
                if (editMode)
                    return updateRegion();
                else
                    return insertRegion();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };


        }

        function load() {

            $scope.isLoading = true;

            if (editMode) {
                getRegion().then(function () {
                    loadAllControls()
                        .finally(function () {
                            regionEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else if (isViewHistoryMode) {
                getRegionHistory().then(function () {
                    loadAllControls().finally(function () {
                        regionEntity = undefined;
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
        function getRegionHistory() {
            return VRCommon_RegionAPIService.GetRegionHistoryDetailbyHistoryId(context.historyId).then(function (response) {
                regionEntity = response;

            });
        }
        function getRegion() {
            return VRCommon_RegionAPIService.GetRegion(regionId).then(function (region) {
                regionEntity = region;
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
            if (editMode && regionEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(regionEntity.Name, "Region");
            else if (isViewHistoryMode && regionEntity != undefined)
                $scope.title = "View Region: " + regionEntity.Name;
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Region");
        }

        function loadStaticData() {

            if (regionEntity == undefined)
                return;

            $scope.name = regionEntity.Name;
        }

        function loadCountrySelector() {
            var countryLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            countryReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectedIds: regionEntity != undefined ? regionEntity.CountryId : (countryId != undefined) ? countryId : undefined
                    };

                    VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, directivePayload, countryLoadPromiseDeferred);
                });
            return countryLoadPromiseDeferred.promise;
        }

        function buildRegionObjFromScope() {
            var obj = {
                RegionId: (regionId != null) ? regionId : 0,
                Name: $scope.name,
                CountryId: countryDirectiveApi.getSelectedIds()
            };
            return obj;
        }

        
        function insertRegion() {
            $scope.isLoading = true;

            var regionObject = buildRegionObjFromScope();
            return VRCommon_RegionAPIService.AddRegion(regionObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Region", response, "Name")) {
                    if ($scope.onRegionAdded != undefined)
                        $scope.onRegionAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

        }
        function updateRegion() {
            $scope.isLoading = true;

            var regionObject = buildRegionObjFromScope();
            VRCommon_RegionAPIService.UpdateRegion(regionObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Region", response, "Name")) {
                    if ($scope.onRegionUpdated != undefined)
                        $scope.onRegionUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('VRCommon_RegionEditorController', regionEditorController);
})(appControllers);
