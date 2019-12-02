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
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/Cataleya/Synchronizer/Templates/CataleyaSWSyncTemplate.html"
        };

        function CataleyaSWSyncronizerCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var apiConnectionSelectorAPI;
            var apiConnectionSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var cataleyaDataManagerDirectiveAPI;
            var cataleyaDataManagerDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var carrierAccountMappingGridAPI;
            var carrierAccountMappingGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onAPIConnectionSelectorReady = function (api) {
                    apiConnectionSelectorAPI = api;
                    apiConnectionSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onCataleyaDataManagerDirectiveReady = function (api) {
                    cataleyaDataManagerDirectiveAPI = api;
                    cataleyaDataManagerDirectiveReadyDeferred.resolve();
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

                    var carrierMappings;
                    var apiConnectionId;
                    var cataleyaDataManager;

                    if (payload != undefined) {
                        var cataleyaSWSync = payload.switchSynchronizerSettings;

                        if (cataleyaSWSync != undefined) {
                            $scope.scopeModel.isEditMode = true;
                            carrierMappings = cataleyaSWSync.CarrierMappings;
                            apiConnectionId = cataleyaSWSync.APIConnectionId;
                            $scope.scopeModel.nodeID = cataleyaSWSync.NodeID;
                            $scope.scopeModel.blockedCode = cataleyaSWSync.BlockedCode;
                            cataleyaDataManager = cataleyaSWSync.DataManager;
                        }
                    }

                    //Loading API Connection Selector
                    var apiConnectionSelectorLoadPromise = getAPIConnectionSelectorLoadPromise();
                    promises.push(apiConnectionSelectorLoadPromise);

                    //Loading Cataleya Data Manager Directive
                    var cataleyaDataManagerLoadPromise = getCataleyaDataManagerLoadPromise();
                    promises.push(cataleyaDataManagerLoadPromise);

                    //Loading CarrierAccountMapping Grid
                    var carrierAccountMappingGridLoadPromise = getCarrierAccountMappingGridLoadPromise();
                    promises.push(carrierAccountMappingGridLoadPromise);

                    function getAPIConnectionSelectorLoadPromise() {
                        var apiConnectionSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        apiConnectionSelectorReadyPromiseDeferred.promise.then(function () {

                            var apiConnectionSelectorPayload = {
                                filter: {
                                    ConnectionTypeIds: ["071D54D2-463B-4404-8219-45FCD539FF01"] // VRHttpConnectionFilter
                                }
                            };
                            if (apiConnectionId != undefined) {
                                apiConnectionSelectorPayload.selectedIds = apiConnectionId;
                            }
                            VRUIUtilsService.callDirectiveLoad(apiConnectionSelectorAPI, apiConnectionSelectorPayload, apiConnectionSelectorLoadPromiseDeferred);
                        });

                        return apiConnectionSelectorLoadPromiseDeferred.promise;
                    }

                    function getCataleyaDataManagerLoadPromise() {
                        var cataleyaDataManagerDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        cataleyaDataManagerDirectiveReadyDeferred.promise.then(function () {

                            var cataleyaDataManagerDirectivePayload = {
                                cataleyaDataManager: cataleyaDataManager
                            };
                            VRUIUtilsService.callDirectiveLoad(cataleyaDataManagerDirectiveAPI, cataleyaDataManagerDirectivePayload, cataleyaDataManagerDirectiveLoadDeferred);
                        });

                        return cataleyaDataManagerDirectiveLoadDeferred.promise;
                    }

                    function getCarrierAccountMappingGridLoadPromise() {
                        var carrierAccountMappingGridLoadDeferred = UtilsService.createPromiseDeferred();

                        carrierAccountMappingGridReadyPromiseDeferred.promise.then(function () {

                            var carrierAccountMappingGridPayload = {
                                carrierMappings: carrierMappings
                            };
                            VRUIUtilsService.callDirectiveLoad(carrierAccountMappingGridAPI, carrierAccountMappingGridPayload, carrierAccountMappingGridLoadDeferred);
                        });

                        return carrierAccountMappingGridLoadDeferred.promise;
                    }

                    var rootPromiseNode = {
                        promises: promises
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {

                    var data = {
                        $type: "TOne.WhS.RouteSync.Cataleya.CataleyaSWSync, TOne.WhS.RouteSync.Cataleya",
                        APIConnectionId: apiConnectionSelectorAPI.getSelectedIds(),
                        NodeID: $scope.scopeModel.nodeID,
                        BlockedCode: $scope.scopeModel.blockedCode,
                        DataManager: cataleyaDataManagerDirectiveAPI.getData(),
                        CarrierMappings: carrierAccountMappingGridAPI.getData()
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