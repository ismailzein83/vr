(function (appControllers) {

    "use strict";

    carrierProfileManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_BE_CarrierProfileService', 'WhS_BE_CarrierProfileAPIService'];

    function carrierProfileManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, WhS_BE_CarrierProfileService, WhS_BE_CarrierProfileAPIService) {

        var filterSettingsData;
        var gridSettingsData;
        var gridAPI;
        var gridAPIReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var actionBarAPI;
        var actionBarReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.hadAddCarrierProfilePermission = function () {
                return WhS_BE_CarrierProfileAPIService.HasAddCarrierProfilePermission();
            };

            $scope.onActionBarSettingsReady = function (api) {
                actionBarAPI = api;
                actionBarReadyPromiseDeferred.resolve();
            };

            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPIReadyPromiseDeferred.resolve();
            };

            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
            };

            $scope.AddNewCarrierProfile = AddNewCarrierProfile;


        }

        function load() {
            $scope.isLoading = true;
            loadAllControls()
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isLoading = false;
            });
        }

        function loadAllControls() {
            
            var rootPromiseNode = {
                promises: [actionBarReadyPromiseDeferred.promise],
                getChildNode: function () {
                    var loadActionBarPromise = actionBarAPI.load(buildactionBarPayload()).then(function (settingsData) {
                        filterSettingsData = settingsData.getItemByUniqueName("WhSCarrierProfileFilter");
                        gridSettingsData = settingsData.getItemByUniqueName("WhSCarrierProfileGrid");
                    });
                    return {
                        promises: [loadActionBarPromise],
                        getChildNode: function () {
                            var loadFilterPromise = UtilsService.waitMultipleAsyncOperations([loadCountries, loadStaticData]);
                            return {
                                promises: [loadFilterPromise, gridAPIReadyPromiseDeferred.promise],
                                getChildNode: function () {
                                    gridAPI.setPersonalizationItem(gridSettingsData);
                                    var loadGridPromise = gridAPI.loadGrid(getFilterObject());
                                    return {
                                        promises: [loadGridPromise]
                                    };
                                }
                            };
                        }
                    };
                }
            };
            return UtilsService.waitPromiseNode(rootPromiseNode);
        }

        function loadCountries() {
            var loadCountryPromiseDeferred = UtilsService.createPromiseDeferred();
            countryReadyPromiseDeferred.promise.then(function () {
                var payload;

                if (filterSettingsData != undefined)
                    payload = {
                        selectedIds: filterSettingsData.CountryIds
                    };
                VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, payload, loadCountryPromiseDeferred);
            });

            return loadCountryPromiseDeferred.promise;
        }

        function loadStaticData() {
            if (filterSettingsData == undefined)
                return;
            $scope.name = filterSettingsData.Name;
            $scope.company = filterSettingsData.Company;

        }

        function AddNewCarrierProfile() {
            var onCarrierProfileAdded = function (carrierProfileObj) {
                gridAPI.onCarrierProfileAdded(carrierProfileObj);
            };

            WhS_BE_CarrierProfileService.addCarrierProfile(onCarrierProfileAdded);
        }

        function getFilterObject() {
            var data = {
                Name: $scope.name,
                Company: $scope.company,
                CountriesIds: countryDirectiveApi.getSelectedIds()
            };
            return data;
        }

        function buildactionBarPayload() {
            var uniqueKeys = [{
                EntityUniqueName: "WhSCarrierProfileFilter",
                DisplayName: "Filter"
            }, {
                EntityUniqueName: "WhSCarrierProfileGrid",
                DisplayName: "Grid"
            }];
            var context = {
                getPersonalizationItems: function () {
                    var items = [];
                    if(countryDirectiveApi.getSelectedIds() || $scope.name || $scope.company){
                        items.push({
                            EntityUniqueName: "WhSCarrierProfileFilter",
                            ExtendedSetting : {
                                "$type": "TOne.WhS.BusinessEntity.Business.CarrierProfileFilterPersonalizationExtendedSetting, TOne.WhS.BusinessEntity.Business",
                                Name: $scope.name ? $scope.name : null,
                                Company: $scope.company ? $scope.company : null,
                                CountryIds: countryDirectiveApi.getSelectedIds() ? countryDirectiveApi.getSelectedIds() : null
                            }
                        });
                    }
                    if (gridAPI && gridAPI.getPersonalizationItem() != null) {
                        items.push({
                            EntityUniqueName: "WhSCarrierProfileGrid",
                            ExtendedSetting : {
                                "$type": "TOne.WhS.BusinessEntity.Business.CarrierProfileGridPersonalizationExtendedSetting, TOne.WhS.BusinessEntity.Business",
                                BaseGridPersonalization: gridAPI.getPersonalizationItem()
                            }
                        });
                    }
                    return items;
                }
            };
            return {
                uniqueKeys: uniqueKeys,
                context: context
            };
        }
    }

    appControllers.controller('WhS_BE_CarrierProfileManagementController', carrierProfileManagementController);
})(appControllers);