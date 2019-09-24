﻿(function (app) {

    'use strict';

    whsRoutesyncCarrieraccountmappingHuaweiSoftX3000Grid.$inject = ['VRUIUtilsService', 'UtilsService', 'WhS_BE_CarrierAccountAPIService', 'WhS_BE_CarrierAccountTypeEnum', 'WhS_BE_CarrierAccountActivationStatusEnum'];

    function whsRoutesyncCarrieraccountmappingHuaweiSoftX3000Grid(VRUIUtilsService, UtilsService, WhS_BE_CarrierAccountAPIService, WhS_BE_CarrierAccountTypeEnum, WhS_BE_CarrierAccountActivationStatusEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new HuaweiSoftX3000CarrierAccountMappingGridCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_RouteSync/Directives/CarrierAccountMapping/Templates/CarrierAccountMappingHuaweiSoftX3000GridTemplate.html'
        };

        function HuaweiSoftX3000CarrierAccountMappingGridCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var carrierMappings;

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

                $scope.scopeModel.showExpandIcon = function (dataItem) {
                    if (showCustomerMapping(dataItem))
                        return true;

                    if (showSupplierMapping(dataItem))
                        return true;

                    return false;
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        carrierMappings = payload.carrierMappings;
                    }

                    var huaweiSoftX3000CarrierAccountMappingsGridLoadPromise = getHuaweiSoftX3000CarrierAccountMappingsGridLoadPromise();
                    promises.push(huaweiSoftX3000CarrierAccountMappingsGridLoadPromise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var results = {};

                    for (var i = 0; i < $scope.scopeModel.carrierAccountMappings.length; i++) {
                        var carrierAccountMapping = $scope.scopeModel.carrierAccountMappings[i];
                        if (carrierAccountMapping == undefined)
                            continue;

                        if (carrierAccountMapping.customerMappingGridAPI != undefined || carrierAccountMapping.supplierMappingGridAPI != undefined) {
                            var customerMapping = getCustomerMapping(carrierAccountMapping.customerMappingGridAPI, carrierAccountMapping.CarrierAccountId);
                            var supplierMapping = getSupplierMapping(carrierAccountMapping.supplierMappingGridAPI, carrierAccountMapping.CarrierAccountId);

                            if (customerMapping != undefined || supplierMapping != undefined) {
                                results[carrierAccountMapping.CarrierAccountId] = {
                                    CarrierId: carrierAccountMapping.CarrierAccountId,
                                    CustomerMapping: customerMapping,
                                    SupplierMapping: supplierMapping
                                };
                            }

                        } else {
                            results[carrierAccountMapping.CarrierAccountId] = carrierMappings != undefined ? carrierMappings[carrierAccountMapping.CarrierAccountId] : undefined;
                        }
                    }

                    function getCustomerMapping(customerMappingGridAPI, carrierAccountId) {
                        if (customerMappingGridAPI != undefined)
                            return customerMappingGridAPI.getData();

                        if (carrierMappings != undefined && carrierMappings[carrierAccountId] != undefined)
                            return carrierMappings[carrierAccountId].CustomerMapping;

                        return null;
                    }
                    function getSupplierMapping(supplierMappingGridAPI, carrierAccountId) {
                        if (supplierMappingGridAPI != undefined)
                            return supplierMappingGridAPI.getData();

                        if (carrierMappings != undefined && carrierMappings[carrierAccountId] != undefined)
                            return carrierMappings[carrierAccountId].SupplierMapping;

                        return null;
                    }

                    return results;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

            function getHuaweiSoftX3000CarrierAccountMappingsGridLoadPromise() {
                var loadHuaweiSoftX3000CarrierAccountMappingsGridPromiseDeferred = UtilsService.createPromiseDeferred();

                var carrierAccountfilter = { ActivationStatuses: [WhS_BE_CarrierAccountActivationStatusEnum.Active.value, WhS_BE_CarrierAccountActivationStatusEnum.Testing.value] };
                var serilizedCarrierAccountFilter = UtilsService.serializetoJson(carrierAccountfilter);

                WhS_BE_CarrierAccountAPIService.GetCarrierAccountInfo(serilizedCarrierAccountFilter).then(function (response) {
                    if (response != undefined) {
                        for (var i = 0; i < response.length; i++) {
                            var currentCarrierAccountInfo = response[i];

                            var carrierMapping = carrierMappings != undefined ? carrierMappings[response[i].CarrierAccountId] : undefined;
                            var carrierAccountMapping = {
                                CarrierAccountId: currentCarrierAccountInfo.CarrierAccountId,
                                CarrierAccountType: currentCarrierAccountInfo.AccountType,
                                CarrierAccountName: currentCarrierAccountInfo.Name,
                                CustomerMapping: (carrierMapping && carrierMapping.CustomerMapping) ? carrierMapping.CustomerMapping : undefined,
                                SupplierMapping: (carrierMapping && carrierMapping.SupplierMapping) ? carrierMapping.SupplierMapping : undefined
                            };
                            extendCarrierAccountMapping(carrierAccountMapping);
                            $scope.scopeModel.carrierAccountMappings.push(carrierAccountMapping);
                        }
                    }

                    loadMoreCarrierMappings();
                    loadHuaweiSoftX3000CarrierAccountMappingsGridPromiseDeferred.resolve();
                }).catch(function (error) {
                    loadHuaweiSoftX3000CarrierAccountMappingsGridPromiseDeferred.reject(error);
                });

                return loadHuaweiSoftX3000CarrierAccountMappingsGridPromiseDeferred.promise;
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
                    var currentCarrierAccountMapping = $scope.scopeModel.carrierAccountMappings[i];
                    defineCarrierAccountMappingTabs(currentCarrierAccountMapping);
                    items.push(currentCarrierAccountMapping);
                }
                gridAPI.addItemsToSource(items);
            }
            function defineCarrierAccountMappingTabs(carrierAccountMapping) {
                var drillDownTabs = [];

                if (showCustomerMapping(carrierAccountMapping)) {
                    drillDownTabs.push(buildCustomerMappingDrillDownTab());
                }

                if (showSupplierMapping(carrierAccountMapping)) {
                    drillDownTabs.push(buildSupplierMappingDrillDownTab());
                }

                setDrillDownTabs();

                function buildCustomerMappingDrillDownTab() {
                    var drillDownTab = {};
                    drillDownTab.title = "In";
                    drillDownTab.directive = "whs-routesync-huawei-softx3000-customermapping";

                    drillDownTab.loadDirective = function (customerMappingGridAPI, carrierAccountMapping) {
                        carrierAccountMapping.customerMappingGridAPI = customerMappingGridAPI;
                        return carrierAccountMapping.customerMappingGridAPI.load(buildCustomerMappingPayload(carrierAccountMapping));
                    };

                    function buildCustomerMappingPayload(carrierAccountMapping) {
                        var customerMappingPayload = {};
                        customerMappingPayload.customerMapping = carrierAccountMapping.CustomerMapping;
                        customerMappingPayload.context = buildCustomerMappingDirectiveContext(carrierAccountMapping);
                        return customerMappingPayload;
                    }

                    return drillDownTab;
                }
                function buildSupplierMappingDrillDownTab() {
                    var drillDownTab = {};
                    drillDownTab.title = "Out";
                    drillDownTab.directive = "whs-routesync-huawei-softx3000-suppliermapping";

                    drillDownTab.loadDirective = function (supplierMappingGridAPI, carrierAccountMapping) {
                        carrierAccountMapping.supplierMappingGridAPI = supplierMappingGridAPI;
                        return carrierAccountMapping.supplierMappingGridAPI.load(buildSupplierMappingQuery(carrierAccountMapping));
                    };

                    function buildSupplierMappingQuery(carrierAccountMapping) {
                        var supplierMappingQuery = {};
                        supplierMappingQuery.supplierMapping = carrierAccountMapping.SupplierMapping;
                        supplierMappingQuery.context = buildSupplierMappingDirectiveContext(carrierAccountMapping);
                        return supplierMappingQuery;
                    }

                    return drillDownTab;
                }
                function setDrillDownTabs() {
                    var drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(drillDownTabs, gridAPI);
                    drillDownManager.setDrillDownExtensionObject(carrierAccountMapping);
                }
            }
            function showCustomerMapping(carrierMappingItem) {
                if (WhS_BE_CarrierAccountTypeEnum.Exchange.value == carrierMappingItem.CarrierAccountType || WhS_BE_CarrierAccountTypeEnum.Customer.value == carrierMappingItem.CarrierAccountType)
                    return true;
                return false;
            }
            function showSupplierMapping(carrierMappingItem) {
                if (WhS_BE_CarrierAccountTypeEnum.Exchange.value == carrierMappingItem.CarrierAccountType || WhS_BE_CarrierAccountTypeEnum.Supplier.value == carrierMappingItem.CarrierAccountType)
                    return true;
                return false;
            }

            function extendCarrierAccountMapping(carrierAccountMapping) {
                if (carrierAccountMapping == undefined)
                    return;

                if (carrierAccountMapping.CustomerMapping != undefined) {
                    var customerMapping = carrierAccountMapping.CustomerMapping;

                    var isCustomerMappingExists = customerMapping != undefined && customerMapping.RSSC != undefined && customerMapping.DNSet != "";
                    if (isCustomerMappingExists) {
                        carrierAccountMapping.CustomerMappingDescription = buildCustomerMappingDescription(customerMapping);
                    } else {
                        carrierAccountMapping.CustomerMappingDescription = "";
                    }
                }

                if (carrierAccountMapping.SupplierMapping != undefined) {
                    var supplierMapping = carrierAccountMapping.SupplierMapping;

                    var isSupplierMappingExists = supplierMapping != undefined && supplierMapping.SRT != "";
                    if (isSupplierMappingExists) {
                        carrierAccountMapping.SupplierMappingDescription = buildSupplierMappingDescription(supplierMapping);
                    } else {
                        carrierAccountMapping.SupplierMappingDescription = "";
                    }
                }
            }
            function buildCustomerMappingDescription(customerMapping) {
                if (customerMapping == undefined)
                    return "";

                var rsscDescription = customerMapping.RSSC != undefined ? customerMapping.RSSC : "";
                var dnSetDescription = customerMapping.DNSet != undefined ? customerMapping.DNSet : "";

                return "RSSC: " + rsscDescription + "; DN Set: " + dnSetDescription;
            }
            function buildCustomerMappingDirectiveContext(carrierAccountMapping) {
                var context = {
                    updateErrorDescription: function (isValid, fromCustomerMapping) {
                        updateErrorDescription(carrierAccountMapping, isValid, fromCustomerMapping);
                    },
                    updateCustomerMappingDescription: function (customerMapping) {
                        carrierAccountMapping.CustomerMappingDescription = buildCustomerMappingDescription(customerMapping);
                    }
                };
                return context;
            }
            function buildSupplierMappingDescription(supplierMapping) {
                if (supplierMapping == undefined)
                    return "";

                var srt = supplierMapping.SRT != undefined ? supplierMapping.SRT : "";

                return "SRT: " + srt;
            }
            function buildSupplierMappingDirectiveContext(carrierAccountMapping) {
                var suppliercontext = {
                    updateErrorDescription: function (isValid, fromCustomerMapping) {
                        updateErrorDescription(carrierAccountMapping, isValid, fromCustomerMapping);
                    },
                    updateSupplierMappingDescription: function (supplierMapping) {
                        carrierAccountMapping.SupplierMappingDescription = buildSupplierMappingDescription(supplierMapping);
                    }
                };
                return suppliercontext;
            }
            function updateErrorDescription(carrierAccountMapping, isValid, fromCustomerMapping) {
                if (fromCustomerMapping) {
                    carrierAccountMapping.isCustomerMappingInvalid = !isValid;
                } else {
                    carrierAccountMapping.isSupplierMappingInvalid = !isValid;
                }
            }
        }
    }

    app.directive('whsRoutesyncCarrieraccountmappingHuaweiSoftx3000Grid', whsRoutesyncCarrieraccountmappingHuaweiSoftX3000Grid);
})(app);
