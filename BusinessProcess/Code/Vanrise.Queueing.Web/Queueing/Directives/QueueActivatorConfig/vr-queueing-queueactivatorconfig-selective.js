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
            templateUrl: "/Client/Modules/Queueing/Directives/QueueActivatorConfig/Templates/QueueActivatorConfigSelectiveTemplate.html"
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
                        $scope.scopeModal.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModal, directiveAPI, payloadObj, setLoader, directiveReadyDeferred);
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();
                    return VR_Queueing_QueueActivatorConfigAPIService.GetQueueActivatorsConfig().then(function (response) {
                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                $scope.scopeModal.queueActivatorConfig.push(response[i]);
                            }
                            if (payload.stagesDataSource != undefined)
                                 payloadObj.stagesDataSource = payload.stagesDataSource;
                            if (payload != undefined && payload.QueueActivator != undefined) {
                                payloadObj = payload.QueueActivator;
                                payloadObj.stagesDataSource = payload.stagesDataSource;
                                 $scope.scopeModal.selectedqueueActivatorConfig = UtilsService.getItemByVal($scope.scopeModal.queueActivatorConfig, payload.QueueActivator.ConfigId, 'QueueActivatorConfigId');
                            }
                        }
                    });
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

    app.directive('vrQueueingQueueactivatorconfigSelective', QueueactivatorconfigSelectiveDirective);

})(app);