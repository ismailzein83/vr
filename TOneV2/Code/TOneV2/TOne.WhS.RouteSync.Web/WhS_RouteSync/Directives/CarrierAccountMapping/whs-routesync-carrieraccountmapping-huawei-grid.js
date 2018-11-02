'use strict';

app.directive('whsRoutesyncCarrieraccountmappingHuaweiGrid', ['VRNotificationService', 'VRUIUtilsService', 'UtilsService', 'WhS_BE_CarrierAccountAPIService', 'WhS_BE_CarrierAccountTypeEnum',
    function (VRNotificationService, VRUIUtilsService, UtilsService, WhS_BE_CarrierAccountAPIService, WhS_BE_CarrierAccountTypeEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new HuaweiCarrierAccountMappingGridCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_RouteSync/Directives/CarrierAccountMapping/Templates/CarrierAccountMappingHuaweiGridTemplate.html'
        };

        function HuaweiCarrierAccountMappingGridCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var carrierMappings;
            var context;

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

                    if (payload != undefined) {
                        carrierMappings = payload.carrierMappings;
                        context = payload.context;
                    }

                    var promises = [];

                    var huaweiCarrierAccountMappingsGridLoadPromise = getHuaweiCarrierAccountMappingsGridLoadPromise();
                    promises.push(huaweiCarrierAccountMappingsGridLoadPromise);

                    return UtilsService.waitMultiplePromises(promises);
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
                                CustomerMapping: getCustomerMapping(carrierAccountMapping.customerMappingGridAPI, carrierAccountMapping.CarrierAccountId),
                                SupplierMapping: getSupplierMapping(carrierAccountMapping.supplierMappingGridAPI, carrierAccountMapping.CarrierAccountId)
                            };
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

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getHuaweiCarrierAccountMappingsGridLoadPromise() {

                var loadHuaweiCarrierAccountMappingsGridPromiseDeferred = UtilsService.createPromiseDeferred();

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
                                CustomerMapping: (carrierMapping && carrierMapping.CustomerMapping) ? carrierMapping.CustomerMapping : undefined,
                                SupplierMapping: (carrierMapping && carrierMapping.SupplierMapping) ? carrierMapping.SupplierMapping : undefined
                            };
                            extendCarrierAccountMapping(carrierAccountMapping);
                            $scope.scopeModel.carrierAccountMappings.push(carrierAccountMapping);
                        }
                    }

                    loadMoreCarrierMappings();
                    loadHuaweiCarrierAccountMappingsGridPromiseDeferred.resolve();
                }).catch(function (error) {
                    loadHuaweiCarrierAccountMappingsGridPromiseDeferred.reject(error);
                });

                return loadHuaweiCarrierAccountMappingsGridPromiseDeferred.promise;
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
                    drillDownTab.directive = "whs-routesync-huawei-customermapping";

                    drillDownTab.loadDirective = function (customerMappingGridAPI, carrierAccountMapping) {
                        carrierAccountMapping.customerMappingGridAPI = customerMappingGridAPI;
                        return carrierAccountMapping.customerMappingGridAPI.load(buildCustomerMappingPayload(carrierAccountMapping));
                    };

                    function buildCustomerMappingPayload(carrierAccountMapping) {
                        var customerMappingPayload = {};
                        //customerMappingPayload.carrierAccountId = carrierAccountMapping.CarrierAccountId;
                        customerMappingPayload.customerMapping = carrierAccountMapping.CustomerMapping;
                        customerMappingPayload.context = buildCustomerMappingDirectiveContext(carrierAccountMapping);
                        return customerMappingPayload;
                    }

                    return drillDownTab;
                }
                function buildSupplierMappingDrillDownTab() {
                    var drillDownTab = {};
                    drillDownTab.title = "Out";
                    drillDownTab.directive = "whs-routesync-huawei-suppliermapping";

                    drillDownTab.loadDirective = function (supplierMappingGridAPI, carrierAccountMapping) {
                        carrierAccountMapping.supplierMappingGridAPI = supplierMappingGridAPI;
                        return carrierAccountMapping.supplierMappingGridAPI.load(buildSupplierMappingQuery(carrierAccountMapping));
                    };

                    function buildSupplierMappingQuery(carrierAccountMapping) {
                        var supplierMappingQuery = {};
                        //supplierMappingQuery.carrierAccountId = carrierAccountMapping.CarrierAccountId;
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

                    var isCustomerMappingExists = customerMapping != undefined && customerMapping.RSSN != undefined && customerMapping.CSC != "" && customerMapping.DNSet != "";
                    if (isCustomerMappingExists) {
                        carrierAccountMapping.CustomerMappingDescription = buildCustomerMappingDescription(customerMapping);
                    } else {
                        carrierAccountMapping.CustomerMappingDescription = "";
                    }
                }

                if (carrierAccountMapping.SupplierMapping != undefined) {
                    var supplierMapping = carrierAccountMapping.SupplierMapping;

                    var isSupplierMappingExists = supplierMapping != undefined && supplierMapping.RouteName != undefined && supplierMapping.ISUP != "";
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

                var rssnDescription = customerMapping.RSSN != undefined ? customerMapping.RSSN : "";
                var cscName = customerMapping.CSCName != undefined ? customerMapping.CSCName : "";
                var dnSetDescription = customerMapping.DNSet != undefined ? customerMapping.DNSet : "";

                return "RSSN: " + rssnDescription + "; CSC Name: '" + cscName + "'; DN Set: " + dnSetDescription + ".";
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

                var routeName = supplierMapping.RouteName != undefined ? supplierMapping.RouteName : "";
                var isup = supplierMapping.ISUP != undefined ? supplierMapping.ISUP : "";

                return "Route Name: '" + routeName + "'; ISUP: '" + isup + "'.";
            }
            function buildSupplierMappingDirectiveContext(carrierAccountMapping) {
                var suppliercontext = {
                    updateErrorDescription: function (isValid, fromCustomerMapping) {
                        updateErrorDescription(carrierAccountMapping, isValid, fromCustomerMapping);
                    },
                    updateSupplierMappingDescription: function (supplierMapping) {
                        carrierAccountMapping.SupplierMappingDescription = buildSupplierMappingDescription(supplierMapping);
                    },
                    isRouteNameValidated: function (routeName) {
                        if (routeName == undefined || context == undefined)
                            return null;

                        var minRNLength = context.getMinRNLength();

                        if (minRNLength == undefined)
                            return null;

                        var routeNameWithoutEmptySpace = UtilsService.replaceAll(routeName, " ", "");
                        if (routeNameWithoutEmptySpace.length >= minRNLength)
                            return null;

                        return 'Length should be greater than ' + minRNLength;
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
    }]);