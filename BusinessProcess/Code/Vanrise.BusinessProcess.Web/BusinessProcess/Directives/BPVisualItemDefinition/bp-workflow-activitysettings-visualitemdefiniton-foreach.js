//(function (app) {

//    'use strict';

//    VisualItemDefinitonForEach.$inject = ['UtilsService', 'VRUIUtilsService', 'VisualEventTypeEnum'];

//    function VisualItemDefinitonForEach(UtilsService, VRUIUtilsService, VisualEventTypeEnum) {
//        return {
//            restrict: "E",
//            scope: {
//                onReady: "=",
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var visualItemDefinitonForEach = new VisualItemDefinitonForEachController($scope, ctrl, $attrs);
//                visualItemDefinitonForEach.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            templateUrl: "/Client/Modules/BusinessProcess/Directives/BPVisualItemDefinition/Templates/VisualItemDefintionWhileWorkflowTemplate.html"
//        };

//        function VisualItemDefinitonForEachController($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            var visualItemDefinition;
//            var directiveEditor;
//            var childDirectiveAPI;
//            var childDirectivePromiseReadyDeferred = UtilsService.createPromiseDeferred();
//            var isNewIterationArrived = false;

//            function initializeController() {
//                $scope.scopeModel = {};
//                $scope.scopeModel.classLoopCompleted = false;

//                $scope.scopeModel.onChildVisualItemReady = function (api) {
//                    childDirectiveAPI = api;
//                    childDirectivePromiseReadyDeferred.resolve();
//                };

//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var initialPromises = [];

//                    $scope.scopeModel.nbOfIteration = 0;

//                    if (payload != undefined) {
//                        visualItemDefinition = payload.visualItemDefinition;
//                        $scope.scopeModel.conditionDescription = visualItemDefinition.Settings.ConditionDescription;
//                        $scope.scopeModel.childVisualItem = visualItemDefinition.Settings.ChildActivityVisualItemDefinition;
//                    }

//                    var rootPromiseNode = {
//                        promises: initialPromises,
//                        getChildNode: function () {
//                            var directivePromises = [];
//                            if ($scope.scopeModel.childVisualItem != undefined && $scope.scopeModel.childVisualItem.Settings != undefined && $scope.scopeModel.childVisualItem.Settings.Editor != undefined) {
//                                directiveEditor = $scope.scopeModel.childVisualItem.Settings.Editor;
//                                directivePromises.push(loadChildBranchDirective());

//                            }

//                            return {
//                                promises: directivePromises
//                            };
//                        }
//                    };

//                    return UtilsService.waitPromiseNode(rootPromiseNode);
//                };
//                api.tryApplyVisualEvent = function (visualItemEvent) {
//                    var result = {};
//                    result.isEventUsed = false;
//                    if (visualItemEvent.EventTypeId == VisualEventTypeEnum.NewIteration.value.toLowerCase()) {
//                        $scope.scopeModel.nbOfIteration++;
//                        //isNewIterationArrived = true;
//                        result.isEventUsed = true;
//                        tryReloadDirective();
//                    }
//                    return result;
//                };
//                api.reload = function () {
//                    $scope.scopeModel.nbOfIteration = 0;
//                    isNewIterationArrived = false;
//                    $scope.scopeModel.classLoopCompleted = false;
//                    if (childDirectiveAPI != undefined && childDirectiveAPI.reload != undefined) {
//                        childDirectiveAPI.reload();
//                    }
//                };
//                api.tryApplyVisualEventToChilds = function (visualEvents) {
//                    var eventsStatus = [];
//                    var unsucceededVisualEvents = [];
//                    if (visualEvents != undefined && visualEvents.length > 0) {
//                        for (var i = 0; i < visualEvents.length; i++) {
//                            var visualEvent = visualEvents[i];
//                            if (childDirectiveAPI != undefined && childDirectiveAPI.tryApplyVisualEvent != undefined) {
//                                if (visualItemDefinition != undefined && visualItemDefinition.Settings.ChildActivityId == visualEvent.ActivityId) {
//                                    if (childDirectiveAPI.checkIfCompleted != undefined && !childDirectiveAPI.checkIfCompleted()) {
//                                        var childDirectiveResult = childDirectiveAPI.tryApplyVisualEvent(visualEvent);
//                                        if (childDirectiveResult != undefined && childDirectiveResult.isEventUsed) {
//                                            eventsStatus.push({
//                                                event: visualEvent,
//                                                isEventUsed: childDirectiveResult.isEventUsed,
//                                            });
//                                            continue;
//                                        }
//                                    }
//                                }
//                            }
//                            unsucceededVisualEvents.push(visualEvent);
//                        }
//                        if (unsucceededVisualEvents.length > 0) {
//                            if (childDirectiveAPI != undefined && childDirectiveAPI.tryApplyVisualEventToChilds != undefined) {
//                                var childDirectiveResult = childDirectiveAPI.tryApplyVisualEventToChilds(unsucceededVisualEvents);
//                                if (childDirectiveResult != undefined && childDirectiveResult.length > 0) {
//                                    for (var i = 0; i < childDirectiveResult.length; i++) {
//                                        eventsStatus.push(childDirectiveResult[i]);
//                                    }
//                                }
//                            }
//                        }
//                    }
//                    return eventsStatus;
//                };

//                api.onAfterCompleted = function () {
//                    $scope.scopeModel.classLoopCompleted = true;
//                };

//                api.checkIfCompleted = function () {
//                    return isNewIterationArrived;
//                };
//                if (ctrl.onReady != null) {
//                    ctrl.onReady(api);
//                }
//            }

//            function tryReloadDirective() {
//                if (childDirectiveAPI != undefined && childDirectiveAPI.reload != undefined)
//                    childDirectiveAPI.reload();
//            }

//            function loadChildBranchDirective() {
//                var loadChildDirectiveDeferred = UtilsService.createPromiseDeferred();

//                childDirectivePromiseReadyDeferred.promise.then(function () {
//                    var setLoader = function (value) {
//                        $scope.scopeModel.isLoadingDirective = value;
//                    };
//                    var payload = {
//                        visualItemDefinition: $scope.scopeModel.childVisualItem,
//                        activityId: visualItemDefinition.Settings.ChildActivityId
//                    };
//                    VRUIUtilsService.callDirectiveLoad(childDirectiveAPI, payload, loadChildDirectiveDeferred);
//                });
//                return loadChildDirectiveDeferred.promise;
//            }
//        }
//    }

//    app.directive('bpWorkflowActivitysettingsVisualitemdefinitonForeach', VisualItemDefinitonForEach);

//})(app);