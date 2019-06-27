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
            var isCompleted = false;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.classLoopCompleted = false;

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

                api.reload = function () {
                    $scope.scopeModel.nbOfIteration = 0;
                    isCompleted = false;
                    $scope.scopeModel.classLoopCompleted = false;
                    if (childDirectiveAPI != undefined && childDirectiveAPI.reload != undefined) {
                        childDirectiveAPI.reload();
                    }
                };

                api.tryApplyVisualEvent = function (visualItemEvent) {
                    var result = {
                        isEventUsed : false
                    };
                    if (visualItemEvent.EventTypeId == VisualEventTypeEnum.NewIteration.value.toLowerCase()) {
                        result.isEventUsed = true;
                        if ($scope.scopeModel.nbOfIteration > 0)
                            tryReloadDirective();
                        $scope.scopeModel.nbOfIteration++;
                    }
                    if (visualItemEvent.EventTypeId == VisualEventTypeEnum.Completed.value.toLowerCase()) {
                        isCompleted = true;
                    }
                    return result;
                };
             
                api.tryApplyVisualEventToChilds = function (visualEvents) {
                    var eventsStatus = [];
                    var unsucceededVisualEvents = [];
                    if (visualEvents != undefined && visualEvents.length > 0) {
                        for (var i = 0; i < visualEvents.length; i++) {
                            var visualEvent = visualEvents[i];
                            if (childDirectiveAPI != undefined) {
                                if (childDirectiveAPI.tryApplyVisualEvent != undefined) {
                                    if (visualItemDefinition != undefined && visualItemDefinition.Settings.ChildActivityId == visualEvent.ActivityId) {
                                        if (childDirectiveAPI.checkIfCompleted != undefined && !childDirectiveAPI.checkIfCompleted()) {
                                            var childDirectiveResult = childDirectiveAPI.tryApplyVisualEvent(visualEvent);
                                            if (childDirectiveResult != undefined && childDirectiveResult.isEventUsed) {
                                                eventsStatus.push({
                                                    event: visualEvent,
                                                    isEventUsed: childDirectiveResult.isEventUsed,
                                                });
                                                continue;
                                            }
                                        }
                                    }
                                }

                                if (childDirectiveAPI.tryApplyVisualEventToChilds != undefined) {
                                    var childDirectiveResult = childDirectiveAPI.tryApplyVisualEventToChilds([visualEvent]);
                                    if (childDirectiveResult != undefined && childDirectiveResult.length > 0) {
                                        var shouldContinue = false;
                                        for (var k = 0; k < childDirectiveResult.length; k++) {
                                            var childEventsResultItem = childDirectiveResult[k];
                                            eventsStatus.push(childEventsResultItem);
                                            if (childEventsResultItem.isEventUsed)
                                                shouldContinue = true;
                                        }
                                        if (shouldContinue)
                                            continue;
                                    }
                                }
                            }
                          
                        }
                      
                    } 
                    return eventsStatus;
                };

                api.onAfterCompleted = function () {
                    $scope.scopeModel.classLoopCompleted = true;
                };

                api.checkIfCompleted = function () {
                    return isCompleted;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            function tryReloadDirective() {
                if (childDirectiveAPI != undefined && childDirectiveAPI.reload != undefined)
                    childDirectiveAPI.reload();
            }

            function loadChildBranchDirective() {
                var loadChildDirectiveDeferred = UtilsService.createPromiseDeferred();

                childDirectivePromiseReadyDeferred.promise.then(function () {
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var payload = {
                        visualItemDefinition: $scope.scopeModel.childVisualItem,
                        activityId: visualItemDefinition.Settings.ChildActivityId
                    };
                    VRUIUtilsService.callDirectiveLoad(childDirectiveAPI, payload, loadChildDirectiveDeferred);
                });
                return loadChildDirectiveDeferred.promise;
            }
        }
    }

    app.directive('bpWorkflowActivitysettingsVisualitemdefinitonWhile', VisualItemDefinitonWhile);

})(app);