'use strict';

app.directive('whsRoutesyncCataleyaCustomermapping', ['VRUIUtilsService', 'UtilsService',
    function (VRUIUtilsService, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CataleyaCustomerMappingDirectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_RouteSync/Directives/MainExtensions/CataleyaSynchronizer/Templates/CataleyaCustomerMappingTemplate.html'
        };

        function CataleyaCustomerMappingDirectiveCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var context;
            var isFirstLoad = true;

            var customerMappingGridAPI;
            var customerMappingGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.customerMappings = [];
                //$scope.scopeModel.customerMappingExists = false;

                $scope.scopeModel.onCustomerMappingGridReady = function (api) {
                    customerMappingGridAPI = api;
                    customerMappingGridReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.addCustomerMapping = function () {
                    extendCustomerMapping();
                    updateCustomerDescriptions();
                };

                $scope.scopeModel.onCustomerMappingDeleted = function (item) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.customerMappings, item.tempId, 'tempId');
                    $scope.scopeModel.customerMappings.splice(index, 1);
                    updateCustomerDescriptions();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    isFirstLoad = true;
                    var promises = [];

                    var customerMappings;

                    if (payload != undefined) {
                        context = payload.context;

                        customerMappings = payload.customerMappings;
                        if (customerMappings != undefined && customerMappings.length > 0) {

                            for (var i = 0; i < customerMappings.length; i++) {
                                var currentCustomerMapping = customerMappings[i];
                                promises.push(extendCustomerMapping(currentCustomerMapping));
                            }
                            updateCustomerDescriptions();
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        isFirstLoad = false;
                    });
                };

                api.getData = function () {
                    return getCustomerMappings();
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function extendCustomerMapping(customerMapping) {

                if (customerMapping == undefined)
                    customerMapping = {};

                customerMapping.tempId = UtilsService.guid();

                customerMapping.ipAddressLoadDeferred = UtilsService.createPromiseDeferred();

                customerMapping.onIPAddressReady = function (api) {
                    customerMapping.ipAddressDirectiveAPI = api;
                    var defaultIPAddress = { SubnetPrefixLength: 32 };
                    var ipAddressPayload = (customerMapping != undefined && customerMapping.IPAddress != undefined) ? customerMapping.IPAddress : defaultIPAddress;
                    VRUIUtilsService.callDirectiveLoad(customerMapping.ipAddressDirectiveAPI, ipAddressPayload, customerMapping.ipAddressLoadDeferred);
                };

                customerMapping.onFieldBlur = function (field) {
                    updateCustomerDescriptions();
                };

                $scope.scopeModel.customerMappings.push(customerMapping);

                return customerMapping.ipAddressLoadDeferred.promise;
            }

            function updateCustomerDescriptions() {
                setTimeout(function () {
                    $scope.$apply(function () {
                        updateErrorDescription();
                        updateCustomerMappingDescription();
                    });
                }, 0);
            }

            function updateCustomerMappingDescription() {
                if (isFirstLoad || context == undefined)
                    return;

                context.updateCustomerMappingDescription(getCustomerMappings());
            }

            function updateErrorDescription() {
                if (isFirstLoad || context == undefined)
                    return;

                var validatationMessage = $scope.validationContext.validate();
                var isValid = validatationMessage == null;
                context.updateErrorDescription(isValid, true);
            }

            function getCustomerMappings() {

                var customerMappings = [];

                for (var i = 0; i < $scope.scopeModel.customerMappings.length; i++) {
                    var currentCustomerMapping = $scope.scopeModel.customerMappings[i];
                    var customerMappingObject = getCustomerMappingObject(currentCustomerMapping);
                    if (customerMappingObject != undefined)
                        customerMappings.push(customerMappingObject);
                }

                return customerMappings;
            }

            function getCustomerMappingObject(customerMapping) {

                if (customerMapping == undefined)
                    return undefined;

                return {
                    IPAddress: customerMapping.ipAddressDirectiveAPI != undefined ? customerMapping.ipAddressDirectiveAPI.getData() : customerMapping.IPAddress
                };
            }
        }
    }]);