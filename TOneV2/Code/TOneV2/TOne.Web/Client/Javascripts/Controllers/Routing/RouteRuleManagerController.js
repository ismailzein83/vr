appControllers.controller('RouteRuleManagerController',
    function RouteRuleManagerController($scope, $location, $http, $timeout, uiGridConstants) {
        $('.dropdown').on('show.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideDown();
        });

        //ADD SLIDEUP ANIMATION TO DROPDOWN //
        $('.dropdown').on('hide.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideUp();
        });
        $scope.numberoflines = 10;
        $scope.isloadingdata = false;
        $scope.divStyleChange = function (n) {
            $scope.numberoflines = n;
            var h = ((n + 2) * 30) + ($scope.showf==true?0:30);
            angular.element(document.getElementsByClassName('gridroute')[0]).css('height', h + 'px');

        }
       
        $scope.last = false;
        var pageSize = 40;
        //var temp =  '<div ng-class="{ \'sortable\': sortable }">' +
        //           '<div class="ui-grid-vertical-bar">&nbsp;</div>' +
        //           '<div class="ui-grid-cell-contents" col-index="renderIndex">' +
        //           '<span>' +
        //           '{{col.displayName}}' +
        //           '</span>' +
        //           '</div>' +
        //           '<div class="ui-grid-column-menu-button" ng-if="grid.options.enableColumnMenus && !col.isRowHeader  && col.colDef.enableColumnMenu !== false" class="ui-grid-column-menu-button" ng-click="toggleMenu($event)">' +
        //           '<i class="ui-grid-icon-angle-down">&nbsp;</i>' +
        //           '</div>' +
        //           '<div ng-show="grid.appScope.showf" class="ui-grid-filter-container" ng-repeat="colFilter in col.filters">' +
        //           '<input type="text" class="ui-grid-filter-input" ng-model="colFilter.term" />' +
        //           '<div class="ui-grid-filter-button" ng-click="colFilter.term = null">' +
        //           '<i class="ui-grid-icon-cancel" ng-show="!!colFilter.term">&nbsp;</i>' + 
        //           '</div>' +
        //           '</div>' +
        //           '</div>';
        $scope.gridOptionsRouteRule = {
            enableHorizontalScrollbar: 0,
            enableVerticalScrollbar: 2,
            infiniteScrollPercentage: 20,
            enableFiltering: false,
            columnDefs: [
              {
                  name: 'Carrier Account', field: 'CarrierAccountDescription', height: 40, enableHiding: false, enableColumnMenu: false,
                  cellTooltip   : function (row, col) {
                        return  row.entity.CarrierAccountDescription ;
                  },
                  filter: {
                      condition: uiGridConstants.filter.CONTAINS,
                      placeholder: 'contains'
                  }
              },
              { name: 'Code Set', enableColumnMenu: false, field: 'CodeSetDescription', height: 40, width: 100, enableHiding: false},
              { name: 'Action desc', enableColumnMenu: false, field: 'ActionDescription', height: 40, width: 100, enableHiding: false},
              { name: 'Type', enableColumnMenu: false, field: 'TypeDescription', height: 40, width: 100},
              { name: 'Begin Effective Date', enableColumnMenu: false, field: 'BeginEffectiveDate', cellFilter: 'date:"yyyy-MM-dd "', height: 40, width: 170, enableHiding: false},
              { name: 'End Effective Date', enableColumnMenu: false, field: 'EndEffectiveDate', cellFilter: 'date:"yyyy-MM-dd "', height: 40, width: 170, enableHiding: false },
              { name: 'Reason', enableColumnMenu: false, field: 'Reason', height: 40, enableHiding: false},
            ],
            enableColumnResizing: true,
            enableSorting: false,

        };
        $scope.gridOptionsRouteRule.columnDefs[$scope.gridOptionsRouteRule.columnDefs.length] = {
            name: 'Action',
            enableColumnMenu: false,
            enableFiltering: false,
            height: 40,
            enableHiding: false,
            cellTemplate: '<div><button  type="button" class="btn btn-link " style="color:#000" aria-label="Left Align"   ng-click=\"grid.appScope.onDblClick(row)\"><span  class="glyphicon glyphicon glyphicon-edit" aria-hidden="true"></span></button></div>'
        }
        $scope.filter = {};
        $scope.onDblClick=  function (row) {
         $location.path("/RouteRuleEditor/" + row.entity.RouteRuleId).replace();
       }
       
     
        //$scope.toggelFilter();
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
        $scope.getDatalist = function (page, pageSize) {
            $scope.isloadingdata = true;
            $http.get('/api/Routing/GetAllRouteRule',
           {
               params: {
                   pageNumber: page,
                   pageSize: pageSize
               }
           })
         .success(function (data) {
             $scope.isloadingdata = false;
             $scope.gridOptionsRouteRule.data = getData(data, page);
             ++page;
         });
 
        }
        $scope.getDatalist(page,pageSize);
        $scope.gridOptionsRouteRule.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
            gridApi.infiniteScroll.on.needLoadMoreData($scope, function () {
                if (!$scope.last) {
                    $scope.isloadingdata = true;
                    $http.get('/api/Routing/GetAllRouteRule',
                    {
                        params: {
                            pageNumber: page,
                            pageSize: pageSize
                        }
                    })
                  .success(function (data) {
                      $scope.isloadingdata = false;
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
           
        };
       
      
        $scope.AddNewRoute = function () {
            $location.path("/RouteRuleEditor2/undefined").replace();
        }
        $scope.toggelFilter = function () {
            $scope.gridOptionsRouteRule.enableFiltering = !$scope.gridOptionsRouteRule.enableFiltering;
            $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.COLUMN);
            $('.ui-grid-header-canvas').height(($scope.gridOptionsRouteRule.enableFiltering == true) ? 60 : 30);

        }
    });
