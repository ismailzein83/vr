(function (appControllers) {
    "use strict";

    generatedScriptManagementController.$inject = ['$scope', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VR_Tools_ColumnsAPIService','VR_Tools_GeneratedScriptService'];

    function generatedScriptManagementController($scope, VRNotificationService, UtilsService, VRUIUtilsService, VR_Tools_ColumnsAPIService, VR_Tools_GeneratedScriptService) {
        var gridApi;
        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};
            var generatedScripts = [];
            var designs = [];
            var generatedScript;
            var queriesTypeSelectorDirectiveApi;
            var isDynamicallyChanged=false;
            $scope.scopeModel.tableDataSource = "";
            $scope.scopeModel.Errors = [];

            $scope.scopeModel.onGridReady = function (api) {

                gridApi = api;
                gridApi.load();
            };

            $scope.scopeModel.onGeneratedScriptTypeSelectorReady = function (api) {
                queriesTypeSelectorDirectiveApi = api;
                api.load();
            };

            $scope.scopeModel.generateQueries = function () {

                var generatedScriptDesign = gridApi.getData();
                var scripts = [];
                for (var i = 0; i < generatedScriptDesign.length; i++) {
                    scripts.push(generatedScriptDesign[i].Entity);
                }

                VR_Tools_ColumnsAPIService.GenerateQueries({ Tables: { Scripts: scripts }, Type: queriesTypeSelectorDirectiveApi.getSelectedIds() }).then(function (response) {
                    VR_Tools_GeneratedScriptService.displayQueries(response);
                });

                //VR_Tools_ColumnsAPIService.GenerateQueriesFromTextFile("C:\\GenerateQueriesFromFilePath\test.txt", queriesTypeSelectorDirectiveApi.getSelectedIds()).then(function (response) {

                //    console.log(response);

                //});

            };

            $scope.scopeModel.disablegenerateQueries = function () {
                return (queriesTypeSelectorDirectiveApi != undefined && queriesTypeSelectorDirectiveApi.getSelectedIds() != undefined && $scope.scopeModel.tableDataSource != undefined && $scope.scopeModel.Errors.length==0) ? false : true;
            };

            $scope.scopeModel.onDesignTabSelected = function () {
                if ($scope.scopeModel.tableDataSource) {
                    $scope.scopeModel.isLoading = true;
                    designs = [];
                    try {
                        isDynamicallyChanged = false;
                        $scope.scopeModel.Errors = [];

                        generatedScripts = JSON.parse($scope.scopeModel.tableDataSource).Scripts; 
                        VR_Tools_ColumnsAPIService.Validate({ Scripts: generatedScripts }).then(function (response) {
                            if (response) {
                                for (var j = 0; j < response.length; j++) {
                                    if (!response[j].IsValidated) {
                                        $scope.scopeModel.Errors.push({ Errors: response[j].Errors, TableTitle: response[j].TableTitle });
                                    }
                                }
                                if ($scope.scopeModel.Errors.length != 0) {
                                    $scope.scopeModel.sourceTab.isSelected = true;
                                    isDynamicallyChanged = true;
                                }
                                else {
                                    for (var k = 0; k < generatedScripts.length; k++) {
                                        generatedScript = generatedScripts[k];
                                        designs.push({ Entity: generatedScripts[k] });
                                    }
                                    gridApi.load({ designs: designs });
                                }
                            }
                            $scope.scopeModel.isLoading = false;
                        });
                        
                    }
                    catch (error) { $scope.scopeModel.isLoading = false; }
                }
            };

            $scope.scopeModel.onSourceTabSelected = function () {
                if (!isDynamicallyChanged && gridApi && gridApi.getData().length != 0) {
                    generatedScripts = [];
                    designs = gridApi.getData();
                    for (var i = 0; i < designs.length; i++) {
                        generatedScripts.push(designs[i].Entity);
                    }
                    $scope.scopeModel.tableDataSource = angular.toJson({Scripts:generatedScripts});
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

    }

    appControllers.controller('VR_Tools_GeneratedScriptManagementController', generatedScriptManagementController);
})(appControllers);