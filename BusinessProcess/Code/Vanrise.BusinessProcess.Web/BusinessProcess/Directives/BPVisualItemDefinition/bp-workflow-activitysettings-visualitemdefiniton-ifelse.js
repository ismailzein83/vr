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
            var trueBranchUsed = false;
            var falseBranchUsed = false;

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

                api.reload = function () {
                    $scope.scopeModel.trueConditionCompleted = false;
                    $scope.scopeModel.falseConditionCompleted = false;

                    if (trueBranchDirectiveAPI != undefined && trueBranchDirectiveAPI.reload != undefined) {
                        trueBranchDirectiveAPI.reload();
                    }
                    if (falseBranchDirectiveAPI != undefined && falseBranchDirectiveAPI.reload != undefined) {
                        falseBranchDirectiveAPI.reload();
                    }
                };

                api.tryApplyVisualEventToChilds = function (visualEvent) {
                    if (trueBranchUsed || !trueBranchUsed && !falseBranchUsed) {
                        if (trueBranchDirectiveAPI != undefined && trueBranchDirectiveAPI.tryApplyVisualEvent != undefined) {
                            if (visualItemDefinition != undefined && visualItemDefinition.Settings.TrueBranchActivityId == visualEvent.ActivityId) {
                                if (trueBranchDirectiveAPI.tryApplyVisualEvent(visualEvent)) {
                                    $scope.scopeModel.trueConditionCompleted = true;
                                    trueBranchUsed = true;
                                    return true;
                                }
                            }
                        }
                        if (trueBranchDirectiveAPI != undefined && trueBranchDirectiveAPI.tryApplyVisualEventToChilds != undefined) {
                            if (trueBranchDirectiveAPI.tryApplyVisualEventToChilds(visualEvent)) {
                                $scope.scopeModel.trueConditionCompleted = true;
                                trueBranchUsed = true;
                                return true;
                            }
                        }
                    }

                    if (falseBranchUsed || !trueBranchUsed && !falseBranchUsed) {
                        if (falseBranchDirectiveAPI != undefined && falseBranchDirectiveAPI.tryApplyVisualEvent != undefined) {
                            if (visualItemDefinition != undefined && visualItemDefinition.Settings.FalseBranchActivityId == visualEvent.ActivityId) {
                                if (falseBranchDirectiveAPI.tryApplyVisualEvent(visualEvent)) {
                                    $scope.scopeModel.falseConditionCompleted = true;
                                    falseBranchUsed = true;
                                    return true;
                                }
                            }
                        }

                        if (falseBranchDirectiveAPI != undefined && falseBranchDirectiveAPI.tryApplyVisualEventToChilds != undefined) {
                            if (falseBranchDirectiveAPI.tryApplyVisualEventToChilds(visualEvent)) {
                                $scope.scopeModel.falseConditionCompleted = true;
                                falseBranchUsed = true;
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

            function loadTrueBranchDirective() {
                var loadTrueBranchDirectiveDeferred = UtilsService.createPromiseDeferred();

                trueBranchDirectivePromiseReadyDeferred.promise.then(function () {
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var payload = {
                        visualItemDefinition: $scope.scopeModel.trueBranch,
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
                        visualItemDefinition: $scope.scopeModel.falseBranch,
                    };
                    VRUIUtilsService.callDirectiveLoad(falseBranchDirectiveAPI, payload, loadFalseBranchDirectiveDeferred);
                });
                return loadFalseBranchDirectiveDeferred.promise;
            }
        }
    }

    app.directive('bpWorkflowActivitysettingsVisualitemdefinitonIfelse', VisualItemDefinitonIfElse);

})(app);