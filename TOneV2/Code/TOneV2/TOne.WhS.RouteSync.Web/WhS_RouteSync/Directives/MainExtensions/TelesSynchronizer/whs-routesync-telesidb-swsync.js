﻿(function (app) {

    'use strict';

    TelesIdbSWSync.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService', 'WhS_BE_CarrierAccountAPIService'];

    function TelesIdbSWSync(UtilsService, VRUIUtilsService, VRNotificationService, WhS_BE_CarrierAccountAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new TelesIdbSWSyncronizerCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/TelesSynchronizer/Templates/TelesIdbSWSyncTemplate.html"
        };

        function TelesIdbSWSyncronizerCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var idbDataManager;
            var carrierMappings;

            var idbDataManagerSettingsDirectiveAPI;
            var idbDataManagerSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var telesIdbCarrierAccountMappingGridAPI;
            var telesIdbCarrierAccountMappingGridReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.mappingSeparator = ';';

                $scope.scopeModel.onIdbDataManagerSettingsDirectiveReady = function (api) {
                    idbDataManagerSettingsDirectiveAPI = api;
                    idbDataManagerSettingsDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onTelesIdbCarrierAccountMappingGridReady = function (api) {
                    telesIdbCarrierAccountMappingGridAPI = api;
                    telesIdbCarrierAccountMappingGridReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var telesIdbSWSync;

                    if (payload != undefined) {
                        telesIdbSWSync = payload.switchSynchronizerSettings;

                        if (telesIdbSWSync != undefined) {
                            $scope.scopeModel.mappingSeparator = telesIdbSWSync.MappingSeparator;
                            $scope.scopeModel.numberOfOptions = telesIdbSWSync.NumberOfOptions;
                            $scope.scopeModel.useTwoSuppliersMapping = telesIdbSWSync.UseTwoSuppliersMapping;
                            $scope.scopeModel.supplierOptionsSeparator = telesIdbSWSync.SupplierOptionsSeparator;
                            idbDataManager = telesIdbSWSync.DataManager;
                            carrierMappings = telesIdbSWSync.CarrierMappings;
                        }
                    }

                    var switchSyncSettingsDirectiveLoadPromise = getSwitchSyncSettingsDirectiveLoadPromise();
                    promises.push(switchSyncSettingsDirectiveLoadPromise);

                    var carrierAccountMappingGridLoadPromise = getCarrierAccountMappingGridLoadPromise(payload);
                    promises.push(carrierAccountMappingGridLoadPromise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var data = {
                        $type: "TOne.WhS.RouteSync.TelesIdb.TelesIdbSWSync, TOne.WhS.RouteSync.TelesIdb",
                        DataManager: idbDataManagerSettingsDirectiveAPI.getData(),
                        MappingSeparator: $scope.scopeModel.mappingSeparator,
                        NumberOfOptions: $scope.scopeModel.numberOfOptions,
                        UseTwoSuppliersMapping: $scope.scopeModel.useTwoSuppliersMapping,
                        SupplierOptionsSeparator: $scope.scopeModel.supplierOptionsSeparator,
                        CarrierMappings: telesIdbCarrierAccountMappingGridAPI.getData(),
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function getSwitchSyncSettingsDirectiveLoadPromise() {
                var settingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                idbDataManagerSettingsDirectiveReadyDeferred.promise.then(function () {

                    var settingsDirectivePayload;
                    if (idbDataManager != undefined) {
                        settingsDirectivePayload = { idbDataManagersSettings: idbDataManager };
                    }
                    VRUIUtilsService.callDirectiveLoad(idbDataManagerSettingsDirectiveAPI, settingsDirectivePayload, settingsDirectiveLoadDeferred);
                });

                return settingsDirectiveLoadDeferred.promise;
            }
            function getCarrierAccountMappingGridLoadPromise(payload) {
                var carrierAccountMappingGridLoadDeferred = UtilsService.createPromiseDeferred();

                telesIdbCarrierAccountMappingGridReadyDeferred.promise.then(function () {

                    var payload = {
                        context: buildContext(),
                        carrierMappings: carrierMappings
                    };
                    VRUIUtilsService.callDirectiveLoad(telesIdbCarrierAccountMappingGridAPI, payload, carrierAccountMappingGridLoadDeferred);
                });

                return carrierAccountMappingGridLoadDeferred.promise;
            }

            function buildContext() {
                var context = {
                    getUseTwoSuppliersMapping: function () {
                        return $scope.scopeModel.useTwoSuppliersMapping;
                    },
                    getMappingSeparator: function () {
                        return $scope.scopeModel.mappingSeparator;
                    }
                };
                return context;
            }
        }
    }

    app.directive('whsRoutesyncTelesidbSwsync', TelesIdbSWSync);

})(app);