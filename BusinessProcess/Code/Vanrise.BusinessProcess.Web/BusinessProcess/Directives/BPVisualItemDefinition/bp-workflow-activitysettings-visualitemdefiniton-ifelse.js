(function (app) {

    'use strict';

    VisualItemDefinitonIfElse.$inject = ['UtilsService', 'VRUIUtilsService', 'VisualEventTypeEnum'];

    function VisualItemDefinitonIfElse(UtilsService, VRUIUtilsService, VisualEventTypeEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var visualItemDefinitonIfElse = new VisualItemDefinitonIfElseController($scope, ctrl, $attrs);
                visualItemDefinitonIfElse.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/BusinessProcess/Directives/BPVisualItemDefinition/Templates/VisualItemDefintionIfElseWorkflowTemplate.html"
        };

        function VisualItemDefinitonIfElseController($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var visualItemDefinition;

            var trueBranchDirectiveAPI;
            var trueBranchDirectivePromiseReadyDeferred = UtilsService.createPromiseDeferred();

            var falseBranchDirectiveAPI;
            var falseBranchDirectivePromiseReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onTrueBranchDirectiveReady = function (api) {
                    trueBranchDirectiveAPI = api;
                    trueBranchDirectivePromiseReadyDeferred.resolve();
                };

                $scope.scopeModel.onFalseBranchDirectiveReady = function (api) {
                    falseBranchDirectiveAPI = api;
                    falseBranchDirectivePromiseReadyDeferred.resolve();
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
                        $scope.scopeModel.trueBranch = visualItemDefinition.Settings.TrueBranchVisualItemDefinition;
                        $scope.scopeModel.falseBranch = visualItemDefinition.Settings.FalseBranchVisualItemDefinition;
                    }

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];
                            if ($scope.scopeModel.trueBranch != undefined && $scope.scopeModel.trueBranch.Settings != undefined && $scope.scopeModel.trueBranch.Settings.Editor != undefined)
                                directivePromises.push(loadTrueBranchDirective());
                            if ($scope.scopeModel.falseBranch != undefined && $scope.scopeModel.falseBranch.Settings != undefined && $scope.scopeModel.falseBranch.Settings.Editor != undefined)
                                directivePromises.push(loadFalseBranchDirective());

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

                            if (trueBranchDirectiveAPI != undefined && trueBranchDirectiveAPI.tryApplyVisualEvent != undefined) {
                                if (visualItemDefinition != undefined && visualItemDefinition.Settings.TrueBranchActivityId == visualEvent.ActivityId) {
                                    if (trueBranchDirectiveAPI.tryApplyVisualEvent(visualEvent)) {
                                        $scope.scopeModel.trueConditionCompleted = true;
                                        status = true;
                                        continue;
                                    }
                                }
                            }

                            if (falseBranchDirectiveAPI != undefined && falseBranchDirectiveAPI.tryApplyVisualEvent != undefined) {
                                if (visualItemDefinition != undefined && visualItemDefinition.Settings.FalseBranchActivityId == visualEvent.ActivityId) {
                                    if (falseBranchDirectiveAPI.tryApplyVisualEvent(visualEvent)) {
                                        $scope.scopeModel.falseConditionCompleted = true;
                                        status = true;
                                        continue;
                                    }
                                }
                            }

                            unsucceededVisualEvents.push(visualEvent);
                        }

                        if (unsucceededVisualEvents.length > 0) {
                            if (trueBranchDirectiveAPI != undefined && trueBranchDirectiveAPI.tryApplyVisualEventToChilds != undefined) {
                                if (trueBranchDirectiveAPI.tryApplyVisualEventToChilds(unsucceededVisualEvents)) {
                                    $scope.scopeModel.trueConditionCompleted = true;
                                    status = true;
                                }
                            }

                            if (falseBranchDirectiveAPI != undefined && falseBranchDirectiveAPI.tryApplyVisualEventToChilds != undefined) {
                                if (falseBranchDirectiveAPI.tryApplyVisualEventToChilds(unsucceededVisualEvents)) {
                                    $scope.scopeModel.falseConditionCompleted = true;
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

            function loadTrueBranchDirective() {
                var loadTrueBranchDirectiveDeferred = UtilsService.createPromiseDeferred();

                trueBranchDirectivePromiseReadyDeferred.promise.then(function () {
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var payload = {
                        visualItemDefinition: $scope.scopeModel.trueBranch
                    };
                    VRUIUtilsService.callDirectiveLoad(trueBranchDirectiveAPI, payload, loadTrueBranchDirectiveDeferred);
                });
                return loadTrueBranchDirectiveDeferred.promise;
            }

            function loadFalseBranchDirective() {
                var loadFalseBranchDirectiveDeferred = UtilsService.createPromiseDeferred();

                falseBranchDirectivePromiseReadyDeferred.promise.then(function () {
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var payload = {
                        visualItemDefinition: $scope.scopeModel.falseBranch
                    };
                    VRUIUtilsService.callDirectiveLoad(falseBranchDirectiveAPI, payload, loadFalseBranchDirectiveDeferred);
                });
                return loadFalseBranchDirectiveDeferred.promise;
            }
        }
    }

    app.directive('bpWorkflowActivitysettingsVisualitemdefinitonIfelse', VisualItemDefinitonIfElse);

})(app);