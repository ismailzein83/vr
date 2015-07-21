appControllers.controller('BI_ZoneDetailsController',
    function ZoneDetailsController($scope, VRNavigationService, VRNotificationService, uiGridConstants, BIAPIService, BIUtilitiesService, TimeDimensionTypeEnum, BIEntityTypeEnum, BIMeasureTypeEnum, ZonesService) {

        var zoneId;
        var maxTimeDimension = 0;
        var currentDimensionValue;
        var gridMainAPI;

        loadParameters();
        defineScopeObjects();
        defineScopeMethods();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            zoneId = parameters.zoneId;
            $scope.zoneName = parameters.zoneName;
        }

        function defineScopeObjects() {
            
            $scope.timeDimensionTypesOption = {
                datasource: []
            };

            $scope.zonesOption= {
                selectedvalues: [],
                datasource: []
            };

            for (prop in TimeDimensionTypeEnum) {
                var timeDimensionValue = TimeDimensionTypeEnum[prop].value;
                var obj = {
                    name: TimeDimensionTypeEnum[prop].description,
                    value: timeDimensionValue
                };
                $scope.timeDimensionTypesOption.datasource.push(obj);
                if (obj.value == TimeDimensionTypeEnum.Daily.value)
                    $scope.timeDimensionTypesOption.lastselectedvalue = obj;

                if (timeDimensionValue > maxTimeDimension)
                    maxTimeDimension = timeDimensionValue;
            }
            $scope.fromDate = '2015-02-15';
            $scope.toDate = '2015-04-15';


            
            
            $scope.gridOptionsZoneDetails = {};
            defineGrid($scope.gridOptionsZoneDetails, $scope.timeDimensionTypesOption.lastselectedvalue.value);

        }

        function defineGrid(gridOption) {

            gridOption.getHeight = function () {
                var rowHeight = 30; // your row height
                var headerHeight = 30; // your header height
                height = (gridOption.data.length * rowHeight + headerHeight);                
                if (gridOption.subGridOptions != undefined) {
                    height += gridOption.subGridOptions.getHeight();
                }

                return height;
            };

            gridOption.enableHorizontalScrollbar = 0;
            gridOption.enableVerticalScrollbar = 2;
            //gridOption.minRowsToShow = 30;
            gridOption.infiniteScrollPercentage = 20;
            gridOption.enableFiltering = false;
            gridOption.saveFocus = false;
            gridOption.saveScroll = true;
            gridOption.enableColumnResizing = true;
            gridOption.enableSorting = false;
            gridOption.data = [];

            

            if (gridOption.timeDimensionValue == undefined || gridOption.timeDimensionValue  < maxTimeDimension)
            {
                gridOption.expandableRowTemplate = '/Client/Templates/Grid/ExpandableRowGridTemplate.html';
                //gridOption.expandableRowHeight = 150;
                gridOption.onRegisterApi = function (gridApi) {
                    if (gridMainAPI == undefined)
                        gridMainAPI = gridApi;
                    gridApi.expandable.on.rowExpandedStateChanged($scope, function (row) {
                      
                        if (row.isExpanded) {
                            
                            row.entity.subGridOptions = {};
                            row.entity.getGridHeight = $scope.getGridHeight;
                            gridOption.subGridOptions = row.entity.subGridOptions;
                            gridOption.subGridOptions.timeDimensionValue = gridOption.timeDimensionValue + 1;
                            defineGrid(row.entity.subGridOptions);
                            
                           
                            var toDate = BIUtilitiesService.getNextDate(gridOption.timeDimensionValue, row.entity);

                            getData(row.entity.subGridOptions, gridOption.subGridOptions.timeDimensionValue, row.entity.Time, toDate);

                        }
                    });
                };
            }
            var headerTemplate = '<div ng-class="{ \'sortable\': col.colDef.enableSorting }" class="header-custom" ng-click="col.colDef.onSort()" style="background-color: #829EBF;color:#FFF">'
    +'<div class="ui-grid-cell-contents" col-index="renderIndex">'
    +'    <span>'
      +'          <span ng-show="col.colDef.sortDirection==\'ASC\'">&uarr;</span>'
        +'        <span ng-show="col.colDef.sortDirection==\'DESC\'">&darr;</span>'
          +'  {{col.name}}'
           +' </span>'
        +'</div>'
        +'<div class="ui-grid-column-menu-button" ng-if="grid.options.enableColumnMenus && !col.isRowHeader  && col.colDef.enableColumnMenu !== false" class="ui-grid-column-menu-button" ng-click="toggleMenu($event)">'
         +'   <i class="ui-grid-icon-angle-down">&nbsp;</i>'
        +'</div>'
    +'</div>  ';
            gridOption.columnDefs = [];
            var timeColumn = {
                name: 'Time',
                headerCellTemplate: headerTemplate,
                enableColumnMenu: false,
                field: 'dateTimeValue'
            };
            gridOption.columnDefs.push(timeColumn);

            var valColumnIndex = 0;
            for (var prop in BIMeasureTypeEnum) {
                var colDef = {
                    name: BIMeasureTypeEnum[prop].description,
                    headerCellTemplate: headerTemplate,
                    enableColumnMenu: false,
                    field: 'Values[' + valColumnIndex++ + ']',
                    cellFilter: "number:2"
                };
                gridOption.columnDefs.push(colDef);
            }

            var progressColumn = {
                name: 'Progress (ex)',
                headerCellTemplate: headerTemplate,
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
                var height = gridOptions.getHeight();
                if (height > 1000)
                    height = 1000;
                
                return {
                    height: height + "px"
                };
            };

            $scope.searchZones = function (text) {
                return ZonesService.getSalesZones(text);
            }

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

                //if (gridOption == $scope.gridOptionsZoneDetails) {
                //    angular.element(document.getElementsByClassName('gridrouteMain')[0]).css('height', gridOption.getHeight() + 'px');
                //    gridMainAPI.core.notifyDataChange(uiGridConstants.dataChange.OPTIONS);
                //}
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }



    });