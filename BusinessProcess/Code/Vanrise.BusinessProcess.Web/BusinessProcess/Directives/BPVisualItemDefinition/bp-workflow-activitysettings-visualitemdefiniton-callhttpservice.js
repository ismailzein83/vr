(function (app) {

    'use strict';

    VisualItemDefinitonCallHttpService.$inject = ['UtilsService', 'VRUIUtilsService', 'VisualEventTypeEnum', 'BusinessProcess_BPVisualItemDefintionService'];

    function VisualItemDefinitonCallHttpService(UtilsService, VRUIUtilsService, VisualEventTypeEnum, BusinessProcess_BPVisualItemDefintionService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var visualItemDefinitonCallHttpService = new VisualItemDefinitonCallHttpServiceController($scope, ctrl, $attrs);
                visualItemDefinitonCallHttpService.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/BusinessProcess/Directives/BPVisualItemDefinition/Templates/VisualItemDefintionCallHttpServiceWorkflowTemplate.html"
        };

        function VisualItemDefinitonCallHttpServiceController($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var visualItemDefinition;
            var events = [];
            var result = {};

            function initializeController() {

                $(document).ready(function () {
                    $('[data-toggle="tooltip"]').tooltip();
                });

                $scope.scopeModel = {};
                $scope.scopeModel.classEventStarted = false;
                $scope.scopeModel.classEventCompleted = false;
                $scope.scopeModel.classEventError = false;
                $scope.scopeModel.classEventRetrying = false;
                $scope.scopeModel.isHintStarted = false;
                $scope.scopeModel.hint = "Not Started";
                $scope.scopeModel.retryCount = 0;

                $scope.scopeModel.onCallHttpClick = function () {
                    if ($scope.scopeModel.isHintStarted) {
                        BusinessProcess_BPVisualItemDefintionService.openEventsTracking(events, "Call Http Service Tracking Progress");
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
                        $scope.scopeModel.displayName = visualItemDefinition.Settings.DisplayName;
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
                result.isEventUsed = false; 

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
                            result.isCompleted = true;
                        }
                        else if (eventTypeId == VisualEventTypeEnum.Error.value.toLowerCase()) {
                            $scope.scopeModel.classEventStarted = false;
                            $scope.scopeModel.classEventRetrying = false;
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

                        if (visualItemEvent.EventPayload != undefined) {
                            $scope.scopeModel.retryCount = visualItemEvent.EventPayload.RetryCount + 1;
                            $scope.scopeModel.showRetryIcon = true;
                        }
                        result.isEventUsed = true; 
                    }
                    return result;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('bpWorkflowActivitysettingsVisualitemdefinitonCallhttpservice', VisualItemDefinitonCallHttpService);

})(app);