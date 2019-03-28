(function (app) {

    'use strict';

    TelesRadiusSWSync.$inject = ["UtilsService", 'VRUIUtilsService', 'WhS_BE_CarrierAccountAPIService', 'VRNotificationService', 'WhS_BE_CarrierAccountActivationStatusEnum'];

    function TelesRadiusSWSync(UtilsService, VRUIUtilsService, WhS_BE_CarrierAccountAPIService, VRNotificationService, WhS_BE_CarrierAccountActivationStatusEnum) {

        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var telesRadiusSWSyncronizer = new TelesRadiusSWSyncronizer($scope, ctrl, $attrs);
                telesRadiusSWSyncronizer.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/TelesSynchronizer/Templates/TelesRadiusSWSyncTemplate.html"
        };

        function TelesRadiusSWSyncronizer($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            var radiusDataManager;

            var radiusDataManagerSettingsDirectiveAPI;
            var radiusDataManagerSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.isLoading = false;
                $scope.scopeModel.separator = ';';
                $scope.scopeModel.carrierAccountMappings = [];
                $scope.scopeModel.carrierAccountMappingsGridDS = [];

                $scope.onRadiusDataManagerSettingsDirectiveReady = function (api) {
                    radiusDataManagerSettingsDirectiveAPI = api;
                    radiusDataManagerSettingsDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.loadMoreData = function () {
                    return loadMoreCarrierMappings();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var telesRadiusSWSynSettings;

                    if (payload != undefined) {
                        telesRadiusSWSynSettings = payload.switchSynchronizerSettings;
                    }

                    if (telesRadiusSWSynSettings) {
                        $scope.scopeModel.separator = telesRadiusSWSynSettings.MappingSeparator;
                        radiusDataManager = telesRadiusSWSynSettings.DataManager;
                    }

                    var loadCarrierMappingPromise = loadCarrierMappings(payload);
                    promises.push(loadCarrierMappingPromise);

                    var loadDataManagerSettings = loadSwitchSyncSettingsDirective();
                    promises.push(loadDataManagerSettings);


                    function loadCarrierMappings(payload) {
                        $scope.scopeModel.isLoading = true;

                        var carrierAccountfilter = {
                            ActivationStatuses: [WhS_BE_CarrierAccountActivationStatusEnum.Active.value, WhS_BE_CarrierAccountActivationStatusEnum.Testing.value]
                        };

                        var serilizedCarrierAccountFilter = UtilsService.serializetoJson(carrierAccountfilter);

                        return WhS_BE_CarrierAccountAPIService.GetCarrierAccountInfo(serilizedCarrierAccountFilter).then(function (response) {

                            if (response) {
                                if (payload && payload.switchSynchronizerSettings && payload.switchSynchronizerSettings.CarrierMappings) {
                                    for (var i = 0; i < response.length; i++) {

                                        var accountCarrierMappings = payload.switchSynchronizerSettings.CarrierMappings[response[i].CarrierAccountId];
                                        var carrierMapping = {
                                            CarrierAccountId: response[i].CarrierAccountId,
                                            CarrierAccountName: response[i].Name,
                                            CustomerMapping: accountCarrierMappings != undefined && accountCarrierMappings.CustomerMapping != undefined ? accountCarrierMappings.CustomerMapping.join($scope.scopeModel.separator) : undefined,
                                            SupplierMapping: accountCarrierMappings != undefined && accountCarrierMappings.SupplierMapping != undefined ? accountCarrierMappings.SupplierMapping.join($scope.scopeModel.separator) : undefined
                                        };

                                        $scope.scopeModel.carrierAccountMappings.push(carrierMapping);
                                    }
                                }
                                else {
                                    for (var i = 0; i < response.length; i++) {

                                        var carrierMapping = {
                                            CarrierAccountId: response[i].CarrierAccountId,
                                            CarrierAccountName: response[i].Name,
                                            CustomerMapping: '',
                                            SupplierMapping: ''
                                        };
                                        $scope.scopeModel.carrierAccountMappings.push(carrierMapping);
                                    }
                                }

                                loadMoreCarrierMappings();
                            }
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        }).finally(function () {
                            $scope.scopeModel.isLoading = false;
                        });
                    }

                    function loadSwitchSyncSettingsDirective() {
                        var settingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        radiusDataManagerSettingsDirectiveReadyDeferred.promise.then(function () {

                            var settingsDirectivePayload;
                            if (radiusDataManager != undefined) {
                                settingsDirectivePayload = { radiusDataManagersSettings: radiusDataManager };
                            }
                            VRUIUtilsService.callDirectiveLoad(radiusDataManagerSettingsDirectiveAPI, settingsDirectivePayload, settingsDirectiveLoadDeferred);
                        });

                        return settingsDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "TOne.WhS.RouteSync.TelesRadius.TelesRadiusSWSync, TOne.WhS.RouteSync.TelesRadius",
                        DataManager: radiusDataManagerSettingsDirectiveAPI.getData().DataManager,
                        CarrierMappings: getCarrierMappings(),
                        MappingSeparator: $scope.scopeModel.separator
                    };

                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function getCarrierMappings() {

                var result = {};
                for (var i = 0; i < $scope.scopeModel.carrierAccountMappings.length; i++) {
                    var carrierMapping = $scope.scopeModel.carrierAccountMappings[i];
                    result[carrierMapping.CarrierAccountId] = {
                        CarrierId: carrierMapping.CarrierAccountId,
                        CustomerMapping: carrierMapping.CustomerMapping != undefined ? carrierMapping.CustomerMapping.split($scope.scopeModel.separator) : undefined,
                        SupplierMapping: carrierMapping.SupplierMapping != undefined ? carrierMapping.SupplierMapping.split($scope.scopeModel.separator) : undefined
                    };
                }
                return result;
            }

            function loadMoreCarrierMappings() {

                var pageInfo = gridAPI.getPageInfo();
                var itemsLength = pageInfo.toRow;

                if (pageInfo.toRow > $scope.scopeModel.carrierAccountMappings.length) {
                    if (pageInfo.fromRow < $scope.scopeModel.carrierAccountMappings.length)
                        itemsLength = $scope.scopeModel.carrierAccountMappings.length;
                    else
                        return;
                }

                var items = [];

                for (var i = pageInfo.fromRow - 1; i < itemsLength; i++) {
                    var currentCarrierAccountMapping = $scope.scopeModel.carrierAccountMappings[i];
                    items.push(currentCarrierAccountMapping);
                }

                gridAPI.addItemsToSource(items);
            }
        }
    }

    app.directive('whsRoutesyncTelesradiusSwsync', TelesRadiusSWSync);
})(app);