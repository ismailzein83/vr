RoutingRulesManagementController.$inject = ['$scope', 'RoutingRulesAPIService', 'BusinessEntityAPIService', 'VRModalService', 'CarrierTypeEnum'];
function RoutingRulesManagementController($scope, RoutingRulesAPIService, BusinessEntityAPIService, VRModalService, CarrierTypeEnum) {
    var mainGridAPI;
    var sortColumn;
    var resultKey;
    var currentSortedColDef;
    var sortDescending = true;
    defineScope();

    function defineScope() {
        $scope.RoutingRulesDataSource = [];
        //defineMenuActions();

        $scope.customers = [];
        $scope.selectedCustomers = [];

        $scope.zones = [];
        $scope.selectedZones = [];

        $scope.selectedRuleTypes = [];

        $scope.ruleTypes = [
       { name: 'Override Route', url: '/Client/Modules/Routing/Views/Management/EditorTemplates/RouteOverrideTemplate.html', objectType: 'TOne.LCR.Entities.OverrideRouteActionData, TOne.LCR.Entities', typeName: 'OverrideRouteActionData' },
       { name: 'Priority Rule', url: '/Client/Modules/Routing/Views/Management/EditorTemplates/PriorityTemplate.html', objectType: 'TOne.LCR.Entities.PriorityRouteActionData, TOne.LCR.Entities', typeName: 'PriorityRouteActionData' },
       { name: 'Block Route', url: '/Client/Modules/Routing/Views/Management/EditorTemplates/RouteBlockTemplate.html', objectType: 'TOne.LCR.Entities.BlockRouteActionData, TOne.LCR.Entities', typeName: 'BlockRouteActionData' }
        ];

        $scope.zones = function (text) {
            return BusinessEntityAPIService.GetSalesZones(text);
        }
    }

    load();

    function load() {
        loadCustomers();
    }

    function loadCustomers() {
        return BusinessEntityAPIService.GetCarriers(CarrierTypeEnum.Customer.value).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.customers.push(itm);
            });
        });
    }
}

appControllers.controller('Routing_RoutingRulesManagementController', RoutingRulesManagementController);