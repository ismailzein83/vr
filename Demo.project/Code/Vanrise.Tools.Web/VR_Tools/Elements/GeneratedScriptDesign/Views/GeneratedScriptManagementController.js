(function (appControllers) {
    "use strict";

    generatedScriptManagementController.$inject = ['$scope', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VR_Tools_ColumnsAPIService'];

    function generatedScriptManagementController($scope, VRNotificationService, UtilsService, VRUIUtilsService,VR_Tools_ColumnsAPIService) {
        var gridApi;
        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};
            var generatedScripts = [];
            var designs = [];
            var generatedScript;
            $scope.scopeModel.tableDataSource = "";

            $scope.scopeModel.onGridReady = function (api) {

                gridApi = api;
                gridApi.load();
            };

            $scope.scopeModel.onDesignTabSelected = function () {
                if ($scope.scopeModel.tableDataSource) {
                    designs = [];
                    try {
                        generatedScripts = JSON.parse($scope.scopeModel.tableDataSource);
                        for (var k = 0; k < generatedScripts.length; k++) {
                             generatedScript = generatedScripts[k];
                             console.log(generatedScript)
                            designs.push({ Entity: generatedScripts[k] });
                            $scope.scopeModel.ExceptionMessage = VR_Tools_ColumnsAPIService.GetExceptionMessage(generatedScript);

                        }

                        gridApi.load({ designs: designs });
                    }
                    catch (error) {

                    }
                }
            };

            $scope.scopeModel.onSourceTabSelected = function () {
                if (gridApi && gridApi.getData().length != 0) {
                    generatedScripts = [];
                    designs = gridApi.getData();
                    for (var i = 0; i < designs.length; i++) {
                        var design = designs[i];

                        generatedScripts.push(design.Entity);
                    }
                    $scope.scopeModel.tableDataSource = angular.toJson(generatedScripts);
                }
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