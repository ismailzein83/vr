appControllers.controller('ZoneDetailsController',
    function ZoneDetailsController($scope, $routeParams, BIAPIService, BIUtilitiesService, BITimeDimensionTypeEnum, BIEntityTypeEnum, BIMeasureTypeEnum) {

        defineScopeObjects();
        defineScopeMethods();
        load();
        var zoneId;
        function defineScopeObjects() {
            zoneId = $routeParams.zoneId;
              $scope.zoneName = $routeParams.zoneName;
            $scope.testModel = 'ZoneDashboardController: ' + zoneId;
            $scope.timeDimensionTypesOption = {
                datasource: []
            };

            for (prop in BITimeDimensionTypeEnum) {
                var obj = {
                    name: BITimeDimensionTypeEnum[prop].description,
                    value: BITimeDimensionTypeEnum[prop].value
                };
                $scope.timeDimensionTypesOption.datasource.push(obj);
                if (obj.value == BITimeDimensionTypeEnum.Daily.value)
                    $scope.timeDimensionTypesOption.lastselectedvalue = obj;
            }
            $scope.fromDate = '2012-01-02';
            $scope.toDate = '2012-04-28';


            var template = '<div ng-class="{ \'sortable\': sortable }" class="header-custom"  ng-click=\"grid.appScope.toggleHeader($event)\" >' +
                  
                   '<div class="ui-grid-cell-contents" col-index="renderIndex">' +
                   '<span>' +
                   '{{col.displayName}}' +
                   '</span>' +
                   '</div>' +
                   '<div class="ui-grid-column-menu-button" ng-if="grid.options.enableColumnMenus && !col.isRowHeader  && col.colDef.enableColumnMenu !== false" class="ui-grid-column-menu-button" ng-click="toggleMenu($event)">' +
                   '<i class="ui-grid-icon-angle-down">&nbsp;</i>' +
                   '</div>' +
                   '</div>' +
                   '</div>' +
                   '</div>';

            $scope.gridOptionsZoneDetails = {
                enableHorizontalScrollbar: 0,
                enableVerticalScrollbar: 2,
                infiniteScrollPercentage: 20,
                enableFiltering: false,
                saveFocus: false,
                saveScroll: true,
                enableColumnResizing: true,
                enableSorting: false,
                data: []
            };
            $scope.gridOptionsZoneDetails.columnDefs = [];
            var timeColumn = {
                name: 'Time',
                headerCellTemplate: template,
                enableColumnMenu: false,
                cellTemplate: '<div class="ui-grid-cell-contents" >{{row.entity.dateTimeValue}}  </div>'
            };
            $scope.gridOptionsZoneDetails.columnDefs.push(timeColumn);

            var valColumnIndex = 0;
            for (var prop in BIMeasureTypeEnum)
            {
                var colDef = {
                    name: BIMeasureTypeEnum[prop].description,
                    headerCellTemplate: template,
                    enableColumnMenu: false,
                    field: 'Values['+ valColumnIndex++ +']',
                    cellFilter: "number:2"
                };
                $scope.gridOptionsZoneDetails.columnDefs.push(colDef);
            }

            var progressColumn = {
                name: 'Progress (ex)',
                headerCellTemplate: template,
                cellTemplate: '<div class="ui-grid-cell-contents" ><div class="progress progress-custom">'
                              + '<div class="progress-bar progress-bar-success progress-bar-striped active" role="progressbar" aria-valuemin="0" aria-valuemax="100" ng-style={width:row.entity.random}>'                               
                              + '</div>'
                            + '</div><span class="span-summary"> {{row.entity.random  }} % </span></div> ',

                enableColumnMenu: false,
                width: 150,
                cellFilter: "number:2"
            };
            $scope.gridOptionsZoneDetails.columnDefs.push(progressColumn);
           

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
            $scope.randomNumber = function (e) {
               // $scope.r =;
                return Math.round(Math.random() * 100) + 1;
            };
        }

        function load() {
            
            getData();
        }

        function getData() {
            $scope.gridOptionsZoneDetails.data.length = 0;
            var measureTypes = [];
            for (var prop in BIMeasureTypeEnum) {
                measureTypes.push(BIMeasureTypeEnum[prop].value);
            }
            BIAPIService.GetEntityMeasuresValues(BIEntityTypeEnum.SaleZone.value, zoneId, $scope.timeDimensionTypesOption.lastselectedvalue.value,
                $scope.fromDate, $scope.toDate, measureTypes)
            .then(function (response) {
                BIUtilitiesService.fillDateTimeProperties(response, $scope.timeDimensionTypesOption.lastselectedvalue.value, $scope.fromDate, $scope.toDate, true);

                angular.forEach(response, function (itm) {
                    itm.random = Math.round(Math.random() * 100) + 1;
                    $scope.gridOptionsZoneDetails.data.push(itm);
                });
            });
        }



    });