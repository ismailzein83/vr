'use strict';
app.directive('vrWhsRoutingRouterulesettingsSelective', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new selectiveOptionCtor(ctrl, $scope);
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
                return '/Client/Modules/WhS_Routing/Directives/Extensions/RouteRuleSettings/Option/Templates/SelectiveOptionDirective.html';
            }

        };

        function selectiveOptionCtor(ctrl, $scope) {

            this.initializeController = initializeController;

            var context;

            var carrierAccountDirectiveAPI;
            var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var gridAPI;
            var gridPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.gridLeftMenuActions = [];
                ctrl.selectedSuppliers = [];
                ctrl.suppliers = [];

                ctrl.onCarrierAccountDirectiveReady = function (api) {
                    carrierAccountDirectiveAPI = api;
                    carrierAccountReadyPromiseDeferred.resolve();
                };

                ctrl.onGridReady = function (api) {
                    gridAPI = api;
                    gridPromiseDeferred.resolve();
                };

                ctrl.removeFilter = function (dataItem) {
                    var supplierId = dataItem.SupplierId;

                    var index = UtilsService.getItemIndexByVal(ctrl.suppliers, supplierId, 'SupplierId');
                    ctrl.suppliers.splice(index, 1);

                    index = UtilsService.getItemIndexByVal(ctrl.selectedSuppliers, supplierId, 'CarrierAccountId');
                    ctrl.selectedSuppliers.splice(index, 1);
                };

                ctrl.onDeselectSupplier = function (supplier) {
                    var index = UtilsService.getItemIndexByVal(ctrl.selectedSuppliers, supplier.CarrierAccountId, 'CarrierAccountId');
                    ctrl.suppliers.splice(index, 1);
                };

                ctrl.removeAllFilters = function () {
                    ctrl.suppliers = [];
                };

                ctrl.onSelectSupplier = function (selectedItem) {
                    ctrl.isLoadingSelectedSupplier = true;
                    extendAndAddOptionToGrid(selectedItem, undefined, undefined).then(function () {
                        setTimeout(function () {
                            $scope.$apply(function () {
                                ctrl.isLoadingSelectedSupplier = false;
                            });
                        });
                    });
                };

                ctrl.extendSuppliersList = function () {
                    $scope.gridLeftMenuActions.length = 0;
                    if (context != undefined) {
                        var suppliersList = {};
                        context.extendSuppliersList();
                    }
                };

                ctrl.showExtendSuppliersButton = false;

                defineAPI();
            }

            function defineAPI() {


                var api = {};

                api.load = function (payload) {

                    var options;

                    $scope.gridLeftMenuActions.length = 0;

                    if (ctrl.selectedSuppliers != undefined)
                        ctrl.selectedSuppliers.length = 0;

                    if (payload != undefined) {
                        context = payload.context;
                        options = payload.OptionsSettingsGroup != undefined ? payload.OptionsSettingsGroup.Options : undefined;
                    }

                    if (context != undefined && context.showExtendSuppliersButton())
                        $scope.gridLeftMenuActions.push({
                            name: "Reload",
                            onClicked: ctrl.extendSuppliersList
                        });

                    var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();
                    carrierAccountReadyPromiseDeferred.promise.then(function () {
                        var carrierAccountPayload = {
                            filter: { SupplierFilterSettings: payload != undefined ? payload.SupplierFilterSettings : undefined },
                            selectedIds: []
                        };
                        if (options != undefined) {
                            for (var i = 0; i < options.length; i++) {
                                carrierAccountPayload.selectedIds.push(options[i].SupplierId);
                            }
                        }

                        VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, carrierAccountPayload, loadCarrierAccountPromiseDeferred);
                    });

                    var loadSupplierGridPromiseDeferred = UtilsService.createPromiseDeferred();
                    UtilsService.waitMultiplePromises([gridPromiseDeferred.promise, loadCarrierAccountPromiseDeferred.promise]).then(function () {
                        var _promises = [];
                        for (var i = 0; i < ctrl.selectedSuppliers.length; i++) {
                            var selectedSupplier = ctrl.selectedSuppliers[i];
                            var currentOption = options[i];
                            _promises.push(extendAndAddOptionToGrid(selectedSupplier, currentOption));
                        }

                        UtilsService.waitMultiplePromises(_promises).then(function () {
                            loadSupplierGridPromiseDeferred.resolve();
                        });
                    });

                    return loadSupplierGridPromiseDeferred.promise;
                };

                api.getData = function () {

                    function getOptions() {
                        var options = [];

                        for (var i = 0; i < ctrl.suppliers.length; i++) {
                            var supplier = ctrl.suppliers[i];
                            var supplierDealIds = supplier.supplierDealSelectorAPI.getSelectedIds();

                            var supplierDeals = [];
                            if (supplierDealIds != undefined && supplierDealIds.length > 0) {
                                for (var j = 0; j < supplierDealIds.length; j++) {
                                    supplierDeals.push({ SupplierDealId: supplierDealIds[j] });
                                }
                            }

                            options.push({
                                SupplierId: supplier.SupplierId,
                                Percentage: null,
                                Filter: null,
                                SupplierDeals: supplierDeals
                            });
                        }

                        return options;
                    }

                    return {
                        $type: "TOne.WhS.Routing.Business.RouteRules.OptionSettingsGroups.SelectiveOptions, TOne.WhS.Routing.Business",
                        Options: getOptions()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function extendAndAddOptionToGrid(selectedSupplier, currentOption) {

                var selectedCarrierId = selectedSupplier.CarrierAccountId;
                var option = {
                    SupplierId: selectedCarrierId,
                    Name: selectedSupplier.Name
                };

                option.supplierDealLoadDeferred = UtilsService.createPromiseDeferred();
                option.onSupplierDealSelectorReady = function (api) {
                    option.supplierDealSelectorAPI = api;

                    var supplierDealSelectorPayload = { filter: { Filters: [{ $type: "TOne.WhS.Deal.MainExtensions.DealDefinitionFilter.CostDealFilter, TOne.WhS.Deal.MainExtensions" }] } };

                    if (selectedCarrierId != undefined) {
                        supplierDealSelectorPayload.filter.CarrierAccountIds = [selectedCarrierId];
                    }

                    if (currentOption != undefined && currentOption.SupplierDeals != undefined) {
                        supplierDealSelectorPayload.selectedIds = UtilsService.getPropValuesFromArray(currentOption.SupplierDeals, "SupplierDealId");
                    }

                    VRUIUtilsService.callDirectiveLoad(option.supplierDealSelectorAPI, supplierDealSelectorPayload, option.supplierDealLoadDeferred);
                };

                ctrl.suppliers.push(option);

                return option.supplierDealLoadDeferred.promise;
            }

            function getGridLeftMenuActions() {
                return $scope.gridLeftMenuActions;
            }
        }

        return directiveDefinitionObject;
    }]);