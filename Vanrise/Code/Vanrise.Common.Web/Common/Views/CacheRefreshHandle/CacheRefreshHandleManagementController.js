(function (appControllers) {

    "use strict";

    cacheRefreshHandleManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService'];

    function cacheRefreshHandleManagementController($scope, UtilsService, VRUIUtilsService) {
        var gridAPI;
        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {             
                return gridAPI.loadGrid(buildFilterObject());
            };
            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(buildFilterObject());
            };

        }

        function load() {
            loadAllControls();
        }

        function loadAllControls() {

        }

        function buildFilterObject() {            
            return {
                TypeName : $scope.typeName
            }
        }



    }

    appControllers.controller('VRCommon_CacheRefreshHandleManagementController', cacheRefreshHandleManagementController);
})(appControllers);