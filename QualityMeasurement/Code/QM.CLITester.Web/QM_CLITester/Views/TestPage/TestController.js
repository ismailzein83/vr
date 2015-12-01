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
            $scope.isGettingData = false;
            $scope.selectedCountry;
        }

        function load() {
            getCountriesInfo();
        }

        function getCountriesInfo() {
            return QM_CLITester_TestCall.GetCountries().then(function (response) {
                $scope.countries.length = 0;
                angular.forEach(response, function (itm) {
                    $scope.countries.push(itm);
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

                console.log($scope.breakouts);
            });
        }

        function addNewTestCall() {
            QM_CLITester_TestCall.GetCountries();
        }

    }

    appControllers.controller('QM_CLITester_TestController', testController);
})(appControllers);