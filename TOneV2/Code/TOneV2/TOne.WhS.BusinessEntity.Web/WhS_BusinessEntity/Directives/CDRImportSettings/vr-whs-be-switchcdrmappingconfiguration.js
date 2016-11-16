'use strict';

app.directive('vrWhsBeSwitchcdrmappingconfiguration', ['UtilsService', 'VRUIUtilsService',
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

                var ctor = new cdrImportEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/CDRImportSettings/Templates/SwitchCDRMappingConfigurationTemplate.html"
        };

        function cdrImportEditorCtor(ctrl, $scope, $attrs) {

            var cdpnSelectorAPI;
            var cdpnReadyDeferred = UtilsService.createPromiseDeferred();

            var customerCDPNSelectorAPI;
            var customerCDPNReadyDeferred = UtilsService.createPromiseDeferred();

            var supplierCDPNSelectorAPI;
            var supplierCDPNReadyDeferred = UtilsService.createPromiseDeferred();

            var saleZoneCDPNSelectorAPI;
            var saleZoneCDPNReadyDeferred = UtilsService.createPromiseDeferred();

            var supplierZoneCDPNSelectorAPI;
            var supplierZoneCDPNReadyDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onCDPNSelectorReady = function (api) {
                    cdpnSelectorAPI = api;
                    cdpnReadyDeferred.resolve();
                };
                $scope.scopeModel.onCustomerCDPNSelectorReady = function (api) {
                    customerCDPNSelectorAPI = api;
                    customerCDPNReadyDeferred.resolve();
                };
                $scope.scopeModel.onSupplierCDPNSelectorReady = function (api) {
                    supplierCDPNSelectorAPI = api;
                    supplierCDPNReadyDeferred.resolve();
                };
                $scope.scopeModel.onSaleZoneCDPNSelectorReady = function (api) {
                    saleZoneCDPNSelectorAPI = api;
                    saleZoneCDPNReadyDeferred.resolve();
                };
                $scope.scopeModel.onSupplierZoneCDPNSelectorReady = function (api) {
                    supplierZoneCDPNSelectorAPI = api;
                    supplierZoneCDPNReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([customerCDPNReadyDeferred.promise, supplierCDPNReadyDeferred.promise, saleZoneCDPNReadyDeferred.promise,
                    supplierZoneCDPNReadyDeferred.promise, cdpnReadyDeferred.promise]).then(function () {
                        defineAPI();
                    });
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var generalIdentification;
                    var customerIdentification;
                    var supplierIdentification;
                    var saleZoneIdentification;
                    var supplierZoneIdentification;

                    if (payload != undefined && payload.switchCDRMappingConfiguration != undefined) {
                        generalIdentification = payload.switchCDRMappingConfiguration.GeneralIdentification;
                        customerIdentification = payload.switchCDRMappingConfiguration.CustomerIdentification;
                        supplierIdentification = payload.switchCDRMappingConfiguration.SupplierIdentification;
                        saleZoneIdentification = payload.switchCDRMappingConfiguration.SaleZoneIdentification;
                        supplierZoneIdentification = payload.switchCDRMappingConfiguration.SupplierZoneIdentification;
                    }

                    var promises = [];

                    //Loading CDPN Selector
                    var cdpnSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    var cdpnSelectorPayload = {
                        selectedIds: generalIdentification
                    };
                    VRUIUtilsService.callDirectiveLoad(cdpnSelectorAPI, cdpnSelectorPayload, cdpnSelectorLoadDeferred);
                    promises.push(cdpnSelectorLoadDeferred.promise);

                    //Loading CustomerCDPN Selector
                    var customerCDPNSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    var customerCDPNSelectorPayload = {
                        selectedIds: customerIdentification
                    };
                    VRUIUtilsService.callDirectiveLoad(customerCDPNSelectorAPI, customerCDPNSelectorPayload, customerCDPNSelectorLoadDeferred);
                    promises.push(customerCDPNSelectorLoadDeferred.promise);

                    //Loading SupplierCDPN Selector
                    var supplierCDPNSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    var supplierCDPNSelectorPayload = {
                        selectedIds: supplierIdentification
                    };
                    VRUIUtilsService.callDirectiveLoad(supplierCDPNSelectorAPI, supplierCDPNSelectorPayload, supplierCDPNSelectorLoadDeferred);
                    promises.push(supplierCDPNSelectorLoadDeferred.promise);

                    //Loading SaleZoneCDPN Selector
                    var saleZoneCDPNSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    var saleZoneCDPNSelectorPayload = {
                        selectedIds: saleZoneIdentification
                    };
                    VRUIUtilsService.callDirectiveLoad(saleZoneCDPNSelectorAPI, saleZoneCDPNSelectorPayload, saleZoneCDPNSelectorLoadDeferred);
                    promises.push(saleZoneCDPNSelectorLoadDeferred.promise);

                    //Loading SupplierZoneCDPN Selector
                    var supplierZoneCDPNSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    var supplierZoneCDPNSelectorPayload = {
                        selectedIds: supplierZoneIdentification
                    };
                    VRUIUtilsService.callDirectiveLoad(supplierZoneCDPNSelectorAPI, supplierZoneCDPNSelectorPayload, supplierZoneCDPNSelectorLoadDeferred);
                    promises.push(supplierZoneCDPNSelectorLoadDeferred.promise);


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        GeneralIdentification: cdpnSelectorAPI.getSelectedIds(),
                        CustomerIdentification: customerCDPNSelectorAPI.getSelectedIds(),
                        SupplierIdentification: supplierCDPNSelectorAPI.getSelectedIds(),
                        SaleZoneIdentification: saleZoneCDPNSelectorAPI.getSelectedIds(),
                        SupplierZoneIdentification: supplierZoneCDPNSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);