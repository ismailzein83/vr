'use strict';

app.directive('businessprocessVrWorkflowactivityDelay', ['UtilsService', 'VRUIUtilsService', 'VRWorkflowTimeUnitEnum',
	function (UtilsService, VRUIUtilsService, VRWorkflowTimeUnitEnum) {

	    var directiveDefinitionObject = {
	        restrict: 'E',
	        scope: {
	            onReady: '='
	        },
	        controller: function ($scope, $element, $attrs) {
	            var ctrl = this;
	            var ctor = new workflowDelay(ctrl, $scope, $attrs);
	            ctor.initializeController();
	        },
	        controllerAs: 'ctrl',
	        bindToController: true,
	        compile: function (element, attrs) {

	        },
	        templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowDelayTemplate.html'
	    };

	    function workflowDelay(ctrl, $scope, $attrs) {

	        this.initializeController = initializeController;
	        function initializeController() {
	            $scope.scopeModel = {};
	            $scope.scopeModel.timeUnitEnums = UtilsService.getArrayEnum(VRWorkflowTimeUnitEnum);

	            $scope.scopeModel.onTimeUnitSelectorReady = function (api) {
	                defineAPI();
	            };
	        }

	        function defineAPI() {
	            var api = {};

	            api.load = function (payload) {
	                var selectedTimeUnit;
	                if (payload != undefined) {
	                    if (payload.Settings != undefined) {
	                        $scope.scopeModel.delayTime = payload.Settings.DelayTime;
	                        selectedTimeUnit = payload.Settings.TimeUnit;
	                    }

	                    if (payload.Context != null)
	                        $scope.scopeModel.context = payload.Context;
	                }

	                if (selectedTimeUnit != undefined)
	                    $scope.scopeModel.selectedTimeUnit = UtilsService.getItemByVal($scope.scopeModel.timeUnitEnums, selectedTimeUnit, "value");
	                else
	                    $scope.scopeModel.selectedTimeUnit = $scope.scopeModel.timeUnitEnums[0];
	            };

	            api.getData = function () {
	                return {
	                    $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowDelayActivity, Vanrise.BusinessProcess.MainExtensions",
	                    DelayTime: $scope.scopeModel.delayTime,
	                    TimeUnit: $scope.scopeModel.selectedTimeUnit.value
	                };
	            };

	            if (ctrl.onReady != null)
	                ctrl.onReady(api);
	        }
	    }
	    return directiveDefinitionObject;
	}]);