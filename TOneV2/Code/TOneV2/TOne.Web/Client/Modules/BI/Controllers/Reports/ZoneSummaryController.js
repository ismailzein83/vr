appControllers.controller('ZoneSummaryController',
    function ZoneSummaryController($scope, $location, $routeParams, BIAPIService) {

        defineScopeObjects();
        defineScopeMethods();
        load();

        function defineScopeObjects() {

          //  $scope.zoneName = $routeParams.ZoneName;
            $scope.testModel = 'ZoneDetailsController: ' + $scope.zoneId;
            $scope.timeDimensionTypes = [{ name: "Daily", value: 0, fromDate: '2012-1-2', toDate: '2012-2-28' },
           { name: "Weekly", value: 1, fromDate: '2012-1-2', toDate: '2012-04-28' },
           { name: "Monthly", value: 2, fromDate: '2012-1-2', toDate: '2013-12-31' },
           { name: "Yearly", value: 3, fromDate: '2012-1-2', toDate: '2014-1-1' }, ];

            $scope.data = [];
            $scope.columns = ["Total Duration", "Sale", "Cost", "Profit"];
        }
        
        function defineScopeMethods() {

            $scope.updateReport = function () {
                getData();
            };
            
            $scope.gotoDetails = function () {
                //$scope.$root.$apply(function () {
                $scope.$hide();
                    $location.path("/BI/ZoneDetails/" + $scope.zoneId + "/" + $scope.zoneName).replace();
                //});
            };
        }

        function load() {
            $scope.selectedTimeDimensionType = $scope.timeDimensionTypes[0];
            getData();
        }

        function getData() {
            $scope.data.length = 0;
            var measureTypes = [0, 1, 2, 3];
            BIAPIService.GetEntityMeasuresValues(0, $scope.zoneId, $scope.selectedTimeDimensionType.value,
                $scope.selectedTimeDimensionType.fromDate, $scope.selectedTimeDimensionType.toDate, measureTypes)
            .then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.data.push(itm);
                });
            });
        }

       

    });