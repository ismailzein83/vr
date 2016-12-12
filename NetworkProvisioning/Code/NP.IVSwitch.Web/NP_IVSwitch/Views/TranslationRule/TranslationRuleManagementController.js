(function (appControllers) {

    "use strict";
 
    TranslationRuleManagementController.$inject = ['$scope', 'NP_IVSwitch_TranslationRuleService', 'NP_IVSwitch_TranslationRuleAPIService'];
    function TranslationRuleManagementController($scope, NP_IVSwitch_TranslationRuleService, NP_IVSwitch_TranslationRuleAPIService) {

        var gridAPI;

        defineScope();
        load();


        function defineScope() {
            $scope.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };
            $scope.add = function () {
                var onTranslationRuleAdded = function (addedTranslationRule) {
                    gridAPI.onTranslationRuleAdded(addedTranslationRule);
                }
                NP_IVSwitch_TranslationRuleService.addTranslationRule(onTranslationRuleAdded);
            };

            $scope.hasAddTranslationRulePermission = function () {
                return NP_IVSwitch_TranslationRuleAPIService.HasAddTranslationRulePermission();
            }

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({}); 
            };
        }

        function load() {

        }

        function buildGridQuery() {
            return {
                Name: $scope.name
            };
        }
    }        

    appControllers.controller('NP_IVSwitch_TranslationRuleManagementController', TranslationRuleManagementController);

})(appControllers);