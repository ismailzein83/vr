(function (appControllers) {

    "use strict";

    testController.$inject = ['$scope', 'Qm_CliTester_TestCallAPIService', 'VRNotificationService', 'UtilsService', 'QM_BE_SupplierAPIService', 'VRUIUtilsService'];

    function testController($scope, Qm_CliTester_TestCallAPIService, VRNotificationService, UtilsService, QM_BE_SupplierAPIService, VRUIUtilsService) {
        var gridAPI;

        var supplierDirectiveAPI;
        var supplierReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        
        defineScope();
        load();

        function defineScope() {
            $scope.addNewTestCall = addNewTestCall;
            $scope.countries = [];
            $scope.breakouts = [];
            $scope.suppliers = [];
           
            $scope.selectedSupplier = [];
            $scope.selectedCountry;
            $scope.selectedBreakout;

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid();
            }

            $scope.onSupplierDirectiveReady = function (api) {
                supplierDirectiveAPI = api;
                supplierReadyPromiseDeferred.resolve();
            }

        }

        function load() {
            $scope.isGettingData = true;
            UtilsService.waitMultipleAsyncOperations([getCountriesInfo, getSuppliersInfo]).then(function () {
            }).finally(function () {
                $scope.isGettingData = false;
            });
        }


        function getCountriesInfo() {
            return Qm_CliTester_TestCallAPIService.GetCountries().then(function (response) {
                $scope.countries.length = 0;
                angular.forEach(response, function (itm) {
                    $scope.countries.push(itm);
                });
            });
        }
    
        function getSuppliersInfo() {
            var supplierLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            supplierReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload;

                    VRUIUtilsService.callDirectiveLoad(supplierDirectiveAPI, directivePayload, supplierLoadPromiseDeferred);
                });
            return supplierLoadPromiseDeferred.promise;
        }

        $scope.previewBreakouts = function () {
            if ($scope.selectedCountry != undefined) {
                getBreakoutsInfo($scope.selectedCountry.Id);
            }
        }

        function getBreakoutsInfo(selectedCountryId) {
            $scope.isLoading = true;
            return Qm_CliTester_TestCallAPIService.GetBreakouts(selectedCountryId).then(function(response) {

                $scope.breakouts.length = 0;
                angular.forEach(response, function(itm) {
                    $scope.breakouts.push(itm);
                });
            }).finally(function() {
                $scope.isLoading = false;
            });
        }


        function buildTestCallObjFromScope() {
            console.log($scope.selectedSupplier);
            var obj = {
                TestCallId: 0,
                SupplierID: UtilsService.getPropValuesFromArray($scope.selectedSupplier, "SupplierId"),
                CountryID: $scope.selectedCountry.Id,
                ZoneID: $scope.selectedBreakout.Id
            };
            return obj;
        }

        function addNewTestCall() {
            var testCallObject = buildTestCallObjFromScope();
            
            return Qm_CliTester_TestCallAPIService.AddNewTestCall(testCallObject)
            .then(function (response) {
                $scope.selectedSupplier = [];
                $scope.selectedCountry = undefined;
                $scope.selectedBreakout = undefined;
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