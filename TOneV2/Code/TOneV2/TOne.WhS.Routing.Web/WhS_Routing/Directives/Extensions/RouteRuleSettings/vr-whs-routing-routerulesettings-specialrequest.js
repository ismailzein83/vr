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
                return '/Client/Modules/WhS_Routing/Directives/Extensions/RouteRuleSettings/Templates/RouteRuleSpecialRequestDirective.html';
            }
        };

        function LCRCtor(ctrl, $scope) {

            var carrierAccountSelectorAPI;
            var carrierAccountSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.suppliers = [];

                $scope.scopeModel.onCarrierAccountDirectiveReady = function (api) {
                    carrierAccountSelectorAPI = api;
                    carrierAccountSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.isValid = function () {
                    var suppliers = $scope.scopeModel.suppliers;
                    if (suppliers == undefined)
                        return null;

                    //var positionArray = [];

                    var total = undefined;
                    for (var x = 0; x < suppliers.length; x++) {
                        var currentSupplier = suppliers[x];
                        //positionArray.push(currentSupplier.Position)
                        if (currentSupplier.Percentage != undefined) {
                            if (total == undefined) {
                                total = 0;
                            }
                            total += parseFloat(suppliers[x].Percentage);
                        }
                    }

                    //if (checkForDuplicateValues(positionArray))
                    //    return "Positions should be unique";

                    if (total != undefined && total != 100)
                        return "Sum of all Percentages must be equal to 100";

                    return null;
                };

                $scope.scopeModel.onSelectSupplier = function (selectedItem) {

                    $scope.scopeModel.suppliers.push({
                        SupplierId: selectedItem.CarrierAccountId,
                        Name: selectedItem.Name,
                        //Position: selectedItem.Position,
                        ForceOption: selectedItem.ForceOption,
                        NumberOfTries: selectedItem.NumberOfTries,
                        Percentage: selectedItem.Percentage
                    });
                };

                $scope.scopeModel.onDeselectSupplier = function (deselectedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.suppliers, deselectedItem.SupplierId, 'SupplierId');
                    $scope.scopeModel.suppliers.splice(index, 1);
                };

                $scope.scopeModel.onDeleteRow = function (deletedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedSuppliers, deletedItem.SupplierId, 'SupplierId');
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
                    var options;

                    if (payload != undefined) {
                        supplierFilterSettings = payload.SupplierFilterSettings;

                        if (payload.RouteRuleSettings != undefined) {
                            options = payload.RouteRuleSettings.Options;
                        }
                    }

                    //var getCustomerRouteBuildNumberOfOptionsPromise = getCustomerRouteBuildNumberOfOptions();
                    //promises.push(getCustomerRouteBuildNumberOfOptionsPromise);

                    var loadCarrierAccountSelectorPromise = getLoadCarrierAccountSelectorPromise(supplierFilterSettings, options);
                    promises.push(loadCarrierAccountSelectorPromise);

                    loadCarrierAccountSelectorPromise.then(function () {
                        for (var i = 0; i < $scope.scopeModel.selectedSuppliers.length; i++) {
                            var currentSupplier = $scope.scopeModel.selectedSuppliers[i];
                            var currentOption = options[currentSupplier.CarrierAccountId];
                            $scope.scopeModel.suppliers.push({
                                SupplierId: currentSupplier.CarrierAccountId,
                                Name: currentSupplier.Name,
                                Position: currentOption.Position,
                                ForceOption: currentOption.ForceOption,
                                NumberOfTries: currentOption.NumberOfTries,
                                Percentage: currentOption.Percentage
                            });
                            $scope.scopeModel.suppliers.sort(function (firstItem, secondItem) {
                                return firstItem.Position - secondItem.Position;
                            });
                        }
                    });

                    //function getCustomerRouteBuildNumberOfOptions() {
                    //    return WhS_Routing_RoutRuleSettingsAPIService.GetCustomerRouteBuildNumberOfOptions().then(function (response) {
                    //        $scope.scopeModel.maxPositionValue = response;
                    //    });
                    //}

                    function getLoadCarrierAccountSelectorPromise(supplierFilterSettings, options) {
                        var loadCarrierAccountSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        carrierAccountSelectorReadyPromiseDeferred.promise.then(function () {

                            var carrierAccountPayload = {
                                filter: { SupplierFilterSettings: supplierFilterSettings },
                                selectedIds: []
                            };
                            if (options != undefined) {
                                for (var key in options) {
                                    if (key != "$type") {
                                        carrierAccountPayload.selectedIds.push(options[key].SupplierId);
                                    }
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
                        $type: "TOne.WhS.Routing.Business.SpecialRequestRouteRule, TOne.WhS.Routing.Business",
                        Options: getOptions()
                    };

                    function getOptions() {
                        var options = {};
                        for (var i = 0; i < $scope.scopeModel.suppliers.length; i++) {
                            var supplier = $scope.scopeModel.suppliers[i];
                            supplier.Position = i + 1;
                            options[supplier.SupplierId] = supplier;
                        }
                        return options;
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            //function checkForDuplicateValues(array) {
            //    var array_sorted = array.slice().sort();

            //    var results = [];
            //    for (var i = 0; i < array.length - 1; i++) {
            //        if (array_sorted[i] != undefined && array_sorted[i + 1] == array_sorted[i])
            //            return true;
            //    }
            //    return false;
            //}

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);