appControllers.directive("vrDevtoolsGeneratedScriptDesignGrid", ["UtilsService", "VR_Devtools_GeneratedScriptService", "VR_Devtools_ColumnsAPIService",
    function (UtilsService, VR_Devtools_GeneratedScriptService, VR_Devtools_ColumnsAPIService) {
        "use strict";

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var generatedScriptDesignGrid = new GeneratedScriptDesignGrid($scope, ctrl, $attrs);
                generatedScriptDesignGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/VR_DevTools/Elements/GeneratedScript/Directives/VRGeneratedScript/Templates/VRGeneratedScriptDesignGridTemplate.html"
        };

        function GeneratedScriptDesignGrid($scope, ctrl) {


            var gridApi;
            var addText;

            $scope.scopeModel = {};
            $scope.scopeModel.designs = [];

            this.initializeController = initializeController;

            function initializeController() {

                $scope.scopeModel.onGridReady = function (api) {
                    gridApi = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                        ctrl.onReady(getDirectiveApi());
                    }

                    function getDirectiveApi() {

                        var directiveApi = {};

                        directiveApi.load = function (payload) {
                            var promises = [];
                            $scope.scopeModel.designs = [];
                            if (payload != undefined && payload.designs != undefined) {
                                for (var i = 0; i < payload.designs.length; i++) {
                                    $scope.scopeModel.designs.push({ Entity: payload.designs[i] });
                                }
                            }
                            return UtilsService.waitMultiplePromises(promises);
                        };

                        directiveApi.getData = function () {
                            return $scope.scopeModel.designs;

                        };
                        return directiveApi;
                    }
                };
                $scope.scopeModel.getRowStyle = function (row) {
                    var rowStyle;
                    if (row.IsSimilar==false)
                        rowStyle = { CssClass: 'alert-danger' };
                    return rowStyle;
                };

                $scope.scopeModel.editGeneratedScriptDesign = function (Design) {

                    var index = $scope.scopeModel.designs.indexOf(Design);
                    var onGeneratedScriptDesignUpdated = function (design) {
                        $scope.scopeModel.designs[index] = design;
                        $scope.scopeModel.compareItems();
                    };

                    VR_Devtools_GeneratedScriptService.editGeneratedScriptDesign(onGeneratedScriptDesignUpdated, Design);
                }
                $scope.scopeModel.compareItems = function () {
                    $scope.scopeModel.isLoading = true;
                    var designs=[];
                    for (var j = 0; j < $scope.scopeModel.designs.length; j++) {
                        designs.push($scope.scopeModel.designs[j].Entity);
                    }
                    VR_Devtools_ColumnsAPIService.CompareItems({ Tables: { Scripts: designs }, IncludeOverriddenValuesInComparison: $scope.scopeModel.includeOverriddenValuesInComparison }).then(function (response) {
                        if (response != undefined && response.length > 0) {
                            for (var i = 0; i < response.length; i++) {
                                var output = response[i];
                                $scope.scopeModel.designs[i].IsSimilar = output.IsSimilar;
                                $scope.scopeModel.designs[i].Message = output.Message;
                            }
                        }
                    }).finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                };
                $scope.scopeModel.onDeleteRow = function (dataItem) {
                    var index = $scope.scopeModel.designs.indexOf(dataItem);
                    $scope.scopeModel.designs.splice(index, 1);
                };

                $scope.scopeModel.onGeneratedScriptDesignAdded = function () {
                    var onGeneratedScriptDesignAdded = function (design) {
                        $scope.scopeModel.designs.push(design);
                        $scope.scopeModel.compareItems();
                    };

                    VR_Devtools_GeneratedScriptService.addGeneratedScriptDesign(onGeneratedScriptDesignAdded);
                };
                $scope.scopeModel.addTemplate = function () {
                    var onTemplateAdded = function (designs) {
                        for (var design in designs) {
                            $scope.scopeModel.designs.push(design);
                        }
                    };
                    VR_Devtools_GeneratedScriptService.addTemplate(onTemplateAdded);
                };
            }

        }

        return directiveDefinitionObject;

    }]);
