RoutingManagementController.$inject = ['$scope', 'RoutingAPIService', 'CarrierAPIService', 'CarrierTypeEnum', 'BusinessEntityAPIService_temp', 'RouteDetailFilterOrderEnum'];

function RoutingManagementController($scope, RoutingAPIService, CarrierAPIService, CarrierTypeEnum, BusinessEntityAPIService, RouteDetailFilterOrderEnum) {
    var mainGridAPI;
    var sortColumn;
    var sortDescending = true;
    load();
    defineScope();


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
        console.log(mainGridAPI);
        var pageInfo = mainGridAPI.getPageInfo();


        var getRoutingInput = {
            Filter: filter,
            FromRow: pageInfo.fromRow,
            ToRow: pageInfo.toRow,
            OrderBy: sortColumn.value,
            IsDescending: sortDescending
        };

        return RoutingAPIService.GetRoutes(getRoutingInput).then(function (response) {
            mainGridAPI.addItemsToSource(response);
        });
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
}

appControllers.controller('Routing_RoutingManagementController', RoutingManagementController);
