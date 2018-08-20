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
	            hidelabel: '='
	        },
	        controller: function ($scope, $element, $attrs) {
	            var ctrl = this;
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
	            if (ctrl.label == undefined)
	                ctrl.label = "Value";

	            $scope.scopeModel = {};
	            $scope.scopeModel.openExpressionBuilder = function () {
	                var args = (ctrl.getworkflowarguments != undefined) ? ctrl.getworkflowarguments() : undefined;
	                var vars = (ctrl.getactivityvariables != undefined) ? ctrl.getactivityvariables() : undefined;
	                var onSetExpressionBuilder = function (expressionBuilderValue) {
	                    ctrl.value = expressionBuilderValue;
	                };
	                BusinessProcess_VRWorkflowService.openExpressionBuilder(onSetExpressionBuilder, args, vars, ctrl.value);
	            };
	        }
	    }
	    return directiveDefinitionObject;
	}]);