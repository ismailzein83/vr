RoutingRulesGridController.$inject = ['$scope', 'VRModalService', 'VRNotificationService', 'RoutingRulesAPIService'];
function RoutingRulesGridController($scope, VRModalService, VRNotificationService, RoutingRulesAPIService) {
    var gridAPI;
    defineScope();
    load();
    defineMenuActions();

    function defineScope() {
        if ($scope.dataItem != undefined)//the template is located in a data-grid expandable section
            $scope.routingRulesDataSource = [];
        $scope.gridReady = function (api) {
            gridAPI = api;
        }
    }

    function load() {
        checkParentDataItem();
    }

    function checkParentDataItem() {
        if ($scope.dataItem != undefined) {//the template is located in a data-grid expandable section
            var items = $scope.viewScope.getRouteRulesItems($scope.dataItem);
            if (items != null)
                for (var i = 0; i < items.length; i++)
                    $scope.routingRulesDataSource.push(items[i]);
        }
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [{
            name: "Edit",
            clicked: editRule
        }];
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
                gridAPI.itemUpdated(ruleUpdated);

            };
        };
        VRModalService.showModal('/Client/Modules/Routing/Views/RoutingRules/RouteRuleEditor.html', parameters, modalSettings);
    }

    function endRule(ruleObject) {
        ruleObject.EndEffectiveDate = new Date();
        return RoutingRulesAPIService.UpdateRouteRule(ruleObject);
    }

    function deleteRule(ruleObject) {
        return RoutingRulesAPIService.DeleteRouteRule(ruleObject);
    }
}

appControllers.controller('Routing_RoutingRulesGridController', RoutingRulesGridController);