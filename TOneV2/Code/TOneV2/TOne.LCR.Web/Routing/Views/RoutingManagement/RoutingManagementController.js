RoutingManagementController.$inject = ['$scope', 'RoutingAPIService', 'CarrierAPIService', 'CarrierTypeEnum', 'BusinessEntityAPIService_temp', 'RouteDetailFilterOrderEnum', 'RoutingRulesEnum'];

function RoutingManagementController($scope, RoutingAPIService, CarrierAPIService, CarrierTypeEnum, BusinessEntityAPIService, RouteDetailFilterOrderEnum, RoutingRulesEnum) {
    var mainGridAPI;
    var sortColumn;
    var sortDescending = true;
    load();
    defineScope();
    defineMenuActions();

    function load() {
        loadCustomers();
        resetSorting();
    }

    function defineScope() {
        $scope.routingDataSource = [];

        $scope.customers = [];
        $scope.selectedCustomers = [];

        $scope.zones = [];
        $scope.selectedZones = [];

        $scope.zones = function (text) {
            return BusinessEntityAPIService.GetSalesZones(text);
        }


        $scope.searchClicked = function () {
            mainGridAPI.clearDataAndContinuePaging();
            return getData();
        }
        $scope.gridReady = function (api) {
            mainGridAPI = api;
            getData();
        };


        $scope.onMainGridSortChanged = function (colDef, sortDirection) {
            sortColumn = colDef.tag;
            sortDescending = (sortDirection == "DESC");
            return getData();
        }


        $scope.loadMoreData = function () {
            return getData();
        }
    }

    function getData() {
        var filter = buildFilter();
        var pageInfo = mainGridAPI.getPageInfo();

        var getRoutingInput = {
            Filter: filter,
            FromRow: pageInfo.fromRow,
            ToRow: pageInfo.toRow,
            OrderBy: sortColumn.value,
            IsDescending: sortDescending
        };

        return RoutingAPIService.GetRoutes(getRoutingInput).then(function (response) {
            var routeData = [];
            angular.forEach(response, function (itm) {
                itm.suppliers = itm.Options != null ? GetSupplierNames(itm.Options) : '';
                routeData.push(itm);
            });
            mainGridAPI.addItemsToSource(routeData);
        });
    }

    function GetSupplierNames(array) {
        var names = '';
        for (var i = 0; i < array.length; i++) {
            names += array[i].Supplier + '   ';
        }
        return names;
    }

    function buildFilter() {
        var filter = {};
        filter.CustomerIds = getFilterIds($scope.selectedCustomers, "CarrierAccountID");
        filter.ZoneIds = getFilterIds($scope.selectedZones, "ZoneId");
        filter.Code = $scope.code;
        return filter;
    }

    function getFilterIds(values, idProp) {
        var filterIds;
        if (values.length > 0) {
            filterIds = [];
            angular.forEach(values, function (val) {
                filterIds.push(val[idProp]);
            });
        }
        return filterIds;
    }

    function loadCustomers() {
        return CarrierAPIService.GetCarriers(CarrierTypeEnum.Customer.value).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.customers.push(itm);
            });
        });
    }

    function resetSorting() {
        sortColumn = RouteDetailFilterOrderEnum.Code;
        sortDescending = true;
    }


    function defineMenuActions() {
        $scope.gridMenuActions = [{
            name: "Override",
            clicked: function (dataItem) {
                AddRule(dataItem, RoutingRulesEnum.Override);
            }
        },
        {
            name: "Priority",
            clicked: function (dataItem) {
                AddRule(dataItem, RoutingRulesEnum.Priority);
            }
        },
        {
            name: "Block",
            clicked: function (dataItem) {
                AddRule(dataItem, RoutingRulesEnum.Block);
            }
        }
        ];
    }

    function AddRule(route, ruleType) {
        console.log(route);
        switch (ruleType.value) {
            case 0:
                console.log(0);
                break;
            case 1:
                console.log(1);
                break;
            case 2:
                console.log(2);
                break;
            default:
                break;

        }
    }
}

appControllers.controller('Routing_RoutingManagementController', RoutingManagementController);
