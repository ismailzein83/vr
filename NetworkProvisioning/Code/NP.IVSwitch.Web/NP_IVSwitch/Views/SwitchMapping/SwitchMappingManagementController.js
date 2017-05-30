(function (appControllers) {

    "use strict";

    SwitchMappingManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService'];
    function SwitchMappingManagementController($scope, UtilsService, VRUIUtilsService) {

        var gridAPI;

        defineScope();

        function defineScope() {
            $scope.search = function () {
                return gridAPI.loadGrid(buildGridQuery());
            };
            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(buildGridQuery());
            };
           
        }

        function buildGridQuery() {
            return {
                
            };
        }
    }
    appControllers.controller('NP_IVSwitch_SwitchMappingManagementController', SwitchMappingManagementController);

})(appControllers);