'use strict';
app.directive('vrWhsBeCustomergroup', ['UtilsService', '$compile', 'WhS_BE_SaleZoneAPIService', 'WhS_BE_CarrierAccountAPIService', 'VRNotificationService', 'VRUIUtilsService',
function (UtilsService, $compile, WhS_BE_SaleZoneAPIService, WhS_BE_CarrierAccountAPIService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new beCustomerGroup(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/CustomerGroup/Templates/CustomerGroupTemplate.html"

    };


    function beCustomerGroup(ctrl, $scope, $attrs) {

        var customerSelectorAPI;
        var customerGroupDirectiveAPI;
        var customerGroupDirectiveReadyPromiseDeferred;
        var linkedCustomerId;

        function initializeController() {
            $scope.customerGroupTemplates = [];

            $scope.onCustomerSelectorReady = function (api) {
                customerSelectorAPI = api;
                defineAPI();
            };

            $scope.onCustomerGroupDirectiveReady = function (api) {
                customerGroupDirectiveAPI = api;
                var setLoader = function (value) { $scope.isLoadingCustomerGroupDirective = value; };
                var customerGroupDirectivePayload = { linkedCustomerId: linkedCustomerId };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, customerGroupDirectiveAPI, customerGroupDirectivePayload, setLoader, customerGroupDirectiveReadyPromiseDeferred);

            };
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {

                var customerGroupSettings;
                if ($scope.selectedCustomerGroupTemplate != undefined) {
                    if (customerGroupDirectiveAPI != undefined) {
                        customerGroupSettings = customerGroupDirectiveAPI.getData();
                        customerGroupSettings.ConfigId = $scope.selectedCustomerGroupTemplate.ExtensionConfigurationId;
                    }
                }
                return customerGroupSettings;
            };

            api.load = function (payload) {
                customerSelectorAPI.clearDataSource();

                var customerConfigId;
                var customerGroupSettings;

                if (payload != undefined) {
                    $scope.disableCriteria = payload.disableCriteria;
                    linkedCustomerId = payload.linkedCustomerId;

                    customerGroupSettings = payload.customerGroupSettings;
                    if (customerGroupSettings != undefined) {
                        customerConfigId = customerGroupSettings.ConfigId;
                    }
                }

                var promises = [];
                var loadCustomerGroupTemplatesPromise = WhS_BE_CarrierAccountAPIService.GetCustomerGroupTemplates().then(function (response) {
                    angular.forEach(response, function (item) {
                        $scope.customerGroupTemplates.push(item);
                    });
                    if (customerConfigId != undefined)
                        $scope.selectedCustomerGroupTemplate = UtilsService.getItemByVal($scope.customerGroupTemplates, customerConfigId, "ExtensionConfigurationId");

                });
                promises.push(loadCustomerGroupTemplatesPromise);
                if (customerGroupSettings != undefined) {
                    var customerGroupDirectivePayload = { linkedCustomerId: linkedCustomerId, customerGroupSettings: customerGroupSettings };
                    customerGroupDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                    var customerGroupDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(customerGroupDirectiveLoadPromiseDeferred.promise);

                    customerGroupDirectiveReadyPromiseDeferred.promise.then(function () {
                        customerGroupDirectiveReadyPromiseDeferred = undefined;
                        VRUIUtilsService.callDirectiveLoad(customerGroupDirectiveAPI, customerGroupDirectivePayload, customerGroupDirectiveLoadPromiseDeferred);
                    });
                }
                return UtilsService.waitMultiplePromises(promises);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);