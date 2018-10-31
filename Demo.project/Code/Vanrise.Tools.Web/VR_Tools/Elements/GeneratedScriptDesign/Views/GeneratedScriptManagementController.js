(function (appControllers) {
    "use strict";

    generatedScriptManagementController.$inject = ['$scope',  'VRNotificationService', 'UtilsService', 'VRUIUtilsService'];

    function generatedScriptManagementController($scope, VRNotificationService, UtilsService, VRUIUtilsService) {
        var gridApi;
        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onGridReady = function (api) {

                gridApi = api;
                function addText(text) {
                    $scope.scopeModel.tableDataSource = text;
                }
                gridApi.load({ addText: addText });
            };
        }
       
        function load() {
            $scope.scopeModel.isLoading = true;
            
            loadAllControls();
        }
        function loadAllControls() {
                 $scope.scopeModel.isLoading = false;
        }

        function getFilter() {
           
        }

    };

    appControllers.controller('VR_Tools_GeneratedScriptManagementController', generatedScriptManagementController);
})(appControllers);