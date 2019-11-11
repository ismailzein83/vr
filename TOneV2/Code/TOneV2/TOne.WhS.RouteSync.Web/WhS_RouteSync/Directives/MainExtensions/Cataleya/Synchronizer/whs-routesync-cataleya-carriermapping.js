(function (app) {

    'use strict';

    CataleyaCarrierMappingDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'WhS_BE_CarrierAccountTypeEnum'];

    function CataleyaCarrierMappingDirective(UtilsService, VRUIUtilsService, WhS_BE_CarrierAccountTypeEnum) {

        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CataleyaCarrierMappingDirectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_RouteSync/Directives/MainExtensions/Cataleya/Synchronizer/Templates/CataleyaCarrierMappingTemplate.html'
        };

        function CataleyaCarrierMappingDirectiveCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var carrierAccountMapping;
            var context;

            var customerMappingDirectiveAPI;
            var customerMappingDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var supplierMappingDirectiveAPI;
            var supplierMappingDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onZoneIDValueChanged = function () {
                    updateZoneIDDescription();
                };

                $scope.scopeModel.onCustomerMappingDirectiveReady = function (api) {
                    customerMappingDirectiveAPI = api;
                    customerMappingDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onSupplierMappingDirectiveReady = function (api) {
                    supplierMappingDirectiveAPI = api;
                    supplierMappingDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var customerMappings;
                    var supplierMappings;

                    if (payload != undefined) {
                        carrierAccountMapping = payload.carrierAccountMapping;
                        context = payload.context;
                    }

                    if (carrierAccountMapping != undefined) {
                        $scope.scopeModel.zoneID = carrierAccountMapping.ZoneID;
                        updateZoneIDDescription();

                        if (showCustomerMapping(carrierAccountMapping)) {
                            $scope.scopeModel.showCustomerMapping = true;
                            customerMappings = carrierAccountMapping.CustomerMappings;

                            var loadCustomerMappingDirectivePromise = getLoadCustomerMappingDirectivePromise();
                            promises.push(loadCustomerMappingDirectivePromise);
                        }

                        if (showSupplierMapping(carrierAccountMapping)) {
                            $scope.scopeModel.showSupplierMapping = true;
                            supplierMappings = carrierAccountMapping.SupplierMappings;

                            var loadSupplierMappingDirectivePromise = getLoadSupplierMappingDirectivePromise();
                            promises.push(loadSupplierMappingDirectivePromise);
                        }
                    }

                    function getLoadCustomerMappingDirectivePromise() {
                        var loadCustomerMappingDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                        customerMappingDirectiveReadyDeferred.promise.then(function () {

                            var customerMappingDirectivePayload = {
                                customerMappings: customerMappings,
                                context: buildCustomerMappingContext()
                            };
                            VRUIUtilsService.callDirectiveLoad(customerMappingDirectiveAPI, customerMappingDirectivePayload, loadCustomerMappingDirectivePromiseDeferred);
                        });

                        return loadCustomerMappingDirectivePromiseDeferred.promise;
                    }

                    function getLoadSupplierMappingDirectivePromise() {
                        var loadSupplierMappingDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                        supplierMappingDirectiveReadyDeferred.promise.then(function () {

                            var supplierMappingDirectivePayload = {
                                supplierMappings: supplierMappings,
                                context: buildSupplierMappingContext()
                            };
                            VRUIUtilsService.callDirectiveLoad(supplierMappingDirectiveAPI, supplierMappingDirectivePayload, loadSupplierMappingDirectivePromiseDeferred);
                        });

                        return loadSupplierMappingDirectivePromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        UtilsService.watchFunction($scope, 'validationContext.validate()', updateErrorDescription);
                    });
                };

                api.getData = function () {
                    if (carrierAccountMapping == undefined || $scope.scopeModel.zoneID == undefined)
                        return undefined;

                    var data = {
                        CarrierId: carrierAccountMapping.CarrierAccountId,
                        ZoneID: $scope.scopeModel.zoneID,
                        CustomerMappings: getCustomerMapping(customerMappingDirectiveAPI, carrierAccountMapping),
                        SupplierMappings: getSupplierMapping(supplierMappingDirectiveAPI, carrierAccountMapping)
                    };

                    function getCustomerMapping(customerMappingDirectiveAPI, carrierAccountMapping) {
                        if (customerMappingDirectiveAPI != undefined)
                            return customerMappingDirectiveAPI.getData();

                        if (carrierAccountMapping != undefined)
                            return carrierAccountMapping.CustomerMappings;

                        return null;
                    }
                    function getSupplierMapping(supplierMappingDirectiveAPI, carrierAccountMapping) {
                        if (supplierMappingDirectiveAPI != undefined)
                            return supplierMappingDirectiveAPI.getData();

                        if (carrierAccountMapping != undefined)
                            return carrierAccountMapping.SupplierMappings;

                        return null;
                    }

                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function showCustomerMapping(carrierAccountMapping) {
                if (WhS_BE_CarrierAccountTypeEnum.Exchange.value == carrierAccountMapping.CarrierAccountType || WhS_BE_CarrierAccountTypeEnum.Customer.value == carrierAccountMapping.CarrierAccountType)
                    return true;
                return false;
            }

            function showSupplierMapping(carrierAccountMapping) {
                if (WhS_BE_CarrierAccountTypeEnum.Exchange.value == carrierAccountMapping.CarrierAccountType || WhS_BE_CarrierAccountTypeEnum.Supplier.value == carrierAccountMapping.CarrierAccountType)
                    return true;
                return false;
            }

            function buildCustomerMappingContext() {
                return {
                    updateCustomerMappingDescription: context.updateCustomerMappingDescription,
                    updateIsCustomerMappingExists: function (isCustomerMappingExists) {
                        $scope.scopeModel.isCustomerMappingExists = isCustomerMappingExists;
                    }
                };
            }

            function buildSupplierMappingContext() {
                return {
                    updateSupplierMappingDescription: context.updateSupplierMappingDescription,
                    updateIsSupplierMappingExists: function (isSupplierMappingExists) {
                        $scope.scopeModel.isSupplierMappingExists = isSupplierMappingExists;
                    }
                };
            }

            function updateZoneIDDescription() {
                if (context == undefined)
                    return;

                context.updateZoneIDDescription($scope.scopeModel.zoneID);
            }

            function updateErrorDescription() {
                if (context == undefined)
                    return;

                var validatationMessage = $scope.validationContext.validate();
                var isValid = validatationMessage == null;
                context.updateErrorDescription(isValid);
            }
        }
    }

    app.directive('whsRoutesyncCataleyaCarriermapping', CataleyaCarrierMappingDirective);
})(app);