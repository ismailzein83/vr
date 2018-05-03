(function (app) {

    'use strict';

    ivswitchSwSync.$inject = ["UtilsService", 'VRUIUtilsService', 'WhS_BE_CarrierAccountAPIService', 'VRNotificationService'];

    function ivswitchSwSync(utilsService, vruiUtilsService, whSBeCarrierAccountApiService, vrNotificationService) {
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
            console.log($scope.scopeModel.Separator);
            var gridAPI;
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
            function loadCarrierMappings(payload) {
                $scope.scopeModel.isLoading = true;
                var serializedFilter = {};
                return whSBeCarrierAccountApiService.GetCarrierAccountInfo(serializedFilter)
                 .then(function (response) {
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
                     }
                 })
                  .catch(function (error) {
                      vrNotificationService.notifyException(error, $scope);
                      $scope.scopeModel.isLoading = false;
                  }).finally(function () {
                      $scope.scopeModel.isLoading = false;
                  });
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
                        $scope.scopeModel.Uid = utilsService.guid();
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
                    return utilsService.waitMultiplePromises(promises);
                };

                function getData() {
                    var data = {
                        $type: "TOne.WhS.RouteSync.IVSwitch.IVSwitchSWSync,TOne.WhS.RouteSync.IVSwitch",
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
                }

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.isLoading = false;
                $scope.scopeModel.carrierAccountMappings = [];
                defineApi();
            }

            this.initializeController = initializeController;
        }
    }

    app.directive('whsRoutesyncIvswitchSwsync', ivswitchSwSync);

})(app);