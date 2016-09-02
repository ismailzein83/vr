(function (appControllers) {

    'use strict';

   OperatorDeclaredInfoManagementController.$inject = ['$scope', 'Retail_BE_OperatorDeclaredInfoService', 'Retail_BE_OperatorDeclaredInfoAPIService', 'UtilsService'];

    function OperatorDeclaredInfoManagementController($scope, Retail_BE_OperatorDeclaredInfoService, Retail_BE_OperatorDeclaredInfoAPIService, UtilsService) {
        var gridAPI;

        defineScope();
        load();

        function defineScope() {

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid({});
            };

            $scope.search = function () {
                var query = buildGridQuery();
                return gridAPI.loadGrid(query);
            };

            $scope.add = function () {
                var onOperatorDeclaredInfoAdded = function (addedOperatorDeclaredInfo) {
                    gridAPI.onOperatorDeclaredInfoAdded(addedOperatorDeclaredInfo);
                };
                Retail_BE_OperatorDeclaredInfoService.addOperatorDeclaredInfo(onOperatorDeclaredInfoAdded);
            };

            $scope.hasAddOperatorDeclaredInfoPermission = function () {
                return Retail_BE_OperatorDeclaredInfoAPIService.HasAddOperatorDeclaredInfoPermission();
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

    appControllers.controller('Retail_BE_OperatorDeclaredInfoManagementController',OperatorDeclaredInfoManagementController);

})(appControllers);