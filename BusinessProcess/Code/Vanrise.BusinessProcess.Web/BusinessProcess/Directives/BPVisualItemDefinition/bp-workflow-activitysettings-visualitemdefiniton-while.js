(function (app) {

    'use strict';

    VisualItemDefinitonWhile.$inject = ['UtilsService', 'VRUIUtilsService', 'VisualEventTypeEnum'];

    function VisualItemDefinitonWhile(UtilsService, VRUIUtilsService, VisualEventTypeEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var visualItemDefinitonWhile = new VisualItemDefinitonWhileController($scope, ctrl, $attrs);
                visualItemDefinitonWhile.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/BusinessProcess/Directives/BPVisualItemDefinition/Templates/VisualItemDefintionWhileWorkflowTemplate.html"
        };

        function VisualItemDefinitonWhileController($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var visualItemDefinition;
            var directiveEditor;
            var childDirectiveAPI;
            var childDirectivePromiseReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onChildVisualItemReady = function (api) {
                    childDirectiveAPI = api;
                    childDirectivePromiseReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};
               
                api.load = function (payload) {
                    var initialPromises = [];

                    $scope.scopeModel.nbOfIteration = 0;

                    if (payload != undefined) {
                        visualItemDefinition = payload.visualItemDefinition;
                        $scope.scopeModel.conditionDescription = visualItemDefinition.Settings.ConditionDescription;
                        $scope.scopeModel.childVisualItem = visualItemDefinition.Settings.ChildActivityVisualItemDefinition;
                    }

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];
                            if ($scope.scopeModel.childVisualItem != undefined && $scope.scopeModel.childVisualItem.Settings != undefined && $scope.scopeModel.childVisualItem.Settings.Editor != undefined) {
                                directiveEditor = $scope.scopeModel.childVisualItem.Settings.Editor;
                                directivePromises.push(loadChildBranchDirective());

                            }

                            return {
                                promises: directivePromises
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.tryApplyVisualEventToChilds = function (visualEvents) {
                    var eventsStatus = [];

                    if (visualEvents != undefined && visualEvents.length > 0) {
                        var hasNewIteration = false;
                        var visualEventsItems = [];
                        for (var i = 0; i < visualEvents.length; i++) {
                            var visualEvent = visualEvents[i];
                            if (visualEvent.EventTypeId == VisualEventTypeEnum.NewIteration.value.toLowerCase()) {
                                eventsStatus.push({
                                    event: visualEvent,
                                    isEventUsed: true,
                                });
                                $scope.scopeModel.nbOfIteration++;
                                hasNewIteration = true;
                                for (var j = i+1; j < visualEvents.length; j++) {
                                    visualEventsItems.push(visualEvents[j]);
                                }
                                break;
                            }
                        }
                        if (hasNewIteration) {
                            tryReloadDirective().then(function () {
                                var unsucceededVisualEvents = [];
                                for (var i = 0; i < visualEventsItems.length; i++) {
                                    var visualEvent = visualEventsItems[i];
                                    if (applyEvents(visualEvent, unsucceededVisualEvents, eventsStatus))
                                        continue;
                                }
                            });
                        } else {
                            var unsucceededVisualEvents = [];
                            for (var i = 0; i < visualEvents.length; i++) {
                                var visualEvent = visualEvents[i];
                                if (applyEvents(visualEvent, unsucceededVisualEvents, eventsStatus))
                                    continue;
                            }
                        }
                     
                    }
                    return eventsStatus;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            function tryReloadDirective() {
                $scope.scopeModel.childVisualItem.Settings.Editor = undefined;
                childDirectivePromiseReadyDeferred = UtilsService.createPromiseDeferred();
                setTimeout(function () {
                    $scope.scopeModel.childVisualItem.Settings.Editor = directiveEditor;
                });
                var loadPromiseDeferred = UtilsService.createPromiseDeferred();
                childDirectivePromiseReadyDeferred.promise.then(function () {
                    var payload = {
                        visualItemDefinition: $scope.scopeModel.childVisualItem
                    };
                    VRUIUtilsService.callDirectiveLoad(childDirectiveAPI, payload, loadPromiseDeferred);
                });
                return loadPromiseDeferred.promise;
            }

            function applyEvents(visualEvent, unsucceededVisualEvents, eventsStatus) {
                if (childDirectiveAPI != undefined && childDirectiveAPI.tryApplyVisualEvent != undefined) {
                    if (visualItemDefinition != undefined && visualItemDefinition.Settings.ChildActivityId == visualEvent.ActivityId) {
                        if (childDirectiveAPI.checkIfCompleted != undefined && !childDirectiveAPI.checkIfCompleted()) {
                            var childDirectiveResult = childDirectiveAPI.tryApplyVisualEvent(visualEvent);
                            if (childDirectiveResult != undefined && childDirectiveResult.isEventUsed) {
                                $scope.scopeModel.loopConditionCompleted = true;
                                eventsStatus.push({
                                    event: visualEvent,
                                    isEventUsed: childDirectiveResult.isEventUsed,
                                });
                                return true;
                            }
                        }
                    }
                }
                unsucceededVisualEvents.push(visualEvent);

                if (unsucceededVisualEvents.length > 0) {
                    if (childDirectiveAPI != undefined && childDirectiveAPI.tryApplyVisualEventToChilds != undefined) {
                        var childDirectiveResult = childDirectiveAPI.tryApplyVisualEventToChilds(unsucceededVisualEvents);
                        if (childDirectiveResult != undefined && childDirectiveResult.length > 0) {
                            $scope.scopeModel.loopConditionCompleted = true;
                            for (var i = 0; i < childDirectiveResult.length; i++) {
                                eventsStatus.push(childDirectiveResult[i]);
                            }
                        }

                    }
                }
            }

            function loadChildBranchDirective() {
                var loadChildDirectiveDeferred = UtilsService.createPromiseDeferred();

                childDirectivePromiseReadyDeferred.promise.then(function () {
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var payload = {
                        visualItemDefinition: $scope.scopeModel.childVisualItem
                    };
                    VRUIUtilsService.callDirectiveLoad(childDirectiveAPI, payload, loadChildDirectiveDeferred);
                });
                return loadChildDirectiveDeferred.promise;
            }
        }
    }

    app.directive('bpWorkflowActivitysettingsVisualitemdefinitonWhile', VisualItemDefinitonWhile);

})(app);