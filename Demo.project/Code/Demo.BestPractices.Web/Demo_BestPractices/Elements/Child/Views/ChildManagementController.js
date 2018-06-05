(function (appControllers) {
    "use strict";

    childManagementController.$inject = ['$scope', 'VRNotificationService', 'Demo_BestPractices_ChildService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_CompanyService'];

    function childManagementController($scope, VRNotificationService, Demo_BestPractices_ChildService, UtilsService, VRUIUtilsService, Demo_Module_CompanyService) {

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


            $scope.scopeModel.addChild = function () {
                var onChildAdded = function (child) {
                    if (gridApi != undefined) {
                        gridApi.onChildAdded(child);
                    }
                };
                Demo_BestPractices_ChildService.addChild(onChildAdded);
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

    appControllers.controller('Demo_BestPractices_ChildManagementController', childManagementController);
})(appControllers);