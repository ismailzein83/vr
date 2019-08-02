'use strict';

app.directive('businessprocessVrWorkflowactivityWritetrackingmessage', ['UtilsService', 'VRUIUtilsService', 'VRWorkflowTrackingMessageSeverityEnum','VRCommon_FieldTypesService',
    function (UtilsService, VRUIUtilsService, VRWorkflowTrackingMessageSeverityEnum, VRCommon_FieldTypesService) {

		var directiveDefinitionObject = {
			restrict: 'E',
			scope: {
				onReady: '='
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new workflowTrackingMessage(ctrl, $scope, $attrs);
				ctor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			compile: function (element, attrs) {

			},
			templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowWriteTrackingMessageTemplate.html'
		};

		function workflowTrackingMessage(ctrl, $scope, $attrs) {
            var messageExpressionBuilderDirectiveAPI;
            var messageExpressionBuilderPromiseReadyDeffered = UtilsService.createPromiseDeferred();
            var settings;
            var context;
			this.initializeController = initializeController;
			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.isVRWorkflowActivityDisabled = false;
				$scope.scopeModel.severityEnums = UtilsService.getArrayEnum(VRWorkflowTrackingMessageSeverityEnum);

                $scope.scopeModel.onMessageExpressionBuilderDirectiveReady = function (api) {
                    messageExpressionBuilderDirectiveAPI = api;
                    messageExpressionBuilderPromiseReadyDeffered.resolve();
                };
				$scope.scopeModel.onSeveritySelectorReady = function (api) {
					defineAPI();
				};
			}

            function loadMessageExpressionBuilder() {

                var messageExpressionBuilderPromiseLoadDeffered = UtilsService.createPromiseDeferred();
                messageExpressionBuilderPromiseReadyDeffered.promise.then(function () {
                    var payload = {
                        context: context,
                        value: settings != undefined ? settings.Message : undefined,
                        fieldEntity: {
                            fieldType: VRCommon_FieldTypesService.getTextFieldType(),
                            fieldTitle: "Message"
                        }
                    };
                    VRUIUtilsService.callDirectiveLoad(messageExpressionBuilderDirectiveAPI, payload, messageExpressionBuilderPromiseLoadDeffered);
                });
                return messageExpressionBuilderPromiseLoadDeffered.promise;
            }
			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					var promises = [];
					var selectedSeverity;
                    if (payload != undefined) {
                        settings = payload.Settings;
                        if (settings!= undefined) {
                            $scope.scopeModel.isVRWorkflowActivityDisabled = settings.IsDisabled;
                            selectedSeverity = settings.Severity;
						}

						if (payload.Context != null)
						  context = payload.Context;
                    }
                    promises.push(loadMessageExpressionBuilder());

					if (selectedSeverity != undefined)
						$scope.scopeModel.selectedSeverity = UtilsService.getItemByVal($scope.scopeModel.severityEnums, selectedSeverity, "value");
					else
						$scope.scopeModel.selectedSeverity = $scope.scopeModel.severityEnums[0];

					return UtilsService.waitMultiplePromises(promises);
				};

				api.getData = function () {
					return {
                        $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowWriteTrackingMessageActivity, Vanrise.BusinessProcess.MainExtensions",
                        Message: messageExpressionBuilderDirectiveAPI != undefined ? messageExpressionBuilderDirectiveAPI.getData() : undefined,
						Severity: $scope.scopeModel.selectedSeverity.value
					};
				};

				api.changeActivityStatus = function (isVRWorkflowActivityDisabled) {
					$scope.scopeModel.isVRWorkflowActivityDisabled = isVRWorkflowActivityDisabled;
				};

				api.getActivityStatus = function () {
					return $scope.scopeModel.isVRWorkflowActivityDisabled;
				};

                api.isActivityValid = function () {
                    return messageExpressionBuilderDirectiveAPI != undefined && messageExpressionBuilderDirectiveAPI.getData() != undefined;
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}
		return directiveDefinitionObject;
	}]);