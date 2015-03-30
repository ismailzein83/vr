appControllers.controller('ZoneDetailsController',
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
            $scope.data = [];
            $scope.columns = ["Total Duration", "Sale", "Cost", "Profit", "Successful Attempts", "ACD", "PDD"];

            $scope.gridOptionsZoneDetails = {
                enableHorizontalScrollbar: 0,
                enableVerticalScrollbar: 2,
                infiniteScrollPercentage: 20,
                enableFiltering: false,
                saveFocus: false,
                saveScroll: true,
                enableColumnResizing: true,
                enableSorting: false,

            };
            $scope.gridOptionsZoneDetails.columnDefs = [];
            $scope.gridOptionsZoneDetails.columnDefs[$scope.gridOptionsZoneDetails.columnDefs.length] = {
                name: 'Time',
                headerCellTemplate: template,
                enableColumnMenu: false,
                cellTemplate: '<div class="ui-grid-cell-contents" >{{row.entity.TimeGroupName}} <span ng-hide="row.entity.TimeGroupName == null">-</span> {{row.entity.TimeValue}}  </div>'
            }
            $scope.gridOptionsZoneDetails.columnDefs[$scope.gridOptionsZoneDetails.columnDefs.length] = {
                name: 'Total Duration',
                headerCellTemplate: template,
                enableColumnMenu: false,
                field: 'Values[0]',
                cellFilter: "number:2"
            }
            $scope.gridOptionsZoneDetails.columnDefs[$scope.gridOptionsZoneDetails.columnDefs.length] = {
                name: 'Sale',
                headerCellTemplate: template,
                enableColumnMenu: false,
                field: 'Values[1]',
                cellFilter: "number:2"
            }
            $scope.gridOptionsZoneDetails.columnDefs[$scope.gridOptionsZoneDetails.columnDefs.length] = {
                name: 'Cost',
                headerCellTemplate: template,
               
                enableColumnMenu: false,
                field: 'Values[2]',
                cellFilter: "number:2"
            }
            $scope.gridOptionsZoneDetails.columnDefs[$scope.gridOptionsZoneDetails.columnDefs.length] = {
                name: 'Profit',
                headerCellTemplate: template,
                cellTemplate: '<div class="ui-grid-cell-contents" ><div class="progress progress-custom">'
                              + '<div class="progress-bar progress-bar-success progress-bar-striped active" role="progressbar" aria-valuemin="0" aria-valuemax="100" ng-style={width:row.entity.random}>'
                               + '<span class="sr-only">{{row.entity.Values[3]}} % </span>'
                              + '</div>'
                            + '</div><span class="span-summary"> {{row.entity.random | number:2 }} % </span></div> ',
                
                enableColumnMenu: false,
                field: 'Values[3]',
                width:150,
                cellFilter: "number:2"
            }
            $scope.gridOptionsZoneDetails.columnDefs[$scope.gridOptionsZoneDetails.columnDefs.length] = {
                name: 'Successful Attempts',
                headerCellTemplate: template,
                enableColumnMenu: false,
                field: 'Values[4]',
                cellFilter: "number:2"

            }
            $scope.gridOptionsZoneDetails.columnDefs[$scope.gridOptionsZoneDetails.columnDefs.length] = {
                name: 'ACD',
                headerCellTemplate: template,
                enableColumnMenu: false,
                field: 'Values[5]',
                cellFilter: "number:2"

            }
            $scope.gridOptionsZoneDetails.columnDefs[$scope.gridOptionsZoneDetails.columnDefs.length] = {
                name: 'PDD',
                headerCellTemplate: template,
                enableColumnMenu: false,
                field: 'Values[6]',
                cellFilter: "number:2"

            }
           

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
            $scope.selectedTimeDimensionType = $scope.timeDimensionTypes[0];
            getData();
        }

        function getData() {
            $scope.data.length = 0;
            var measureTypes = [0, 1, 2, 3, 4, 5, 6];
            BIAPIService.GetEntityMeasuresValues(0, zoneId, $scope.selectedTimeDimensionType.value,
                $scope.selectedTimeDimensionType.fromDate, $scope.selectedTimeDimensionType.toDate, measureTypes)
            .then(function (response) {
                for (var i = 0 ; i < response.length ; i++) {
                    response[i].random = Math.round(Math.random() * 100) + 1
                }
               
                $scope.gridOptionsZoneDetails.data = response;//.splice(0,10);

                angular.forEach(response, function (itm) {
                    $scope.data.push(itm);
                });
            });
        }



    });