(function (appControllers) {

    "use strict";

    testController.$inject = ['$scope', 'Qm_CliTester_TestCallAPIService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService'];

    function testController($scope, Qm_CliTester_TestCallAPIService, VRNotificationService, UtilsService, VRUIUtilsService) {
        var gridAPI;

        var profileDirectiveAPI;
        var profileReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var supplierDirectiveAPI;
        var supplierReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        
        var zoneDirectiveAPI;
        var zoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var countryDirectiveAPI;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.quantity = 1;
            $scope.addNewTestCall = addNewTestCall;
            $scope.hasAddTestCallPermission = function () {
                return Qm_CliTester_TestCallAPIService.HasAddTestCallPermission();
            };

            $scope.countries = [];
            $scope.zones = [];
            $scope.suppliers = [];

            $scope.selectedProfile;
            $scope.selectedSupplier = [];
            $scope.selectedCountry;
            $scope.selectedZone = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid();
            }

            $scope.onProfileDirectiveReady = function (api) {
                profileDirectiveAPI = api;
                profileReadyPromiseDeferred.resolve();
            }

            $scope.onSupplierDirectiveReady = function (api) {
                supplierDirectiveAPI = api;
                supplierReadyPromiseDeferred.resolve();
            }

            $scope.onZoneDirectiveReady = function (api) {
                zoneDirectiveAPI = api;
                zoneReadyPromiseDeferred.resolve();
            }

            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveAPI = api;
                countryReadyPromiseDeferred.resolve();
            }


            $scope.onCountrySelectItem = function (selectedItem) {
                if (selectedItem != undefined) {
                    var setLoader = function (value) { $scope.isLoadingZonesSelector = value };
                    var payload = {
                        filter: {
                            CountryId: [selectedItem.CountryId]
                        }
                    }

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, zoneDirectiveAPI, payload, setLoader);
                }
            }
        }

        function load() {
            $scope.isGettingData = true;
            UtilsService.waitMultipleAsyncOperations([loadCountries, loadSuppliers, loadProfiles]).then(function () {
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isGettingData = false;
            });
        }
    

        function loadProfiles() {
            var profileLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            supplierReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload;

                    VRUIUtilsService.callDirectiveLoad(profileDirectiveAPI, directivePayload, profileLoadPromiseDeferred);
                });
            return profileLoadPromiseDeferred.promise;
        }

        function loadSuppliers() {
            var supplierLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            supplierReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload;

                    VRUIUtilsService.callDirectiveLoad(supplierDirectiveAPI, directivePayload, supplierLoadPromiseDeferred);
                });
            return supplierLoadPromiseDeferred.promise;
        }

        function loadCountries() {
            var countryLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            countryReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload;

                    VRUIUtilsService.callDirectiveLoad(countryDirectiveAPI, directivePayload, countryLoadPromiseDeferred);
                });
            return countryLoadPromiseDeferred.promise;
        }

        function buildTestCallObjFromScope() {
            var obj = {
                SuppliersIds: UtilsService.getPropValuesFromArray($scope.selectedSupplier, "SupplierId"),
                SuppliersSourceIds:"",
                CountryIds: [$scope.selectedCountry.CountryId],
                ZoneIds: UtilsService.getPropValuesFromArray($scope.selectedZone, "ZoneId"),
                ZoneSourceId:"",
                ProfileID: $scope.selectedProfile.ProfileId,
                UserId: 0,
                ScheduleId: 0,
                Quantity: $scope.quantity
            };
            return obj;
        }

        function addNewTestCall() {
            var testCallObject = buildTestCallObjFromScope();
            
            return Qm_CliTester_TestCallAPIService.AddNewTestCall(testCallObject)
            .then(function (response) {
                //$scope.selectedSupplier = [];
                //$scope.selectedCountry = undefined;
                //$scope.selectedProfile = undefined;
                //$scope.selectedZone = undefined;
                if (VRNotificationService.notifyOnItemAdded("Test Call", response, "Name")) {
                    if ($scope.onTestCallAdded != undefined)
                        $scope.onTestCallAdded(response.InsertedObject);
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
    }
    appControllers.controller('QM_CLITester_TestController', testController);
})(appControllers);