(function (app) {

    'use strict';

    VisualItemDefinitonSubProcess.$inject = ['UtilsService', 'VRUIUtilsService', 'VisualEventTypeEnum'];

    function VisualItemDefinitonSubProcess(UtilsService, VRUIUtilsService, VisualEventTypeEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var visualItemDefinitonSubProcess = new VisualItemDefinitonSubProcessController($scope, ctrl, $attrs);
                visualItemDefinitonSubProcess.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/BusinessProcess/Directives/BPVisualItemDefinition/Templates/VisualItemDefintionSubProcessWorkflowTemplate.html"
        };

        function VisualItemDefinitonSubProcessController($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var visualItemDefinition;
            var isCompleted = false;

            var subProcessDirectiveAPI;
            var subProcessDirectivePromiseReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSubProcessDirectiveReady = function (api) {
                    subProcessDirectiveAPI = api;
                    subProcessDirectivePromiseReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];
                    if (payload != undefined) {
                        visualItemDefinition = payload.visualItemDefinition;
                        $scope.scopeModel.childActivityVisualItemDefinition = visualItemDefinition.Settings.ChildActivityVisualItemDefinition;
                    }

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];
                            if ($scope.scopeModel.childActivityVisualItemDefinition != undefined && $scope.scopeModel.childActivityVisualItemDefinition.Settings != undefined && $scope.scopeModel.childActivityVisualItemDefinition.Settings.Editor != undefined)
                                directivePromises.push(loadChildsOfSubProcessDirective());

                            return {
                                promises: directivePromises
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.reload = function () {
                    isCompleted = false;
                    if (subProcessDirectiveAPI != undefined && subProcessDirectiveAPI.reload != undefined) {
                        subProcessDirectiveAPI.reload();
                    }
                };

                api.tryApplyVisualEvent = function (visualItemEvent) {
                    if (isCompleted)
                        return false;

                    if (visualItemEvent != undefined) {
                        if (visualItemEvent.EventTypeId == VisualEventTypeEnum.Completed.value.toLowerCase()) {
                            isCompleted = true;
                        }
                    }
                    return true;
                };

                api.tryApplyVisualEventToChilds = function (visualEvent) {
                    if (isCompleted)
                        return false;

                    if (subProcessDirectiveAPI != undefined && subProcessDirectiveAPI.tryApplyVisualEvent != undefined) {
                        if (visualItemDefinition != undefined && visualItemDefinition.Settings.ChildActivityId == visualEvent.ActivityId) {
                            if (subProcessDirectiveAPI.tryApplyVisualEvent(visualEvent)) {
                                return true;
                            }
                        }
                    }
                    if (subProcessDirectiveAPI.tryApplyVisualEventToChilds(visualEvent)) {
                        return true;
                    }
                    return false;
                };

                api.onAfterCompleted = function () {

                };

                api.checkIfCompleted = function () {
                    return isCompleted;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            function loadChildsOfSubProcessDirective() {
                var loadSubProcessDirectiveDeferred = UtilsService.createPromiseDeferred();

                subProcessDirectivePromiseReadyDeferred.promise.then(function () {
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var payload = {
                        visualItemDefinition: $scope.scopeModel.childActivityVisualItemDefinition,
                    };
                    VRUIUtilsService.callDirectiveLoad(subProcessDirectiveAPI, payload, loadSubProcessDirectiveDeferred);
                });
                return loadSubProcessDirectiveDeferred.promise;
            }
        }
    }

    app.directive('bpWorkflowActivitysettingsVisualitemdefinitonSubprocess', VisualItemDefinitonSubProcess);

})(app);