'use strict';

app.directive('whsRoutesyncCarrieraccountmappingCataleyaGrid', ['VRUIUtilsService', 'UtilsService', 'WhS_BE_CarrierAccountAPIService', 'WhS_BE_CarrierAccountTypeEnum', 'WhS_BE_CarrierAccountActivationStatusEnum',
    function (VRUIUtilsService, UtilsService, WhS_BE_CarrierAccountAPIService, WhS_BE_CarrierAccountTypeEnum, WhS_BE_CarrierAccountActivationStatusEnum) {
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

                    if (payload != undefined) {
                        carrierMappings = payload.carrierMappings;
                    }

                    var promises = [];

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

                        if (carrierAccountMapping.carrierMappingDirectiveAPI != undefined) {
                            results[carrierAccountMapping.CarrierAccountId] = carrierAccountMapping.carrierMappingDirectiveAPI.getData();
                        } else {
                            results[carrierAccountMapping.CarrierAccountId] = carrierMappings != undefined ? carrierMappings[carrierAccountMapping.CarrierAccountId] : undefined;
                        }
                    }

                    return results;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getCataleyaCarrierAccountMappingsGridLoadPromise() {

                var loadCataleyaCarrierAccountMappingsGridPromiseDeferred = UtilsService.createPromiseDeferred();

                var carrierAccountfilter = {
                    ActivationStatuses: [WhS_BE_CarrierAccountActivationStatusEnum.Active.value, WhS_BE_CarrierAccountActivationStatusEnum.Testing.value]
                };

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
                                ZoneID: carrierMapping != undefined ? carrierMapping.ZoneID : undefined,
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
                    items.push(currentCarrierAccountMapping);
                }
                gridAPI.addItemsToSource(items);
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

                carrierAccountMapping.ZoneIDDescription = carrierAccountMapping.ZoneID;

                if (carrierAccountMapping.CustomerMappings != undefined) {
                    var customerMappings = carrierAccountMapping.CustomerMappings;

                    var isCustomerMappingExists = customerMappings.InTrunks != undefined ? customerMappings.InTrunks.length > 0 : false;
                    if (isCustomerMappingExists) {
                        carrierAccountMapping.CustomerMappingDescription = buildCustomerMappingDescription(customerMappings);
                    } else {
                        carrierAccountMapping.CustomerMappingDescription = "";
                    }
                }

                if (carrierAccountMapping.SupplierMappings != undefined) {
                    var supplierMappings = carrierAccountMapping.SupplierMappings;

                    var isSupplierMappingExists = supplierMappings.OutTrunks != undefined ? supplierMappings.OutTrunks.length > 0 : false;
                    if (isSupplierMappingExists) {
                        carrierAccountMapping.SupplierMappingDescription = buildSupplierMappingDescription(supplierMappings);
                    } else {
                        carrierAccountMapping.SupplierMappingDescription = "";
                    }
                }

                carrierAccountMapping.onCarrierMappingDirectiveReady = function (api) {
                    carrierAccountMapping.carrierMappingDirectiveAPI = api;

                    var carrierMappingDirectivePayload = {
                        carrierAccountMapping: carrierAccountMapping,
                        context: buildContext(carrierAccountMapping)
                    };
                    VRUIUtilsService.callDirectiveLoad(carrierAccountMapping.carrierMappingDirectiveAPI, carrierMappingDirectivePayload);
                };
            }

            function buildCustomerMappingDescription(customerMappings) {
                if (customerMappings == undefined || customerMappings.InTrunks == undefined || customerMappings.InTrunks.length == 0)
                    return "";

                var customerMappingDescription = [];
                for (var i = 0; i < customerMappings.InTrunks.length; i++) {
                    var inTrunk = customerMappings.InTrunks[i];
                    if (inTrunk != undefined && inTrunk.Trunk != undefined) {
                        var customerDescription = inTrunk.Trunk;
                        customerMappingDescription.push(customerDescription);
                    }
                }
                return 'Trunks: ' + customerMappingDescription.join("; ");
            }

            function buildSupplierMappingDescription(supplierMappings) {
                if (supplierMappings == undefined || supplierMappings.OutTrunks == undefined || supplierMappings.OutTrunks.length == 0)
                    return "";

                var supplierMappingDescription = [];
                for (var i = 0; i < supplierMappings.OutTrunks.length; i++) {
                    var outTrunk = supplierMappings.OutTrunks[i];
                    if (outTrunk != undefined && outTrunk.Trunk != undefined) {
                        var supplierDescription = outTrunk.Trunk;

                        if (outTrunk.Percentage != undefined) {
                            supplierDescription = supplierDescription + ' (' + outTrunk.Percentage + '%)';
                        }

                        supplierMappingDescription.push(supplierDescription);
                    }
                }
                return 'Trunks: ' + supplierMappingDescription.join("; ");
            }

            function buildContext(carrierAccountMapping) {
                var context = {
                    updateZoneIDDescription: function (zoneID) {
                        carrierAccountMapping.ZoneIDDescription = zoneID;
                    },
                    updateCustomerMappingDescription: function (customerMapping) {
                        carrierAccountMapping.CustomerMappingDescription = buildCustomerMappingDescription(customerMapping);
                    },
                    updateSupplierMappingDescription: function (supplierMapping) {
                        carrierAccountMapping.SupplierMappingDescription = buildSupplierMappingDescription(supplierMapping);
                    },
                    updateErrorDescription: function (isValid) {
                        updateErrorDescription(carrierAccountMapping, isValid);
                    }
                };
                return context;
            }

            function updateErrorDescription(carrierAccountMapping, isValid) {
                carrierAccountMapping.isCarrierAccountMappingInvalid = !isValid;
            }
        }
    }]);