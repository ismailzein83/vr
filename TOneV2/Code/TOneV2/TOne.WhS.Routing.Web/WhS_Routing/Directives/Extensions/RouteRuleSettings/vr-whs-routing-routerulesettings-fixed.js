'use strict';

app.directive('vrWhsRoutingRouterulesettingsFixed', ['UtilsService', 'VRUIUtilsService', 'WhS_Routing_RateOptionEnum', 'WhS_Routing_RateOptionTypeEnum', 'WhS_BE_CarrierAccountAPIService',
    function (UtilsService, VRUIUtilsService, WhS_Routing_RateOptionEnum, WhS_Routing_RateOptionTypeEnum, WhS_BE_CarrierAccountAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new FixedCtor(ctrl, $scope);
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
                return '/Client/Modules/WhS_Routing/Directives/Extensions/RouteRuleSettings/Templates/RouteRuleFixedDirective.html';
            }
        };

        function FixedCtor(ctrl, $scope) {

            var carrierAccountSelectorAPI;
            var carrierAccountSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.fixedSuppliers = [];

                $scope.scopeModel.onCarrierAccountDirectiveReady = function (api) {
                    carrierAccountSelectorAPI = api;
                    carrierAccountSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.isValid = function () {
                    var suppliers = $scope.scopeModel.fixedSuppliers;
                    if (suppliers == undefined || suppliers.length == 0)
                        return "At least one supplier should be selected";

                    var total = undefined;
                    for (var x = 0; x < suppliers.length; x++) {
                        var currentSupplier = suppliers[x];
                        if (currentSupplier.PercentageValue != undefined) {
                            if (total == undefined) {
                                total = 0;
                            }
                            total += parseFloat(suppliers[x].PercentageValue);
                        }
                    }

                    if (total == undefined)
                        return null;

                    if (total != 100)
                        return "Sum of all Percentages must be equal to 100";

                    return null;
                };

                $scope.scopeModel.onSelectSupplier = function (selectedItem) {
                    $scope.scopeModel.fixedSuppliers.push({
                        tempId: UtilsService.guid(),
                        CarrierAccountId: selectedItem.CarrierAccountId,
                        Name: selectedItem.Name
                    });
                    $scope.scopeModel.selectedSuppliers = [];
                };

                $scope.scopeModel.onDeleteRow = function (deletedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.fixedSuppliers, deletedItem.tempId, 'tempId');
                    $scope.scopeModel.fixedSuppliers.splice(index, 1);
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var supplierFilterSettings;
                    var options;
                    var selectedSupplierIds = [];

                    if (payload != undefined) {
                        supplierFilterSettings = payload.SupplierFilterSettings;

                        if (payload.RouteRuleSettings != undefined) {
                            options = payload.RouteRuleSettings.Options;
                        }
                    }


                    var loadCarrierAccountSelectorPromise = getLoadCarrierAccountSelectorPromise(supplierFilterSettings, options);
                    promises.push(loadCarrierAccountSelectorPromise);


                    var loadSuppliersPromise = getLoadSuppliersPromise();
                    promises.push(loadSuppliersPromise);

                    function getLoadSuppliersPromise() {
                        var loadSupplierPromiseDeferred = UtilsService.createPromiseDeferred();

                        loadCarrierAccountSelectorPromise.then(function () {
                            if (selectedSupplierIds.length > 0) {
                                var serializedCarrierAccountIds = UtilsService.serializetoJson(selectedSupplierIds);
                                WhS_BE_CarrierAccountAPIService.GetCarrierAccountInfos(serializedCarrierAccountIds).then(function (response) {
                                    var suppliers = response;

                                    for (var i = 0; i < options.length; i++) {
                                        var currentOption = options[i];

                                        var isRemoveLoss = false;
                                        if (currentOption.Filters != undefined && currentOption.Filters.length > 0) {
                                            isRemoveLoss = true;
                                        }

                                        $scope.scopeModel.fixedSuppliers.push({
                                            tempId: UtilsService.guid(),
                                            CarrierAccountId: currentOption.SupplierId,
                                            Name: UtilsService.getItemByVal(suppliers, currentOption.SupplierId, 'CarrierAccountId').Name,
                                            PercentageValue: currentOption.Percentage,
                                            IsRemoveLoss: isRemoveLoss
                                        });
                                    }
                                    loadSupplierPromiseDeferred.resolve();
                                });
                            }
                            else {
                                loadSupplierPromiseDeferred.resolve();
                            }
                        });
                        return loadSupplierPromiseDeferred.promise;
                    };

                    function getLoadCarrierAccountSelectorPromise(supplierFilterSettings, options) {
                        var loadCarrierAccountSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        carrierAccountSelectorReadyPromiseDeferred.promise.then(function () {

                            var carrierAccountPayload = {
                                filter: { SupplierFilterSettings: supplierFilterSettings },
                                selectedIds: []
                            };
                            if (options != undefined) {
                                for (var i = 0; i < options.length; i++) {
                                    var currentOption = options[i];
                                    //carrierAccountPayload.selectedIds.push(currentOption.SupplierId);
                                    selectedSupplierIds.push(currentOption.SupplierId);
                                }
                            }

                            VRUIUtilsService.callDirectiveLoad(carrierAccountSelectorAPI, carrierAccountPayload, loadCarrierAccountSelectorPromiseDeferred);
                        });

                        return loadCarrierAccountSelectorPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Business.FixedRouteRule, TOne.WhS.Routing.Business",
                        Options: getOptions()
                    };

                    function getOptions() {
                        var options = [];
                        for (var i = 0; i < $scope.scopeModel.fixedSuppliers.length; i++) {

                            var supplier = $scope.scopeModel.fixedSuppliers[i];
                            var filters = undefined;
                            if (supplier.IsRemoveLoss) {
                                filters = [{
                                    $type: "TOne.WhS.Routing.Business.RouteRules.Filters.RateOptionFilter, TOne.WhS.Routing.Business",
                                    RateOption: WhS_Routing_RateOptionEnum.MaximumLoss.value,
                                    RateOptionType: WhS_Routing_RateOptionTypeEnum.Fixed.value,
                                    RateOptionValue: 0
                                }];
                            }
                            options.push({
                                SupplierId: supplier.CarrierAccountId,
                                Percentage: supplier.PercentageValue,
                                Filters: filters
                            });
                        }
                        return options;
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);