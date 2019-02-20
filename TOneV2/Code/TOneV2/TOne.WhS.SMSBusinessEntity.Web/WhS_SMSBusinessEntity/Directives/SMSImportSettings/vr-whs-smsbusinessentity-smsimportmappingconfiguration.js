'use strict';

app.directive('vrWhsSmsbusinessentitySmsimportmappingconfiguration', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new smsImportEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_SMSBusinessEntity/Directives/SMSImportSettings/Templates/SMSImportMappingConfigurationTemplate.html"
        };

        function smsImportEditorCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var receiverSelectorAPI;
            var receiverReadyDeferred = UtilsService.createPromiseDeferred();

            var customerReceiverSelectorAPI;
            var customerReceiverReadyDeferred = UtilsService.createPromiseDeferred();

            var supplierReceiverSelectorAPI;
            var supplierReceiverReadyDeferred = UtilsService.createPromiseDeferred();

            var mobileNetworkReceiverSelectorAPI;
            var mobileNetworkReceiverReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onReceiverSelectorReady = function (api) {
                    receiverSelectorAPI = api;
                    receiverReadyDeferred.resolve();
                };
                $scope.scopeModel.onCustomerReceiverSelectorReady = function (api) {
                    customerReceiverSelectorAPI = api;
                    customerReceiverReadyDeferred.resolve();
                };
                $scope.scopeModel.onSupplierReceiverSelectorReady = function (api) {
                    supplierReceiverSelectorAPI = api;
                    supplierReceiverReadyDeferred.resolve();
                };
                $scope.scopeModel.onMobileNetworkReceiverSelectorReady = function (api) {
                    mobileNetworkReceiverSelectorAPI = api;
                    mobileNetworkReceiverReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([customerReceiverReadyDeferred.promise, supplierReceiverReadyDeferred.promise, mobileNetworkReceiverReadyDeferred.promise, receiverReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var generalIdentification;
                    var customerIdentification;
                    var supplierIdentification;
                    var mobileNetworkIdentification;

                    if (payload != undefined && payload.smsImportMappingConfiguration != undefined) {
                        generalIdentification = payload.smsImportMappingConfiguration.GeneralIdentification;
                        customerIdentification = payload.smsImportMappingConfiguration.CustomerIdentification;
                        supplierIdentification = payload.smsImportMappingConfiguration.SupplierIdentification;
                        mobileNetworkIdentification = payload.smsImportMappingConfiguration.MobileNetworkIdentification;
                    }

                    var promises = [];

                    //Loading Receiver Selector
                    var receiverSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    var receiverSelectorPayload = {
                        selectedIds: generalIdentification
                    };
                    VRUIUtilsService.callDirectiveLoad(receiverSelectorAPI, receiverSelectorPayload, receiverSelectorLoadDeferred);
                    promises.push(receiverSelectorLoadDeferred.promise);

                    //Loading CustomerReceiver Selector
                    var customerReceiverSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    var customerReceiverSelectorPayload = {
                        selectedIds: customerIdentification
                    };
                    VRUIUtilsService.callDirectiveLoad(customerReceiverSelectorAPI, customerReceiverSelectorPayload, customerReceiverSelectorLoadDeferred);
                    promises.push(customerReceiverSelectorLoadDeferred.promise);

                    //Loading SupplierReceiver Selector
                    var supplierReceiverSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    var supplierReceiverSelectorPayload = {
                        selectedIds: supplierIdentification
                    };
                    VRUIUtilsService.callDirectiveLoad(supplierReceiverSelectorAPI, supplierReceiverSelectorPayload, supplierReceiverSelectorLoadDeferred);
                    promises.push(supplierReceiverSelectorLoadDeferred.promise);

                    //Loading MobileNetworkReceiver Selector
                    var mobileNetworkReceiverSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    var mobileNetworkReceiverSelectorPayload = {
                        selectedIds: mobileNetworkIdentification
                    };
                    VRUIUtilsService.callDirectiveLoad(mobileNetworkReceiverSelectorAPI, mobileNetworkReceiverSelectorPayload, mobileNetworkReceiverSelectorLoadDeferred);
                    promises.push(mobileNetworkReceiverSelectorLoadDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        GeneralIdentification: receiverSelectorAPI.getSelectedIds(),
                        CustomerIdentification: customerReceiverSelectorAPI.getSelectedIds(),
                        SupplierIdentification: supplierReceiverSelectorAPI.getSelectedIds(),
                        MobileNetworkIdentification: mobileNetworkReceiverSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);