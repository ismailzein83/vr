(function (app) {

    'use strict';

    VisualItemDefinitonInsertVisualItem.$inject = ['UtilsService', 'VRUIUtilsService', 'VisualEventTypeEnum', 'BusinessProcess_BPVisualItemDefintionService'];

    function VisualItemDefinitonInsertVisualItem(UtilsService, VRUIUtilsService, VisualEventTypeEnum, BusinessProcess_BPVisualItemDefintionService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var visualItemDefinitonInsertVisualItem = new VisualItemDefinitonInsertVisualItemController($scope, ctrl, $attrs);
                visualItemDefinitonInsertVisualItem.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/BusinessProcess/Directives/BPVisualItemDefinition/Templates/VisualItemDefintionInsertVisualItemTemplate.html"
        };

        function VisualItemDefinitonInsertVisualItemController($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var visualItemDefinition;
            var events = [];
            var isCompleted = false;

            function initializeController() {

                $(document).ready(function () {
                    $('[data-toggle="tooltip"]').tooltip();
                });

                $scope.scopeModel = {};
                $scope.scopeModel.classEventStarted = false;
                $scope.scopeModel.isHintStarted = false;
                $scope.scopeModel.classEventCompleted = false;
                $scope.scopeModel.classEventError = false;
                $scope.scopeModel.classEventRetrying = false;
                $scope.scopeModel.hint = "Not Started";

                $scope.scopeModel.onInsertVisualEventActivityClick = function () {
                    if ($scope.scopeModel.isHintStarted) {
                        BusinessProcess_BPVisualItemDefintionService.openEventsTracking(events, "Insert Visual Item Tracking Progress");
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

                api.reload = function () {
                    events = [];
                    isCompleted = false;
                    $scope.scopeModel.classEventStarted = false;
                    $scope.scopeModel.isHintStarted = false;
                    $scope.scopeModel.classEventCompleted = false;
                    $scope.scopeModel.classEventError = false;
                    $scope.scopeModel.classEventRetrying = false;
                    $scope.scopeModel.hint = "Not Started";

                };

                api.tryApplyVisualEvent = function (visualItemEvent) {
                    var result = {};
                    result.isEventUsed = false;

                    if (visualItemEvent != undefined) {
                        var eventTypeId = visualItemEvent.EventTypeId;

                        if (eventTypeId == VisualEventTypeEnum.Started.value.toLowerCase()) {
                            $scope.scopeModel.classEventStarted = true;
                            $scope.scopeModel.isHintStarted = true;
                            $scope.scopeModel.classEventCompleted = false;
                            $scope.scopeModel.classEventError = false;
                            $scope.scopeModel.classEventRetrying = false;
                            $scope.scopeModel.hint = "Started";
                        }
                        else if (eventTypeId == VisualEventTypeEnum.Completed.value.toLowerCase()) {
                            $scope.scopeModel.classEventStarted = false;
                            $scope.scopeModel.classEventCompleted = true;
                            $scope.scopeModel.isHintStarted = true;
                            $scope.scopeModel.hint = "Completed";
                            isCompleted = true;
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
                        result.isEventUsed = true;
                    }
                    return result;
                };

                api.checkIfCompleted = function () {
                    return isCompleted;
                };

                api.onAfterCompleted = function () {

                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('bpWorkflowActivitysettingsVisualitemdefinitonInsertvisualitem', VisualItemDefinitonInsertVisualItem);

})(app);