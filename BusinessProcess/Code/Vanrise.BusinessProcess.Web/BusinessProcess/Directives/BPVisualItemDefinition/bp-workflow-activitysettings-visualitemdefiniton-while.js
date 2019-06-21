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

            var childDirectiveAPI;
            var childDirectivePromiseReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.nbOfIteration = 1;

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

                    if (payload != undefined) {
                        visualItemDefinition = payload.visualItemDefinition;
                        $scope.scopeModel.conditionDescription = visualItemDefinition.Settings.ConditionDescription;
                        $scope.scopeModel.childVisualItem = visualItemDefinition.Settings.ChildActivityVisualItemDefinition;
                    }

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];
                            if ($scope.scopeModel.childVisualItem != undefined && $scope.scopeModel.childVisualItem.Settings != undefined && $scope.scopeModel.childVisualItem.Settings.Editor != undefined)
                                directivePromises.push(loadChildBranchDirective());

                            return {
                                promises: directivePromises
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.tryApplyVisualEventToChilds = function (visualEvents) {
                    var status = false;
                    if (visualEvents != undefined && visualEvents.length >= 0) {
                        var unsucceededVisualEvents = [];

                        for (var i = 0; i < visualEvents.length; i++) {
                            var visualEvent = visualEvents[i];

                            if (visualEvent.EventTypeId == VisualEventTypeEnum.NewIteration.value.toLowerCase()) {
                                $scope.scopeModel.nbOfIteration++;
                            }

                            if (childDirectiveAPI != undefined && childDirectiveAPI.tryApplyVisualEvent != undefined) {
                                if (visualItemDefinition != undefined && visualItemDefinition.Settings.ChildActivityId == visualEvent.ActivityId) {
                                    if (childDirectiveAPI.tryApplyVisualEvent(visualEvent)) {
                                        $scope.scopeModel.trueConditionCompleted = true;
                                        status = true;
                                        continue;
                                    }
                                }
                            }

                            unsucceededVisualEvents.push(visualEvent);
                        }

                        if (unsucceededVisualEvents.length > 0) {
                            if (childDirectiveAPI != undefined && childDirectiveAPI.tryApplyVisualEventToChilds != undefined) {
                                if (childDirectiveAPI.tryApplyVisualEventToChilds(unsucceededVisualEvents)) {
                                    $scope.scopeModel.trueConditionCompleted = true;
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