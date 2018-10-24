(function (appControllers) {
    "use strict";

    generatedScriptManagementController.$inject = ['$scope', 'Demo_Module_PageRunTimeService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService'];

    function generatedScriptManagementController($scope, Demo_Module_PageRunTimeService, VRNotificationService, UtilsService, VRUIUtilsService) {

        var gridApi;

        $scope.scopeModel = {};
        defineScope();
        load();

        function defineScope() {

            $scope.scopeModel.search = function () {
               
                return gridApi.load(getFilter());

            };

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;
            };

        }
       
        function load() {

            $scope.scopeModel.isLoading = true;
            loadAllControls();

        }

        function loadAllControls() {

            //return UtilsService.waitMultipleAsyncOperations([loadPageDefinitionSelector])
            //  .catch(function (error) {
            //      VRNotificationService.notifyExceptionWithClose(error, $scope);
            //  })
            // .finally(function () {
            //     $scope.scopeModel.isLoading = false;
            // });
        }

        function getFilter() {
           
        }

    };

    appControllers.controller('Vanrise_Tools_GeneratedScriptManagementController', generatedScriptManagementController);
})(appControllers);