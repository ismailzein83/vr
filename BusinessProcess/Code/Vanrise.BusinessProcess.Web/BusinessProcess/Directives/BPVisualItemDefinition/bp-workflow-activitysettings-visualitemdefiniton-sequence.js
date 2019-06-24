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

                api.tryApplyVisualEventToChilds = function (visualEvents) {
                    var eventsStatus = [];

                    if (visualEvents != undefined && visualEvents.length > 0) {
                        var childVisualItemsWithSubChilds = [];
                        var succeededVisualEvents = [];
                        for (var i = 0; i < childVisualItems.length; i++) {
                            var childVisualItem = childVisualItems[i];
                            if (childVisualItem.directiveAPI.tryApplyVisualEventToChilds != undefined) {
                                childVisualItemsWithSubChilds.push({
                                    childVisualItem: childVisualItem,
                                    index: i
                                });
                            } else {
                                for (var j = 0; j < visualEvents.length; j++) {
                                    var visualEvent = visualEvents[j];
                                    //if (visualEvent.ActivityId == VisualEventTypeEnum.NewIteration.value.toLowerCase()) {
                                    //    var reloadedObject = UtilsService.getItemByVal(childVisualItems, visualEvent.ActivityId, "ChildActivityId");
                                    //    if (reloadedObject != undefined) {

                                    //    }
                                    //}
                                    if (visualEvent.ActivityId == childVisualItem.ChildActivityId) {
                                        var childItemResult = childVisualItem.directiveAPI.tryApplyVisualEvent(visualEvent);

                                        if (childItemResult != undefined && childItemResult.isEventUsed) {
                                            if (i != 0)
                                                childVisualItems[i - 1].classEventCompleted = true;
                                            eventsStatus.push({
                                                event: visualEvent,
                                                isEventUsed: childItemResult.isEventUsed,
                                            })
                                        }
                                    }
                                }
                            }
                        }

                        var unsucceededVisualEvents = getAvailableEvents(visualEvents, eventsStatus); 

                        if (childVisualItemsWithSubChilds.length > 0) {
                            for (var j = 0; j < childVisualItemsWithSubChilds.length; j++) {
                                var childVisualItemsWithSubChild = childVisualItemsWithSubChilds[j];
                                var childEventsResult = childVisualItemsWithSubChild.childVisualItem.directiveAPI.tryApplyVisualEventToChilds(unsucceededVisualEvents);
                                if (childEventsResult != undefined && childEventsResult.length > 0) {
                                    for (var k = 0; k < childEventsResult.length; k++) {
                                        eventsStatus.push(childEventsResult[k]);
                                    }
                                    unsucceededVisualEvents = getAvailableEvents(unsucceededVisualEvents, childEventsResult);
                                    if (childVisualItemsWithSubChild.index != 0)
                                        childVisualItems[childVisualItemsWithSubChild.index - 1].classEventCompleted = true;
                                }
                            }
                        }
                    }
                    return eventsStatus;
                };

                function getAvailableEvents(visualEvents, eventsStatus) {
                    var unsucceededVisualEvents = [];
                    if (visualEvents != undefined && eventsStatus != undefined) {
                        for (var j = 0; j < visualEvents.length; j++) {
                            var visualEvent = visualEvents[j];
                            var eventItem = UtilsService.getItemByVal(eventsStatus, visualEvent.BPVisualEventId, "event.BPVisualEventId");
                            if (eventItem == undefined || !eventItem.isEventUsed)
                                unsucceededVisualEvents.push(visualEvent);
                        }
                    }
                    
                    return unsucceededVisualEvents;
                }

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