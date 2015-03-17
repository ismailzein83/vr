appControllers.controller('RouteRuleManagerController',
    function RouteRuleManagerController($scope, $location, $http) {
        $scope.myData = [];
        $scope.gridOptionsRouteRule = {
            data: 'myData',
            enableHorizontalScrollbar: 0,
            enableVerticalScrollbar: 1,
            enableSelection: true,
            columnDefs: [
              { name: 'Carrier Account', field: 'CarrierAccountDescription', height: 40 },
              { name: 'Code Set', field: 'CodeSetDescription', height: 40, width: 100 },
              { name: 'Action', field: 'ActionDescription', height: 40, width: 100 },
              { name: 'Type', field: 'TypeDescription', height: 40, width: 100 },
              { name: 'Begin Effective Date', field: 'BeginEffectiveDate', cellFilter: 'date:"yyyy-MM-dd "', height: 40, width: 170 },
              { name: 'End Effective Date', field: 'EndEffectiveDate', cellFilter: 'date:"yyyy-MM-dd "', height: 40, width: 170 },
              { name: 'Reason', field: 'Reason', height: 40 }
            ]
            ,
            //paginationPageSizes: [10, 25, 50, 75],
            //paginationPageSize: 10,
            appScopeProvider: {
                onDblClick: function (row) {
                    $location.path("/RouteRuleEditor/" + row.entity.RouteRuleId).replace();
                }
            },
            rowTemplate: '<div ng-dblclick=\"grid.appScope.onDblClick(row)\"   ng-repeat="col in colContainer.renderedColumns track by col.colDef.name" class="ui-grid-cell hand-cursor" ui-grid-cell></div>'

        };
        $scope.getAllRouteRule = function () {
            $scope.isloadingdata = true;
            $http.get($scope.baseurl + "/api/Routing/GetAllRouteRule")
              .success(function (response) {
                  $scope.isloadingdata = false;
                  $scope.myData = response;
              });

        }
        $scope.getAllRouteRule();
      
        $scope.AddNewRoute = function () {
            $location.path("/RouteRuleEditor/undefined").replace();
        }
    });
