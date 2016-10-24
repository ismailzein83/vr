'use strict';
app.directive('vrWhsRoutingRouterulesettingsFixed', ['UtilsService', 'VRUIUtilsService', 'WhS_Routing_RateOptionEnum', 'WhS_Routing_RateOptionTypeEnum',
    function (UtilsService, VRUIUtilsService, WhS_Routing_RateOptionEnum, WhS_Routing_RateOptionTypeEnum) {

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
                }
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
                }

                $scope.scopeModel.isValid = function () {
                    var suppliers = $scope.scopeModel.fixedSuppliers;
                    if (suppliers == undefined)
                        return null;

                    var total = 0;
                    for (var x = 0; x < suppliers.length; x++) {
                        total += parseFloat(suppliers[x].percentageValue);
                    }

                    if (total != 100)
                        return "Sum of all Percentages must be equal to 100";

                    return null;
                }

                $scope.scopeModel.onSelectSupplier = function (selectedItem) {
                    $scope.scopeModel.fixedSuppliers.push({
                        CarrierAccountId: selectedItem.CarrierAccountId,
                        Name: selectedItem.Name
                    });
                };

                $scope.scopeModel.onDeselectSupplier = function (deselectedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.fixedSuppliers, selectedItem.CarrierAccountId, 'CarrierAccountId');
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
                    var filters;

                    if (payload != undefined) {
                        supplierFilterSettings = payload.SupplierFilterSettings;

                        if(payload.RouteRuleSettings != undefined ){
                            options = payload.RouteRuleSettings.Options;
                            filters = payload.RouteRuleSettings.Filters;
                        }
                    }


                    var loadCarrierAccountSelectorPromise = getLoadCarrierAccountSelectorPromise(supplierFilterSettings, options);
                    promises.push(loadCarrierAccountSelectorPromise);

                    loadCarrierAccountSelectorPromise.then(function () {
                        for (var i = 0; i < $scope.scopeModel.fixedSuppliers.length; i++) {
                            var currentSupplier = $scope.scopeModel.fixedSuppliers[i];
                            var currentOption = options[i];
                            currentSupplier.percentageValue = currentOption.Percentage;
                            currentSupplier.isRemoveLoss = filters[currentOption.SupplierId] ? true : false;
                        }
                    });

                    function getLoadCarrierAccountSelectorPromise(supplierFilterSettings, options) {
                        var loadCarrierAccountSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        carrierAccountSelectorReadyPromiseDeferred.promise.then(function () {

                            var carrierAccountPayload = {
                                filter: { SupplierFilterSettings: supplierFilterSettings },
                                selectedIds: []
                            };
                            if (options != undefined) {
                                for (var i = 0; i < options.length; i++) {
                                    carrierAccountPayload.selectedIds.push(options[i].SupplierId);
                                }
                            }

                            VRUIUtilsService.callDirectiveLoad(carrierAccountSelectorAPI, carrierAccountPayload, loadCarrierAccountSelectorPromiseDeferred);
                        });

                        return loadCarrierAccountSelectorPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Business.FixedRouteRule, TOne.WhS.Routing.Business",
                        Options: getOptions(),
                        Filters: getFilters()
                    };

                    function getOptions() {
                        var options = [];
                        for (var i = 0; i < $scope.scopeModel.fixedSuppliers.length; i++) {

                            var supplier = $scope.scopeModel.fixedSuppliers[i];
                            options.push({
                                SupplierId: supplier.CarrierAccountId,
                                Percentage: supplier.percentageValue,
                            });
                        }

                        return options;
                    }
                    function getFilters() {
                        var filters = {};
                        for (var i = 0; i < $scope.scopeModel.selectedSuppliers.length; i++) {

                            var supplier = $scope.scopeModel.selectedSuppliers[i];
                            if (supplier.isRemoveLoss) {
                                filters[supplier.CarrierAccountId] = [{
                                    $type: "TOne.WhS.Routing.Business.RouteRules.Filters.RateOptionFilter, TOne.WhS.Routing.Business",
                                    RateOption: WhS_Routing_RateOptionEnum.MaximumLoss.value,
                                    RateOptionType: WhS_Routing_RateOptionTypeEnum.Fixed.value,
                                    RateOptionValue: 0
                                }]
                            }
                        }
                        return filters;
                    }
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);