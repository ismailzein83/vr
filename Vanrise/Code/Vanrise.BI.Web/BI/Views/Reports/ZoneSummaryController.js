/// <reference path="../../../../Templates/Grid/HeaderTemplate.html" />
/// <reference path="../../../../Templates/Grid/HeaderTemplate.html" />
appControllers.controller('ZoneSummaryController',
    function ZoneSummaryController($scope, VRNavigationService, BIAPIService, BIUtilitiesService, TimeDimensionTypeEnum, BIEntityTypeEnum, BIMeasureTypeEnum) {
        var measureTypes = [BIMeasureTypeEnum.DurationInMinutes, BIMeasureTypeEnum.Sale, BIMeasureTypeEnum.Cost, BIMeasureTypeEnum.Profit];
        var gridApi;

        loadParameters();
        defineScopeObjects();
        defineScopeMethods();
        load();

        function loadParameters()
        {
            var parameters = VRNavigationService.getParameters($scope);
            $scope.zoneId = parameters.zoneId;
            $scope.fromDate = parameters.fromDate;
            $scope.toDate = parameters.toDate;
        }

        
        function defineScopeObjects() {
            

            $scope.timeDimensionTypes = [];
            for (prop in TimeDimensionTypeEnum) {
                var obj = {
                    name: TimeDimensionTypeEnum[prop].description,
                    value: TimeDimensionTypeEnum[prop].value
                };
                $scope.timeDimensionTypes.push(obj);
                if (obj.value == TimeDimensionTypeEnum.Daily.value)
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
                $scope.$hide();
                var parameters = {
                    zoneId: $scope.zoneId,
                    zoneName: $scope.zoneName
                };
                VRNavigationService.goto("/BI/Views/Reports/ZoneDetails", parameters);
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