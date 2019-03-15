'use strict';

app.directive('whsRoutesyncCataleyaSuppliermapping', ['VRNotificationService', 'VRUIUtilsService', 'UtilsService',
    function (VRNotificationService, VRUIUtilsService, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CataleyaSupplierMappingDirectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_RouteSync/Directives/MainExtensions/CataleyaSynchronizer/Templates/CataleyaSupplierMappingTemplate.html'
        };

        function CataleyaSupplierMappingDirectiveCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var context;
            var isFirstLoad = true;

            var supplierMappingGridAPI;
            var supplierMappingGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.supplierMappings = [];
                //$scope.scopeModel.supplierMappingExists = false;

                $scope.scopeModel.onSupplierMappingGridReady = function (api) {
                    supplierMappingGridAPI = api;
                    supplierMappingGridReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.addSupplierMapping = function () {
                    extendSupplierMapping();
                    updateSupplierDescriptions();
                };

                $scope.scopeModel.onSupplierMappingDeleted = function (item) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.supplierMappings, item.tempId, 'tempId');
                    $scope.scopeModel.supplierMappings.splice(index, 1);
                    updateSupplierDescriptions();
                };

                $scope.scopeModel.onFieldBlur = function (field) {
                    updateSupplierDescriptions();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    isFirstLoad = true;

                    var promises = [];

                    var supplierMappings;

                    if (payload != undefined) {
                        context = payload.context;

                        supplierMappings = payload.supplierMappings;
                        if (supplierMappings != undefined && supplierMappings.length > 0) {

                            for (var i = 0; i < supplierMappings.length; i++) {
                                var currentSupplierMapping = supplierMappings[i];
                                promises.push(extendSupplierMapping(currentSupplierMapping));
                            }
                            updateSupplierDescriptions();
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        isFirstLoad = false;
                    });
                };

                api.getData = function () {
                    return getSupplierMappings();
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function extendSupplierMapping(supplierMapping) {
                var extendSupplierMappingPromises = [];

                if (supplierMapping == undefined)
                    supplierMapping = {};

                supplierMapping.tempId = UtilsService.guid();

                supplierMapping.ipAddressLoadDeferred = UtilsService.createPromiseDeferred();
                extendSupplierMappingPromises.push(supplierMapping.ipAddressLoadDeferred.promise);

                supplierMapping.onIPAddressReady = function (api) {
                    supplierMapping.ipAddressDirectiveAPI = api;
                    var defaultIPAddress = { SubnetPrefixLength: 32 };
                    var ipAddressPayload = supplierMapping != undefined && supplierMapping.IPAddress != undefined ? supplierMapping.IPAddress : defaultIPAddress;
                    VRUIUtilsService.callDirectiveLoad(supplierMapping.ipAddressDirectiveAPI, ipAddressPayload, supplierMapping.ipAddressLoadDeferred);
                };

                supplierMapping.transportProtocolLoadDeferred = UtilsService.createPromiseDeferred();
                extendSupplierMappingPromises.push(supplierMapping.transportProtocolLoadDeferred.promise);

                supplierMapping.onTransportProtocolReady = function (api) {
                    supplierMapping.transportProtocolSelectorAPI = api;
                    var transportProtocolPayload = {
                        selectedIds: supplierMapping != undefined ? supplierMapping.TransportProtocol : undefined,
                        setDefaultValue: true
                    };
                    VRUIUtilsService.callDirectiveLoad(supplierMapping.transportProtocolSelectorAPI, transportProtocolPayload, supplierMapping.transportProtocolLoadDeferred);
                };

                supplierMapping.onFieldBlur = function (field) {
                    updateSupplierDescriptions();
                };

                $scope.scopeModel.supplierMappings.push(supplierMapping);

                return UtilsService.waitMultiplePromises(extendSupplierMappingPromises);
            }

            function updateSupplierDescriptions() {
                setTimeout(function () {
                    $scope.$apply(function () {
                        updateErrorDescription();
                        updateSupplierMappingDescription();
                    });
                }, 0);
            }

            function updateSupplierMappingDescription() {
                if (isFirstLoad || context == undefined)
                    return;

                context.updateSupplierMappingDescription(getSupplierMappings());
            }

            function updateErrorDescription() {
                if (isFirstLoad || context == undefined)
                    return;

                var validatationMessage = $scope.validationContext.validate();
                var isValid = validatationMessage == null;
                context.updateErrorDescription(isValid, false);
            }

            function getSupplierMappings() {

                var supplierMappings = [];

                for (var i = 0; i < $scope.scopeModel.supplierMappings.length; i++) {
                    var currentSupplierMapping = $scope.scopeModel.supplierMappings[i];
                    supplierMappings.push(getSupplierMappingObject(currentSupplierMapping));
                }

                return supplierMappings;
            }

            function getSupplierMappingObject(supplierMapping) {
                if (supplierMapping == undefined)
                    return undefined;

                var supplierMappingObject = {
                    IPAddress: supplierMapping.ipAddressDirectiveAPI != undefined ? supplierMapping.ipAddressDirectiveAPI.getData() : supplierMapping.IPAddress,
                    Domain: supplierMapping.Domain,
                    Priority: supplierMapping.Priority,
                    Weight: supplierMapping.Weight,
                    TransportProtocol: supplierMapping.transportProtocolSelectorAPI != undefined ? supplierMapping.transportProtocolSelectorAPI.getSelectedIds() : supplierMapping.TransportProtocol,
                    NationalCountryCode: supplierMapping.NationalCountryCode,
                    ReuseConnection: supplierMapping.ReuseConnection,
                    IsSwitch: supplierMapping.IsSwitch
                };

                return supplierMappingObject;
            }
        }
    }]);