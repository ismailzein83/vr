'use strict';

app.directive('vrWhsRoutingRouterulesettingsSpecialrequest', ['UtilsService', 'VRUIUtilsService', 'WhS_Routing_RoutRuleSettingsAPIService', 'WhS_BE_CarrierAccountAPIService',
    function (UtilsService, VRUIUtilsService, WhS_Routing_RoutRuleSettingsAPIService, WhS_BE_CarrierAccountAPIService) {

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
                    if (suppliers == undefined || suppliers.length == 0)
                        return "At least one supplier should be selected";

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
                        tempId: UtilsService.guid(),
                        SupplierId: selectedItem.CarrierAccountId,
                        Name: selectedItem.Name,
                        //Position: selectedItem.Position,
                        ForceOption: selectedItem.ForceOption,
                        NumberOfTries: selectedItem.NumberOfTries,
                        Percentage: selectedItem.Percentage
                    });
                    $scope.scopeModel.selectedSuppliers = [];

                };

                $scope.scopeModel.onDeleteRow = function (deletedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.suppliers, deletedItem.tempId, 'tempId');
                    $scope.scopeModel.suppliers.splice(index, 1);
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var selectedSupplierIds = [];
                    var supplierFilterSettings;
                    var options;

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
                                        var currentOption= options[i];
                                        $scope.scopeModel.suppliers.push({
                                            tempId: UtilsService.guid(),
                                            SupplierId: currentOption.SupplierId,
                                            Name: UtilsService.getItemByVal(suppliers, currentOption.SupplierId, 'CarrierAccountId').Name,
                                            Position: currentOption.Position,
                                            ForceOption: currentOption.ForceOption,
                                            NumberOfTries: currentOption.NumberOfTries,
                                            Percentage: currentOption.Percentage
                                        });
                                        $scope.scopeModel.suppliers.sort(function (firstItem, secondItem) {
                                            return firstItem.Position - secondItem.Position;
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
                                filter: {
                                    SupplierFilterSettings: supplierFilterSettings
                                },
                            };
                            if (options != undefined) {
                                for (var key in options) {
                                    if (key != "$type") {
                                        selectedSupplierIds.push(options[key].SupplierId);
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
                        var options = [];
                        for (var i = 0; i < $scope.scopeModel.suppliers.length; i++) {
                            var supplier = $scope.scopeModel.suppliers[i];
                            supplier.Position = i + 1;
                            options.push(supplier);
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