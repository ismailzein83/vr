(function (app) {

    'use strict';

    HuaweiSWSync.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function HuaweiSWSync(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new HuaweiSWSyncronizerCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/HuaweiSynchronizer/Templates/HuaweiSWSyncTemplate.html"
        };

        function HuaweiSWSyncronizerCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var idbDataManager;
            var carrierMappings;
            var manualRoutes;

            var sshCommunicationList;
            var switchLoggerList;
            var huaweiSWSync;

            var huaweiCarrierAccountMappingGridAPI;
            var huaweiCarrierAccountMappingGridReadyDeferred = UtilsService.createPromiseDeferred();

            var switchCommunicationAPI;
            var switchCommunicationReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.mappingSeparator = ';';
                $scope.scopeModel.supplierMappingLength = '4';

                $scope.scopeModel.onHuaweiCarrierAccountMappingGridReady = function (api) {
                    huaweiCarrierAccountMappingGridAPI = api;
                    huaweiCarrierAccountMappingGridReadyDeferred.resolve();
                };

                $scope.scopeModel.onHuaweiSwitchCommunicationReady = function (api) {
                    switchCommunicationAPI = api;
                    switchCommunicationReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];



                    if (payload != undefined) {
                        huaweiSWSync = payload.switchSynchronizerSettings;

                        if (huaweiSWSync != undefined) {
                            $scope.scopeModel.mappingSeparator = huaweiSWSync.MappingSeparator;
                            $scope.scopeModel.numberOfMappings = huaweiSWSync.NumberOfMappings;
                            $scope.scopeModel.supplierMappingLength = huaweiSWSync.SupplierMappingLength;
                            $scope.scopeModel.supplierOptionsSeparator = huaweiSWSync.SupplierOptionsSeparator;
                            $scope.scopeModel.numberOfOptions = huaweiSWSync.NumberOfOptions;
                            idbDataManager = huaweiSWSync.DataManager;
                            carrierMappings = huaweiSWSync.CarrierMappings;
                            manualRoutes = huaweiSWSync.ManualRoutes;
                            sshCommunicationList = huaweiSWSync.SwitchCommunicationList;
                            switchLoggerList = huaweiSWSync.SwitchLoggerList;
                        }
                    }

                    //Loading Carrier Mapping
                    var carrierAccountMappingGridLoadPromise = getCarrierAccountMappingGridLoadPromise(payload);
                    promises.push(carrierAccountMappingGridLoadPromise);



                    //Loading Switch Communication
                    var switchCommunicationLoadPromise = getSwitchCommunicationLoadPromise();
                    promises.push(switchCommunicationLoadPromise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var switchCommunicationData = switchCommunicationAPI.getData();

                    var data = {
                        $type: "TOne.WhS.RouteSync.Huawei.HuaweiSWSync, TOne.WhS.RouteSync.Huawei",
                        MappingSeparator: $scope.scopeModel.mappingSeparator,
                        NumberOfMappings: $scope.scopeModel.numberOfMappings,
                        SupplierMappingLength: $scope.scopeModel.supplierMappingLength,
                        SupplierOptionsSeparator: $scope.scopeModel.supplierOptionsSeparator,
                        NumberOfOptions: $scope.scopeModel.numberOfOptions,
                        CarrierMappings: huaweiCarrierAccountMappingGridAPI.getData(),
                        SwitchCommunicationList: switchCommunicationData != undefined ? switchCommunicationData.sshCommunicationList : undefined,
                        SwitchLoggerList: switchCommunicationData != undefined ? switchCommunicationData.switchLoggerList : undefined,
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function getCarrierAccountMappingGridLoadPromise(payload) {

                var carrierAccountMappingGridLoadDeferred = UtilsService.createPromiseDeferred();

                huaweiCarrierAccountMappingGridReadyDeferred.promise.then(function () {
                    var carrierAccountMappingGridPayload = { carrierMappings: carrierMappings };
                    VRUIUtilsService.callDirectiveLoad(huaweiCarrierAccountMappingGridAPI, carrierAccountMappingGridPayload, carrierAccountMappingGridLoadDeferred);
                });

                return carrierAccountMappingGridLoadDeferred.promise;
            }

            function getSwitchCommunicationLoadPromise() {
                var switchCommunicationLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                switchCommunicationReadyPromiseDeferred.promise.then(function () {
                    var switchCommunicationPayload;
                    if (huaweiSWSync != undefined) {
                        switchCommunicationPayload = { sshCommunicationList: sshCommunicationList, switchLoggerList: switchLoggerList };
                    }
                    VRUIUtilsService.callDirectiveLoad(switchCommunicationAPI, switchCommunicationPayload, switchCommunicationLoadPromiseDeferred);
                });

                return switchCommunicationLoadPromiseDeferred.promise;
            }

            function buildContext() {
                var context = {
                    getMappingSeparator: function () {
                        return $scope.scopeModel.mappingSeparator;
                    },
                    getSupplierMappingLength: function () {
                        return $scope.scopeModel.supplierMappingLength;
                    }
                };
                return context;
            }
        }
    }

    app.directive('whsRoutesyncHuaweiSwsync', HuaweiSWSync);

})(app);