(function (appControllers) {
    "use strict";

    itemManagementController.$inject = ['$scope', 'VRNotificationService', 'Demo_Module_ItemService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_ProductService'];

    function itemManagementController($scope, VRNotificationService, Demo_Module_ItemService, UtilsService, VRUIUtilsService, Demo_Module_ProductService) {

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


            $scope.scopeModel.addItem = function () {
                var onItemAdded = function (item) {
                    if (gridApi != undefined) {
                        gridApi.onItemAdded(item);
                    }
                };
                Demo_Module_ItemService.addItem(onItemAdded);
            };
        };

        function load() {

        }


        function getFilter() {
            return {
                query: {
                    Name: $scope.scopeModel.name
                }
            };
        };

    };

    appControllers.controller('Demo_Module_ItemManagementController', itemManagementController);
})(appControllers);