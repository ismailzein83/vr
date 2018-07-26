'use strict';

app.directive('businessprocessVrWorkflowWritetrackingmessage', ['UtilsService', 'VRUIUtilsService', 'VRWorkflowTrackingMessageSeverityEnum',
	function (UtilsService, VRUIUtilsService, VRWorkflowTrackingMessageSeverityEnum) {

	    var directiveDefinitionObject = {
	        restrict: 'E',
	        scope: {
	            onReady: '='
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
	        templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowWriteTrackingMessageTemplate.html'
	    };

	    function workflowAssign(ctrl, $scope, $attrs) {

	        this.initializeController = initializeController;
	        function initializeController() {
	            $scope.scopeModel = {};
	            $scope.scopeModel.severityEnums = UtilsService.getArrayEnum(VRWorkflowTrackingMessageSeverityEnum);
	            defineAPI();
	        }

	        function defineAPI() {
	            var api = {};

	            api.load = function (payload) {
	                var selectedSeverity;
	                if (payload != undefined) {
	                    if (payload.Settings != undefined) {
	                        $scope.scopeModel.message = payload.Settings.Message;
	                        selectedSeverity = payload.Settings.Severity;
	                    }

	                    if (payload.Context != null)
	                        $scope.scopeModel.context = payload.Context;
	                }

	                if (selectedSeverity != undefined)
	                    $scope.scopeModel.selectedSeverity = UtilsService.getItemByVal($scope.scopeModel.severityEnums, selectedSeverity, "value");
	                else
	                    $scope.scopeModel.selectedSeverity = $scope.scopeModel.severityEnums[0];
	            };

	            api.getData = function () {
	                return {
	                    $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowWriteTrackingMessageActivity, Vanrise.BusinessProcess.MainExtensions",
	                    Message: $scope.scopeModel.message,
	                    Severity: $scope.scopeModel.selectedSeverity.value
	                };
	            };

	            if (ctrl.onReady != null)
	                ctrl.onReady(api);
	        }
	    }
	    return directiveDefinitionObject;
	}]);