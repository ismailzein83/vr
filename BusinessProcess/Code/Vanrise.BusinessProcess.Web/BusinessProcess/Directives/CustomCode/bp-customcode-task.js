"use strict";

app.directive("bpCustomcodeTask", ['UtilsService', 'VRUIUtilsService', 'VRValidationService', 'VRNotificationService', 'BusinessProcess_CustomCodeTaskService', 'BusinessProcess_CustomCodeTaskAPIService',
    function (UtilsService, VRUIUtilsService, VRValidationService, VRNotificationService, BusinessProcess_CustomCodeTaskService, BusinessProcess_CustomCodeTaskAPIService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var directiveConstructor = new DirectiveConstructor($scope, ctrl);
                directiveConstructor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/BusinessProcess/Directives/CustomCode/Templates/CustomCodeTaskTemplate.html'
        };

        function DirectiveConstructor($scope, ctrl) {
            this.initializeController = initializeController;
            var context;
            var compiled = false;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onCompileClick = function () {
                    $scope.scopeModel.isLoadingDirective = true;
                    return tryCompile().then(function (response) {
                        $scope.scopeModel.isLoadingDirective = false;
                        if (response)
                            VRNotificationService.showSuccess("Custom Code compiled successfully.");
                    });
                };
                $scope.scopeModel.validateCustomCodeTask = function () {
                    if (compiled == false)
                        return "Custom Code must be compiled first.";
                    return null;
                };
                $scope.scopeModel.onTaskInputsValueChanged = function () {
                    compiled = false;
                };
                defineAPI();
            }

            function defineAPI() {

                var api = {};
                api.load = function (payload) {
                    $scope.scopeModel.isLoadingDirective = true;
                    if (payload != undefined) {
                        if (payload.data != undefined) {
                            $scope.scopeModel.classDefinitions = payload.data.ClassDefinitions;
                            $scope.scopeModel.taskCode = payload.data.TaskCode;
                            compiled = true;
                        }
                    }
                    context = payload.context;
                    $scope.scopeModel.isLoadingDirective = false;
                };


                api.getData = function () {
                    var name = getContext().getTaskName();
                    return {
                        $type: "Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments.CustomCodeBPArgument, Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments",
                        Name: name,
                        ClassDefinitions: $scope.scopeModel.classDefinitions,
                        TaskCode: $scope.scopeModel.taskCode
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function tryCompile() {
                return BusinessProcess_CustomCodeTaskAPIService.TryCompileCustomCodeTask(buildComipleObject()).then(function (response) {
                    if (response) {
                        if (response.Result) {
                            compiled = true;
                            return true;
                        } else {
                            compiled = false;
                            BusinessProcess_CustomCodeTaskService.tryCompilationResult(response.ErrorMessages);
                            return false;
                        }
                    }
                });
            }

            function buildComipleObject() {
                return {
                    TaskCode: $scope.scopeModel.taskCode,
                    ClassDefinitions: $scope.scopeModel.classDefinitions
                }
            }

            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }
        }

        return directiveDefinitionObject;
    }]);
