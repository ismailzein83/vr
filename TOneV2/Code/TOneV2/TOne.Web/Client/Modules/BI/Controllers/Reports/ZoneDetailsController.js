﻿appControllers.controller('ZoneDetailsController',
    function ZoneDetailsController($scope, $routeParams, BIAPIService) {

        defineScopeObjects();
        defineScopeMethods();
        load();
        var zoneId;
        function defineScopeObjects() {
            zoneId = $routeParams.zoneId;
              $scope.zoneName = $routeParams.zoneName;
            $scope.testModel = 'ZoneDashboardController: ' + zoneId;
            $scope.timeDimensionTypes = [{ name: "Daily", value: 0, fromDate: '2012-1-2', toDate: '2012-2-28' },
           { name: "Weekly", value: 1, fromDate: '2012-1-2', toDate: '2012-04-28' },
           { name: "Monthly", value: 2, fromDate: '2012-1-2', toDate: '2013-12-31' },
           { name: "Yearly", value: 3, fromDate: '2012-1-2', toDate: '2014-1-1' }, ];

            $scope.data = [];
            $scope.columns = ["Total Duration", "Sale", "Cost", "Profit", "Successful Attempts", "ACD", "PDD"];
        }

        function defineScopeMethods() {

            $scope.updateReport = function () {
                getData();
            };
        }

        function load() {
            $scope.selectedTimeDimensionType = $scope.timeDimensionTypes[0];
            getData();
        }

        function getData() {
            $scope.data.length = 0;
            var measureTypes = [0, 1, 2, 3, 4, 5, 6];
            BIAPIService.GetEntityMeasuresValues(0, zoneId, $scope.selectedTimeDimensionType.value,
                $scope.selectedTimeDimensionType.fromDate, $scope.selectedTimeDimensionType.toDate, measureTypes)
            .then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.data.push(itm);
                });
            });
        }



    });