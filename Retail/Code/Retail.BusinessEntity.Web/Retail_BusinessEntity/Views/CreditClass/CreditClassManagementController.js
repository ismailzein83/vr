
(function (appControllers) {

    "use strict";

    CreditClassManagementController.$inject = ['$scope', 'Retail_BE_CreditClassService', 'UtilsService', 'VRUIUtilsService'];

    function CreditClassManagementController($scope, Reprocess_CreditClassService, UtilsService, VRUIUtilsService) {

        var gridAPI;

        defineScope();
        load();


        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };

            $scope.scopeModel.add = function () {
                var onCreditClassAdded = function (addedCreditClass) {
                    gridAPI.onCreditClassAdded(addedCreditClass);
                };
                Reprocess_CreditClassService.addCreditClass(onCreditClassAdded);
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
        }

        function load() {

        }

        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name
            };
        }
    }

    appControllers.controller('Retail_BE_CreditClassManagementController', CreditClassManagementController);

})(appControllers);