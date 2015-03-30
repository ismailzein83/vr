appControllers.controller('ZoneSummaryController',
    function ZoneSummaryController($scope, $location, $routeParams, BIAPIService, BIUtilitiesService, BITimeDimensionTypeEnum, BIEntityTypeEnum) {

        defineScopeObjects();
        defineScopeMethods();
        load();

        function defineScopeObjects() {

          //  $scope.zoneName = $routeParams.ZoneName;
            $scope.testModel = 'ZoneDetailsController: ' + $scope.zoneId;

            $scope.timeDimensionTypes = [];
            for (prop in BITimeDimensionTypeEnum) {
                var obj = {
                    name: BITimeDimensionTypeEnum[prop].description,
                    value: BITimeDimensionTypeEnum[prop].value
                };
                $scope.timeDimensionTypes.push(obj);
                if (obj.value == BITimeDimensionTypeEnum.Daily.value)
                    $scope.selectedTimeDimensionType = obj;
            }

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
            getData();
        }

        function getData() {
            $scope.data.length = 0;
            var measureTypes = [0, 1, 2, 3];
            BIAPIService.GetEntityMeasuresValues(BIEntityTypeEnum.SaleZone.value, $scope.zoneId, $scope.selectedTimeDimensionType.value,
                $scope.fromDate, $scope.toDate, measureTypes)
            .then(function (response) {
                BIUtilitiesService.fillDateTimeProperties(response, $scope.selectedTimeDimensionType.value, $scope.fromDate, $scope.toDate, true);
                angular.forEach(response, function (itm) {
                    $scope.data.push(itm);
                });
            });
        }

       

    });