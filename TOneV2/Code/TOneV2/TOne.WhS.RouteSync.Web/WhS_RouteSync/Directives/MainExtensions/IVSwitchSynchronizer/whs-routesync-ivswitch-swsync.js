(function (app) {

    'use strict';

    ivswitchSwSync.$inject = ['UtilsService', 'WhS_BE_CarrierAccountAPIService', 'VRNotificationService', 'WhS_BE_CarrierAccountActivationStatusEnum'];

    function ivswitchSwSync(UtilsService, WhS_BE_CarrierAccountAPIService, VRNotificationService, WhS_BE_CarrierAccountActivationStatusEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ivSWSyncronizer = new IVSWSyncronizer($scope, ctrl, $attrs);
                ivSWSyncronizer.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/IVSwitchSynchronizer/Templates/IVSwitchSWSyncTemplate.html"

        };

        function IVSWSyncronizer($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.isLoading = false;
                $scope.scopeModel.carrierAccountMappings = [];
                $scope.scopeModel.carrierAccountMappingsGridDS = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineApi();
                };

                $scope.scopeModel.loadMoreData = function () {
                    return loadMoreCarrierMappings();
                };
            }

            function defineApi() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    var ivSwSynSettings;

                    if (payload != undefined) {
                        ivSwSynSettings = payload.switchSynchronizerSettings;
                    }
                    else {
                        $scope.scopeModel.Uid = UtilsService.guid();
                    }

                    if (ivSwSynSettings) {
                        $scope.scopeModel.MasterConnectionString = ivSwSynSettings.MasterConnectionString;
                        $scope.scopeModel.RouteConnectionString = ivSwSynSettings.RouteConnectionString;
                        $scope.scopeModel.TariffConnectionString = ivSwSynSettings.TariffConnectionString;
                        $scope.scopeModel.OwnerName = ivSwSynSettings.OwnerName;
                        $scope.scopeModel.NumberOfOptions = ivSwSynSettings.NumberOfOptions;
                        $scope.scopeModel.Separator = ivSwSynSettings.Separator;
                        $scope.scopeModel.BlockedAccountMapping = ivSwSynSettings.BlockedAccountMapping;
                        $scope.scopeModel.Uid = ivSwSynSettings.Uid;
                    }

                    var loadCarrierMappingPromise = loadCarrierMappings(payload);
                    promises.push(loadCarrierMappingPromise);

                    function loadCarrierMappings(payload) {
                        $scope.scopeModel.isLoading = true;

                        var carrierAccountfilter = {
                            ActivationStatuses: [WhS_BE_CarrierAccountActivationStatusEnum.Active.value, WhS_BE_CarrierAccountActivationStatusEnum.Testing.value]
                        };

                        var serilizedCarrierAccountFilter = UtilsService.serializetoJson(carrierAccountfilter);

                        return WhS_BE_CarrierAccountAPIService.GetCarrierAccountInfo(serilizedCarrierAccountFilter).then(function (response) {
                            if (response) {
                                var carrierMapping;
                                var i;
                                if (payload && payload.switchSynchronizerSettings && payload.switchSynchronizerSettings.CarrierMappings) {
                                    for (i = 0; i < response.length; i++) {

                                        var accountCarrierMappings = payload.switchSynchronizerSettings.CarrierMappings[response[i].CarrierAccountId];
                                        carrierMapping = {
                                            CarrierAccountId: response[i].CarrierAccountId,
                                            CarrierAccountName: response[i].Name,
                                            CustomerMapping: accountCarrierMappings != undefined && accountCarrierMappings.CustomerMapping != null ? accountCarrierMappings.CustomerMapping.join($scope.scopeModel.Separator) : undefined,
                                            SupplierMapping: accountCarrierMappings != undefined && accountCarrierMappings.SupplierMapping != null ? accountCarrierMappings.SupplierMapping.join($scope.scopeModel.Separator) : undefined
                                        };
                                        $scope.scopeModel.carrierAccountMappings.push(carrierMapping);
                                    }
                                }
                                else {
                                    for (i = 0; i < response.length; i++) {
                                        carrierMapping = {
                                            CarrierAccountId: response[i].CarrierAccountId,
                                            CarrierAccountName: response[i].Name,
                                            CustomerMapping: '',
                                            SupplierMapping: '',
                                            InnerPrefix: ''
                                        };
                                        $scope.scopeModel.carrierAccountMappings.push(carrierMapping);
                                    }
                                }

                                loadMoreCarrierMappings();
                            }
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                            $scope.scopeModel.isLoading = false;
                        }).finally(function () {
                            $scope.scopeModel.isLoading = false;
                        });
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var data = {
                        $type: "TOne.WhS.RouteSync.IVSwitch.IVSwitchSWSync, TOne.WhS.RouteSync.IVSwitch",
                        CarrierMappings: getCarrierMappings(),
                        MasterConnectionString: $scope.scopeModel.MasterConnectionString,
                        RouteConnectionString: $scope.scopeModel.RouteConnectionString,
                        TariffConnectionString: $scope.scopeModel.TariffConnectionString,
                        OwnerName: $scope.scopeModel.OwnerName,
                        NumberOfOptions: $scope.scopeModel.NumberOfOptions,
                        Separator: $scope.scopeModel.Separator,
                        BlockedAccountMapping: $scope.scopeModel.BlockedAccountMapping,
                        Uid: $scope.scopeModel.Uid
                    };

                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function getCarrierMappings() {
                var result = {};
                if ($scope.scopeModel.carrierAccountMappings != undefined)
                    for (var i = 0; i < $scope.scopeModel.carrierAccountMappings.length; i++) {
                        var carrierMapping = $scope.scopeModel.carrierAccountMappings[i];
                        result[carrierMapping.CarrierAccountId] = {
                            CarrierId: carrierMapping.CarrierAccountId,
                            CustomerMapping: carrierMapping.CustomerMapping == undefined ? null : carrierMapping.CustomerMapping.split($scope.scopeModel.Separator),
                            SupplierMapping: carrierMapping.SupplierMapping == undefined ? null : carrierMapping.SupplierMapping.split($scope.scopeModel.Separator)
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

    app.directive('whsRoutesyncIvswitchSwsync', ivswitchSwSync);
})(app);