/// <reference path="ZoneMonitorSettings.html" />
/// <reference path="ZoneMonitor.html" />
appControllers.controller('SwitchManagmentController',
    function ZoneMonitorController($scope, uiGridConstants, $q, SwitchManagmentAPIService) {




        $scope.switches = [];
        $scope.switchName = '';
        $scope.rowFrom = 0;
        $scope.rowTo = 20;
        $scope.SwitchsDataSource = [];




        function loadSwitches() {
            SwitchManagmentAPIService.GetFilteredSwitches($scope.switchName, $scope.rowFrom, $scope.rowTo).then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.switches.push(itm);
                });
            });
        }

        loadSwitches();
        $scope.GetSwitchsDataSource = function (asyncHandle) {
            SwitchManagmentAPIService.GetFilteredSwitches($scope.switchName, $scope.rowFrom, $scope.rowTo).then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.SwitchsDataSource.push(itm);
                });
            }).finally(function () {
                if (asyncHandle)
                    asyncHandle.operationDone();
            });;
        }
    });