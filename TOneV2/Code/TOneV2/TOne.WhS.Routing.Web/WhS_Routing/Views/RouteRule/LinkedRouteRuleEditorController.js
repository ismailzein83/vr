(function (appControllers) {

    "use strict";

    linkedRouteRuleEditorController.$inject = ['$scope', 'VRNavigationService'];

    function linkedRouteRuleEditorController($scope, VRNavigationService) {

        var linkedRouteRuleIds;
        var customerRouteData;
        var onRouteRuleUpdated;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                linkedRouteRuleIds = parameters.linkedRouteRuleIds;
                customerRouteData = parameters.customerRouteData;
                onRouteRuleUpdated = parameters.onRouteRuleUpdated;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                api.loadGrid(getFilterObject());
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }
        function load() {
            $scope.title = "Linked Route Rules";
        }

        function getFilterObject() {
            var query = {
                LinkedRouteRuleIds: linkedRouteRuleIds,
                areRulesLinked: true,
                customerRouteData: customerRouteData,
                onRouteRuleUpdated: onRouteRuleUpdated
            };
            return query;
        }
    }

    appControllers.controller('WhS_Routing_LinkedRouteRuleEditorController', linkedRouteRuleEditorController);
})(appControllers);
