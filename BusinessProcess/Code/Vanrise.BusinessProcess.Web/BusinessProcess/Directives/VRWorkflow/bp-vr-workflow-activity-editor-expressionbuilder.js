'use strict';

app.directive('businessprocessVrWorkflowActivityEditorExpressionbuilder', ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowService',
    function (UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                isrequired: '=',
                onReady: '=',
                label: '@',
                value: '=',
                hidelabel: '=',
                nbOfRows: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                if (ctrl.nbOfRows != undefined)
                    $scope.nbOfRows = ctrl.nbOfRows;
                else
                    $scope.nbOfRows = 1;
                var ctor = new workflowAssign(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowActivityEditorExpressionBuilderTemplate.html'
        };

        function workflowAssign(ctrl, $scope, $attrs) {

            this.initializeController = initializeController;
            var context;
            var expression;
            var isDisabled = false;
            var fieldType;
            var fieldTitle;
            function initializeController() {
                $scope.scopeModel = {};

                if (ctrl.label == undefined)
                    ctrl.label = "Value";

                $scope.scopeModel.disableExpression = function () {
                    isDisabled = expression != undefined && expression.Value != undefined;
                    return isDisabled;
                };
                $scope.scopeModel.openExpressionBuilder = function () {

                    var onSetValue = function (value) {
                        expression = value;
                        if (value != undefined) {

                            if (value.CodeExpression != undefined)
                                $scope.scopeModel.expression = value.CodeExpression;
                            else if (expression.Value != undefined)
                                $scope.scopeModel.expression = value.Value;

                        }
                        else {
                            expression = {};
                            $scope.scopeModel.expression = undefined;
                        }
                       
                    }; 
                    var params = {
                        variables: context != undefined ? context.getParentVariables() : undefined,
                        arguments: context != undefined ? context.getWorkflowArguments() : undefined,
                        fieldType: fieldType,
                        fieldTitle: fieldTitle,
                        expression: getExpression()
                    };
                    BusinessProcess_VRWorkflowService.openExpressionEditorBuilder(onSetValue, params);
                };

                defineAPI();
            }
            function getExpression() {
                if ($scope.scopeModel.expression == undefined || $scope.scopeModel.expression == '')
                    expression = undefined;
                else if (!isDisabled) {
                    if (expression == undefined)
                        expression = {
                            $type: "Vanrise.BusinessProcess.Entities.VRWorkflowCodeExpression, Vanrise.BusinessProcess.Entities"
                        };
                    expression.CodeExpression = $scope.scopeModel.expression;
                }
                return expression;
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        expression = payload.value;
                        var fieldEntity = payload.fieldEntity;
                        if (fieldEntity != undefined) {
                            fieldType = fieldEntity.fieldType;
                            fieldTitle = fieldEntity.fieldTitle;
                        }
                    }
                    $scope.scopeModel.expression = undefined;

                    if (expression != undefined) {
                        if (expression.CodeExpression != undefined)
                            $scope.scopeModel.expression = expression.CodeExpression;
                        else if (expression.Value != undefined)
                            $scope.scopeModel.expression = expression.Value;
                    }
                    var rootPromiseNode = {
                        promises: []
                    };
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    return getExpression();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }]);