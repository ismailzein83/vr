(function (app) {

    'use strict';

    MVTSRadiusSWSync.$inject = ["UtilsService", 'VRUIUtilsService', 'WhS_BE_CarrierAccountAPIService', 'VRNotificationService'];

    function MVTSRadiusSWSync(UtilsService, VRUIUtilsService, WhS_BE_CarrierAccountAPIService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var mvtsRadiusSWSyncronizer = new MVTSRadiusSWSyncronizer($scope, ctrl, $attrs);
                mvtsRadiusSWSyncronizer.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/MVTSSynchronizer/Templates/MVTSRadiusSWSyncTemplate.html"

        };
        function MVTSRadiusSWSyncronizer($scope, ctrl, $attrs) {

            var gridAPI;
            var radiusDataManager;

            var carrierAccountsAPI;
            var carrierAccountsPromiseDiffered;

            var radiusDataManagerSettingsDirectiveAPI;
            var radiusDataManagerSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            this.initializeController = initializeController;

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.mappingColumns = getMappingColumns();
                $scope.scopeModel.isLoading = false;
                $scope.scopeModel.separator = ';';
                $scope.scopeModel.carrierAccountMappings = [];
                $scope.scopeModel.filterdCarrierMappings = [];

                $scope.onRadiusDataManagerSettingsDirectiveReady = function (api) {
                    radiusDataManagerSettingsDirectiveAPI = api;
                    radiusDataManagerSettingsDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var mvtsRadiusSWSynSettings;

                    if (payload != undefined) {
                        mvtsRadiusSWSynSettings = payload.switchSynchronizerSettings;
                    }

                    if (mvtsRadiusSWSynSettings) {
                        $scope.scopeModel.separator = mvtsRadiusSWSynSettings.MappingSeparator;
                        $scope.scopeModel.numberOfOptions = mvtsRadiusSWSynSettings.NumberOfOptions;

                        radiusDataManager = mvtsRadiusSWSynSettings.DataManager;
                        //redundantRadiusDataManager = mvtsRadiusSWSynSettings.RedundantDataManager;
                    }

                    var loadDataManagerSettings = loadSwitchSyncSettingsDirective();
                    promises.push(loadDataManagerSettings);

                    var loadGridPromise = loadGrid(payload);
                    promises.push(loadGridPromise);

                    //var loadRedundantDataManagerSettings = loadRedundantSwitchSyncSettingsDirective();
                    //promises.push(loadRedundantDataManagerSettings);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = getData;

                function getData() {
                    var data = {
                        $type: "TOne.WhS.RouteSync.MVTSRadius.MVTSRadiusSWSync, TOne.WhS.RouteSync.MVTSRadius",
                        DataManager: getDataManager(),
                        //RedundantDataManager: getRedundantDataManager(),
                        CarrierMappings: gridAPI.getData(),
                        MappingSeparator: $scope.scopeModel.separator,
                        NumberOfOptions: $scope.scopeModel.numberOfOptions
                    };
                    return data;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
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

            function loadGrid(payload) {
                return gridAPI.load(payload);
            }

            function getMappingColumns() {
                var mappings = [];
                mappings.push({
                    Column: 'CustomerMapping',
                    Name: 'Customer Mapping',
                    Type: 'text'
                });
                mappings.push({
                    Column: 'SupplierMapping',
                    Name: 'Supplier Mapping',
                    Type: 'text'
                });
                return mappings;
            }

        }
    }

    app.directive('whsRoutesyncMvtsradiusSwsync', MVTSRadiusSWSync);

})(app);