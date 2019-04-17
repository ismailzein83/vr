(function (app) {

    'use strict';

    VisualItemDefinitonSequence.$inject = ['UtilsService', 'VRUIUtilsService'];

    function VisualItemDefinitonSequence(UtilsService, VRUIUtilsService) {
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
                            childVisualItem.Id = i;
                            childVisualItem.readyPromiseDeferred = UtilsService.createPromiseDeferred();
                            childVisualItem.loadPromiseDeferred = UtilsService.createPromiseDeferred();

                            initialPromises.push(childVisualItem.loadPromiseDeferred.promise);
                            loadChildDirective(childVisualItem);
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
                    var status = false;
                    if (visualEvents != undefined && visualEvents.length > 0) {
                        var childVisualItemsWithSubChilds = [];
                        var unsucceededVisualEvents = [];
                        for (var i = 0; i < childVisualItems.length; i++) {
                            var childVisualItem = childVisualItems[i];
                            if (childVisualItem.directiveAPI.tryApplyVisualEventToChilds != undefined) {
                                childVisualItemsWithSubChilds.push({

                                    childVisualItem: childVisualItem,
                                    index:i
                                });
                            } else {
                                for (var j = 0; j < visualEvents.length; j++) {
                                    var visualEvent = visualEvents[j];
                                    if (visualEvent.ActivityId == childVisualItem.ChildActivityId) {
                                        if (childVisualItem.directiveAPI.tryApplyVisualEvent(visualEvent)) {
                                            if (i != 0)
                                                childVisualItems[i - 1].classEventCompleted = true;
                                            status = true;
                                        }
                                    }
                                    else {
                                        unsucceededVisualEvents.push(visualEvent);
                                    }
                                }
                            }
                        }

                        if (childVisualItemsWithSubChilds.length > 0) {
                            for (var j = 0; j < childVisualItemsWithSubChilds.length; j++) {
                                var childVisualItemsWithSubChild = childVisualItemsWithSubChilds[j];
                                if (childVisualItemsWithSubChild.childVisualItem.directiveAPI.tryApplyVisualEventToChilds(unsucceededVisualEvents)) {
                                    if (childVisualItemsWithSubChild.index != 0)
                                        childVisualItems[childVisualItemsWithSubChild.index - 1].classEventCompleted = true;
                                    status = true;
                                }
                            }
                        }
                    }
                    return status;
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