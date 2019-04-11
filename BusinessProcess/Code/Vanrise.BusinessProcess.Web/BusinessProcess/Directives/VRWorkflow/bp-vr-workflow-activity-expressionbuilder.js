'use strict';

app.directive('businessprocessVrWorkflowActivityExpressionbuilder', ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowService', 'BusinessProcess_CustomCodeTaskService',
	function (UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowService, BusinessProcess_CustomCodeTaskService) {

		var directiveDefinitionObject = {
			restrict: 'E',
			scope: {
				isrequired: '=',
				value: '=',
				label: '@',
				getworkflowarguments: '=',
				getactivityvariables: '=',
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
			templateUrl: '/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowActivityExpressionBuilderTemplate.html'
		};

		function workflowAssign(ctrl, $scope, $attrs) {

			this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};

				if (ctrl.label == undefined)
                    ctrl.label = "Value";
                setTimeout(function () {
                    if (ctrl.value != undefined) {
                        $scope.scopeModel.codeExpression = ctrl.value.CodeExpression; 
                        $scope.$apply();
                    }
                    $scope.$watch('ctrl.value', function (newValue) {
                            $scope.scopeModel.codeExpression = newValue != undefined ? newValue.CodeExpression : undefined;
                    });
                    $scope.$watch('scopeModel.codeExpression', function (newCodeExpression) {
                        if (newCodeExpression == undefined || newCodeExpression == '') {
                            ctrl.value = undefined;
                        }
                        else {
                            ctrl.value = {
                                "$type": "Vanrise.BusinessProcess.Entities.VRWorkflowCodeExpression, Vanrise.BusinessProcess.Entities",
                                "CodeExpression": newCodeExpression
                            };
                        }
                    });

                }, 500);
               
               
                
				
				$scope.scopeModel.openExpressionBuilder = function () {
					var args = (ctrl.getworkflowarguments != undefined) ? ctrl.getworkflowarguments() : undefined;
					var vars = (ctrl.getactivityvariables != undefined) ? ctrl.getactivityvariables() : undefined;
					var onSetExpressionBuilder = function (expressionBuilderValue) {
                        $scope.scopeModel.codeExpression = expressionBuilderValue;
					};
                    BusinessProcess_VRWorkflowService.openExpressionBuilder(onSetExpressionBuilder, args, vars, $scope.scopeModel.codeExpression);
				};
			}
		}
		return directiveDefinitionObject;
	}]);