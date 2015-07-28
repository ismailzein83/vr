RoutingManagementController.$inject = ['$scope', 'RoutingAPIService', 'CarrierAccountAPIService', 'CarrierTypeEnum', 'BusinessEntityAPIService_temp', 'RouteDetailFilterOrderEnum', 'RoutingRulesEnum', 'VRModalService', 'RoutingRulesTemplatesEnum', 'UtilsService', 'RoutingRulesAPIService', 'VRNotificationService'];

function RoutingManagementController($scope, RoutingAPIService, CarrierAccountAPIService, CarrierTypeEnum, BusinessEntityAPIService, RouteDetailFilterOrderEnum, RoutingRulesEnum, VRModalService, RoutingRulesTemplatesEnum, UtilsService, RoutingRulesAPIService, VRNotificationService) {
    var mainGridAPI;
    var sortColumn;
    var sortDescending = true;
    defineScope();
    load();

    defineMenuActions();

    function load() {
        loadRuleTypes();
        loadCustomers();
        resetSorting();
    }

    function defineScope() {
        $scope.routingDataSource = [];
        $scope.ruleTypes = [];
        $scope.customers = [];
        $scope.selectedCustomers = [];

        $scope.zones = [];
        $scope.selectedZones = [];

        $scope.zones = function (text) {
            return BusinessEntityAPIService.GetSalesZones(text);
        }

        $scope.getRouteRulesItems = function (dataItem, fromRow, ToRow) {
            return dataItem.Rules;
        };

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

        $scope.onRuleClicked = function (dataItem, colDef) {
            editRule(dataItem);
        };
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
                itm.ruleTypeName = itm.ActionType != null && itm.ActionType != undefined ? UtilsService.getItemByVal($scope.ruleTypes, itm.ActionType, 'value').name : '';
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
        return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Customer.value).then(function (response) {
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

    function addRouteRule(ruleType) {
        var modalSettings = {
            useModalTemplate: true,
            width: "80%",
            maxHeight: "800px"
        };
        var parameters = {
            ruleType: ruleType
        };
        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "New Route Rule";
            modalScope.onRouteRuleAdded = function (routeRuleObj) {
                mainGridAPI.itemAdded(routeRuleObj);
            };
        };
        VRModalService.showModal('/Client/Modules/Routing/Views/RoutingRules/RouteRuleEditor.html', parameters, modalSettings);
    }

    function AddRule(route, ruleType) {

        addRouteRule(ruleType.value);
    }

    function loadRuleTypes() {
        for (var prop in RoutingRulesTemplatesEnum) {
            $scope.ruleTypes.push(RoutingRulesTemplatesEnum[prop]);
        }
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
            modalScope.onRouteRuleUpdated = function (ruleUpdated) {
                mainGridAPI.itemUpdated(ruleUpdated);

            };
        };
        VRModalService.showModal('/Client/Modules/Routing/Views/RoutingRules/RouteRuleEditor.html', parameters, modalSettings);
    }


}

appControllers.controller('Routing_RoutingManagementController', RoutingManagementController);
