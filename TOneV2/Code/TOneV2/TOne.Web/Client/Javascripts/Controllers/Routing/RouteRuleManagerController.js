appControllers.controller('RouteRuleManagerController',
    function RouteRuleManagerController($scope, $location, $http) {
        $scope.last = false;
        $scope.gridOptionsRouteRule = {
           // data: 'myData',
            enableHorizontalScrollbar: 0,
            enableVerticalScrollbar: 1,
            infiniteScrollPercentage :100,
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
        var page = 0;
        var pageUp = 0;
        var getData = function(data, page) {
            var res = [];
            for (var i = (page * 20); i < (page + 1) * 20 && i < data.length; ++i) {
                res.push(data[i]);
            }
            return res;
        };
 
        var getDataUp = function(data, page) {
            var res = [];
            for (var i = data.length - (page * 20) - 1; (data.length - i) < ((page + 1) * 20) && (data.length - i) > 0; --i) {
                data[i].id = -(data.length - data[i].id)
                res.push(data[i]);
            }
            return res;
        };
 
        $http.get('/api/Routing/GetAllRouteRule',
            {
                params: {
                    pageNumber: page,
                    pageSize:20
                }
            })
          .success(function(data) {
              $scope.gridOptionsRouteRule.data = getData(data, page);
              ++page;
          });
 
        $scope.gridOptionsRouteRule.onRegisterApi = function(gridApi){
            gridApi.infiniteScroll.on.needLoadMoreData($scope, function () {
               // alert($scope.last)
                //if ($scope.last) {

                    $http.get('/api/Routing/GetAllRouteRule',
                    {
                        params: {
                            pageNumber: page,
                            pageSize: 20
                        }
                    })
                  .success(function (data) {
                      $scope.gridOptionsRouteRule.data = $scope.gridOptionsRouteRule.data.concat(data);
                      ++page;
                      gridApi.infiniteScroll.dataLoaded();
                      $scope.last = (data.length < 20) ? false : true;
                  })
                  .error(function () {
                      gridApi.infiniteScroll.dataLoaded();
                  });

              //  }
                
            });
            gridApi.infiniteScroll.on.needLoadMoreDataTop($scope,function(){
                $http.get('/api/Routing/GetAllRouteRule',
                    {
                        params: {
                            pageNumber: page,
                            pageSize:20
                        }
                    })
                  .success(function(data) {
                      $scope.gridOptionsRouteRule.data = getDataUp(data, pageUp).reverse().concat($scope.gridOptionsRouteRule.data);
                      ++pageUp;
                      gridApi.infiniteScroll.dataLoaded();
                  })
                  .error(function() {
                      gridApi.infiniteScroll.dataLoaded();
                  });
            });
        };
        //$scope.getAllRouteRule = function () {
        //    $scope.isloadingdata = true;
        //    $http.get($scope.baseurl + "/api/Routing/GetAllRouteRule")
        //      .success(function (response) {
        //          $scope.isloadingdata = false;
        //          $scope.myData = response;
        //      });

        //}
        //$scope.getAllRouteRule();
      
        $scope.AddNewRoute = function () {
            $location.path("/RouteRuleEditor/undefined").replace();
        }
    });
