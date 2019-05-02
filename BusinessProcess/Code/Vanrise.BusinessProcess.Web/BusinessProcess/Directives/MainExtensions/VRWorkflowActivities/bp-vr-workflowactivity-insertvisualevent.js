'use strict';

app.directive('businessprocessVrWorkflowactivityInsertvisualevent', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

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

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.isVRWorkflowActivityDisabled = false;
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var initialPromises = [];
                    if (payload != undefined) {
                        if (payload.Settings != undefined) {
                            $scope.scopeModel.isVRWorkflowActivityDisabled = payload.Settings.IsDisabled;
                            $scope.scopeModel.displayName = payload.Settings.DisplayName;
                            $scope.scopeModel.eventTitle = payload.Settings.EventTitle;
                        }

                        if (payload.Context != null)
                            $scope.scopeModel.context = payload.Context;
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
                        EventTitle: $scope.scopeModel.eventTitle
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }]);