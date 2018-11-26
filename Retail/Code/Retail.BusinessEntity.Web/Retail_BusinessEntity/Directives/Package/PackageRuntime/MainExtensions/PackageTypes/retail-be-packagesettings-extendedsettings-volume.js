(function (app) {

    'use strict';

    VolumePackageSettingsDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_VolumePackageSettingsService', 'VRUIUtilsService'];

    function VolumePackageSettingsDirective(UtilsService, VRNotificationService, Retail_BE_VolumePackageSettingsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new VolumePackageSettings($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Package/PackageRuntime/MainExtensions/PackageTypes/Templates/VolumePackageSettingsTemplate.html'
        };

        function VolumePackageSettings($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;

            var currencyDirectiveAPI;
            var currencyDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var recurringPeriodUsageSelectorAPI;
            var recurringPeriodUsageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                };

                $scope.scopeModel.AddVolumePackageItem = function () {
                    var onVolumePackageItemAdded = function (addedVolumePackageItem) {
                        $scope.scopeModel.volumePackageItems.push(addedVolumePackageItem);
                    };
                    Retail_BE_VolumePackageSettingsService.addVolumePackageItem(onVolumePackageItemAdded);
                };

                $scope.scopeModel.DeleteVolumePackageItem = function (volumePackageItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.volumePackageItems, volumePackageItem.VolumePackageItemId, 'VolumePackageItemId');
                    $scope.scopeModel.volumePackageItems.splice(index, 1);
                };

                $scope.scopeModel.onCurrencyDirectiveReady = function (api) {
                    currencyDirectiveAPI = api;
                    currencyDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onRecurringPeriodUsageSelectorReady = function (api) {
                    recurringPeriodUsageSelectorAPI = api;
                    recurringPeriodUsageSelectorReadyDeferred.resolve();
                };

                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    //$scope.scopeModel.volumePackageItems.length = 0;

                    console.log(payload);
                    if (payload != undefined && payload.extendedSettings != undefined) {
                        $scope.scopeModel.volumePackageItems = payload.extendedSettings.Items;
                        $scope.scopeModel.price = payload.extendedSettings.Price;
                        $scope.scopeModel.reducePrice = payload.extendedSettings.ReducePriceForIncompletePeriods;
                    }

                    var loadCurrencyDirectivePromise = loadCurrencyDirective();
                    promises.push(loadCurrencyDirectivePromise);

                    function loadCurrencyDirective() {
                        var loadCurrencyDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                        currencyDirectiveReadyDeferred.promise.then(function () {
                            var currencyDirectiverPayload;
                            if (payload != undefined && payload.extendedSettings != undefined)
                                currencyDirectiverPayload = {
                                    selectedIds: payload.extendedSettings.CurrencyId
                                };

                            VRUIUtilsService.callDirectiveLoad(currencyDirectiveAPI, currencyDirectiverPayload, loadCurrencyDirectivePromiseDeferred);
                        });
                        return loadCurrencyDirectivePromiseDeferred.promise;
                    }

                    var loadRecurringPeriodUsageSelectorPromise = loadRecurringPeriodUsageSelector();
                    promises.push(loadRecurringPeriodUsageSelectorPromise);

                    function loadRecurringPeriodUsageSelector() {
                        var loadRecurringPeriodUsageSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        recurringPeriodUsageSelectorReadyDeferred.promise.then(function () {
                            var recurringPeriodUsageSelectorPayload;
                            if (payload != undefined && payload.extendedSettings != undefined) 
                                recurringPeriodUsageSelectorPayload = {
                                    selectedIds: payload.extendedSettings.RecurringPeriod
                                };
                            
                            VRUIUtilsService.callDirectiveLoad(recurringPeriodUsageSelectorAPI, recurringPeriodUsageSelectorPayload, loadRecurringPeriodUsageSelectorPromiseDeferred);
                        });
                        return loadRecurringPeriodUsageSelectorPromiseDeferred.promise;
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = {
                        $type: "Retail.BusinessEntity.MainExtensions.PackageTypes.VolumePackageSettings, Retail.BusinessEntity.MainExtensions",
                        Items: $scope.scopeModel.volumePackageItems,
                        Price: $scope.scopeModel.price,
                        CurrencyId: currencyDirectiveAPI.getSelectedIds(),
                        RecurringPeriod: recurringPeriodUsageSelectorAPI.getData(),
                        ReducePriceForIncompletePeriods: $scope.scopeModel.reducePrice
                    };
                    console.log(obj);
                    return obj;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: 'Edit',
                    clicked: editVolumePackageItem
                }];
            }

            function editVolumePackageItem(volumePackageItem) {
                var onVolumePackageItemUpdated = function (updatedVolumePackageItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.volumePackageItems, volumePackageItem.VolumePackageItemId, 'VolumePackageItemId');
                    $scope.scopeModel.volumePackageItems[index] = updatedVolumePackageItem;
                };

                Retail_BE_VolumePackageSettingsService.editVolumePackageItem(volumePackageItem, onVolumePackageItemUpdated);
            }
        }
    }

    app.directive('retailBePackagesettingsExtendedsettingsVolume', VolumePackageSettingsDirective);

})(app);