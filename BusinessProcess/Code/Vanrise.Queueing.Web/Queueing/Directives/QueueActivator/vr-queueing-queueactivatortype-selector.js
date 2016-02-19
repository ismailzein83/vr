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

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;

            var payloadObj = {};
            function initializeController() {
                $scope.scopeModal = {};
                $scope.scopeModal.queueActivatorConfig = [];
                $scope.scopeModal.selectedqueueActivatorConfig;

                $scope.scopeModal.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                $scope.scopeModal.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModal.isLoading = value;
                    };

                    var activatorPayload = {};                    
                    activatorPayload.StagesDataSource = payloadObj.StagesDataSource;
                    activatorPayload.DataRecordTypeId = payloadObj.DataRecordTypeId;

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModal, directiveAPI, activatorPayload, setLoader, directiveReadyDeferred);
                };
            } 

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    payloadObj = payload;
                    selectorAPI.clearDataSource();
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
                            directiveReadyDeferred = UtilsService.createPromiseDeferred();
                            $scope.scopeModal.selectedqueueActivatorConfig = UtilsService.getItemByVal($scope.scopeModal.queueActivatorConfig, payload.QueueActivator.ConfigId, 'QueueActivatorConfigId');

                            directiveReadyDeferred.promise.then(function () {
                                directiveReadyDeferred = undefined;
                                var activatorPayload = {};
                                activatorPayload.QueueActivator = payload.QueueActivator;
                                activatorPayload.StagesDataSource = payload.StagesDataSource;
                                activatorPayload.DataRecordTypeId = payload.DataRecordTypeId;
                                VRUIUtilsService.callDirectiveLoad(directiveAPI, activatorPayload, loadActivatorPromiseDeferred);
                            });
                        }
                    });

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {};

                    if ($scope.scopeModal.selectedqueueActivatorConfig != undefined) {
                        data = directiveAPI.getData();
                        data.ConfigId = $scope.scopeModal.selectedqueueActivatorConfig.QueueActivatorConfigId;
                    }
                    return data;
                };


                return api;
            }
        }
    }

    app.directive('vrQueueingQueueactivatortypeSelector', QueueactivatorconfigSelectiveDirective);

})(app);