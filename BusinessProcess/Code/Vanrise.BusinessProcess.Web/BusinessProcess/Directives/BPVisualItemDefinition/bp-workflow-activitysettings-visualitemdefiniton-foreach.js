(function (app) {

    'use strict';

    VisualItemDefinitonForEach.$inject = ['UtilsService', 'VRUIUtilsService', 'VisualEventTypeEnum'];

    function VisualItemDefinitonForEach(UtilsService, VRUIUtilsService, VisualEventTypeEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var visualItemDefinitonForEach = new VisualItemDefinitonForEachController($scope, ctrl, $attrs);
                visualItemDefinitonForEach.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/BusinessProcess/Directives/BPVisualItemDefinition/Templates/VisualItemDefintionWhileWorkflowTemplate.html"
        };

        function VisualItemDefinitonForEachController($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var visualItemDefinition;
            var directiveEditor;
            var isCompleted = false;

            var childDirectiveAPI;
            var childDirectivePromiseReadyDeferred = UtilsService.createPromiseDeferred();
            
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
                    if (isCompleted)
                        return false;

                    if (visualItemEvent != undefined) {
                        if (visualItemEvent.EventTypeId == VisualEventTypeEnum.NewIteration.value.toLowerCase()) {
                            if ($scope.scopeModel.nbOfIteration > 0)
                                tryReloadDirective();
                            $scope.scopeModel.nbOfIteration++;
                        }
                        if (visualItemEvent.EventTypeId == VisualEventTypeEnum.Completed.value.toLowerCase()) {
                            isCompleted = true;
                            $scope.scopeModel.classLoopCompleted = true;
                        }
                    }
                    return true;
                };

                api.tryApplyVisualEventToChilds = function (visualEvent) {
                    if (isCompleted)
                        return false;

                    if (childDirectiveAPI != undefined) {
                        if (childDirectiveAPI.tryApplyVisualEvent != undefined) {
                            if (visualItemDefinition != undefined && visualItemDefinition.Settings.ChildActivityId == visualEvent.ActivityId) {
                                if (childDirectiveAPI.tryApplyVisualEvent(visualEvent))
                                    return true;
                            }
                        }
                        if (childDirectiveAPI.tryApplyVisualEventToChilds != undefined && childDirectiveAPI.tryApplyVisualEventToChilds(visualEvent)) {
                            return true;
                        }
                    }
                    return false;
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

    app.directive('bpWorkflowActivitysettingsVisualitemdefinitonForeach', VisualItemDefinitonForEach);

})(app);