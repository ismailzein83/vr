(function (app) {

    'use strict';

    WorkflowActivitySettingsVisualItemSelective.$inject = ['UtilsService', 'VRUIUtilsService', 'VRTimerService' ,'BusinessProcess_VRWorkflowAPIService', 'BusinessProcess_BPDefinitionAPIService', 'BusinessProcess_BPVisualEventAPIService'];

    function WorkflowActivitySettingsVisualItemSelective(UtilsService, VRUIUtilsService, VRTimerService, BusinessProcess_VRWorkflowAPIService, BusinessProcess_BPDefinitionAPIService, BusinessProcess_BPVisualEventAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var workflowActivitySettingsVisualItemSelective = new VisualItemSelectiveController($scope, ctrl, $attrs);
                workflowActivitySettingsVisualItemSelective.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/BusinessProcess/Directives/BPVisualItemDefinition/Templates/VisualItemDefintionDirectiveTemplate.html"
        };

        function VisualItemSelectiveController($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var input = {};
            var bpInstanceID;
            var bpDefinitionEntity;

            var directiveAPI;
            var directiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.bpVisualItemsDetails = [];
                $scope.scopeModel = {};

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    directiveReadyDeferred.resolve();
                };

                defineAPI();
            }
            function loadDirective() {
                var loadDirectiveDeferred = UtilsService.createPromiseDeferred();
                directiveReadyDeferred.promise.then(function () {
                    var payload = {
                        visualItemDefinition: $scope.scopeModel.visualItemDefinition,
                    };

                    VRUIUtilsService.callDirectiveLoad(directiveAPI, payload, loadDirectiveDeferred);
                });
                return loadDirectiveDeferred.promise;
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];

                    if (payload != undefined) {
                        initialPromises.push(getBPDefintion(payload.BPDefinitionID));
                        bpInstanceID = payload.BPInstanceID;
                    }

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];
                            
                            if (bpDefinitionEntity != undefined && bpDefinitionEntity.VRWorkflowId != undefined)
                                directivePromises.push(getVRWorkflowVisualItemDefinition(bpDefinitionEntity.VRWorkflowId));
                            return {
                                promises: directivePromises,
                                getChildNode: function () {
                                    return {
                                        promises: [loadDirective()]
                                    }
                                }
                            }
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode).then(function () {
                        onInit();
                        bpDefinitionEntity = undefined;
                    });

                };

                api.clearTimer = function () {
                    if ($scope.jobIds) {
                        VRTimerService.unregisterJobByIds($scope.jobIds);
                        $scope.jobIds.length = 0;
                    }
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            function getBPDefintion(bpDefinitionId) {
                return BusinessProcess_BPDefinitionAPIService.GetBPDefintion(bpDefinitionId).then(function (response) {
                    bpDefinitionEntity = response;
                });
            }

            function getVRWorkflowVisualItemDefinition(vrWorkflowId) {
                return BusinessProcess_VRWorkflowAPIService.GetVisualItemDefinition(vrWorkflowId).then(function (response) {
                    if (response != undefined) {
                        $scope.scopeModel.visualItemDefinition = response;
                    }
                });
            }

            function onInit() {
                $scope.isLoading = true;
                input.GreaterThanID = undefined;
                input.BPInstanceID = bpInstanceID;
                createTimer();
            }

            function createTimer() {
                if ($scope.jobIds) {
                    VRTimerService.unregisterJobByIds($scope.jobIds);
                    $scope.jobIds.length = 0;
                }
                VRTimerService.registerJob(onTimerElapsed, $scope, 2);
            }

            function onTimerElapsed() {
                return BusinessProcess_BPVisualEventAPIService.GetAfterId(input).then(function (response) {
                    manipulateDataToBeExecuted(response);
                    $scope.isLoading = false;
                },
                    function (excpetion) {
                        $scope.isLoading = false;
                    });
            }

            function manipulateDataToBeExecuted(visualEntity) {
                if (visualEntity != undefined && visualEntity.ListBPVisualEventDetails != undefined && visualEntity.ListBPVisualEventDetails.length > 0) {
                    if (directiveAPI != undefined) {
                        if (directiveAPI.tryApplyVisualEventToChilds != undefined) {
                            directiveAPI.tryApplyVisualEventToChilds(visualEntity.ListBPVisualEventDetails);
                        } else {
                            if (visualEntity.ListBPVisualEventDetails.length == 1) {
                                directiveAPI.tryApplyVisualEvent(visualEntity.ListBPVisualEventDetails[0]);
                            }
                        }
                        var lastItem = visualEntity.ListBPVisualEventDetails[visualEntity.ListBPVisualEventDetails.length -1 ];
                        input.GreaterThanID = lastItem.BPVisualEventId;
                    }
                }
            }
        }
    }

    app.directive('bpWorkflowActivitysettingsVisualitemdefiniton', WorkflowActivitySettingsVisualItemSelective);

})(app);