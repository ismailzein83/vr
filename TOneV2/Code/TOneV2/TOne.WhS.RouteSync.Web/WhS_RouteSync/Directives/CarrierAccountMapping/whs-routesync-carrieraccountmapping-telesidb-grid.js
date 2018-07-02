'use strict';

app.directive('whsRoutesyncCarrieraccountmappingTelesidbGrid', ['VRNotificationService', 'VRUIUtilsService', 'UtilsService', 'WhS_BE_CarrierAccountAPIService', 'WhS_BE_CarrierAccountTypeEnum',
    function (VRNotificationService, VRUIUtilsService, UtilsService, WhS_BE_CarrierAccountAPIService, WhS_BE_CarrierAccountTypeEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new TelesIdbCarrierAcountMappingGridCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_RouteSync/Directives/CarrierAccountMapping/Templates/CarrierAccountMappingTelesIdbGridTemplate.html'
        };

        function TelesIdbCarrierAcountMappingGridCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var carrierMappings;
            var context;

            var supplierMappingValidationFunction;
            var showCustomerMappingFunction;
            var showSupplierMappingFunction;
            
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

                    if (supplierMappingValue == undefined || supplierMappingValue == '' || context == undefined)
                        return null;

                    var mappingSeparator = context.getMappingSeparator();
                    if (mappingSeparator == undefined || mappingSeparator == '')
                        return null;

                    var oneSupplierMappingLength = context.getSupplierMappingLength();
                    if (supplierMappingValue.length == oneSupplierMappingLength)
                        return null;

                    var supplierMappingValues = supplierMappingValue.split(mappingSeparator);
                    var supplierMappingWithoutSeparatorsLength = supplierMappingValue.length - (mappingSeparator.length * (supplierMappingValues.length - 1));

                    if (supplierMappingWithoutSeparatorsLength % oneSupplierMappingLength == 0) {
                        if (supplierMappingValues.length >= 2) {
                            var isValid = true;

                            for (var index = 0; index < supplierMappingValues.length; index++) {
                                var supplierMappingValue = supplierMappingValues[index];
                                if (supplierMappingValue.length != oneSupplierMappingLength) {
                                    isValid = false;
                                    break;
                                }
                            }

                            if (isValid)
                                return null;
                        }
                    }

                    return 'Invalid Mapping';
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

                    var telesIdbCarrierAccountMappingsGridLoadPromise = getTelesIdbCarrierAccountMappingsGridLoadPromise();
                    promises.push(telesIdbCarrierAccountMappingsGridLoadPromise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var mappingSeparator = context != undefined ? context.getMappingSeparator() : undefined;

                    var results = {};
                    for (var i = 0; i < $scope.scopeModel.carrierAccountMappings.length; i++) {
                        var carrierMapping = $scope.scopeModel.carrierAccountMappings[i];
                        results[carrierMapping.CarrierAccountId] = {
                            CarrierId: carrierMapping.CarrierAccountId,
                            CustomerMapping: (carrierMapping.CustomerMapping != undefined && carrierMapping.CustomerMapping != '') ? carrierMapping.CustomerMapping.split(mappingSeparator) : undefined,
                            SupplierMapping: (carrierMapping.SupplierMapping != undefined && carrierMapping.SupplierMapping != '') ? carrierMapping.SupplierMapping.split(mappingSeparator) : undefined
                        };
                    }
                    return results;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getTelesIdbCarrierAccountMappingsGridLoadPromise(payload) {

                var LoadPromisedeferred = UtilsService.createPromiseDeferred();

                var serializedFilter = {};
                WhS_BE_CarrierAccountAPIService.GetCarrierAccountInfo(serializedFilter).then(function (response) {

                    if (response != undefined) {
                        var mappingSeparator = context != undefined ? context.getMappingSeparator() : undefined;

                        for (var i = 0; i < response.length; i++) {
                            var currentCarrierAccountInfo = response[i];

                            var carrierAccountMapping = carrierMappings != undefined ? carrierMappings[response[i].CarrierAccountId] : undefined;
                            var carrierMapping = {
                                CarrierAccountId: currentCarrierAccountInfo.CarrierAccountId,
                                CarrierAccountType: currentCarrierAccountInfo.AccountType,
                                CarrierAccountName: currentCarrierAccountInfo.Name,
                                CustomerMapping: (carrierAccountMapping && carrierAccountMapping.CustomerMapping) ? carrierAccountMapping.CustomerMapping.join(mappingSeparator) : '',
                                SupplierMapping: (carrierAccountMapping && carrierAccountMapping.SupplierMapping) ? carrierAccountMapping.SupplierMapping.join(mappingSeparator) : '',
                                ShowCustomerMapping: showCustomerMappingFunction,
                                ShowSupplierMapping: showSupplierMappingFunction,
                                SupplierMappingValidation: supplierMappingValidationFunction
                            };
                            $scope.scopeModel.carrierAccountMappings.push(carrierMapping);
                        }
                    }

                    loadMoreCarrierMappings();
                    LoadPromisedeferred.resolve();

                }).catch(function (error) {
                    LoadPromisedeferred.reject(error);
                });

                return LoadPromisedeferred.promise;
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
