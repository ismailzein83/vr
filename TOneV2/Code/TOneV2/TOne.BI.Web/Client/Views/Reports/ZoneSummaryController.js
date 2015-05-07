/// <reference path="../../../../Templates/Grid/HeaderTemplate.html" />
/// <reference path="../../../../Templates/Grid/HeaderTemplate.html" />
appControllers.controller('ZoneSummaryController',
    function ZoneSummaryController($scope, $location, $routeParams, $interval, BIAPIService, BIUtilitiesService, BITimeDimensionTypeEnum, BIEntityTypeEnum, BIMeasureTypeEnum) {
        var measureTypes = [BIMeasureTypeEnum.DurationInMinutes, BIMeasureTypeEnum.Sale, BIMeasureTypeEnum.Cost, BIMeasureTypeEnum.Profit];
        var gridApi;

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

            $scope.measureTypes = measureTypes;

            $scope.data = [];
          
        }
        
        function defineScopeMethods() {

            $scope.updateReport = function () {
                getData();
            };

            $scope.toggleHeader = function (e) {
                $('.header-custom').removeClass("active-header");
                angular.element(e.currentTarget).addClass("active-header");
                // console.log(e);
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
            
            var measureTypeValues = [];
            angular.forEach(measureTypes, function (measureType) {
                measureTypeValues.push(measureType.value);
            });
            $scope.isGettingData = true;
            BIAPIService.GetEntityMeasuresValues(BIEntityTypeEnum.SaleZone.value, $scope.zoneId, $scope.selectedTimeDimensionType.value,
                $scope.fromDate, $scope.toDate, measureTypeValues)
            .then(function (response) {
                $scope.data.length = 0;
                BIUtilitiesService.fillDateTimeProperties(response, $scope.selectedTimeDimensionType.value, $scope.fromDate, $scope.toDate, true);

                angular.forEach(response, function (itm) {
                    $scope.data.push(itm);
                });
            }).finally(function () {
                $scope.isGettingData = false;
            });
        }

       

    });