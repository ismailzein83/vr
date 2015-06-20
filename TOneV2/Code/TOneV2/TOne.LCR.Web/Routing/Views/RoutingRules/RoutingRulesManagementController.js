RoutingRulesManagementController.$inject = ['$scope', 'RoutingRulesAPIService', 'BusinessEntityAPIService_temp', 'CarrierAPIService', 'VRModalService', 'CarrierTypeEnum', 'RoutingRulesTemplatesEnum'];
function RoutingRulesManagementController($scope, RoutingRulesAPIService, BusinessEntityAPIService, CarrierAPIService, VRModalService, CarrierTypeEnum, RoutingRulesTemplatesEnum) {
    var mainGridAPI;
    var sortColumn;
    var resultKey;
    var sortDescending = true;
    defineScope();
    load();

    function load() {
        loadCustomers();
    }

    function defineScope() {

        $scope.routingRulesDataSource = [];

        $scope.addRouteRule = addRouteRule;

        $scope.customers = [];
        $scope.selectedCustomers = [];

        $scope.zones = [];
        $scope.selectedZones = [];

        $scope.selectedRuleTypes = [];

        $scope.ruleTypes = RoutingRulesTemplatesEnum;

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

        $scope.loadMoreData = function () {
            return getData();
        }
        defineMenuActions();

        $scope.filter = {
            resultKey: resultKey,
            filter: {},
            sortDescending: sortDescending
        };

    }

    function loadCustomers() {
        return CarrierAPIService.GetCarriers(CarrierTypeEnum.Customer.value).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.customers.push(itm);
            });
        });
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [{
            name: "Edit",
            clicked: editRule
        }
        ];
    }

    function addRouteRule() {
        var modalSettings = {
            useModalTemplate: true,
            width: "80%",
            maxHeight: "800px"
        };
        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "New Route Rule";
            modalScope.onSwitchAdded = function (routeRuleObj) {
                gridApi.itemAdded(routeRuleObj);
            };
        };
        VRModalService.showModal('/Client/Modules/Routing/Views/RoutingRules/RouteRuleEditor.html', null, modalSettings);
    }

    function editRule(ruleObj) {
        var modalSettings = {
            useModalTemplate: true,
            width: "80%",
            maxHeight: "800px"
        };
        var parameters = {
            ruleId: ruleObj.RouteRuleId
        };
        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Rule Info(" + ruleObj.RouteRuleId + ")";
            modalScope.onSwitchUpdated = function (ruleUpdated) {
                gridApi.itemUpdated(ruleUpdated);

            };
        };
        VRModalService.showModal('/Client/Modules/Routing/Views/RoutingRules/RouteRuleEditor.html', parameters, modalSettings);
    }

    function getData() {
        var filter = buildFilter();
        console.log(mainGridAPI);
        var pageInfo = mainGridAPI.getPageInfo();


        var getRoutingRulesInput = {
            Filter: filter,
            FromRow: pageInfo.fromRow,
            ToRow: pageInfo.toRow,
            IsDescending: $scope.filter.sortDescending
        };

        return RoutingRulesAPIService.GetFilteredRouteRules(getRoutingRulesInput).then(function (response) {
            mainGridAPI.addItemsToSource(response);
        });
    }

    function buildFilter() {
        var filter = {};
        filter.CustomerIds = getFilterIds($scope.selectedCustomers, "CarrierAccountID");
        filter.ZoneIds = getFilterIds($scope.selectedZones, "ZoneId");
        filter.Code = $scope.code;
        filter.RuleTypes = getFilterIds($scope.selectedRuleTypes, "typeName");
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

}

appControllers.controller('RoutingRules_RoutingRulesManagementController', RoutingRulesManagementController);