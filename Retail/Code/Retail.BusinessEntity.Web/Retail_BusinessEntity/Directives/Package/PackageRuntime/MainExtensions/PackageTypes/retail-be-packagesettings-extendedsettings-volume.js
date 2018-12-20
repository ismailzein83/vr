(function (app) {

    'use strict';

    VolumePackageSettingsDirective.$inject = ['UtilsService', 'Retail_BE_VolumePackageService', 'VRUIUtilsService'];

    function VolumePackageSettingsDirective(UtilsService, Retail_BE_VolumePackageService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VolumePackageSettings($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Package/PackageRuntime/MainExtensions/PackageTypes/Templates/VolumePackageSettingsTemplate.html'
        };

        function VolumePackageSettings($scope, ctrl) {
            this.initializeController = initializeController;

            var volumePackageDefinitionId;
            var volumePackageDefinitionItems;

            var currencyDirectiveAPI;
            var currencyDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var recurringPeriodUsageDirectiveAPI;
            var recurringPeriodUsageDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.volumePackageItems = [];

                $scope.scopeModel.onCurrencyDirectiveReady = function (api) {
                    currencyDirectiveAPI = api;
                    currencyDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onRecurringPeriodUsageDirectiveReady = function (api) {
                    recurringPeriodUsageDirectiveAPI = api;
                    recurringPeriodUsageDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                };

                $scope.scopeModel.isGridValid = function () {
                    if ($scope.scopeModel.volumePackageItems.length < 1)
                        return 'Grid should contains at least one item';
                    return null;
                };

                $scope.scopeModel.addVolumePackageItem = function () {
                    var onVolumePackageItemAdded = function (addedVolumePackageItem) {
                        extendVolumePackageDefinitionItem(addedVolumePackageItem);
                        $scope.scopeModel.volumePackageItems.push({ Entity: addedVolumePackageItem });
                    };

                    Retail_BE_VolumePackageService.addVolumePackageItem(onVolumePackageItemAdded, volumePackageDefinitionItems, volumePackageDefinitionId);
                };

                $scope.scopeModel.removeVolumePackageItem = function (volumePackageItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.volumePackageItems, volumePackageItem.Entity.VolumePackageItemId, 'Entity.VolumePackageItemId');
                    $scope.scopeModel.volumePackageItems.splice(index, 1);
                };

                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.price = undefined;
                    $scope.scopeModel.reducePrice = false;
                    $scope.scopeModel.volumePackageItems.length = 0;

                    if (payload != undefined) {
                        volumePackageDefinitionId = payload.packageDefinitionId;

                        var extendedSettingsDefinition = payload.extendedSettingsDefinition;
                        if (extendedSettingsDefinition != undefined) {
                            volumePackageDefinitionItems = extendedSettingsDefinition.Items;
                        }

                        var extendedSettings = payload.extendedSettings;
                        if (extendedSettings != undefined) {
                            $scope.scopeModel.price = extendedSettings.Price;
                            $scope.scopeModel.reducePrice = extendedSettings.ReducePriceForIncompletePeriods;

                            if (extendedSettings.Items != undefined) {
                                for (var i = 0; i < extendedSettings.Items.length; i++) {
                                    var currentItem = extendedSettings.Items[i];
                                    extendVolumePackageDefinitionItem(currentItem);
                                    $scope.scopeModel.volumePackageItems.push({ Entity: currentItem });
                                }
                            }
                        }
                    }

                    var promises = [];

                    var loadCurrencyDirectivePromise = loadCurrencyDirective();
                    promises.push(loadCurrencyDirectivePromise);

                    var loadRecurringPeriodUsageDirectivePromise = loadRecurringPeriodUsageDirective();
                    promises.push(loadRecurringPeriodUsageDirectivePromise);

                    function loadCurrencyDirective() {
                        var loadCurrencyDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                        currencyDirectiveReadyDeferred.promise.then(function () {

                            var currencyDirectiverPayload;
                            if (extendedSettings != undefined) {
                                currencyDirectiverPayload = { selectedIds: extendedSettings.CurrencyId };
                            }
                            VRUIUtilsService.callDirectiveLoad(currencyDirectiveAPI, currencyDirectiverPayload, loadCurrencyDirectivePromiseDeferred);
                        });

                        return loadCurrencyDirectivePromiseDeferred.promise;
                    }
                    function loadRecurringPeriodUsageDirective() {
                        var loadRecurringPeriodUsageDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                        recurringPeriodUsageDirectiveReadyDeferred.promise.then(function () {

                            var recurringPeriodUsageDirectivePayload;
                            if (extendedSettings != undefined) {
                                recurringPeriodUsageDirectivePayload = { packageUsageVolumeRecurringPeriod: extendedSettings.RecurringPeriod };
                            }
                            VRUIUtilsService.callDirectiveLoad(recurringPeriodUsageDirectiveAPI, recurringPeriodUsageDirectivePayload, loadRecurringPeriodUsageDirectivePromiseDeferred);
                        });

                        return loadRecurringPeriodUsageDirectivePromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    function getVolumePackageItems() {
                        var items = [];
                        for (var i = 0; i < $scope.scopeModel.volumePackageItems.length; i++) {
                            var currentItem = $scope.scopeModel.volumePackageItems[i].Entity;
                            items.push({
                                VolumePackageItemId: currentItem.VolumePackageItemId,
                                VolumePackageDefinitionItemId: currentItem.VolumePackageDefinitionItemId,
                                Volume: currentItem.Volume,
                                Condition: currentItem.Condition
                            });
                        }
                        return items;
                    }

                    return {
                        $type: "Retail.BusinessEntity.MainExtensions.PackageTypes.VolumePackageSettings, Retail.BusinessEntity.MainExtensions",
                        Price: $scope.scopeModel.price,
                        CurrencyId: currencyDirectiveAPI.getSelectedIds(),
                        RecurringPeriod: recurringPeriodUsageDirectiveAPI.getData(),
                        ReducePriceForIncompletePeriods: $scope.scopeModel.reducePrice,
                        Items: getVolumePackageItems()
                    };
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
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.volumePackageItems, volumePackageItem.Entity.VolumePackageItemId, 'Entity.VolumePackageItemId');
                    extendVolumePackageDefinitionItem(updatedVolumePackageItem);
                    $scope.scopeModel.volumePackageItems[index] = { Entity: updatedVolumePackageItem };
                };

                Retail_BE_VolumePackageService.editVolumePackageItem(onVolumePackageItemUpdated, volumePackageItem.Entity, volumePackageDefinitionItems, volumePackageDefinitionId);
            }

            function extendVolumePackageDefinitionItem(volumePackageDefinitionItem) {
                var item = UtilsService.getItemByVal(volumePackageDefinitionItems, volumePackageDefinitionItem.VolumePackageDefinitionItemId, 'VolumePackageDefinitionItemId');
                volumePackageDefinitionItem.VolumePackageDefinitionItemName = item.Name;
            }
        }
    }

    app.directive('retailBePackagesettingsExtendedsettingsVolume', VolumePackageSettingsDirective);
})(app);