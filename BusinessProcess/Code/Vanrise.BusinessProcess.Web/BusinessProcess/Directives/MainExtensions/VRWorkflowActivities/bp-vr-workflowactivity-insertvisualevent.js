'use strict';

app.directive('businessprocessVrWorkflowactivityInsertvisualevent', ['UtilsService', 'VRUIUtilsService','VRCommon_FieldTypesService',
    function (UtilsService, VRUIUtilsService, VRCommon_FieldTypesService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new workflowInsertVisualEvent(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowInsertVisualEventTemplate.html'
        };

        function workflowInsertVisualEvent(ctrl, $scope, $attrs) {

            var eventTitleExpressionBuilderDirectiveAPI;
            var eventTitleExpressionBuilderPromiseReadyDeffered = UtilsService.createPromiseDeferred();
            var settings;
            var context;
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onEventTitleExpressionBuilderDirectiveReady = function (api) {
                    eventTitleExpressionBuilderDirectiveAPI = api;
                    eventTitleExpressionBuilderPromiseReadyDeffered.resolve();
                };
                $scope.scopeModel.isVRWorkflowActivityDisabled = false;
                defineAPI();
            }
            function loadEventTitleExpressionBuilder() {

                var eventTitleExpressionBuilderPromiseLoadDeffered = UtilsService.createPromiseDeferred();
                eventTitleExpressionBuilderPromiseReadyDeffered.promise.then(function () {
                    var payload = {
                        context: context,
                        value: settings != undefined ? settings.EventTitle : undefined,
                        fieldEntity: {
                            fieldType: VRCommon_FieldTypesService.getTextFieldType(),
                            fieldTitle: "Event Title"
                        }
                    };
                    VRUIUtilsService.callDirectiveLoad(eventTitleExpressionBuilderDirectiveAPI, payload, eventTitleExpressionBuilderPromiseLoadDeffered);
                });
                return eventTitleExpressionBuilderPromiseLoadDeffered.promise;
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var initialPromises = [];
                    if (payload != undefined) {
                        settings = payload.Settings;
                        if (payload != undefined) {
                            $scope.scopeModel.isVRWorkflowActivityDisabled = settings.IsDisabled;
                            $scope.scopeModel.displayName = settings.DisplayName;
                        }

                        if (payload.Context != null)
                            context = payload.Context;
                        initialPromises.push(loadEventTitleExpressionBuilder());
                    }
                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            return {
                                promises: []
                            };
                        }
                    };
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowInsertVisualEventActivity, Vanrise.BusinessProcess.MainExtensions",
                        DisplayName: $scope.scopeModel.displayName,
                        EventTitle: eventTitleExpressionBuilderDirectiveAPI != undefined ? eventTitleExpressionBuilderDirectiveAPI.getData() : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }]);