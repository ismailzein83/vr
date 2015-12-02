(function (appControllers) {

    "use strict";

    testController.$inject = ['$scope', 'QM_CLITester_TestCall', 'VRNotificationService'];

    function testController($scope, QM_CLITester_TestCall, VRNotificationService) {


        defineScope();
        load();

        function defineScope() {
            $scope.addNewTestCall = addNewTestCall;
            $scope.countries = [];
            $scope.breakouts = [];
            $scope.suppliers = [];
            $scope.isGettingData = false;
            $scope.selectedCountry;
            $scope.selectedBreakout;
            $scope.selectedSupplier;
        }

        function load() {
            getCountriesInfo();
            getSuppliersInfo();
        }

        function getCountriesInfo() {
            return QM_CLITester_TestCall.GetCountries().then(function (response) {
                $scope.countries.length = 0;
                angular.forEach(response, function (itm) {
                    $scope.countries.push(itm);
                });
            });
        }

        function getSuppliersInfo() {
            return QM_CLITester_TestCall.GetSuppliers().then(function (response) {
                $scope.suppliers.length = 0;
                angular.forEach(response, function (itm) {
                    $scope.suppliers.push(itm);
                });
            });
        }

        $scope.previewBreakouts = function () {
            if ($scope.selectedCountry != undefined) {
                $scope.isGettingData = true;
                getBreakoutsInfo($scope.selectedCountry.Id);
            }
        }

        function getBreakoutsInfo(selectedCountryId) {
            return QM_CLITester_TestCall.GetBreakouts(selectedCountryId).then(function (response) {
                $scope.breakouts.length = 0;
                angular.forEach(response, function (itm) {
                    $scope.breakouts.push(itm);
                });
            });
        }


        function buildTestCallObjFromScope() {
            var obj = {
                SellingNumberPlanId: 0,
                SupplierID: $scope.selectedSupplier.Id,
                CountryID: $scope.selectedCountry.Id,
                ZoneID: $scope.selectedBreakout.Id
            };
            return obj;
        }

        function addNewTestCall() {
            var testCallObject = buildTestCallObjFromScope();
            return QM_CLITester_TestCall.AddNewTestCall(testCallObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Test Call", response, "Name")) {

                    if ($scope.onTestCallAdded != undefined)
                        $scope.onTestCallAdded(response.InsertedObject);

                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }


        //function addNewTestCall() {
        //    return QM_CLITester_TestCall.AddNewTestCall($scope.selectedCountry.Id, $scope.selectedBreakout.Id, $scope.selectedSupplier.Id).then(function (response) {
        //        console.log(response);
        //        //angular.forEach(response, function (itm) {


        //        //});

        //    });

        //}

    }

    appControllers.controller('QM_CLITester_TestController', testController);
})(appControllers);