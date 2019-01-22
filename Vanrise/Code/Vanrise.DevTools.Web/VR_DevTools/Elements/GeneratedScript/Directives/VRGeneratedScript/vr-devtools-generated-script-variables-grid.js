appControllers.directive("vrDevtoolsGeneratedScriptVariablesGrid", ["UtilsService", "VRNotificationService", "VR_Devtools_GeneratedScriptService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
    function (UtilsService, VRNotificationService, VR_Devtools_GeneratedScriptService, VRUIUtilsService, VRCommon_ObjectTrackingService) {
        "use strict";

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var generatedScriptVariablesGrid = new GeneratedScriptVariablesGrid($scope, ctrl, $attrs);
                generatedScriptVariablesGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/VR_DevTools/Elements/GeneratedScript/Directives/VRGeneratedScript/Templates/VRGeneratedScriptVariablesGridTemplate.html"
        };

        function GeneratedScriptVariablesGrid($scope, ctrl) {


            var gridApi;
            var addText;
            var generatedScripts;
            var connectionStringId;
            $scope.scopeModel = {};
            $scope.scopeModel.variables = [];

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
                            $scope.scopeModel.variables = [];
                            if (payload != undefined) {
                                connectionStringId = payload.connectionStringId;

                                if (payload.variables != undefined) {

                                    for (var i = 0; i < payload.variables.length; i++) {
                                        var variableEntity = payload.variables[i];
                                        $scope.scopeModel.variables.push({ Entity: variableEntity });
                                    }
                                }
                            }

                            return UtilsService.waitMultiplePromises(promises);
                        };

                        directiveApi.getData = function () {
                            var variables = [];
                            for (var i = 0; i < $scope.scopeModel.variables.length; i++) {
                                var variable = $scope.scopeModel.variables[i].Entity;
                                variables.push(variable);
                            }
                            return variables;

                        };
                        return directiveApi;
                    }
                };


                $scope.scopeModel.onGeneratedScriptVariableAdded = function () {
                    var onGeneratedScriptVariableAdded = function (variable) {
                        $scope.scopeModel.variables.push(variable);
                    };

                    VR_Devtools_GeneratedScriptService.addGeneratedScriptVariable(onGeneratedScriptVariableAdded, connectionStringId);
                };

                defineMenuActions();
            }

            function defineMenuActions() {
                $scope.scopeModel.gridMenuActions = [{
                    name: "Edit",
                    clicked: editGeneratedScriptVariable,
                }];
            }

            function editGeneratedScriptVariable(generatedScriptVariable) {

                var index = $scope.scopeModel.variables.indexOf(generatedScriptVariable);
                var onGeneratedScriptVariableUpdated = function (variable) {
                    $scope.scopeModel.variables[index] = variable;
                };

                VR_Devtools_GeneratedScriptService.editGeneratedScriptVariable(onGeneratedScriptVariableUpdated, generatedScriptVariable.Entity, connectionStringId);
            }

        }

        return directiveDefinitionObject;

    }]);
