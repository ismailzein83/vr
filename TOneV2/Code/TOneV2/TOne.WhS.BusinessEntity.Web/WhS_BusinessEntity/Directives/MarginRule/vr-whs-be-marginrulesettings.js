'use strict';

app.directive('vrWhsBeMarginrulesettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                var ctor = new MarginRuleSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/MarginRule/Templates/MarginRuleSettings.html"
        };

        function MarginRuleSettings($scope, ctrl, attrs) {

            this.initializeController = initializeController;

            var marginCategoryBEDefinitionId;

            var currencyDirectiveAPI;
            var currencyDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var gridAPI;
            var gridPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.marginSettingItems = []

                $scope.scopeModel.onCurrencySelectReady = function (api) {
                    currencyDirectiveAPI = api;
                    currencyDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    gridPromiseDeferred.resolve();
                };

                $scope.scopeModel.addMarginSettingItem = function () {
                    extendAndAddMarginSettingItemToGrid();
                };

                $scope.scopeModel.isGridItemsValid = function () {
                    if ($scope.scopeModel.marginSettingItems.length == 0)
                        return "You should add At Least one Item";

                    var length = $scope.scopeModel.marginSettingItems.length;
                    for (var i = 0; i < length; i++) {
                        var currentItem = $scope.scopeModel.marginSettingItems[i];
                        var nextItem = (i + 1) < length ? $scope.scopeModel.marginSettingItems[i + 1] : undefined;

                        if (nextItem != undefined) {
                            if (currentItem.UpTo == undefined || currentItem.UpTo == null) {
                                return "Up To should have value at Row  " + (i + 1);
                            }

                            if (currentItem.UpTo > nextItem.UpTo) {
                                return "Up To value '" + nextItem.UpTo + "' (Row  " + (i + 2) + ") should be greater than   '" + currentItem.UpTo + "' (Row  " + (i + 1) + ")";
                            }
                        }
                        else { //currentItem == lastItem
                            if (currentItem.UpTo != undefined && currentItem.UpTo != null) {
                                return "Last Item shouldn't have Up To value";
                            }
                        }
                    }

                    return null;
                };

                $scope.scopeModel.onDeleteRow = function (deletedItem) {
                    var index = $scope.scopeModel.marginSettingItems.indexOf(deletedItem);
                    $scope.scopeModel.marginSettingItems.splice(index, 1);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];
                    var settings;
                    var genericRuleDefinition;
                    var marginSettingItems;

                    if (payload != undefined) {
                        settings = payload.settings;
                        genericRuleDefinition = payload.genericRuleDefinition;

                        if (genericRuleDefinition != undefined && genericRuleDefinition.SettingsDefinition != undefined) {
                            marginCategoryBEDefinitionId = genericRuleDefinition.SettingsDefinition.MarginCategoryBEDefinitionId;
                        }

                        if (settings != undefined) {
                            marginSettingItems = settings.MarginSettingItems;
                        }
                    }

                    var loadCurrencySelectorPromise = loadCurrencySelector();
                    initialPromises.push(loadCurrencySelectorPromise);

                    if (marginSettingItems != undefined && marginSettingItems.length > 0) {
                        var loadMarginItemsGridPromise = getMarginItemsGridLoadPromise();
                        initialPromises.push(loadMarginItemsGridPromise);
                    }

                    function loadCurrencySelector() {
                        var loadCurrencySelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        currencyDirectiveReadyPromiseDeferred.promise.then(function () {
                            var currencyPayload;
                            if (settings != undefined && settings.CurrencyId > 0) {
                                currencyPayload = { selectedIds: settings.CurrencyId };
                            }
                            else {
                                currencyPayload = { selectSystemCurrency: true };
                            }

                            VRUIUtilsService.callDirectiveLoad(currencyDirectiveAPI, currencyPayload, loadCurrencySelectorPromiseDeferred);

                        });
                        return loadCurrencySelectorPromiseDeferred.promise;
                    }

                    function getMarginItemsGridLoadPromise() {
                        var loadMarginItemsGridPromiseDeferred = UtilsService.createPromiseDeferred();

                        gridPromiseDeferred.promise.then(function () {
                            var _promises = [];
                            for (var i = 0; i < marginSettingItems.length; i++) {
                                var currentMarginSettingItem = marginSettingItems[i];
                                _promises.push(extendAndAddMarginSettingItemToGrid(currentMarginSettingItem));
                            }

                            UtilsService.waitMultiplePromises(_promises).then(function () {
                                loadMarginItemsGridPromiseDeferred.resolve();
                            });
                        });

                        return loadMarginItemsGridPromiseDeferred.promise;
                    }

                    return UtilsService.waitPromiseNode({ promises: initialPromises });
                };

                api.getData = function () {
                    var marginItems = [];

                    for (var i = 0; i < $scope.scopeModel.marginSettingItems.length; i++)
                        marginItems.push(buildMarginSettingItem($scope.scopeModel.marginSettingItems[i]));

                    return {
                        $type: 'TOne.WhS.BusinessEntity.Entities.MarginRuleSettings, TOne.WhS.BusinessEntity.Entities',
                        MarginSettingItems: marginItems.length > 0 ? marginItems : undefined,
                        CurrencyId: currencyDirectiveAPI.getSelectedIds()
                    };
                };

                function buildMarginSettingItem(marginSettingItem) {
                    return {
                        UpTo: marginSettingItem.UpTo,
                        Category: marginSettingItem.statusDefinitionSelectorAPI != undefined ? marginSettingItem.statusDefinitionSelectorAPI.getSelectedIds() : undefined
                    };
                }

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function extendAndAddMarginSettingItemToGrid(marginSettingItem) {

                var extendedPromises = [];

                var marginSettingDataItem = {
                    statusDefinitionSelectorLoadDeferred: UtilsService.createPromiseDeferred(),
                    isStatusDefinitionSelectorLoading: true
                };

                extendedPromises.push(marginSettingDataItem.statusDefinitionSelectorLoadDeferred.promise);

                if (marginSettingItem != undefined) {
                    marginSettingDataItem.UpTo = marginSettingItem.UpTo != null ? marginSettingItem.UpTo : undefined;
                    marginSettingDataItem.isStatusDefinitionSelectorLoading = false;
                }

                marginSettingDataItem.onStatusDefinitionSelectorReady = function (api) {
                    marginSettingDataItem.statusDefinitionSelectorAPI = api;

                    var statusDefinitionPayload = {
                        filter: { BusinessEntityDefinitionId: marginCategoryBEDefinitionId }
                    };

                    if (marginSettingItem != undefined) {
                        statusDefinitionPayload.selectedIds = marginSettingItem.Category;
                    }

                    VRUIUtilsService.callDirectiveLoad(marginSettingDataItem.statusDefinitionSelectorAPI, statusDefinitionPayload, marginSettingDataItem.statusDefinitionSelectorLoadDeferred);
                };

                $scope.scopeModel.marginSettingItems.push(marginSettingDataItem);

                return UtilsService.waitMultiplePromises(extendedPromises).then(function () {
                    marginSettingDataItem.isStatusDefinitionSelectorLoading = false;
                });
            }
        }

    }]);