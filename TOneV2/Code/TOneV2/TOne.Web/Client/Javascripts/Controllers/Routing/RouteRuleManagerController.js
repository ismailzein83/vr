appControllers.controller('RouteRuleManagerController',
    function RouteRuleManagerController($scope, $location, $http) {
        $scope.last = false;
        var pageSize = 20;
        $scope.gridOptionsRouteRule = {
            enableHorizontalScrollbar: 0,
            enableVerticalScrollbar: 2,
            infiniteScrollPercentage :25,
            enableSelection: true,           
            columnDefs: [
              {
                  name: 'Carrier Account', field: 'CarrierAccountDescription', height: 40, enableHiding: false,
                  cellTooltip   : function (row, col) {
                        return  row.entity.CarrierAccountDescription ;
                    }
              },
              { name: 'Code Set',enableColumnMenu: false, field: 'CodeSetDescription', height: 40, width: 100, enableHiding: false },
              { name: 'Action desc', enableColumnMenu: false, field: 'ActionDescription', height: 40, width: 100, enableHiding: false },
              { name: 'Type', enableColumnMenu: false, field: 'TypeDescription', height: 40, width: 100 },
              { name: 'Begin Effective Date', enableColumnMenu: false, field: 'BeginEffectiveDate', cellFilter: 'date:"yyyy-MM-dd "', height: 40, width: 170, enableHiding: false },
              { name: 'End Effective Date', enableColumnMenu: false, field: 'EndEffectiveDate', cellFilter: 'date:"yyyy-MM-dd "', height: 40, width: 170, enableHiding: false },
              { name: 'Reason', enableColumnMenu: false, field: 'Reason', height: 40, enableHiding: false }

            ],
            enableColumnResizing: true,
            enableSorting: false,
            appScopeProvider: {
                onDblClick: function (row) {
                    $location.path("/RouteRuleEditor/" + row.entity.RouteRuleId).replace();
                }
            }

        };
        $scope.gridOptionsRouteRule.columnDefs[$scope.gridOptionsRouteRule.columnDefs.length] = {
            name: 'Action',
            enableColumnMenu: false,
            height: 40,
            enableHiding: false,
            cellTemplate: '<div><button  type="button" class="btn btn-link " style="color:#000" aria-label="Left Align"   ng-click=\"grid.appScope.onDblClick(row)\"><span  class="glyphicon glyphicon glyphicon-edit" aria-hidden="true"></span></button></div>'
        }


        var page = 0;
        var pageUp = 0;
        var getData = function(data, page) {
            var res = [];
            for (var i = (page * pageSize) ; i < (page + 1) * pageSize && i < data.length; ++i) {
                res.push(data[i]);
            }
            return res;
        };
 
        var getDataUp = function(data, page) {
            var res = [];
            for (var i = data.length - (page * pageSize) - 1; (data.length - i) < ((page + 1) * pageSize) && (data.length - i) > 0; --i) {
                try{
                    data[i].id = -(data.length - data[i].id)
                    res.push(data[i]);
                }
                catch (e){}
               
            }
            return res;
        };
 
        $http.get('/api/Routing/GetAllRouteRule',
            {
                params: {
                    pageNumber: page,
                    pageSize: pageSize
                }
            })
          .success(function(data) {
              $scope.gridOptionsRouteRule.data = getData(data, page);
              ++page;
          });
 
        $scope.gridOptionsRouteRule.onRegisterApi = function(gridApi){
            gridApi.infiniteScroll.on.needLoadMoreData($scope, function () {
                if (!$scope.last) {

                    $http.get('/api/Routing/GetAllRouteRule',
                    {
                        params: {
                            pageNumber: page,
                            pageSize: pageSize
                        }
                    })
                  .success(function (data) {
                      $scope.gridOptionsRouteRule.data = $scope.gridOptionsRouteRule.data.concat(data);
                      ++page;
                      gridApi.infiniteScroll.dataLoaded();
                      $scope.last = (data.length < pageSize) ? true : false;
                  })
                  .error(function () {
                      gridApi.infiniteScroll.dataLoaded();
                  });

                }
                
            });
            gridApi.infiniteScroll.on.needLoadMoreDataTop($scope,function(){
                $http.get('/api/Routing/GetAllRouteRule',
                    {
                        params: {
                            pageNumber: page,
                            pageSize: pageSize
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
