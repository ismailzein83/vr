(function (appControllers) {
    "use strict";

    parentManagementController.$inject = ['$scope', 'VRNotificationService', 'Demo_BestPractices_ParentService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_CompanyService'];

    function parentManagementController($scope, VRNotificationService, Demo_BestPractices_ParentService, UtilsService, VRUIUtilsService, Demo_Module_CompanyService) {

       var gridApi;
        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;
                api.load(getFilter());
            };

            $scope.scopeModel.search = function () {
                return gridApi.load(getFilter());
            };

           
            $scope.scopeModel.addParent = function () {
                var onParentAdded = function (parent) {
                    if (gridApi != undefined) {
                        gridApi.onParentAdded(parent);
                    }
                };
                Demo_BestPractices_ParentService.addParent(onParentAdded);
            };
        };

        function load() {

        }


        function getFilter() {
            return {
                Name: $scope.scopeModel.name
            };
        };

    };

    appControllers.controller('Demo_BestPractices_ParentManagementController', parentManagementController);
})(appControllers);