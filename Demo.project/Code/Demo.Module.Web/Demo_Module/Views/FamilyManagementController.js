(function (appControllers) {
    "use strict";

    familyManagementController.$inject = ['$scope', 'VRNotificationService', 'Demo_Module_FamilyService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_CompanyService'];

    function familyManagementController($scope, VRNotificationService, Demo_Module_FamilyService, UtilsService, VRUIUtilsService, Demo_Module_CompanyService) {

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


            $scope.scopeModel.addFamily = function () {
                console.log("add");

                var onFamilyAdded = function (family) {
                    if (gridApi != undefined) {
                       

                        gridApi.onFamilyAdded(family);
                    }
                };
                Demo_Module_FamilyService.addFamily(onFamilyAdded);
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

    appControllers.controller('Demo_Module_FamilyManagementController', familyManagementController);
})(appControllers);