(function (appControllers) {

    "use strict";

    childManagementController.$inject = ['$scope', 'VRNotificationService', 'Demo_BestPractices_ChildService', 'UtilsService', 'VRUIUtilsService'];

    function childManagementController($scope, VRNotificationService, Demo_BestPractices_ChildService, UtilsService, VRUIUtilsService) {

        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;             
                api.load(getFilter());
            };

            $scope.scopeModel.search = function () {
                return gridAPI.load(getFilter());
            };

            $scope.scopeModel.addChild = function () {
                var onChildAdded = function (child) {
                    if (gridAPI != undefined) {
                        gridAPI.onChildAdded(child);
                    }
                };
                Demo_BestPractices_ChildService.addChild(onChildAdded);
            };
        };

        function load() {

        }

        function getFilter() {
            return {
                query:{
                    Name: $scope.scopeModel.name
                }
            };
        };
    };

    appControllers.controller('Demo_BestPractices_ChildManagementController', childManagementController);
})(appControllers);