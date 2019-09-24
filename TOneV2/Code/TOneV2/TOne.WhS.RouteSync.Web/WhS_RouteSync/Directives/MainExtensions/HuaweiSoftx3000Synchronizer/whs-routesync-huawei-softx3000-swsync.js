(function (app) {

    'use strict';

    HuaweiSoftX3000SWSync.$inject = ['UtilsService', 'VRUIUtilsService'];

    function HuaweiSoftX3000SWSync(UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new HuaweiSoftX3000SWSyncCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_RouteSync/Directives/MainExtensions/HuaweiSoftX3000Synchronizer/Templates/HuaweiSoftX3000SWSyncTemplate.html'
        };

        function HuaweiSoftX3000SWSyncCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var huaweiSoftX3000SWSync;
            var carrierMappings;
            var sshCommunicationList;
            var switchLoggerList;

            var switchCommunicationAPI;
            var switchCommunicationReadyDeferred = UtilsService.createPromiseDeferred();

            var huaweiSoftX3000CarrierAccountMappingGridAPI;
            var huaweiSoftX3000CarrierAccountMappingGridReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onHuaweiSoftX3000SwitchCommunicationReady = function (api) {
                    switchCommunicationAPI = api;
                    switchCommunicationReadyDeferred.resolve();
                };

                $scope.scopeModel.onHuaweiSoftX3000CarrierAccountMappingGridReady = function (api) {
                    huaweiSoftX3000CarrierAccountMappingGridAPI = api;
                    huaweiSoftX3000CarrierAccountMappingGridReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        huaweiSoftX3000SWSync = payload.switchSynchronizerSettings;

                        if (huaweiSoftX3000SWSync != undefined) {
                            $scope.scopeModel.numberOfOptions = huaweiSoftX3000SWSync.NumberOfOptions;
                            carrierMappings = huaweiSoftX3000SWSync.CarrierMappings;
                            sshCommunicationList = huaweiSoftX3000SWSync.SwitchCommunicationList;
                            switchLoggerList = huaweiSoftX3000SWSync.SwitchLoggerList;
                        }
                    }

                    var switchCommunicationLoadPromise = getSwitchCommunicationLoadPromise();
                    promises.push(switchCommunicationLoadPromise);

                    var carrierAccountMappingGridLoadPromise = getCarrierAccountMappingGridLoadPromise();
                    promises.push(carrierAccountMappingGridLoadPromise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var switchCommunicationData = switchCommunicationAPI.getData();

                    var data = {
                        $type: "TOne.WhS.RouteSync.Huawei.SoftX3000.HuaweiSoftX3000SWSync, TOne.WhS.RouteSync.Huawei",
                        NumberOfOptions: $scope.scopeModel.numberOfOptions,
                        CarrierMappings: huaweiSoftX3000CarrierAccountMappingGridAPI.getData(),
                        SwitchCommunicationList: switchCommunicationData != undefined ? switchCommunicationData.sshCommunicationList : undefined,
                        SwitchLoggerList: switchCommunicationData != undefined ? switchCommunicationData.switchLoggerList : undefined
                    };

                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function getSwitchCommunicationLoadPromise() {
                var switchCommunicationLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                switchCommunicationReadyDeferred.promise.then(function () {

                    var switchCommunicationPayload = {
                        sshCommunicationList: sshCommunicationList,
                        switchLoggerList: switchLoggerList
                    };
                    VRUIUtilsService.callDirectiveLoad(switchCommunicationAPI, switchCommunicationPayload, switchCommunicationLoadPromiseDeferred);
                });

                return switchCommunicationLoadPromiseDeferred.promise;
            }

            function getCarrierAccountMappingGridLoadPromise() {
                var carrierAccountMappingGridLoadDeferred = UtilsService.createPromiseDeferred();

                huaweiSoftX3000CarrierAccountMappingGridReadyDeferred.promise.then(function () {

                    var carrierAccountMappingGridPayload = {
                        carrierMappings: carrierMappings
                    };
                    VRUIUtilsService.callDirectiveLoad(huaweiSoftX3000CarrierAccountMappingGridAPI, carrierAccountMappingGridPayload, carrierAccountMappingGridLoadDeferred);
                });

                return carrierAccountMappingGridLoadDeferred.promise;
            }
        }
    }

    app.directive('whsRoutesyncHuaweiSoftx3000Swsync', HuaweiSoftX3000SWSync);
})(app);