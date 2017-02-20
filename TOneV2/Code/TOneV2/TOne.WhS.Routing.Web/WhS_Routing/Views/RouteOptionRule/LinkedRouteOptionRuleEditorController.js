(function (appControllers) {

    "use strict";

    linkedRouteOptionRuleEditorController.$inject = ['$scope', 'VRNavigationService'];

    function linkedRouteOptionRuleEditorController($scope, VRNavigationService) {

        var linkedRouteOptionRuleIds;
        var linkedCode;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                linkedRouteOptionRuleIds = parameters.linkedRouteOptionRuleIds;
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
            $scope.title = "Linked Route Option Rules";
        }

        function getFilterObject() {
            var query = {
                LinkedRouteOptionRuleIds: linkedRouteOptionRuleIds,
                areRulesLinked: true,
                linkedCode: linkedCode
            };
            return query;
        }
    }

    appControllers.controller('WhS_Routing_LinkedRouteOptionRuleEditorController', linkedRouteOptionRuleEditorController);
})(appControllers);
