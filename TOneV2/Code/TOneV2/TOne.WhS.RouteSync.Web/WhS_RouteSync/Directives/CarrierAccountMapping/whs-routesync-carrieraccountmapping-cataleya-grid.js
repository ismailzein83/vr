'use strict';

app.directive('whsRoutesyncCarrieraccountmappingCataleyaGrid', ['VRValidationService', 'VRUIUtilsService', 'UtilsService', 'WhS_BE_CarrierAccountAPIService', 'WhS_BE_CarrierAccountTypeEnum',
    function (VRValidationService, VRUIUtilsService, UtilsService, WhS_BE_CarrierAccountAPIService, WhS_BE_CarrierAccountTypeEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CataleyaCarrierAccountMappingGridCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_RouteSync/Directives/CarrierAccountMapping/Templates/CarrierAccountMappingCataleyaGridTemplate.html'
        };

        function CataleyaCarrierAccountMappingGridCtor($scope, ctrl, $attrs) {
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

                    var cataleyaCarrierAccountMappingsGridLoadPromise = getCataleyaCarrierAccountMappingsGridLoadPromise();
                    promises.push(cataleyaCarrierAccountMappingsGridLoadPromise);

                    var rootPromiseNode = {
                        promises: promises
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {

                    var results = {};
                    for (var i = 0; i < $scope.scopeModel.carrierAccountMappings.length; i++) {
                        var carrierAccountMapping = $scope.scopeModel.carrierAccountMappings[i];
                        if (carrierAccountMapping == undefined)
                            continue;

                        if (carrierAccountMapping.customerMappingGridAPI != undefined || carrierAccountMapping.supplierMappingGridAPI != undefined) {
                            results[carrierAccountMapping.CarrierAccountId] = {
                                CarrierId: carrierAccountMapping.CarrierAccountId,
                                CustomerMappings: getCustomerMapping(carrierAccountMapping.customerMappingGridAPI, carrierAccountMapping.CarrierAccountId),
                                SupplierMappings: getSupplierMapping(carrierAccountMapping.supplierMappingGridAPI, carrierAccountMapping.CarrierAccountId)
                            };
                        } else {
                            results[carrierAccountMapping.CarrierAccountId] = carrierMappings != undefined ? carrierMappings[carrierAccountMapping.CarrierAccountId] : undefined;
                        }
                    }

                    function getCustomerMapping(customerMappingGridAPI, carrierAccountId) {
                        if (customerMappingGridAPI != undefined)
                            return customerMappingGridAPI.getData();

                        if (carrierMappings != undefined && carrierMappings[carrierAccountId] != undefined)
                            return carrierMappings[carrierAccountId].CustomerMappings;

                        return null;
                    }
                    function getSupplierMapping(supplierMappingGridAPI, carrierAccountId) {
                        if (supplierMappingGridAPI != undefined)
                            return supplierMappingGridAPI.getData();

                        if (carrierMappings != undefined && carrierMappings[carrierAccountId] != undefined)
                            return carrierMappings[carrierAccountId].SupplierMappings;

                        return null;
                    }

                    return results;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getCataleyaCarrierAccountMappingsGridLoadPromise() {

                var loadCataleyaCarrierAccountMappingsGridPromiseDeferred = UtilsService.createPromiseDeferred();

                var serializedFilter = {};
                WhS_BE_CarrierAccountAPIService.GetCarrierAccountInfo(serializedFilter).then(function (response) {

                    if (response != undefined) {

                        for (var i = 0; i < response.length; i++) {
                            var currentCarrierAccountInfo = response[i];

                            var carrierMapping = carrierMappings != undefined ? carrierMappings[response[i].CarrierAccountId] : undefined;
                            var carrierAccountMapping = {
                                CarrierAccountId: currentCarrierAccountInfo.CarrierAccountId,
                                CarrierAccountType: currentCarrierAccountInfo.AccountType,
                                CarrierAccountName: currentCarrierAccountInfo.Name,
                                CustomerMappings: (carrierMapping && carrierMapping.CustomerMappings) ? carrierMapping.CustomerMappings : undefined,
                                SupplierMappings: (carrierMapping && carrierMapping.SupplierMappings) ? carrierMapping.SupplierMappings : undefined
                            };
                            extendCarrierAccountMapping(carrierAccountMapping);
                            $scope.scopeModel.carrierAccountMappings.push(carrierAccountMapping);
                        }
                    }

                    loadMoreCarrierMappings();
                    loadCataleyaCarrierAccountMappingsGridPromiseDeferred.resolve();
                }).catch(function (error) {
                    loadCataleyaCarrierAccountMappingsGridPromiseDeferred.reject(error);
                });

                return loadCataleyaCarrierAccountMappingsGridPromiseDeferred.promise;
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
                    drillDownTab.directive = "whs-routesync-cataleya-customermapping";

                    drillDownTab.loadDirective = function (customerMappingGridAPI, carrierAccountMapping) {
                        carrierAccountMapping.customerMappingGridAPI = customerMappingGridAPI;
                        return carrierAccountMapping.customerMappingGridAPI.load(buildCustomerMappingPayload(carrierAccountMapping));
                    };

                    function buildCustomerMappingPayload(carrierAccountMapping) {
                        var customerMappingPayload = {};
                        //customerMappingPayload.carrierAccountId = carrierAccountMapping.CarrierAccountId;
                        customerMappingPayload.customerMappings = carrierAccountMapping.CustomerMappings;
                        customerMappingPayload.context = buildCustomerMappingDirectiveContext(carrierAccountMapping);
                        return customerMappingPayload;
                    }

                    return drillDownTab;
                }
                function buildSupplierMappingDrillDownTab() {
                    var drillDownTab = {};
                    drillDownTab.title = "Out";
                    drillDownTab.directive = "whs-routesync-cataleya-suppliermapping";

                    drillDownTab.loadDirective = function (supplierMappingGridAPI, carrierAccountMapping) {
                        carrierAccountMapping.supplierMappingGridAPI = supplierMappingGridAPI;
                        return carrierAccountMapping.supplierMappingGridAPI.load(buildSupplierMappingPayload(carrierAccountMapping));
                    };

                    function buildSupplierMappingPayload(carrierAccountMapping) {
                        var supplierMappingQuery = {};
                        //supplierMappingQuery.carrierAccountId = carrierAccountMapping.CarrierAccountId;
                        supplierMappingQuery.supplierMappings = carrierAccountMapping.SupplierMappings;
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

                if (carrierAccountMapping.CustomerMappings != undefined) {
                    var customerMappings = carrierAccountMapping.CustomerMappings;

                    var isCustomerMappingExists = customerMappings.length > 0;
                    if (isCustomerMappingExists) {
                        carrierAccountMapping.CustomerMappingDescription = buildCustomerMappingDescription(customerMappings);
                    } else {
                        carrierAccountMapping.CustomerMappingDescription = "";
                    }
                }

                if (carrierAccountMapping.SupplierMappings != undefined) {
                    var supplierMappings = carrierAccountMapping.SupplierMappings;

                    var isSupplierMappingExists = supplierMappings.length > 0;
                    if (isSupplierMappingExists) {
                        carrierAccountMapping.SupplierMappingDescription = buildSupplierMappingDescription(supplierMappings);
                    } else {
                        carrierAccountMapping.SupplierMappingDescription = "";
                    }
                }
            }

            function buildCustomerMappingDescription(customerMappings) {
                if (customerMappings == undefined || customerMappings.length == 0)
                    return "";

                var customerMappingDescription = "";
                for (var i = 0; i < customerMappings.length; i++) {
                    var currentCustomerMapping = customerMappings[i];
                    var ipAddressObj = currentCustomerMapping.IPAddress;
                    if (ipAddressObj != undefined && ipAddressObj.IPAddress != undefined && ipAddressObj.IPAddress != "") {
                        var ipAddressValue = ipAddressObj.IPAddress;

                        if (VRValidationService.validateIp(ipAddressValue) || VRValidationService.validateIpV6(ipAddressValue))
                            customerMappingDescription += ipAddressValue + "; ";
                    }
                }
                return customerMappingDescription;
            }

            function buildSupplierMappingDescription(supplierMappings) {
                if (supplierMappings == undefined || supplierMappings.length == 0)
                    return "";

                var supplierMappingDescription = "";
                for (var i = 0; i < supplierMappings.length; i++) {
                    var currentsupplierMapping = supplierMappings[i];
                    var ipAddressObj = currentsupplierMapping.IPAddress;
                    if (ipAddressObj != undefined && ipAddressObj.IPAddress != undefined && ipAddressObj.IPAddress != "") {
                        var ipAddressValue = ipAddressObj.IPAddress;

                        if (VRValidationService.validateIp(ipAddressValue) || VRValidationService.validateIpV6(ipAddressValue))
                            supplierMappingDescription += ipAddressValue + "; ";
                    }
                }
                return supplierMappingDescription;
            }

            function buildCustomerMappingDirectiveContext(carrierAccountMapping) {
                var context = {
                    updateCustomerMappingDescription: function (customerMapping) {
                        carrierAccountMapping.CustomerMappingDescription = buildCustomerMappingDescription(customerMapping);
                    },
                    updateErrorDescription: function (isValid, fromCustomerMapping) {
                        updateErrorDescription(carrierAccountMapping, isValid, fromCustomerMapping);
                    }
                };
                return context;
            }

            function buildSupplierMappingDirectiveContext(carrierAccountMapping) {
                var context = {
                    updateSupplierMappingDescription: function (supplierMapping) {
                        carrierAccountMapping.SupplierMappingDescription = buildSupplierMappingDescription(supplierMapping);
                    },
                    updateErrorDescription: function (isValid, fromCustomerMapping) {
                        updateErrorDescription(carrierAccountMapping, isValid, fromCustomerMapping);
                    }
                };
                return context;
            }

            function updateErrorDescription(carrierAccountMapping, isValid, fromCustomerMapping) {

                if (fromCustomerMapping) {
                    carrierAccountMapping.isCustomerMappingInvalid = !isValid;
                } else {
                    carrierAccountMapping.isSupplierMappingInvalid = !isValid;
                }
            }
        }
    }]);