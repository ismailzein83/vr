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
                cellTemplate: '<div class="ui-grid-cell-contents" >{{row.entity.TimeGroupName}} <span ng-hide="row.entity.TimeGroupName == null">-</span> {{row.entity.TimeValue}}  </div>'
            }
            $scope.gridOptionsZoneDetails.columnDefs[$scope.gridOptionsZoneDetails.columnDefs.length] = {
                name: 'Total Duration',
                field: 'Values[0]',
                cellFilter: "number:2"
            }
            $scope.gridOptionsZoneDetails.columnDefs[$scope.gridOptionsZoneDetails.columnDefs.length] = {
                name: 'Sale',
                field: 'Values[1]',
                cellFilter: "number:2"
            }
            $scope.gridOptionsZoneDetails.columnDefs[$scope.gridOptionsZoneDetails.columnDefs.length] = {
                name: 'Cost',
                field: 'Values[2]',
                cellFilter: "number:2"
            }
            $scope.gridOptionsZoneDetails.columnDefs[$scope.gridOptionsZoneDetails.columnDefs.length] = {
                name: 'Profit',
                field: 'Values[3]',
                cellFilter: "number:2"
            }
            $scope.gridOptionsZoneDetails.columnDefs[$scope.gridOptionsZoneDetails.columnDefs.length] = {
                name: 'Successful Attempts',
                field: 'Values[4]',
                cellFilter: "number:2"

            }
            $scope.gridOptionsZoneDetails.columnDefs[$scope.gridOptionsZoneDetails.columnDefs.length] = {
                name: 'ACD',
                field: 'Values[5]',
                cellFilter: "number:2"

            }
            $scope.gridOptionsZoneDetails.columnDefs[$scope.gridOptionsZoneDetails.columnDefs.length] = {
                name: 'PDD',
                field: 'Values[6]',
                cellFilter: "number:2"

            }
           

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
                $scope.gridOptionsZoneDetails.data = response;
                angular.forEach(response, function (itm) {
                    $scope.data.push(itm);
                });
            });
        }



    });