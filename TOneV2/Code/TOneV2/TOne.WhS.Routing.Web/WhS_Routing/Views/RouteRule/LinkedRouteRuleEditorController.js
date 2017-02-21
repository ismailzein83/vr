(function (appControllers) {

    "use strict";

    linkedRouteRuleEditorController.$inject = ['$scope', 'VRNavigationService'];

    function linkedRouteRuleEditorController($scope, VRNavigationService) {

        var linkedRouteRuleIds;
        var linkedCode;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                linkedRouteRuleIds = parameters.linkedRouteRuleIds;
                linkedCode = parameters.linkedCode;
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
                linkedCode: linkedCode
            };
            return query;
        }
    }

    appControllers.controller('WhS_Routing_LinkedRouteRuleEditorController', linkedRouteRuleEditorController);
})(appControllers);
