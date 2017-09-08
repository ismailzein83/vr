'use strict';

app.directive('vrWhsRoutingRouterulesettingsLcr', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new LCRCtor(ctrl, $scope);
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
                return '/Client/Modules/WhS_Routing/Directives/Extensions/RouteRuleSettings/Templates/RouteRuleLCRDirective.html';
            }
        };

        function LCRCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var carrierAccountSelectorAPI;
            var carrierAccountSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.fixedSuppliers = [];

                $scope.scopeModel.onCarrierAccountDirectiveReady = function (api) {
                    carrierAccountSelectorAPI = api;
                    carrierAccountSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onSelectSupplier = function (selectedItem) {

                    $scope.scopeModel.fixedSuppliers.push({
                        CarrierAccountId: selectedItem.CarrierAccountId,
                        Name: selectedItem.Name
                    });
                };

                $scope.scopeModel.onDeselectSupplier = function (deselectedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.fixedSuppliers, deselectedItem.CarrierAccountId, 'CarrierAccountId');
                    $scope.scopeModel.fixedSuppliers.splice(index, 1);
                };

                $scope.scopeModel.onDeleteRow = function (deletedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedSuppliers, deletedItem.CarrierAccountId, 'CarrierAccountId');
                    $scope.scopeModel.selectedSuppliers.splice(index, 1);
                    $scope.scopeModel.onDeselectSupplier(deletedItem);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var supplierFilterSettings;
                    var excludedOptions;

                    if (payload != undefined) {
                        supplierFilterSettings = payload.SupplierFilterSettings;

                        if (payload.RouteRuleSettings != undefined) {
                            excludedOptions = payload.RouteRuleSettings.ExcludedOptions;
                        }
                    }


                    var carrierAccountSelectorLoadPromise = getCarrierAccountSelectorLoadPromise(supplierFilterSettings, excludedOptions);
                    promises.push(carrierAccountSelectorLoadPromise);

                    carrierAccountSelectorLoadPromise.then(function () {
                        for (var i = 0; i < $scope.scopeModel.selectedSuppliers.length; i++) {
                            var currentSupplier = $scope.scopeModel.selectedSuppliers[i];

                            $scope.scopeModel.fixedSuppliers.push({
                                CarrierAccountId: currentSupplier.CarrierAccountId,
                                Name: currentSupplier.Name
                            });
                        }
                    });

                    function getCarrierAccountSelectorLoadPromise(supplierFilterSettings, excludedOptions) {
                        var loadCarrierAccountSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        carrierAccountSelectorReadyPromiseDeferred.promise.then(function () {

                            var carrierAccountPayload = {
                                filter: { SupplierFilterSettings: supplierFilterSettings },
                                selectedIds: []
                            };
                            if (excludedOptions != undefined) {
                                carrierAccountPayload.selectedIds = UtilsService.getPropValuesFromArray(excludedOptions, 'SupplierId');
                            }
                            VRUIUtilsService.callDirectiveLoad(carrierAccountSelectorAPI, carrierAccountPayload, loadCarrierAccountSelectorPromiseDeferred);
                        });

                        return loadCarrierAccountSelectorPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    function getExcludedOptions() {
                        var options = [];
                        for (var i = 0; i < $scope.scopeModel.fixedSuppliers.length; i++) {

                            var supplier = $scope.scopeModel.fixedSuppliers[i];
                            options.push({
                                SupplierId: supplier.CarrierAccountId
                            });
                        }
                        return options;
                    }

                    return {
                        $type: "TOne.WhS.Routing.Business.LCRRouteRule, TOne.WhS.Routing.Business",
                        ExcludedOptions: getExcludedOptions()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);