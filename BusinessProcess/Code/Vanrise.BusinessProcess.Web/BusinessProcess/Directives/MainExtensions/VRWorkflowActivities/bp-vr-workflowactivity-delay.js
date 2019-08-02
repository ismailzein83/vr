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

            var delayExpressionBuilderDirectiveAPI;
            var delayExpressionBuilderPromiseReadyDeffered = UtilsService.createPromiseDeferred();
            var settings;
            var context;
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.isVRWorkflowActivityDisabled = false;

                $scope.scopeModel.onDelayExpressionBuilderDirectiveReady = function (api) {
                    delayExpressionBuilderDirectiveAPI = api;
                    delayExpressionBuilderPromiseReadyDeffered.resolve();
                };
                defineAPI();
            }
            function loadDelayExpressionBuilder() {

                var delayExpressionBuilderPromiseLoadDeffered = UtilsService.createPromiseDeferred();
                delayExpressionBuilderPromiseReadyDeffered.promise.then(function () {
                    var payload = {
                        context: context,
                        value: settings != undefined ? settings.Delay : undefined,
                    };
                    VRUIUtilsService.callDirectiveLoad(delayExpressionBuilderDirectiveAPI, payload, delayExpressionBuilderPromiseLoadDeffered);
                });
                return delayExpressionBuilderPromiseLoadDeffered.promise;
            }
            function defineAPI() {
                var api = {};

				api.load = function (payload) {
					var promises = [];
                    if (payload != undefined) {
                        settings = payload.Settings;
                        if (settings != undefined) {
                            $scope.scopeModel.isVRWorkflowActivityDisabled = settings.IsDisabled;
                        }

                        context = payload.Context;
                        promises.push(loadDelayExpressionBuilder());
					}
					return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowDelayActivity, Vanrise.BusinessProcess.MainExtensions",
                        Delay: delayExpressionBuilderDirectiveAPI != undefined ? delayExpressionBuilderDirectiveAPI.getData() : undefined,
                    };
                };

                api.changeActivityStatus = function (isVRWorkflowActivityDisabled) {
                    $scope.scopeModel.isVRWorkflowActivityDisabled = isVRWorkflowActivityDisabled;
                };

                api.getActivityStatus = function () {
                    return $scope.scopeModel.isVRWorkflowActivityDisabled;
                };

                api.isActivityValid = function () {
                    return delayExpressionBuilderDirectiveAPI != undefined && delayExpressionBuilderDirectiveAPI.getData() != undefined;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }]);