
(function (appControllers) {

    "use strict";

    RevenueComparisonManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService'];

    function RevenueComparisonManagementController($scope, UtilsService, VRUIUtilsService) {

        var gridAPI;

        defineScope();
        load();


        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
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
                FromDate: $scope.scopeModel.fromDate,
                ToDate:$scope.scopeModel.toDate 
            };
        }
    }

    appControllers.controller('Retail_BE_RevenueComparisonManagementController', RevenueComparisonManagementController);

})(appControllers);