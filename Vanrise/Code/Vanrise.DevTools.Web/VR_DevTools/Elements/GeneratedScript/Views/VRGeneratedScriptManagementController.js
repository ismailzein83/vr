(function (appControllers) {
    "use strict";

    generatedScriptManagementController.$inject = ['$scope', 'VR_Devtools_ColumnsAPIService', 'VR_Devtools_GeneratedScriptService'];

    function generatedScriptManagementController($scope, VR_Devtools_ColumnsAPIService, VR_Devtools_GeneratedScriptService) {
        var gridApi;
        var isDynamicallyChanged = false;
        var designs = [];
        var generatedScripts = [];

        defineScope();

        function defineScope() {
            $scope.scopeModel = {};

            var queriesTypeSelectorDirectiveApi;
            var isCompareToTabSelected = false;
            var isSourceTabSelected = false;
            var isDesignTabSelected = false;

            $scope.scopeModel.tableDataSource = "";
            $scope.scopeModel.Errors = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;
                gridApi.load();
            };

            $scope.scopeModel.compareScripts = function () {
                $scope.scopeModel.isLoading = true;
                VR_Devtools_ColumnsAPIService.CompareJsonScripts({ NewScripts: $scope.scopeModel.tableDataSource, OldScripts: $scope.scopeModel.compareToDataSource }).then(function (response) {

                    if (response == undefined)
                        $scope.scopeModel.scriptsDifferneces = "No Differences";
                    else
                        $scope.scopeModel.scriptsDifferneces = response;
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };

            $scope.scopeModel.onGeneratedScriptTypeSelectorReady = function (api) {
                queriesTypeSelectorDirectiveApi = api;
                api.load();
            };

            $scope.scopeModel.generateQueries = function () {
                var scripts = [];
                $scope.scopeModel.isLoading = true;

                if (isCompareToTabSelected) {
                    scripts = JSON.parse($scope.scopeModel.scriptsDifferneces).Scripts;
                }

                else {
                    var generatedScriptDesign = gridApi.getData();

                    for (var i = 0; i < generatedScriptDesign.length; i++) {
                        scripts.push(generatedScriptDesign[i].Entity);
                    }
                }
                VR_Devtools_ColumnsAPIService.GenerateQueries({ Tables: { Scripts: scripts }, Type: queriesTypeSelectorDirectiveApi.getSelectedIds() }).then(function (response) {
                    VR_Devtools_GeneratedScriptService.displayQueries(response);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                    });

                //VR_Devtools_ColumnsAPIService.GenerateQueriesFromTextFile("C:\\GenerateQueriesFromFilePath\test.txt", queriesTypeSelectorDirectiveApi.getSelectedIds()).then(function (response) {

                //    console.log(response);

                //});

            };

            $scope.scopeModel.disablegenerateQueries = function () {
                var tableData = gridApi != undefined ? gridApi.getData() : undefined;

                if (isCompareToTabSelected && ($scope.scopeModel.scriptsDifferneces == undefined || $scope.scopeModel.scriptsDifferneces == "No Differences"))
                    return true;

                if (queriesTypeSelectorDirectiveApi != undefined && queriesTypeSelectorDirectiveApi.getSelectedIds() != undefined &&
                    (isCompareToTabSelected || ($scope.scopeModel.tableDataSource != undefined && $scope.scopeModel.Errors.length == 0 && tableData != undefined && tableData.length > 0))
                )
                    return false;

                return true;
            };

            $scope.scopeModel.onDesignTabSelected = function () {
                if (isSourceTabSelected)
                    loadGrid();

                isDesignTabSelected = true;
                isCompareToTabSelected = false;
                isSourceTabSelected = false;
            };

            $scope.scopeModel.onSourceTabSelected = function () {
                if (isDesignTabSelected)
                    stringifyDesigns();

                isDesignTabSelected = false;
                isCompareToTabSelected = false;
                isSourceTabSelected = true;
            };

            $scope.scopeModel.onCompareToTabSelected = function () {
                isCompareToTabSelected = true;

                if (isSourceTabSelected) {
                    isSourceTabSelected = false;
                    loadGrid();
                }
                if (isDesignTabSelected) {
                    isDesignTabSelected = false;
                    stringifyDesigns();
                }
            };
        }
        function loadGrid()  {
            if ($scope.scopeModel.tableDataSource) {
                $scope.scopeModel.isLoading = true;
                designs = [];

                try {
                    isDynamicallyChanged = false;
                    $scope.scopeModel.Errors = [];

                    generatedScripts = JSON.parse($scope.scopeModel.tableDataSource).Scripts;
                    VR_Devtools_ColumnsAPIService.Validate({ Scripts: generatedScripts }).then(function (response) {

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
                                    designs.push(generatedScripts[k]);
                                }

                                gridApi.load({ designs: designs });
                            }
                        }
                    }).finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });

                }
                catch (error) { $scope.scopeModel.isLoading = false; }
            }

            else {
                gridApi.load();
            }
        }
        function stringifyDesigns() {
            $scope.scopeModel.isLoading = true;

            if (!isDynamicallyChanged && gridApi) {
                generatedScripts = [];
                designs = gridApi.getData();

                if (designs != undefined && designs.length > 0)
                    for (var i = 0; i < designs.length; i++) {
                        generatedScripts.push(designs[i].Entity);
                    }
                if (generatedScripts.length > 0)
                    $scope.scopeModel.tableDataSource = JSON.stringify({ Scripts: generatedScripts }, null, 2);
                else
                    $scope.scopeModel.tableDataSource = undefined;
            }
            $scope.scopeModel.isLoading = false;
        }
    }

    appControllers.controller('VR_Devtools_GeneratedScriptManagementController', generatedScriptManagementController);
})(appControllers);