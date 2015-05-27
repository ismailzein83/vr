var RouteRuleManagerController = function ($scope, $location, $http, $timeout, uiGridConstants, RoutingAPIService,CarriersService,ZonesService, $modal) {
    var pageSize = 40;
    var page = 0;
    var pageUp = 0;
    var last = false;
    

    defineScopeObjects();
    defineScopeMethods();
    load();

    function defineScopeObjects() {
        $scope.numberoflines = 10;
        $scope.isloadingdata = false;
      
        $scope.optionsZonesFilter = {
            selectedvalues: [],
            datasource: []
        };
        
        $scope.optionsCustomersF = {
            selectedvalues: [],
            datasource: []
        };
        $scope.optionsRouteTypeF = {
            datasource: [],
            selectedvalues: []
        };
        $scope.optionsRouteTypeF.datasource = [
            { name: 'Override Route', url: '/Client/Modules/Routing/Views/Management/EditorTemplates/RouteOverrideTemplate.html', objectType: 'TOne.LCR.Entities.OverrideRouteActionData, TOne.LCR.Entities', typeName: 'OverrideRouteActionData' },
            { name: 'Priority Rule', url: '/Client/Modules/Routing/Views/Management/EditorTemplates/PriorityTemplate.html', objectType: 'TOne.LCR.Entities.PriorityRouteActionData, TOne.LCR.Entities', typeName: 'PriorityRouteActionData' },
            { name: 'Block Route', url: '/Client/Modules/Routing/Views/Management/EditorTemplates/RouteBlockTemplate.html', objectType: 'TOne.LCR.Entities.BlockRouteActionData, TOne.LCR.Entities', typeName: 'BlockRouteActionData' }
        ];
        var last = false;
        $scope.gridOptionsRouteRule = {
            enableHorizontalScrollbar: 0,
            enableVerticalScrollbar: 2,
            infiniteScrollPercentage: 20,
            infiniteScrollDown: true,
            enableFiltering: false,
            saveFocus: false,
            saveScroll: true,
            enableCellEdit : false,
            columnDefs: [
              {
                  name: 'Carrier Account', field: 'CarrierAccountDescription', height: 40, enableHiding: false, enableColumnMenu: false, width: 300,
                  cellTooltip: function (row, col) {
                      return row.entity.CarrierAccountDescription;
                  },
                  filter: {
                      condition: uiGridConstants.filter.CONTAINS,
                      placeholder: 'contains'
                  }
              },
              {
                  name: 'Code Set', enableColumnMenu: false, field: 'CodeSetDescription', height: 40, width: 300, enableHiding: false,
                  cellTooltip: function (row, col) {
                      return row.entity.CodeSetDescription;
                  },
              },
              { name: 'Rule Type', enableColumnMenu: false, field: 'ActionDescription', height: 40, width: 100, enableHiding: false },
              { name: 'Start Date', enableColumnMenu: false, field: 'BeginEffectiveDate', cellFilter: 'date:"yyyy-MM-dd "', height: 40, width: 100, enableHiding: false },
              { name: 'End Date', enableColumnMenu: false, field: 'EndEffectiveDate', cellFilter: 'date:"yyyy-MM-dd "', height: 40, width: 100, enableHiding: false }//,
            ],
            enableColumnResizing: true,
            enableSorting: false,

        };       
        $scope.gridOptionsRouteRule.columnDefs[$scope.gridOptionsRouteRule.columnDefs.length] = {
            name: ' ',
            enableColumnMenu: false,
            enableFiltering: false,
            height: 40,
            enableHiding: false, 
            cellTemplate: '/Client/Templates/Grid/CellAction.html'
        }

        $scope.gridOptionsHistory = {
            enableHorizontalScrollbar: 0,
            enableVerticalScrollbar: 2,
            data:[],
            infiniteScrollPercentage: 100,
            enableFiltering: false,
            saveFocus: false,
            saveScroll: true,
            enableCellEdit: false,
            columnDefs: [
              {
                  name: 'Roule type', height: 40, enableHiding: false, enableColumnMenu: false,
                  cellTemplate: '<div><a   class="btn btn-link " style="color:#000;font-size:12px" aria-label="Left Align"   ng-click=\"grid.appScope.openEdit(row)\">{{row.entity.ActionDescription}}</a></div>'
              },
              { name: 'Time', enableColumnMenu: false, field: 'Time', height: 40, width: 200, enableHiding: false, cellFilter: 'date:"yyyy-MM-dd hh:mm:ss "' },
              { name: 'Action', enableColumnMenu: false, field: 'Action', height: 40, width: 200, enableHiding: false }
            ],
            enableColumnResizing: true,
            enableSorting: false,

        };

        $scope.showMain = true;
        $scope.showHis = false;

    }
    function defineScopeMethods() {
        $scope.switchGrids = function () {
            if ($scope.showMain) {
                $scope.showMain = false;
                setTimeout(function () {
                    $scope.$apply(function () {
                        $scope.showHis = true;
                    });
                }, 700);
            }
            else {
                $scope.showHis = false;
                setTimeout(function () {
                    $scope.$apply(function () {

                        $scope.showMain = true;
                    });
                }, 700);
            }
        };
        var refreshRowData = function (rowEntity, index) {
             $scope.gridOptionsRouteRule.data[index] = null;
             $scope.gridOptionsRouteRule.data[index] = rowEntity;
             
        };
        var callBackHistory = function (rowEntity) {
            $scope.gridOptionsHistory.data.unshift(rowEntity);

        };
        $scope.openEdit = function (row, i) {

            var scopeDetails = $scope.$root.$new();
            scopeDetails.title = "Update Route Rule";
            scopeDetails.index = $scope.gridOptionsRouteRule.data.indexOf(row.entity);
            scopeDetails.RouteRuleId = row.entity.RouteRuleId;
            scopeDetails.refreshRowData = refreshRowData;
            scopeDetails.callBackHistory = callBackHistory;
            var addModal = $modal({ scope: scopeDetails, template: '/Client/Modules/Routing/Views/Management/RouteRuleEditor.html', show: true, animation: "am-fade-and-scale" });
            

        }
        $scope.adjustpost = function (e) {

            var self = angular.element(e.currentTarget);
            var selfHeight = $(this).parent().height()+12;
            var selfWidth = $(this).parent().width();
            var selfOffset = $(self).offset();
            var selfOffsetRigth = $(document).width() - selfOffset.left - selfWidth;
            var dropDown = self.parent().find('ul');
            $(dropDown).css({ position: 'fixed', top: selfOffset.top + selfHeight, left: 'auto' });


        }
         var buildFilter =  function (page, pageSize){

             var filter = {
                 PageNumber: page,
                 PageSize : pageSize 
             }
             if ($scope.optionsCustomersF.selectedvalues.length > 0) {
                 filter.CustomerIds = [];
                 angular.forEach($scope.optionsCustomersF.selectedvalues, function (customers) {
                     filter.CustomerIds.push(customers.CarrierAccountID);
                 });
             }
             if ($scope.optionsRouteTypeF.selectedvalues.length > 0) {
                 filter.RuleTypes = [];
                 angular.forEach($scope.optionsRouteTypeF.selectedvalues, function (type) {
                     filter.RuleTypes.push(type.typeName);
                 });
             }
             if($scope.option ==1 && $scope.optionsZonesFilter.selectedvalues.length > 0 ){
                 filter.ZoneIds = [];
                 angular.forEach($scope.optionsZonesFilter.selectedvalues, function (zone) {
                     filter.ZoneIds.push(zone.ZoneId);
                 });
             }
             if($scope.option ==2 && $scope.code!='' ){
                 filter.code =  $scope.code;                
             }
             return filter;
         }

        var getData = function (data, page) {
            var res = [];
            for (var i = (page * pageSize) ; i < (page + 1) * pageSize && i < data.length; ++i) {
                res.push(data[i]);
            }
            
            return res;
        };

        var getDataUp = function (data, page) {
            var res = [];
            for (var i = data.length - (page * pageSize) - 1; (data.length - i) < ((page + 1) * pageSize) && (data.length - i) > 0; --i) {
                try {
                    data[i].id = -(data.length - data[i].id)
                    res.push(data[i]);
                }
                catch (e) { }

            }
            return res;
        };

        $scope.getDatalist = function (page, pageSize) {
            $scope.isloadingdata = true;
            return RoutingAPIService.GetFilteredRouteRules(buildFilter(page, pageSize))
             .then(function (data) {
                 $scope.isloadingdata = false;
                 $scope.gridOptionsRouteRule.data = getData(data, page);
                 ++page;
             })

        }
        $scope.getDatalistFilterd = function () {
            
            page = 0;
            last = false;            
            $scope.gridApi.infiniteScroll.resetScroll(false, true);
            return $scope.getDatalist(page, pageSize);
           

        }
        $scope.resetDatalistFilterd = function () {

            page = 0;
            last = false;
            $scope.gridApi.infiniteScroll.resetScroll(false, true);
            $scope.optionsCustomersF.selectedvalues.length = 0;
            $scope.optionsRouteTypeF.selectedvalues.length = 0;
            $scope.optionsZonesFilter.selectedvalues.length = 0;
            $scope.code = '';
            $scope.getDatalist(page, pageSize);


        }
        $scope.gridOptionsRouteRule.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
            gridApi.infiniteScroll.on.needLoadMoreData($scope, function () {
                if (last==false) {
                    $scope.isloadingdata = true;
                                           
                    RoutingAPIService.GetFilteredRouteRules(buildFilter(page, pageSize))
                    .then(function (data) {
                        $scope.isloadingdata = false;
                        $scope.gridOptionsRouteRule.data = $scope.gridOptionsRouteRule.data.concat(data);
                        ++page;
                        gridApi.infiniteScroll.dataLoaded();
                        last = (data.length < pageSize) ? true : false;
                    }).finally(function () {
                        gridApi.infiniteScroll.dataLoaded();
                    })
                }
            });

        };
        
        $scope.AddNewRoute = function () {

            var scopeDetails = $scope.$root.$new();
            scopeDetails.title = "New Route Rule";
            scopeDetails.RouteRuleId = 'undefined';
            scopeDetails.callBackHistory = callBackHistory;
            var addModal = $modal({ scope: scopeDetails, template: '/Client/Modules/Routing/Views/Management/RouteRuleEditor.html', show: true, animation: "am-fade-and-scale" });

        }
        $scope.toggelFilter = function () {
            $scope.gridOptionsRouteRule.enableFiltering = !$scope.gridOptionsRouteRule.enableFiltering;
            $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.COLUMN);
            $('#grid1').find('.ui-grid-header-canvas').height(($scope.gridOptionsRouteRule.enableFiltering == true) ? 60 : 30);

        }
        $scope.divStyleChange = function (n) {
            $scope.numberoflines = n;
            var h = ((n + 2) * 30) + ($scope.showf == true ? 0 : 30);
            angular.element(document.getElementById('grid1')).css('height', h + 'px');
            $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.OPTIONS);
            $scope.gridApi.infiniteScroll.resetScroll(false, true);

        }
        $scope.searchZones = function (text) {
            return ZonesService.getSalesZones(text);
        }

    }
    function load() {
        $('.action-bar-ddl').on('show.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideDown();
        });
        //ADD SLIDEUP ANIMATION TO DROPDOWN //
        $('.action-bar-ddl').on('hide.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideUp();
        });
        var dropdownHidingTimeoutHandleropt;
        $('#optionddl').on('show.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideDown();
        });

        //ADD SLIDEUP ANIMATION TO DROPDOWN //
        $('#optionddl').on('hide.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideUp();
        });

        $('.option-filter').on('mouseenter', function () {
            var $this = $(this);
            clearTimeout(dropdownHidingTimeoutHandleropt);
            if (!$this.hasClass('open')) {
                $('.dropdown-toggle', $this).dropdown('toggle');
            }
        });

        $('.option-filter').on('mouseleave', function () {
            var $this = $(this);
            dropdownHidingTimeoutHandleropt = setTimeout(function () {
                if ($this.hasClass('open')) {
                    $('.dropdown-toggle', $this).dropdown('toggle');
                }
            }, 150);
        });



        $scope.getDatalist(page, pageSize);

        CarriersService.getCustomers().then(function (response) {
            $scope.optionsCustomersF.datasource = response;
           
        })
    }
    

}
RouteRuleManagerController.$inject = ['$scope', '$location', '$http', '$timeout', 'uiGridConstants', 'RoutingAPIService', 'CarriersService', 'ZonesService', '$modal'];

appControllers.controller('RouteRuleManagerController', RouteRuleManagerController)

