(function (appControllers) {

    "use strict";

    testController.$inject = ['$scope', 'QM_CLITester_TestCall'];

    function testController($scope, QM_CLITester_TestCall) {


        defineScope();
        load();

        function defineScope() {
            $scope.addNewTestCall = addNewTestCall;
            $scope.countries = [];
            $scope.breakouts = [];
            $scope.suppliers = [];
            $scope.isGettingData = false;
            $scope.selectedCountry;
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

        function addNewTestCall() {
            return QM_CLITester_TestCall.GetTestCall($scope.selectedCountry.Id, $scope.selectedBreakout.Id, $scope.selectedSupplier.Id).then(function (response) {
                console.log(response);
                //angular.forEach(response, function (itm) {


                //});

            });

        }

    }

    appControllers.controller('QM_CLITester_TestController', testController);
})(appControllers);