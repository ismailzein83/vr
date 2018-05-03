'use strict';

app.directive('whsRoutesyncCarrieraccountmappingFreeradiusGrid', ['VRNotificationService', 'VRUIUtilsService', 'UtilsService', 'WhS_BE_CarrierAccountAPIService', 'WhS_BE_CarrierAccountTypeEnum',
    function (VRNotificationService, VRUIUtilsService, UtilsService, WhS_BE_CarrierAccountAPIService, WhS_BE_CarrierAccountTypeEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FreeRadiusCarrierAcountMappingGridCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_RouteSync/Directives/CarrierAccountMapping/Templates/CarrierAccountMappingFreeRadiusGridTemplate.html'
        };

        function FreeRadiusCarrierAcountMappingGridCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var carrierMappings;
            var context;

            var supplierMappingValidationFunction;
            var showCustomerMappingFunction;
            var showSupplierMappingFunction;
            //var oneSupplierMappingLength = 4;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.carrierAccountMappings = [];
                $scope.scopeModel.carrierAccountMappingsGridDS = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.loadMoreData = function () {
                    return loadMoreCarrierMappings();
                };

                supplierMappingValidationFunction = function (supplierMappingValue) {

                    return null;

                    //if (supplierMappingValue == undefined || supplierMappingValue == '' || context == undefined)
                    //    return null;

                    //var mappingSeparator = context.getMappingSeparator();
                    //if (mappingSeparator == undefined || mappingSeparator == '')
                    //    return null;

                    //if (supplierMappingValue.length == oneSupplierMappingLength)
                    //    return null;

                    //var supplierMappingValues = supplierMappingValue.split(mappingSeparator);
                    //var supplierMappingWithoutSeparatorsLength = supplierMappingValue.length - (mappingSeparator.length * (supplierMappingValues.length - 1));

                    //if (supplierMappingWithoutSeparatorsLength % oneSupplierMappingLength == 0) {
                    //    if (supplierMappingValues.length >= 2) {
                    //        var isValid = true;

                    //        for (var index = 0; index < supplierMappingValues.length; index++) {
                    //            var supplierMappingValue = supplierMappingValues[index];
                    //            if (supplierMappingValue.length != oneSupplierMappingLength) {
                    //                isValid = false;
                    //                break;
                    //            }
                    //        }

                    //        if (isValid)
                    //            return null;
                    //    }
                    //}

                    //return 'Invalid Mapping';
                };

                showCustomerMappingFunction = function (carrierMappingIem) {
                    if (WhS_BE_CarrierAccountTypeEnum.Exchange.value == carrierMappingIem.CarrierAccountType || WhS_BE_CarrierAccountTypeEnum.Customer.value == carrierMappingIem.CarrierAccountType)
                        return true;
                    return false;
                };

                showSupplierMappingFunction = function (carrierMappingIem) {
                    if (WhS_BE_CarrierAccountTypeEnum.Exchange.value == carrierMappingIem.CarrierAccountType || WhS_BE_CarrierAccountTypeEnum.Supplier.value == carrierMappingIem.CarrierAccountType)
                        return true;
                    return false;
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        carrierMappings = payload.carrierMappings;
                        context = payload.context;
                    }

                    var promises = [];

                    var freeRadiusCarrierAccountMappingsGridLoadPromise = getFreeRadiusCarrierAccountMappingsGridLoadPromise();
                    promises.push(freeRadiusCarrierAccountMappingsGridLoadPromise);

                    function getFreeRadiusCarrierAccountMappingsGridLoadPromise(payload) {

                        var loadPromiseDeferred = UtilsService.createPromiseDeferred();

                        var serializedFilter = {};
                        WhS_BE_CarrierAccountAPIService.GetCarrierAccountInfo(serializedFilter).then(function (response) {

                            if (response != undefined) {
                                var mappingSeparator = context != undefined ? context.getMappingSeparator() : undefined;

                                for (var i = 0; i < response.length; i++) {
                                    var currentCarrierAccountInfo = response[i];

                                    var carrierAccountMapping = carrierMappings != undefined ? carrierMappings[response[i].CarrierAccountId] : undefined;
                                    var customerMappings = (carrierAccountMapping && carrierAccountMapping.CustomerMappings) ? UtilsService.getPropValuesFromArray(carrierAccountMapping.CustomerMappings, "Mapping") : undefined;
                                    var supplierMappings = (carrierAccountMapping && carrierAccountMapping.SupplierMappings) ? UtilsService.getPropValuesFromArray(carrierAccountMapping.SupplierMappings, "Mapping") : undefined;

                                    var carrierMapping = {
                                        CarrierAccountId: currentCarrierAccountInfo.CarrierAccountId,
                                        CarrierAccountType: currentCarrierAccountInfo.AccountType,
                                        CarrierAccountName: currentCarrierAccountInfo.Name,
                                        CustomerMappings: customerMappings != undefined ? customerMappings.join(mappingSeparator) : '',
                                        SupplierMappings: supplierMappings != undefined ? supplierMappings.join(mappingSeparator) : '',
                                        ShowCustomerMapping: showCustomerMappingFunction,
                                        ShowSupplierMapping: showSupplierMappingFunction,
                                        SupplierMappingValidation: supplierMappingValidationFunction
                                    };
                                    $scope.scopeModel.carrierAccountMappings.push(carrierMapping);
                                }
                            }

                            loadMoreCarrierMappings();
                            loadPromiseDeferred.resolve();

                        }).catch(function (error) {
                            loadPromiseDeferred.reject(error);
                        });

                        return loadPromiseDeferred.promise;
                    }


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var mappingSeparator = context != undefined ? context.getMappingSeparator() : undefined;

                    var results = {};
                    for (var i = 0; i < $scope.scopeModel.carrierAccountMappings.length; i++) {
                        var carrierMapping = $scope.scopeModel.carrierAccountMappings[i];
                        results[carrierMapping.CarrierAccountId] = {
                            CarrierId: carrierMapping.CarrierAccountId,
                            CustomerMappings: getCustomerMappings(carrierMapping),
                            SupplierMappings: getSupplierMappings(carrierMapping)
                        };
                    }

                    function getCustomerMappings(carrierMapping) {
                        if (carrierMapping.CustomerMappings == undefined || carrierMapping.CustomerMappings == '')
                            return;

                        var customerMappings = carrierMapping.CustomerMappings.split(mappingSeparator);

                        var _results = [];
                        for (var i = 0; i < customerMappings.length; i++) {
                            _results.push({ Mapping: customerMappings[i] });
                        }
                        return _results;
                    }
                    function getSupplierMappings(carrierMapping) {
                        if (carrierMapping.SupplierMappings == undefined || carrierMapping.SupplierMappings == '')
                            return;

                        var supplierMappings = carrierMapping.SupplierMappings.split(mappingSeparator);

                        var _results = [];
                        for (var i = 0; i < supplierMappings.length; i++) {
                            _results.push({ Mapping: supplierMappings[i] });
                        }
                        return _results;
                    }

                    return results;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


            function loadMoreCarrierMappings() {

                var pageInfo = gridAPI.getPageInfo();
                var itemsLength = pageInfo.toRow;

                if (pageInfo.toRow > $scope.scopeModel.carrierAccountMappings.length) {
                    if (pageInfo.fromRow < $scope.scopeModel.carrierAccountMappings.length)
                        itemsLength = $scope.scopeModel.carrierAccountMappings.length;
                    else
                        return;
                }

                var items = [];

                for (var i = pageInfo.fromRow - 1; i < itemsLength; i++) {
                    items.push($scope.scopeModel.carrierAccountMappings[i]);
                }
                gridAPI.addItemsToSource(items);
            }
        }
    }]);
