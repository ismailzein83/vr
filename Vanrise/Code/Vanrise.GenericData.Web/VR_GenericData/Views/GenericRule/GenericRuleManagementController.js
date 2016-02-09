(function (appControllers) {
    'use strict';

    genericRuleManagementController.$inject = ['$scope', 'VRNavigationService', 'VR_GenericData_GenericRule'];

    function genericRuleManagementController($scope, VRNavigationService, VR_GenericData_GenericRule) {

        var gridAPI;

        loadParameters();
        defineScope();
        load();

        var ruleDefinitionId

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
          
            if (parameters != null) {
                ruleDefinitionId = parameters.ruleDefinitionId;
            }
        }

        function defineScope() {
            $scope.onGridReady = function (api) {
                gridAPI = api;
                var defFilter = {
                    RuleDefinitionId: ruleDefinitionId
                };
                gridAPI.loadGrid(defFilter);
            };

            $scope.search = function () {
                var filter = getFilterObject();
                return gridAPI.loadGrid(filter);
            };

            $scope.addGenericRule = function () {
                var onGenericRuleAdded = function (ruleObj) {
                    gridAPI.onGenericRuleAdded(ruleObj);
                };

                VR_GenericData_GenericRule.addGenericRule(ruleDefinitionId, onGenericRuleAdded);
            };

            function getFilterObject() {
                return {
                    RuleDefinitionId: ruleDefinitionId
                };
            }
        }

        function load() {

        }
    }

    appControllers.controller('VR_GenericData_GenericRuleManagementController', genericRuleManagementController);

})(appControllers);
