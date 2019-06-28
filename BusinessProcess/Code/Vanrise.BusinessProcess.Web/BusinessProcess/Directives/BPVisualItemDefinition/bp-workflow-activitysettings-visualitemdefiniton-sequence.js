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

                api.tryApplyVisualEventToChilds = function (visualEvent) {
                    for (var j = 0; j < childVisualItems.length; j++) {
                        var childVisualItem = childVisualItems[j];
                        if (childVisualItem.directiveAPI.tryApplyVisualEvent != undefined) {
                            if (visualEvent.ActivityId == childVisualItem.ChildActivityId) {
                                if (childVisualItem.directiveAPI.tryApplyVisualEvent(visualEvent)) {
                                    if (j != 0) {
                                        childVisualItems[j - 1].classEventCompleted = true;
                                    }
                                    return true;
                                }
                            }
                        }

                        if (childVisualItem.directiveAPI.tryApplyVisualEventToChilds != undefined) {
                            if (childVisualItem.directiveAPI.tryApplyVisualEventToChilds(visualEvent)) {
                                if (j != 0) {
                                    childVisualItems[j - 1].classEventCompleted = true;
                                }
                                return true;
                            }
                        }
                    }
                    return false;
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