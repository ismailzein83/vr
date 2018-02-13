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

            var gridAPI;
            var gridPromiseDeferred = UtilsService.createPromiseDeferred();

            var carrierAccountSelectorAPI;
            var carrierAccountSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var overallBackupOptionsDirectiveAPI;
            var overallBackupOptionsDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var supplierZoneDetails;
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

                $scope.scopeModel.onSelectSupplier = function (selectedItem) {
                    $scope.scopeModel.isLoadingSelectedSupplier = true;
                    extendAndAddOptionToGrid(selectedItem, undefined, undefined).then(function () {
                        setTimeout(function () {
                            $scope.scopeModel.isLoadingSelectedSupplier = false;
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

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    $scope.scopeModel.showRateServices = false;
                    var options;
                    var overallBackupOptions;
                    var customerRouteData;

                    if (payload != undefined) {
                        customerRouteData = payload.customerRouteData;
                        supplierZoneDetails = payload.supplierZoneDetails;
                        if (supplierZoneDetails != undefined)
                            $scope.scopeModel.showRateServices = true;

                        supplierFilterSettings = payload.SupplierFilterSettings;

                        if (payload.RouteRuleSettings != undefined) {
                            options = payload.RouteRuleSettings.Options;
                            overallBackupOptions = payload.RouteRuleSettings.OverallBackupOptions;
                        }
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


                    function getOptionsSectionLoadPromise() {
                        //Loading CarrierAccount Selector
                        var carrierAccountSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        carrierAccountSelectorReadyPromiseDeferred.promise.then(function () {

                            var carrierAccountPayload = {
                                filter: { SupplierFilterSettings: supplierFilterSettings },
                                //selectedIds: []
                            };
                            if (options != undefined) {
                                //for (var key in options) {
                                //    if (key != "$type") {
                                //        carrierAccountPayload.selectedIds.push(options[key].SupplierId); 
                                //    }
                                //}
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
                        return loadOptionGridPromiseDeferred.promise.then(function () {
                            $scope.scopeModel.selectedSuppliers = [];
                        });
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

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        $scope.scopeModel.selectedSuppliers = [];
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

                    return {
                        $type: "TOne.WhS.Routing.Business.SpecialRequestRouteRule, TOne.WhS.Routing.Business",
                        Options: getOptions(),
                        OverallBackupOptions: getOverallBackupOptions()
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
        }

        return directiveDefinitionObject;
    }]);