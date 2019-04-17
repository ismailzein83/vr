(function (app) {

    'use strict';

    VisualItemDefinitonHumanTask.$inject = ['UtilsService', 'VRUIUtilsService', 'VisualEventTypeEnum', 'BusinessProcess_BPVisualItemDefintionService'];

    function VisualItemDefinitonHumanTask(UtilsService, VRUIUtilsService, VisualEventTypeEnum, BusinessProcess_BPVisualItemDefintionService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var visualItemDefinitonHumanTask = new VisualItemDefinitonHumanTaskController($scope, ctrl, $attrs);
                visualItemDefinitonHumanTask.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/BusinessProcess/Directives/BPVisualItemDefinition/Templates/VisualItemDefintionHumanTaskWorkflowTemplate.html"
        };

        function VisualItemDefinitonHumanTaskController($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var visualItemDefinition;
            var events = [];
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.classEventStarted = false;
                $scope.scopeModel.classEventCompleted = false;
                $scope.scopeModel.classEventError = false;
                $scope.scopeModel.classEventRetrying = false;
                $scope.scopeModel.isHintStarted = false;
                $scope.scopeModel.hint = "Not Started";


                $scope.scopeModel.onHumanTasksClick = function () {
                    if ($scope.scopeModel.isHintStarted) {
                        BusinessProcess_BPVisualItemDefintionService.openHumanTaskTrackingProgress(events);
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];

                    if (payload != undefined) {
                        visualItemDefinition = payload.visualItemDefinition;
                        $scope.scopeModel.displayName = visualItemDefinition.Settings.TaskName;
                    }

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];

                            return {
                                promises: directivePromises
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.tryApplyVisualEvent = function (visualItemEvent) {
                    if (visualItemEvent != undefined) {
                        events.push(visualItemEvent);
                        var eventTypeId = visualItemEvent.EventTypeId;

                        if (eventTypeId == VisualEventTypeEnum.Started.value.toLowerCase()) {
                            $scope.scopeModel.classEventStarted = true;
                            $scope.scopeModel.isHintStarted = true;
                            $scope.scopeModel.hint = "Started";
                        }
                        else if (eventTypeId == VisualEventTypeEnum.Completed.value.toLowerCase()) {
                            $scope.scopeModel.classEventStarted = false;
                            $scope.scopeModel.classEventCompleted = true;
                            $scope.scopeModel.isHintStarted = true;
                            $scope.scopeModel.hint = "Completed";
                          
                        }
                        else if (eventTypeId == VisualEventTypeEnum.Error.value.toLowerCase()) {
                            $scope.scopeModel.classEventStarted = false;
                            $scope.scopeModel.classEventError = true;
                            $scope.scopeModel.isHintStarted = true;
                            $scope.scopeModel.hint = "Error";
                        }
                        else if (eventTypeId == VisualEventTypeEnum.Retrying.value.toLowerCase()) {
                            $scope.scopeModel.classEventStarted = false;
                            $scope.scopeModel.classEventRetrying = true;
                            $scope.scopeModel.isHintStarted = true;
                            $scope.scopeModel.hint = "Retrying";
                        }
                        return true;
                    }
                    return false;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('bpWorkflowActivitysettingsVisualitemdefinitonHumantask', VisualItemDefinitonHumanTask);

})(app);