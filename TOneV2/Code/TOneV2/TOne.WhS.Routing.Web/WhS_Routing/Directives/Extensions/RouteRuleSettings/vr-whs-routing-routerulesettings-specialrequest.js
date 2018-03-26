'use strict';

app.directive('vrWhsRoutingRouterulesettingsSpecialrequest', ['UtilsService', 'VRUIUtilsService', 'WhS_Routing_RoutRuleSettingsAPIService',
    function (UtilsService, VRUIUtilsService, WhS_Routing_RoutRuleSettingsAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SpecialRequestCtor(ctrl, $scope);
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
            templateUrl: function (element, attrs) {
                return '/Client/Modules/WhS_Routing/Directives/Extensions/RouteRuleSettings/Templates/RouteRuleSpecialRequestDirective.html';
            }
        };

        function SpecialRequestCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var supplierFilterSettings;
            var supplierZoneDetails;

            var gridAPI;
            var gridPromiseDeferred = UtilsService.createPromiseDeferred();

            var carrierAccountSelectorAPI;
            var carrierAccountSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var overallBackupOptionsDirectiveAPI;
            var overallBackupOptionsDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var excludedSuppliersSelectorAPI;
            var excludedSuppliersSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var saleServiceViewerAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.suppliers = [];
                $scope.scopeModel.showBackupTabs = false;

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    gridPromiseDeferred.resolve();
                };

                $scope.scopeModel.onCarrierAccountDirectiveReady = function (api) {
                    carrierAccountSelectorAPI = api;
                    carrierAccountSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onOverallBackupOptionsDirectiveReady = function (api) {
                    overallBackupOptionsDirectiveAPI = api;
                    overallBackupOptionsDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onExcludedSuppliersSelectorReady = function (api) {
                    excludedSuppliersSelectorAPI = api;
                    excludedSuppliersSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onSelectSupplier = function (selectedItem) {
                    $scope.scopeModel.isLoadingSelectedSupplier = true;
                    extendAndAddOptionToGrid(selectedItem, undefined, undefined).then(function () {
                        setTimeout(function () {
                            $scope.$apply(function () {
                                $scope.scopeModel.isLoadingSelectedSupplier = false;
                            });
                        });
                    });
                    $scope.scopeModel.selectedSuppliers = [];
                };

                //$scope.scopeModel.onDeselectSupplier = function (deselectedItem) {
                //    var index = UtilsService.getItemIndexByVal($scope.scopeModel.suppliers, deselectedItem.CarrierAccountId, 'SupplierId');
                //    $scope.scopeModel.suppliers.splice(index, 1);
                //};

                $scope.scopeModel.onDeleteRow = function (deletedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.suppliers, deletedItem.tempId, 'tempId');
                    $scope.scopeModel.suppliers.splice(index, 1);

                    if ($scope.scopeModel.suppliers.length == 0)
                        $scope.scopeModel.showBackupTabs = false;
                };

                $scope.scopeModel.isValid = function () {
                    var suppliers = $scope.scopeModel.suppliers;
                    if (suppliers == undefined || suppliers.length == 0)
                        return "At least one supplier should be selected";

                    var total = undefined;
                    for (var x = 0; x < suppliers.length; x++) {
                        var currentSupplier = suppliers[x];
                        if (currentSupplier.Percentage != undefined && currentSupplier.Percentage != "") {
                            if (total == undefined) {
                                total = 0;
                            }
                            total += parseFloat(suppliers[x].Percentage);
                        }
                    }

                    if (total != undefined && total != 100)
                        return "Sum of all Percentages must be equal to 100";

                    return null;
                };

                $scope.scopeModel.onPercentageValueChanged = function () {
                    var suppliers = $scope.scopeModel.suppliers;
                    if (suppliers == undefined)
                        return;

                    var total = undefined;
                    for (var x = 0; x < suppliers.length; x++) {
                        var currentSupplier = suppliers[x];
                        if (currentSupplier.Percentage != undefined && currentSupplier.Percentage != "") {
                            if (total == undefined)
                                total = 0;
                            total += parseFloat(currentSupplier.Percentage);
                        }
                    }

                    if (total != undefined) {
                        if (!$scope.scopeModel.showBackupTabs) {
                            $scope.scopeModel.showBackupTabs = true;
                            expandRows();
                        }
                    }
                    else {
                        if ($scope.scopeModel.showBackupTabs) {
                            $scope.scopeModel.showBackupTabs = false;
                            collapseRows();
                        }
                    }
                };

                $scope.scopeModel.showExpand = function (dataItem) {
                    return $scope.scopeModel.showBackupTabs;
                };

                $scope.scopeModel.excludeOption = function (dataItem) {
                    removeExcludedCarrierAccountFromGrid(dataItem.SupplierId);

                    if ($scope.scopeModel.suppliers.length == 0)
                        $scope.scopeModel.showBackupTabs = false;

                    if (excludedSuppliersSelectorAPI != undefined)
                        excludedSuppliersSelectorAPI.selectItem(dataItem.SupplierId);

                    excludeCarrierAccount(carrierAccountSelectorAPI, dataItem.SupplierId);
                    excludeCarrierAccount(overallBackupOptionsDirectiveAPI, dataItem.SupplierId);
                    excludeCarrierAccountFromDrillDownSelectors(dataItem.SupplierId);
                };

                $scope.scopeModel.onSelectExcludedSupplier = function (dataItem) {
                    removeExcludedCarrierAccountFromGrid(dataItem.CarrierAccountId);

                    excludeCarrierAccount(carrierAccountSelectorAPI, dataItem.CarrierAccountId);
                    excludeCarrierAccount(overallBackupOptionsDirectiveAPI, dataItem.CarrierAccountId);
                    excludeCarrierAccountFromDrillDownSelectors(dataItem.CarrierAccountId);
                };

                $scope.scopeModel.onDeselectExcludedSupplier = function (dataItem) {
                    cancelExcludeCarrierAccount(carrierAccountSelectorAPI, dataItem.CarrierAccountId);
                    cancelExcludeCarrierAccount(overallBackupOptionsDirectiveAPI, dataItem.CarrierAccountId);
                    cancelExcludeCarrierAccountFromDrillDownSelectors(dataItem.CarrierAccountId);
                };

                $scope.scopeModel.onDeselectAllExcludedSuppliers = function (deselectedItems) {
                    cancelExcludeCarrierAccounts(carrierAccountSelectorAPI, deselectedItems);
                    cancelExcludeCarrierAccounts(overallBackupOptionsDirectiveAPI, deselectedItems);
                    cancelExcludeCarrierAccountsFromDrillDownSelectors(deselectedItems);
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.showRateServices = false;

                    var promises = [];

                    var options;
                    var overallBackupOptions;
                    var excludedOptionIds;
                    var supplierFilterSettings;
                    var customerRouteData;

                    if (payload != undefined) {
                        supplierFilterSettings = payload.SupplierFilterSettings;
                        customerRouteData = payload.customerRouteData;
                        supplierZoneDetails = payload.supplierZoneDetails;

                        if (payload.RouteRuleSettings != undefined) {
                            options = payload.RouteRuleSettings.Options;
                            overallBackupOptions = payload.RouteRuleSettings.OverallBackupOptions;
                            excludedOptionIds = getExcludedOptionIds(payload.RouteRuleSettings.ExcludedOptions);
                        }
                    }

                    if (supplierZoneDetails != undefined) {
                        $scope.scopeModel.showRateServices = true;
                    }

                    if (customerRouteData != undefined) {
                        $scope.scopeModel.Rate = customerRouteData.Rate;

                        var saleServiceViewerLoadDeferred = UtilsService.createPromiseDeferred();
                        $scope.scopeModel.onSaleServiceViewerReady = function (api) {
                            saleServiceViewerAPI = api;
                            var saleServiceViewerPayload = { selectedIds: customerRouteData.SaleZoneServiceIds };
                            VRUIUtilsService.callDirectiveLoad(saleServiceViewerAPI, saleServiceViewerPayload, saleServiceViewerLoadDeferred);
                        };
                        promises.push(saleServiceViewerLoadDeferred.promise);
                    }

                    //Loading OptionsSection Directives
                    var optionsSectionLoadPromise = getOptionsSectionLoadPromise();
                    promises.push(optionsSectionLoadPromise);

                    //Loading OverallBackupOptions Directive
                    var overallBackupOptionsDirectiveLoadPromise = getOverallBackupOptionsDirectiveLoadPromise();
                    promises.push(overallBackupOptionsDirectiveLoadPromise);

                    //Loading ExcludedSuppliers Selector
                    var excludedSuppliersSelectorLoadPromise = getExcludedSuppliersSelectorLoadPromise();
                    promises.push(excludedSuppliersSelectorLoadPromise);

                    function getExcludedOptionIds(excludedOptions) {
                        if (excludedOptions == undefined)
                            return;

                        var excludedOptionIds = [];
                        for (var key in excludedOptions) {
                            if (key != '$type')
                                excludedOptionIds.push(key);
                        }
                        return excludedOptionIds;
                    }
                    function getOptionsSectionLoadPromise() {
                        //Loading CarrierAccount Selector
                        var carrierAccountSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        carrierAccountSelectorReadyPromiseDeferred.promise.then(function () {

                            var carrierAccountPayload = {
                                filter: { SupplierFilterSettings: supplierFilterSettings },
                                selectedIds: []
                            };
                            if (options != undefined) {
                                carrierAccountPayload.selectedIds = UtilsService.getPropValuesFromArray(options, "SupplierId");
                            }
                            VRUIUtilsService.callDirectiveLoad(carrierAccountSelectorAPI, carrierAccountPayload, carrierAccountSelectorLoadPromiseDeferred);
                        });

                        //Loading Options Grid 
                        var loadOptionGridPromiseDeferred = UtilsService.createPromiseDeferred();
                        UtilsService.waitMultiplePromises([gridPromiseDeferred.promise, carrierAccountSelectorLoadPromiseDeferred.promise]).then(function () {
                            var _promises = [];
                            for (var i = 0; i < $scope.scopeModel.selectedSuppliers.length; i++) {
                                var selectedSupplier = $scope.scopeModel.selectedSuppliers[i];
                                var currentOption = options[i]; //options[selectedSupplier.CarrierAccountId];

                                var backupOptionDirectiveLoadPromiseDeferred = undefined;
                                if (currentOption.Percentage != undefined) {
                                    $scope.scopeModel.showBackupTabs = true;
                                    backupOptionDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                                    _promises.push(backupOptionDirectiveLoadPromiseDeferred.promise);
                                }
                                _promises.push(extendAndAddOptionToGrid(selectedSupplier, currentOption, backupOptionDirectiveLoadPromiseDeferred));
                            }

                            UtilsService.waitMultiplePromises(_promises).then(function () {
                                loadOptionGridPromiseDeferred.resolve();
                            });
                        });
                        return loadOptionGridPromiseDeferred.promise;
                    }
                    function getOverallBackupOptionsDirectiveLoadPromise() {
                        var overallBackupOptionsDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        overallBackupOptionsDirectiveReadyPromiseDeferred.promise.then(function () {

                            var overallBackupOptionsPayload = { filter: { SupplierFilterSettings: supplierFilterSettings } };
                            if (overallBackupOptions != undefined) {
                                overallBackupOptionsPayload.backups = overallBackupOptions;
                            }
                            VRUIUtilsService.callDirectiveLoad(overallBackupOptionsDirectiveAPI, overallBackupOptionsPayload, overallBackupOptionsDirectiveLoadPromiseDeferred);
                        });

                        return overallBackupOptionsDirectiveLoadPromiseDeferred.promise;
                    }
                    function getExcludedSuppliersSelectorLoadPromise() {
                        var loadExcludedSuppliersSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        excludedSuppliersSelectorReadyPromiseDeferred.promise.then(function () {

                            var excludedSuppliersSelectorPayload = {
                                filter: { SupplierFilterSettings: supplierFilterSettings },
                                selectedIds: []
                            };
                            if (excludedOptionIds != undefined) {
                                excludedSuppliersSelectorPayload.selectedIds = excludedOptionIds;
                            }
                            VRUIUtilsService.callDirectiveLoad(excludedSuppliersSelectorAPI, excludedSuppliersSelectorPayload, loadExcludedSuppliersSelectorPromiseDeferred);
                        });

                        return loadExcludedSuppliersSelectorPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        $scope.scopeModel.selectedSuppliers = [];
                        excludeCarrierAccounts(carrierAccountSelectorAPI);
                        excludeCarrierAccounts(overallBackupOptionsDirectiveAPI);
                        excludeCarrierAccountsFromDrillDownSelectors();
                    });
                };

                api.getData = function () {

                    function getOptions() {
                        var options = [];
                        for (var i = 0; i < $scope.scopeModel.suppliers.length; i++) {
                            var supplier = $scope.scopeModel.suppliers[i];
                            var option = {
                                SupplierId: supplier.SupplierId,
                                ForceOption: supplier.ForceOption,
                                NumberOfTries: supplier.NumberOfTries,
                                Percentage: supplier.Percentage
                            };
                            if ($scope.scopeModel.showBackupTabs && supplier.backupOptionGridAPI) {
                                option.Backups = supplier.backupOptionGridAPI.getData();
                            }
                            options.push(option);
                        }
                        return options;
                    }

                    function getOverallBackupOptions() {
                        if (!$scope.scopeModel.showBackupTabs || overallBackupOptionsDirectiveAPI == undefined)
                            return null;

                        return overallBackupOptionsDirectiveAPI.getData();
                    }

                    function getExcludedOptions() {
                        var excludedOptions = {};
                        for (var i = 0; i < $scope.scopeModel.selectedExcludedSuppliers.length; i++) {
                            var excludedSupplier = $scope.scopeModel.selectedExcludedSuppliers[i];
                            var excludedOption = {
                                SupplierId: excludedSupplier.CarrierAccountId
                            };
                            excludedOptions[excludedSupplier.CarrierAccountId] = excludedOption;
                        }
                        return excludedOptions;
                    }

                    return {
                        $type: "TOne.WhS.Routing.Business.SpecialRequestRouteRule, TOne.WhS.Routing.Business",
                        Options: getOptions(),
                        OverallBackupOptions: getOverallBackupOptions(),
                        ExcludedOptions: getExcludedOptions()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function extendAndAddOptionToGrid(selectedSupplier, currentOption, backupOptionDirectiveLoadPromiseDeferred) {
                var extendOptionPromises = [];

                var option = {
                    tempId: UtilsService.guid(),
                    SupplierId: selectedSupplier.CarrierAccountId,
                    Name: selectedSupplier.Name,
                    ForceOption: currentOption != undefined ? currentOption.ForceOption : undefined,
                    NumberOfTries: currentOption != undefined ? currentOption.NumberOfTries : 1,
                    Percentage: currentOption != undefined ? currentOption.Percentage : undefined,
                    Backups: currentOption != undefined ? currentOption.Backups : undefined
                };

                if ($scope.scopeModel.showRateServices) {
                    var services;
                    if (supplierZoneDetails != undefined) {
                        for (var index = 0; index < supplierZoneDetails.length; index++) {
                            var currentItem = supplierZoneDetails[index];
                            if (currentItem.SupplierId == option.SupplierId) {
                                option.Rate = currentItem.EffectiveRateValue;
                                services = currentItem.ExactSupplierServiceIds;
                                break;
                            }
                        }
                    }

                    option.serviceViewerLoadDeferred = UtilsService.createPromiseDeferred();
                    option.onServiceViewerReady = function (api) {
                        option.serviceViewerAPI = api;
                        var serviceViewerPayload = services != undefined ? { selectedIds: services } : undefined;
                        VRUIUtilsService.callDirectiveLoad(option.serviceViewerAPI, serviceViewerPayload, option.serviceViewerLoadDeferred);
                    };
                    extendOptionPromises.push(option.serviceViewerLoadDeferred.promise);
                }

                option.onBackupOptionDirectiveReady = function (api) {
                    option.backupOptionGridAPI = api;

                    var backupOptionGridAPIPayload = { supplierFilterSettings: supplierFilterSettings, supplierZoneDetails: supplierZoneDetails };
                    if (option != undefined && option.Backups != undefined) {
                        backupOptionGridAPIPayload.backups = option.Backups;
                    }
                    VRUIUtilsService.callDirectiveLoad(option.backupOptionGridAPI, backupOptionGridAPIPayload, backupOptionDirectiveLoadPromiseDeferred);
                };

                expandRow(option);
                $scope.scopeModel.suppliers.push(option);

                return UtilsService.waitMultiplePromises(extendOptionPromises);
            }
            function expandRow(option) {
                if ($scope.scopeModel.showBackupTabs) {
                    gridAPI.expandRow(option);
                }
            }
            function expandRows() {
                for (var i = 0; i < $scope.scopeModel.suppliers.length; i++) {
                    gridAPI.expandRow($scope.scopeModel.suppliers[i]);
                }
            }
            function collapseRows() {
                for (var i = 0; i < $scope.scopeModel.suppliers.length; i++) {
                    gridAPI.collapseRow($scope.scopeModel.suppliers[i]);
                }
            }

            function excludeCarrierAccount(directiveAPI, carrierAccountId) {
                if (directiveAPI == undefined)
                    return;

                if (directiveAPI.excludeItem != undefined) {
                    directiveAPI.excludeItem(carrierAccountId);
                    return;
                }

                if (directiveAPI.excludeBackupOption != undefined) {
                    directiveAPI.excludeBackupOption(carrierAccountId);
                    return;
                }
            }
            function excludeCarrierAccounts(directiveAPI) {
                if ($scope.scopeModel.selectedExcludedSuppliers == undefined)
                    return;

                if ($scope.scopeModel.selectedExcludedSuppliers.length == 0)
                    return;

                for (var index = 0; index < $scope.scopeModel.selectedExcludedSuppliers.length; index++) {
                    var currentExcludedSupplier = $scope.scopeModel.selectedExcludedSuppliers[index];
                    excludeCarrierAccount(directiveAPI, currentExcludedSupplier.CarrierAccountId);
                }
            }
            function cancelExcludeCarrierAccount(directiveAPI, carrierAccountId) {
                if (directiveAPI == undefined)
                    return;

                if (directiveAPI.cancelExcludeItem != undefined) {
                    directiveAPI.cancelExcludeItem(carrierAccountId);
                    return;
                }

                if (directiveAPI.cancelExcludeBackupOption != undefined) {
                    directiveAPI.cancelExcludeBackupOption(carrierAccountId);
                    return;
                }
            }
            function cancelExcludeCarrierAccounts(directiveAPI, deselectedCarrierAccounts) {
                if (deselectedCarrierAccounts == undefined || deselectedCarrierAccounts.length == 0)
                    return;

                for (var index = 0; index < deselectedCarrierAccounts.length; index++) {
                    var deselectedCarrierAccount = deselectedCarrierAccounts[index];
                    cancelExcludeCarrierAccount(directiveAPI, deselectedCarrierAccount.CarrierAccountId);
                }
            }

            function excludeCarrierAccountFromDrillDownSelectors(carrierAccountId) {
                for (var index = 0; index < $scope.scopeModel.suppliers.length; index++) {
                    var currentSupplier = $scope.scopeModel.suppliers[index];
                    if (currentSupplier && currentSupplier.backupOptionGridAPI)
                        excludeCarrierAccount(currentSupplier.backupOptionGridAPI, carrierAccountId);
                }
            }
            function excludeCarrierAccountsFromDrillDownSelectors() {
                for (var index = 0; index < $scope.scopeModel.selectedExcludedSuppliers.length; index++) {
                    var currentExcludedSupplier = $scope.scopeModel.selectedExcludedSuppliers[index];
                    excludeCarrierAccountFromDrillDownSelectors(currentExcludedSupplier.CarrierAccountId);
                }
            }
            function cancelExcludeCarrierAccountFromDrillDownSelectors(carrierAccountId) {
                for (var index = 0; index < $scope.scopeModel.suppliers.length; index++) {
                    var currentSupplier = $scope.scopeModel.suppliers[index];
                    if (currentSupplier && currentSupplier.backupOptionGridAPI)
                        cancelExcludeCarrierAccount(currentSupplier.backupOptionGridAPI, carrierAccountId);
                }
            }
            function cancelExcludeCarrierAccountsFromDrillDownSelectors(deselectedCarrierAccounts) {
                if (deselectedCarrierAccounts == undefined || deselectedCarrierAccounts.length == 0)
                    return;

                for (var index = 0; index < deselectedCarrierAccounts.length; index++) {
                    var currentDeselectedCarrierAccount = deselectedCarrierAccounts[index];
                    cancelExcludeCarrierAccountFromDrillDownSelectors(currentDeselectedCarrierAccount.CarrierAccountId);
                }
            }

            function removeExcludedCarrierAccountFromGrid(carrierAccountId) {
                for (var index = $scope.scopeModel.suppliers.length - 1; index >= 0; index--) {
                    var currentItem = $scope.scopeModel.suppliers[index];
                    if (currentItem.SupplierId == carrierAccountId) {
                        $scope.scopeModel.suppliers.splice(index, 1);
                    }
                }
            }
        }

        return directiveDefinitionObject;
    }]);