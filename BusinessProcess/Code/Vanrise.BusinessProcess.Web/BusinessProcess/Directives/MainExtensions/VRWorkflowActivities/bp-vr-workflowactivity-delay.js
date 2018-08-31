'use strict';

app.directive('businessprocessVrWorkflowactivityDelay', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

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
                $scope.scopeModel.isVRWorkflowActivityDisabled = false;
                defineAPI();
            }

            function defineAPI() {
                var api = {};

				api.load = function (payload) {
					var promises = [];
                    if (payload != undefined) {
                        if (payload.Settings != undefined) {
                            $scope.scopeModel.isVRWorkflowActivityDisabled = payload.Settings.IsDisabled;
                            $scope.scopeModel.delay = payload.Settings.Delay;
                        }

                        if (payload.Context != null)
                            $scope.scopeModel.context = payload.Context;
					}
					return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowDelayActivity, Vanrise.BusinessProcess.MainExtensions",
                        Delay: $scope.scopeModel.delay,
                    };
                };

                api.changeActivityStatus = function (isVRWorkflowActivityDisabled) {
                    $scope.scopeModel.isVRWorkflowActivityDisabled = isVRWorkflowActivityDisabled;
                };

                api.getActivityStatus = function () {
                    return $scope.scopeModel.isVRWorkflowActivityDisabled;
                };

                api.isActivityValid = function () {
                    return $scope.scopeModel.delay != undefined;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }]);