(function (app) {

    'use strict';

    QueueactivatorconfigSelectiveDirective.$inject = ['VR_Queueing_QueueActivatorConfigAPIService', 'UtilsService', 'VRUIUtilsService'];

    function QueueactivatorconfigSelectiveDirective(VR_Queueing_QueueActivatorConfigAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var queueactivatorconfigSelective = new QueueactivatorconfigSelective($scope, ctrl, $attrs);
                queueactivatorconfigSelective.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Queueing/Directives/QueueActivator/Templates/QueueActivatorTypeSelectorTemplate.html"
        };

        function QueueactivatorconfigSelective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var queueActivatorSelectorAPI;

            var wrapperDirectiveAPI;
            var wrapperDirectiveReadyDeferred;

            var payloadObj = {};
            function initializeController() {
                $scope.scopeModal = {};
                $scope.scopeModal.queueActivatorConfig = [];
                $scope.scopeModal.selectedqueueActivatorConfig;

                $scope.scopeModal.onQueueActivatorSelectorReady = function (api) {
                    queueActivatorSelectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                $scope.scopeModal.onWrapperDirectiveReady = function (api) {
                    wrapperDirectiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModal.isLoading = value;
                    };

                    var activatorPayload = {};                    
                    activatorPayload.ExistingStages = payloadObj.ExistingStages;
                    activatorPayload.DataRecordTypeId = payloadObj.DataRecordTypeId;

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModal, wrapperDirectiveAPI, activatorPayload, setLoader, wrapperDirectiveReadyDeferred);
                };
            } 

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    payloadObj = payload;
                    queueActivatorSelectorAPI.clearDataSource();
                    var promises = [];
                    var getActivatorTemplatesPromise = VR_Queueing_QueueActivatorConfigAPIService.GetQueueActivatorsConfig();
                    promises.push(getActivatorTemplatesPromise);

                    var loadActivatorPromiseDeferred;

                    if (payload != undefined && payload.QueueActivator != undefined) {
                        loadActivatorPromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadActivatorPromiseDeferred.promise);
                    }

                    getActivatorTemplatesPromise.then(function (response) {
                        $scope.scopeModal.queueActivatorConfig = [];
                        for (var i = 0; i < response.length; i++) {
                            $scope.scopeModal.queueActivatorConfig.push(response[i]);
                        }

                        if (payload != undefined && payload.QueueActivator != undefined) {
                            wrapperDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
                            $scope.scopeModal.selectedqueueActivatorConfig = UtilsService.getItemByVal($scope.scopeModal.queueActivatorConfig, payload.QueueActivator.ConfigId, 'QueueActivatorConfigId');

                            wrapperDirectiveReadyDeferred.promise.then(function () {
                                wrapperDirectiveReadyDeferred = undefined;
                                var activatorPayload = {};
                                activatorPayload.QueueActivator = payload.QueueActivator;
                                activatorPayload.ExistingStages = payload.ExistingStages;
                                activatorPayload.DataRecordTypeId = payload.DataRecordTypeId;
                                VRUIUtilsService.callDirectiveLoad(wrapperDirectiveAPI, activatorPayload, loadActivatorPromiseDeferred);
                            });
                        }
                    });

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {};

                    if ($scope.scopeModal.selectedqueueActivatorConfig != undefined) {
                        data = wrapperDirectiveAPI.getData();
                        data.ConfigId = $scope.scopeModal.selectedqueueActivatorConfig.QueueActivatorConfigId;
                    }
                    return data;
                };

                api.getQueueItemType = function () {
                    if (wrapperDirectiveAPI.getQueueItemType != undefined)
                        return wrapperDirectiveAPI.getQueueItemType();
                    else
                        return {
                            $type: "Vanrise.GenericData.QueueActivators.DataRecordBatchQueueItemType, Vanrise.GenericData.QueueActivators"
                        };
                };

                return api;
            }
        }
    }

    app.directive('vrQueueingQueueactivatortypeSelector', QueueactivatorconfigSelectiveDirective);

})(app);