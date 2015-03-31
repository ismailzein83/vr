/// <reference path="../../../../Templates/Grid/HeaderTemplate.html" />
/// <reference path="../../../../Templates/Grid/HeaderTemplate.html" />
appControllers.controller('ZoneSummaryController',
    function ZoneSummaryController($scope, $location, $routeParams, $interval, BIAPIService, BIUtilitiesService, BITimeDimensionTypeEnum, BIEntityTypeEnum, BIMeasureTypeEnum) {
        var measureTypes = [BIMeasureTypeEnum.DurationInMinutes, BIMeasureTypeEnum.Sale, BIMeasureTypeEnum.Cost, BIMeasureTypeEnum.Profit];

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


            $scope.gridOptionsZoneDetails = {
                enableHorizontalScrollbar: 0,
                enableVerticalScrollbar: 2,
                infiniteScrollPercentage: 20,
                enableFiltering: false,
                saveFocus: false,
                saveScroll: true,
                enableColumnResizing: true,
                enableSorting: false,
                data: [],
                onRegisterApi: function (gridApi) {
                    $scope.gridApi = gridApi;

                    // call resize every 200 ms for 2 s after modal finishes opening - usually only necessary on a bootstrap modal
                    $interval(function () {
                        $scope.gridApi.core.handleWindowResize();
                    }, 10, 500);
                }
            };
            $scope.gridOptionsZoneDetails.columnDefs = [];
            var timeColumn = {
                name: 'Time',
                headerCellTemplate: '/Client/Templates/Grid/HeaderTemplate.html',//template,
                enableColumnMenu: false,
                field: 'dateTimeValue'
            };
            $scope.gridOptionsZoneDetails.columnDefs.push(timeColumn);
            
            var valColumnIndex = 0;
            angular.forEach(measureTypes, function (measureType) {
                var colDef = {
                    name: measureType.description,
                    headerCellTemplate: '/Client/Templates/Grid/HeaderTemplate.html',//template,
                    enableColumnMenu: false,
                    field: 'Values[' + valColumnIndex++ + ']',
                    cellFilter: "number:2"
                };
                $scope.gridOptionsZoneDetails.columnDefs.push(colDef);
            });
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
            $scope.gridOptionsZoneDetails.data.length = 0;
            var measureTypeValues = [];
            angular.forEach(measureTypes, function (measureType) {
                measureTypeValues.push(measureType.value);
            });
            BIAPIService.GetEntityMeasuresValues(BIEntityTypeEnum.SaleZone.value, $scope.zoneId, $scope.selectedTimeDimensionType.value,
                $scope.fromDate, $scope.toDate, measureTypeValues)
            .then(function (response) {
                BIUtilitiesService.fillDateTimeProperties(response, $scope.selectedTimeDimensionType.value, $scope.fromDate, $scope.toDate, true);

                angular.forEach(response, function (itm) {
                    $scope.gridOptionsZoneDetails.data.push(itm);
                });
            });
        }

       

    });