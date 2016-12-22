
(function (appControllers) {

    "use strict";

    CreditClassManagementController.$inject = ['$scope', 'Retail_BE_CreditClassService', 'UtilsService', 'VRUIUtilsService', 'Retail_BE_CreditClassAPIService'];

    function CreditClassManagementController($scope, Retail_BE_CreditClassService, UtilsService, VRUIUtilsService, Retail_BE_CreditClassAPIService) {

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
                Retail_BE_CreditClassService.addCreditClass(onCreditClassAdded);
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };

            $scope.scopeModel.hasAddCreditClassPermission = function () {
                return Retail_BE_CreditClassAPIService.HasAddCreditClassPermission();
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