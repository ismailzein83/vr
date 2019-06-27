(function (app) {

    'use strict';

    VisualItemDefinitonSequence.$inject = ['UtilsService', 'VRUIUtilsService', 'VisualEventTypeEnum'];

    function VisualItemDefinitonSequence(UtilsService, VRUIUtilsService, VisualEventTypeEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var visualItemDefinitonSequence = new VisualItemDefinitonSequenceController($scope, ctrl, $attrs);
                visualItemDefinitonSequence.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/BusinessProcess/Directives/BPVisualItemDefinition/Templates/VisualItemDefintionSequenceWorkflowTemplate.html"
        };

        function VisualItemDefinitonSequenceController($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var visualItemDefinition;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.childVisualItems = [];
                defineAPI();
            }

            function defineAPI() {
                var api = {};
                var childVisualItems = [];

                api.load = function (payload) {
                    var initialPromises = [];

                    if (payload != undefined) {
                        visualItemDefinition = payload.visualItemDefinition;
                        childVisualItems = visualItemDefinition.Settings.ChildVisualItems;
                    }

                    if (childVisualItems != undefined) {
                        for (var i = 0; i < childVisualItems.length; i++) {
                            var childVisualItem = childVisualItems[i];
                            if (childVisualItem.ChildItemDefinition != undefined && childVisualItem.ChildItemDefinition.Settings != undefined && childVisualItem.ChildItemDefinition.Settings.Editor != undefined) {
                                childVisualItem.Id = i;
                                childVisualItem.readyPromiseDeferred = UtilsService.createPromiseDeferred();
                                childVisualItem.loadPromiseDeferred = UtilsService.createPromiseDeferred();
                                initialPromises.push(childVisualItem.loadPromiseDeferred.promise);
                                loadChildDirective(childVisualItem);
                            }
                        }
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
                    if ($scope.scopeModel.childVisualItems != undefined) {
                        for (var i = 0; i < $scope.scopeModel.childVisualItems.length; i++) {
                            var childVisualItem = $scope.scopeModel.childVisualItems[i];
                            if (childVisualItem.directiveAPI != undefined && childVisualItem.directiveAPI.reload != undefined) {
                                childVisualItem.classEventCompleted = false;
                                childVisualItem.directiveAPI.reload();
                            }
                        }
                    }
                };

                api.tryApplyVisualEventToChilds = function (visualEvents) {
                    var eventsStatus = [];

                    if (visualEvents != undefined && visualEvents.length > 0) {
                        for (var i = 0; i < visualEvents.length; i++) {
                            var visualEvent = visualEvents[i];

                            for (var j = 0; j < childVisualItems.length; j++) {
                                var childVisualItem = childVisualItems[j];
                                if (childVisualItem.directiveAPI.tryApplyVisualEvent != undefined) {
                                    var eventItem = UtilsService.getItemByVal(eventsStatus, visualEvent.BPVisualEventId, "event.BPVisualEventId");
                                    if (eventItem == undefined || !eventItem.isEventUsed) {
                                        if (visualEvent.ActivityId == childVisualItem.ChildActivityId) {
                                            if (childVisualItem.directiveAPI.checkIfCompleted != undefined && !childVisualItem.directiveAPI.checkIfCompleted()) {
                                                var childItemResult = childVisualItem.directiveAPI.tryApplyVisualEvent(visualEvent);
                                                if (childItemResult != undefined && childItemResult.isEventUsed) {
                                                    if (j != 0) {
                                                        var preItem = childVisualItems[j - 1];
                                                        preItem.classEventCompleted = true;
                                                        if (preItem.directiveAPI != undefined && preItem.directiveAPI.onAfterCompleted != undefined)
                                                            preItem.directiveAPI.onAfterCompleted();
                                                    }
                                                    eventsStatus.push({
                                                        event: visualEvent,
                                                        isEventUsed: childItemResult.isEventUsed,
                                                    });
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (childVisualItem.directiveAPI.tryApplyVisualEventToChilds != undefined) {
                                    var childEventsResult = childVisualItem.directiveAPI.tryApplyVisualEventToChilds([visualEvent]);
                                    if (childEventsResult != undefined && childEventsResult.length > 0) {
                                        if (j != 0) {
                                            var preItem = childVisualItems[j - 1];
                                            preItem.classEventCompleted = true;
                                            if (preItem.directiveAPI != undefined && preItem.directiveAPI.onAfterCompleted != undefined)
                                                preItem.directiveAPI.onAfterCompleted();
                                        }
                                        var shouldBreak = false;
                                        for (var k = 0; k < childEventsResult.length; k++) {
                                            var childEventsResultItem = childEventsResult[k];
                                            eventsStatus.push(childEventsResultItem);
                                            if (childEventsResultItem.isEventUsed)
                                                shouldBreak = true;
                                        }
                                        if (shouldBreak)
                                            break;
                                    }
                                }
                            }
                        }
                    } 
                    return eventsStatus;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            function loadChildDirective(childVisualItem) {
                childVisualItem.onDirectiveReady = function (api) {
                    childVisualItem.directiveAPI = api;
                    childVisualItem.readyPromiseDeferred.resolve();
                };

                childVisualItem.readyPromiseDeferred.promise.then(function () {
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var payload = {
                        visualItemDefinition: childVisualItem.ChildItemDefinition
                    };
                    VRUIUtilsService.callDirectiveLoad(childVisualItem.directiveAPI, payload, childVisualItem.loadPromiseDeferred);
                });
                $scope.scopeModel.childVisualItems.push(childVisualItem);
            }
        }
    }

    app.directive('bpWorkflowActivitysettingsVisualitemdefinitonSequence', VisualItemDefinitonSequence);

})(app);