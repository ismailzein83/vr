(function (app) {

    'use strict';

    FreeRadiusSWSync.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function FreeRadiusSWSync(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FreeRadiusSWSyncronizerCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/FreeRadius/FreeRadiusSynchronizer/Templates/FreeRadiusSWSyncTemplate.html"
        };

        function FreeRadiusSWSyncronizerCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var freeRadiusDataManager;
            var carrierMappings;

            var freeRadiusDataManagerDirectiveAPI;
            var freeRadiusDataManagerDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var freeRadiusCarrierAccountMappingGridAPI;
            var freeRadiusCarrierAccountMappingGridReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.mappingSeparator = ';';

                $scope.scopeModel.onFreeRadiusDataManagersDirectiveReady = function (api) {
                    freeRadiusDataManagerDirectiveAPI = api;
                    freeRadiusDataManagerDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onFreeRadiusCarrierAccountMappingGridReady = function (api) {
                    freeRadiusCarrierAccountMappingGridAPI = api;
                    freeRadiusCarrierAccountMappingGridReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var freeRadiusSWSync;

                    if (payload != undefined) {
                        freeRadiusSWSync = payload.switchSynchronizerSettings;

                        if (freeRadiusSWSync != undefined) {
                            $scope.scopeModel.mappingSeparator = freeRadiusSWSync.MappingSeparator;
                            $scope.scopeModel.supplierOptionsSeparator = freeRadiusSWSync.SupplierOptionsSeparator;
                            $scope.scopeModel.numberOfOptions = freeRadiusSWSync.NumberOfOptions;
                            $scope.scopeModel.syncSaleCodeZones = freeRadiusSWSync.SyncSaleCodeZones;
                            freeRadiusDataManager = freeRadiusSWSync.DataManager;
                            carrierMappings = freeRadiusSWSync.CarrierMappings;
                        }
                    }

                    var freeRadiusDataManagerDirectiveLoadPromise = getFreeRadiusDataManagerDirectiveLoadPromise();
                    promises.push(freeRadiusDataManagerDirectiveLoadPromise);

                    var carrierAccountMappingGridLoadPromise = getCarrierAccountMappingGridLoadPromise(payload);
                    promises.push(carrierAccountMappingGridLoadPromise);

                    function getFreeRadiusDataManagerDirectiveLoadPromise() {
                        var freeRadiusDataManagerDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        freeRadiusDataManagerDirectiveReadyDeferred.promise.then(function () {

                            var payload;
                            if (freeRadiusDataManager != undefined) {
                                payload = { freeRadiusDataManager: freeRadiusDataManager };
                            }
                            VRUIUtilsService.callDirectiveLoad(freeRadiusDataManagerDirectiveAPI, payload, freeRadiusDataManagerDirectiveLoadDeferred);
                        });

                        return freeRadiusDataManagerDirectiveLoadDeferred.promise;
                    }
                    function getCarrierAccountMappingGridLoadPromise(payload) {
                        var carrierAccountMappingGridLoadDeferred = UtilsService.createPromiseDeferred();

                        freeRadiusCarrierAccountMappingGridReadyDeferred.promise.then(function () {

                            var payload = {
                                context: buildContext(),
                                carrierMappings: carrierMappings
                            };
                            VRUIUtilsService.callDirectiveLoad(freeRadiusCarrierAccountMappingGridAPI, payload, carrierAccountMappingGridLoadDeferred);
                        });

                        return carrierAccountMappingGridLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var data = {
                        $type: "TOne.WhS.RouteSync.FreeRadius.FreeRadiusSWSync, TOne.WhS.RouteSync.FreeRadius",
                        DataManager: freeRadiusDataManagerDirectiveAPI.getData(),
                        MappingSeparator: $scope.scopeModel.mappingSeparator,
                        SupplierOptionsSeparator: $scope.scopeModel.supplierOptionsSeparator,
                        NumberOfOptions: $scope.scopeModel.numberOfOptions,
                        SyncSaleCodeZones: $scope.scopeModel.syncSaleCodeZones,
                        CarrierMappings: freeRadiusCarrierAccountMappingGridAPI.getData()
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function buildContext() {
                var context = {
                    getMappingSeparator: function () {
                        return $scope.scopeModel.mappingSeparator;
                    }
                };
                return context;
            }
        }
    }

    app.directive('whsRoutesyncFreeradiusSwsync', FreeRadiusSWSync);

})(app);