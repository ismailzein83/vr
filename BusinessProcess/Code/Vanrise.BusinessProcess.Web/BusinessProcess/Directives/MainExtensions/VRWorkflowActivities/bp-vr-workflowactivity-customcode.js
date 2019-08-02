'use strict';

app.directive('businessprocessVrWorkflowactivityCustomcode', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

	    var directiveDefinitionObject = {
	        restrict: 'E',
	        scope: {
	            onReady: '=',
	            isrequired: '='
	        },
	        controller: function ($scope, $element, $attrs) {
	            var ctrl = this;
	            var ctor = new workflowCustomCode(ctrl, $scope, $attrs);
	            ctor.initializeController();
	        },
	        controllerAs: 'ctrl',
	        bindToController: true,
	        compile: function (element, attrs) {

	        },
	        templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowCustomCodeTemplate.html'
	    };

	    function workflowCustomCode(ctrl, $scope, $attrs) {

            this.initializeController = initializeController;
            var codeExpressionBuilderDirectiveAPI;
            var codeExpressionBuilderPromiseReadyDeffered = UtilsService.createPromiseDeferred();
            var settings;
            var context;
	        function initializeController() {
	            $scope.scopeModel = {};
                $scope.scopeModel.isVRWorkflowActivityDisabled = false;
                $scope.scopeModel.onCodeExpressionBuilderDirectiveReady = function (api) {
                    codeExpressionBuilderDirectiveAPI = api;
                    codeExpressionBuilderPromiseReadyDeffered.resolve();
                };
	            defineAPI();
            }
            function loadCodeExpressionBuilder() {

                var codeExpressionBuilderPromiseLoadDeffered = UtilsService.createPromiseDeferred();
                codeExpressionBuilderPromiseReadyDeffered.promise.then(function () {
                    var payload = {
                        context: context,
                        value: settings != undefined ? settings.Code : undefined,
                    };
                    VRUIUtilsService.callDirectiveLoad(codeExpressionBuilderDirectiveAPI, payload, codeExpressionBuilderPromiseLoadDeffered);
                });
                return codeExpressionBuilderPromiseLoadDeffered.promise;
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
                        promises.push(loadCodeExpressionBuilder());
                    }
                    return UtilsService.waitPromiseNode({ promises: promises });
                };

	            api.getData = function () {
	                return {
                        $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowCustomLogicActivity, Vanrise.BusinessProcess.MainExtensions",
                        Code: codeExpressionBuilderDirectiveAPI != undefined ? codeExpressionBuilderDirectiveAPI.getData() : undefined
	                };
	            };

	            api.changeActivityStatus = function (isVRWorkflowActivityDisabled) {
	                $scope.scopeModel.isVRWorkflowActivityDisabled = isVRWorkflowActivityDisabled;
	            };

	            api.getActivityStatus = function () {
	                return $scope.scopeModel.isVRWorkflowActivityDisabled;
	            };

                api.isActivityValid = function () {
                    return codeExpressionBuilderDirectiveAPI != undefined && codeExpressionBuilderDirectiveAPI.getData() != undefined;
	            };

	            if (ctrl.onReady != null)
	                ctrl.onReady(api);
	        }
	    }
	    return directiveDefinitionObject;
	}]);