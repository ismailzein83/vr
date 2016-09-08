(function (app) {

    'use strict';

    TelesRadiusSWSync.$inject = ["UtilsService", 'VRUIUtilsService', 'WhS_BE_CarrierAccountAPIService', 'VRNotificationService'];

    function TelesRadiusSWSync(UtilsService, VRUIUtilsService, WhS_BE_CarrierAccountAPIService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
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
            var gridAPI;

            var radiusDataManager;
            var carrierAccountsAPI;
            var carrierAccountsPromiseDiffered;

            var radiusDataManagerSettingsDirectiveAPI;
            var radiusDataManagerSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            this.initializeController = initializeController;

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.isLoading = false;
                $scope.scopeModel.separator = ';';
                $scope.scopeModel.carrierAccountMappings = [];

                $scope.onRadiusDataManagerSettingsDirectiveReady = function (api) {
                    radiusDataManagerSettingsDirectiveAPI = api;
                    radiusDataManagerSettingsDirectiveReadyDeferred.resolve();
                };

                defineAPI();
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

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = getData;

                function getData() {
                    var data = {
                        $type: "TOne.WhS.RouteSync.TelesRadius.TelesRadiusSWSync, TOne.WhS.RouteSync.TelesRadius",
                        DataManager: getDataManager(),
                        CarrierMappings: getCarrierMappings(),
                        MappingSeparator: $scope.scopeModel.separator
                    }
                    return data;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function loadCarrierMappings(payload) {
                $scope.scopeModel.isLoading = true;
                var serializedFilter = {};
                return WhS_BE_CarrierAccountAPIService.GetCarrierAccountInfo(serializedFilter)
                 .then(function (response) {

                     if (response) {
                         if (payload && payload.switchSynchronizerSettings && payload.switchSynchronizerSettings.CarrierMappings) {
                             for (var i = 0; i < response.length; i++) {

                                 var accountCarrierMappings = payload.switchSynchronizerSettings.CarrierMappings[response[i].CarrierAccountId];
                                 var carrierMapping = {
                                     CarrierAccountId: response[i].CarrierAccountId,
                                     CarrierAccountName: response[i].Name,
                                     CustomerMapping: accountCarrierMappings != undefined ? accountCarrierMappings.CustomerMapping.join($scope.scopeModel.separator) : undefined,
                                     SupplierMapping: accountCarrierMappings != undefined ? accountCarrierMappings.SupplierMapping.join($scope.scopeModel.separator) : undefined
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
                     }
                 })
                  .catch(function (error) {
                      VRNotificationService.notifyException(error, $scope);
                      $scope.scopeModel.isLoading = false;
                  }).finally(function () {
                      $scope.scopeModel.isLoading = false;
                  });
            }

            function getCarrierMappings() {

                var result = {};
                for (var i = 0; i < $scope.scopeModel.carrierAccountMappings.length; i++) {
                    var carrierMapping = $scope.scopeModel.carrierAccountMappings[i];
                    result[carrierMapping.CarrierAccountId] = {
                        CarrierId: carrierMapping.CarrierAccountId,
                        CustomerMapping: carrierMapping.CustomerMapping.split($scope.scopeModel.separator),
                        SupplierMapping: carrierMapping.SupplierMapping.split($scope.scopeModel.separator)
                    };
                }
                return result;
            }

            function getDataManager() {
                return radiusDataManagerSettingsDirectiveAPI.getData().DataManager;
            }

            function loadSwitchSyncSettingsDirective() {
                var settingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                radiusDataManagerSettingsDirectiveReadyDeferred.promise.then(function () {
                    var settingsDirectivePayload;
                    if (radiusDataManager != undefined) {
                        settingsDirectivePayload = { radiusDataManagersSettings: radiusDataManager }
                    }
                    VRUIUtilsService.callDirectiveLoad(radiusDataManagerSettingsDirectiveAPI, settingsDirectivePayload, radiusDataManagerSettingsDirectiveReadyDeferred);
                });

                return radiusDataManagerSettingsDirectiveReadyDeferred.promise;
            }
        }
    }

    app.directive('whsRoutesyncTelesradiusSwsync', TelesRadiusSWSync);

})(app);