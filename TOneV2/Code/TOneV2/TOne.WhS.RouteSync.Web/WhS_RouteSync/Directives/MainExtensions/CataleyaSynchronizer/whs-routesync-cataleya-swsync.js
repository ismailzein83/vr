(function (app) {

    "use strict";

    CataleyaSWSync.$inject = ["UtilsService", "VRUIUtilsService"];

    function CataleyaSWSync(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CataleyaSWSyncronizerCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/CataleyaSynchronizer/Templates/CataleyaSWSyncTemplate.html"
        };

        function CataleyaSWSyncronizerCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var switchCommunicationAPI;
            var switchCommunicationReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var carrierAccountMappingGridAPI;
            var carrierAccountMappingGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onCataleyaSwitchCommunicationReady = function (api) {
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

                    var cataleyaSWSync;
                    var carrierMappings;
                    var vrConnectionId;

                    if (payload != undefined) {
                        cataleyaSWSync = payload.switchSynchronizerSettings;

                        if (cataleyaSWSync != undefined) {
                            $scope.scopeModel.isEditMode = true;
                            carrierMappings = cataleyaSWSync.CarrierMappings;
                            vrConnectionId = cataleyaSWSync.VRConnectionId;
                        }
                    }

                    //Loading Switch Communication
                    var switchCommunicationLoadPromise = getSwitchCommunicationLoadPromise();
                    promises.push(switchCommunicationLoadPromise);

                    //Loading CarrierAccountMapping Grid
                    var carrierAccountMappingGridLoadPromise = getCarrierAccountMappingGridLoadPromise();
                    promises.push(carrierAccountMappingGridLoadPromise);

                    var rootPromiseNode = {
                        promises: promises
                    };

                    function getSwitchCommunicationLoadPromise() {
                        var switchCommunicationLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        switchCommunicationReadyPromiseDeferred.promise.then(function () {
                            var switchCommunicationPayload;
                            if (cataleyaSWSync != undefined) {
                                switchCommunicationPayload = { vrConnectionId: vrConnectionId};
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

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {

                    var data = {
                        $type: "TOne.WhS.RouteSync.Cataleya.CataleyaSWSync, TOne.WhS.RouteSync.Cataleya",
                        CarrierMappings: carrierAccountMappingGridAPI.getData(),
                        VRConnectionId: switchCommunicationAPI.getData()
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncCataleyaSwsync', CataleyaSWSync);

})(app);