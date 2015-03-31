﻿appControllers.controller('ZoneDetailsController',
    function ZoneDetailsController($scope, $routeParams, BIAPIService, BIUtilitiesService, BITimeDimensionTypeEnum, BIEntityTypeEnum, BIMeasureTypeEnum) {

        var zoneId;
        var maxTimeDimension = 0;
        var currentDimensionValue;

        defineScopeObjects();
        defineScopeMethods();
        load();
        function defineScopeObjects() {
            zoneId = $routeParams.zoneId;
              $scope.zoneName = $routeParams.zoneName;
            $scope.testModel = 'ZoneDashboardController: ' + zoneId;
            $scope.timeDimensionTypesOption = {
                datasource: []
            };

            for (prop in BITimeDimensionTypeEnum) {
                var timeDimensionValue = BITimeDimensionTypeEnum[prop].value;
                var obj = {
                    name: BITimeDimensionTypeEnum[prop].description,
                    value: timeDimensionValue
                };
                $scope.timeDimensionTypesOption.datasource.push(obj);
                if (obj.value == BITimeDimensionTypeEnum.Daily.value)
                    $scope.timeDimensionTypesOption.lastselectedvalue = obj;

                if (timeDimensionValue > maxTimeDimension)
                    maxTimeDimension = timeDimensionValue;
            }
            $scope.fromDate = '2012-01-02';
            $scope.toDate = '2012-04-28';


            
            
            $scope.gridOptionsZoneDetails = {};
            defineGrid($scope.gridOptionsZoneDetails, $scope.timeDimensionTypesOption.lastselectedvalue.value);

        }

        function defineGrid(gridOption) {
            
            gridOption.enableHorizontalScrollbar = 0;
            gridOption.enableVerticalScrollbar = 0;
            gridOption.infiniteScrollPercentage = 20;
            gridOption.enableFiltering = false;
            gridOption.saveFocus = false;
            gridOption.saveScroll = true;
            gridOption.enableColumnResizing = true;
            gridOption.enableSorting = false;
            gridOption.data = [];

            gridOption.getHeight = function () {
                var rowHeight = 30; // your row height
                var headerHeight = 30; // your header height
                height = (gridOption.data.length * rowHeight + headerHeight);
                if (gridOption.subGridOptions != undefined)
                    height += gridOption.subGridOptions.getHeight();
                return height;
            };

            if (gridOption.timeDimensionValue == undefined || (gridOption.timeDimensionValue + 1) < maxTimeDimension)
            {
                gridOption.expandableRowTemplate = '/Client/Templates/Grid/ExpandableRowGridTemplate.html';
                //gridOption.expandableRowHeight = 150;
                gridOption.onRegisterApi = function (gridApi) {
                    //  gridApi = api;
                    gridApi.expandable.on.rowExpandedStateChanged($scope, function (row) {
                      
                        if (row.isExpanded) {
                            
                            row.entity.subGridOptions = {};
                            
                            defineGrid(row.entity.subGridOptions);
                            
                           
                            var toDate = BIUtilitiesService.getNextDate(gridOption.timeDimensionValue, row.entity);
                            console.log('from date ' + row.entity.Time + ' to date ' + toDate);
                            getData(row.entity.subGridOptions, gridOption.timeDimensionValue + 1, row.entity.Time, toDate);

                        }
                    });
                };
            }
            
            gridOption.columnDefs = [];
            var timeColumn = {
                name: 'Time',
                headerCellTemplate: '/Client/Templates/Grid/HeaderTemplate.html',
                enableColumnMenu: false,
                field: 'dateTimeValue'
            };
            gridOption.columnDefs.push(timeColumn);

            var valColumnIndex = 0;
            for (var prop in BIMeasureTypeEnum) {
                var colDef = {
                    name: BIMeasureTypeEnum[prop].description,
                    headerCellTemplate: '/Client/Templates/Grid/HeaderTemplate.html',
                    enableColumnMenu: false,
                    field: 'Values[' + valColumnIndex++ + ']',
                    cellFilter: "number:2"
                };
                gridOption.columnDefs.push(colDef);
            }

            var progressColumn = {
                name: 'Progress (ex)',
                headerCellTemplate: '/Client/Templates/Grid/HeaderTemplate.html',
                cellTemplate: '/Client/Templates/Grid/ProgressCellTemplate.html',
                field: 'random',
                enableColumnMenu: false,
                width: 150,
                cellFilter: "number:2"
            };
            gridOption.columnDefs.push(progressColumn);
        }

        function defineScopeMethods() {

            $scope.updateReport = function () {
                getData();
            };

            $scope.selectedTimeDimensionChanged = function () {
                $scope.updateReport();
            };

            $scope.toggleHeader = function (e) {
                $('.header-custom').removeClass("active-header");
                angular.element(e.currentTarget).addClass("active-header");
               // console.log(e);
            };
            $scope.randomNumber = function (e) {
               // $scope.r =;
                return Math.round(Math.random() * 100) + 1;
            };

            $scope.getGridHeight = function (gridOptions) {
                return {
                    height: gridOptions.getHeight() + "px"
                };
            };

        }

        function load() {
            
            getData();
            //$interval(function () {
            //    $scope.$apply(function () {
            //        $scope.gridOptionsZoneDetails.gridStyle = { height: $scope.gridOptionsZoneDetails.getHeight() + "px" };
            //    });
            //}, 1000)
        }

        function getData(gridOption, timeDimensionValue, fromDate, toDate) {
            if (gridOption == undefined)
                gridOption = $scope.gridOptionsZoneDetails;
            if (timeDimensionValue == undefined)
                timeDimensionValue = $scope.timeDimensionTypesOption.lastselectedvalue.value;
            if (fromDate == undefined)
                fromDate = $scope.fromDate;
            if (toDate == undefined)
                toDate = $scope.toDate;

            gridOption.data.length = 0;
            
            gridOption.timeDimensionValue = timeDimensionValue;
            var measureTypes = [];
            for (var prop in BIMeasureTypeEnum) {
                measureTypes.push(BIMeasureTypeEnum[prop].value);
            }
            BIAPIService.GetEntityMeasuresValues(BIEntityTypeEnum.SaleZone.value, zoneId, timeDimensionValue, fromDate, toDate, measureTypes)
            .then(function (response) {
                BIUtilitiesService.fillDateTimeProperties(response, timeDimensionValue, fromDate, toDate, true);

                angular.forEach(response, function (itm) {
                    itm.random = Math.round(Math.random() * 100) + 1;
                    gridOption.data.push(itm);
                });

                
                
            });
        }



    });