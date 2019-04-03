(function (app) {

    'use strict';

    TelestaxSWSync.$inject = ["UtilsService", 'VRUIUtilsService'];

    function TelestaxSWSync(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new TelestaxSWSyncronizerCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/TelestaxSynchronizer/Templates/TelestaxSWSyncTemplate.html"
        };

        function TelestaxSWSyncronizerCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var switchCommunicationAPI;
            var switchCommunicationReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var carrierAccountMappingGridAPI;
            var carrierAccountMappingGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onEricssonSwitchCommunicationReady = function (api) {
                    switchCommunicationAPI = api;
                    switchCommunicationReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onCarrierAccountMappingGridReady = function (api) {
                    carrierAccountMappingGridAPI = api;
                    carrierAccountMappingGridReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var telestaxSWSync;
                    var carrierMappings;
                    var sshCommunicationList;
                    var switchLoggerList;

                    if (payload != undefined) {
                        telestaxSWSync = payload.switchSynchronizerSettings;

                        if (telestaxSWSync != undefined) {
                            $scope.scopeModel.isEditMode = true;

                            carrierMappings = telestaxSWSync.CarrierMappings;
                            sshCommunicationList = telestaxSWSync.SwitchCommunicationList;
                            switchLoggerList = telestaxSWSync.SwitchLoggerList;
                        }
                    }

                    //Loading Switch Communication
                    var switchCommunicationLoadPromise = getSwitchCommunicationLoadPromise();
                    promises.push(switchCommunicationLoadPromise);

                    //Loading CarrierAccountMapping Grid
                    var carrierAccountMappingGridLoadPromise = getCarrierAccountMappingGridLoadPromise();
                    promises.push(carrierAccountMappingGridLoadPromise);

                    function getSwitchCommunicationLoadPromise() {
                        var switchCommunicationLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        switchCommunicationReadyPromiseDeferred.promise.then(function () {

                            var switchCommunicationPayload;
                            if (telestaxSWSync != undefined) {
                                switchCommunicationPayload = { sshCommunicationList: sshCommunicationList, switchLoggerList: switchLoggerList };
                            }
                            VRUIUtilsService.callDirectiveLoad(switchCommunicationAPI, switchCommunicationPayload, switchCommunicationLoadPromiseDeferred);
                        });

                        return switchCommunicationLoadPromiseDeferred.promise;
                    }
                    function getCarrierAccountMappingGridLoadPromise() {
                        var carrierAccountMappingGridLoadDeferred = UtilsService.createPromiseDeferred();

                        carrierAccountMappingGridReadyPromiseDeferred.promise.then(function () {
                            var carrierAccountMappingGridPayload = { carrierMappings: carrierMappings };
                            VRUIUtilsService.callDirectiveLoad(carrierAccountMappingGridAPI, carrierAccountMappingGridPayload, carrierAccountMappingGridLoadDeferred);
                        });

                        return carrierAccountMappingGridLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    function getManualRoutesSettings() {
                        var manualRouteSettings = {
                            TelestaxManualRoutes: manualRouteSettingsAPI != undefined ? manualRouteSettingsAPI.getData() : null,
                            TelestaxSpecialRoutes: specialRoutingSettingsAPI != undefined ? specialRoutingSettingsAPI.getData() : null
                        };
                        return manualRouteSettings;
                    }

                    var switchCommunicationData = switchCommunicationAPI.getData();
                    var data = {
                        $type: "TOne.WhS.RouteSync.Telestax.TelestaxSWSync, TOne.WhS.RouteSync.Telestax",
                        CarrierMappings: carrierAccountMappingGridAPI.getData(),
                        SwitchCommunicationList: switchCommunicationData != undefined ? switchCommunicationData.sshCommunicationList : undefined,
                        SwitchLoggerList: switchCommunicationData != undefined ? switchCommunicationData.switchLoggerList : undefined,
                    };

                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncTelestaxSwsync', TelestaxSWSync);
})(app);