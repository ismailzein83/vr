'use strict';

app.directive('vrWhsBeSwitchcdrprocessconfiguration', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService, WhS_BE_PrimarySaleEntityEnum) {

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
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/CDRImportSettings/Templates/SwitchCDRProcessConfigurationTemplate.html"
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
                }
                $scope.scopeModel.onCustomerCDPNSelectorReady = function (api) {
                    customerCDPNSelectorAPI = api;
                    customerCDPNReadyDeferred.resolve();
                }
                $scope.scopeModel.onSupplierCDPNSelectorReady = function (api) {
                    supplierCDPNSelectorAPI = api;
                    supplierCDPNReadyDeferred.resolve();
                }
                $scope.scopeModel.onSaleZoneCDPNSelectorReady = function (api) {
                    saleZoneCDPNSelectorAPI = api;
                    saleZoneCDPNReadyDeferred.resolve();
                }
                $scope.scopeModel.onSupplierZoneCDPNSelectorReady = function (api) {
                    supplierZoneCDPNSelectorAPI = api;
                    supplierZoneCDPNReadyDeferred.resolve();
                }


                UtilsService.waitMultiplePromises([customerCDPNReadyDeferred.promise, supplierCDPNReadyDeferred.promise, saleZoneCDPNReadyDeferred.promise,
                    supplierZoneCDPNReadyDeferred.promise, cdpnReadyDeferred.promise]).then(function () {
                        defineAPI();
                    })

            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var generalIdentification;
                    var customerIdentification;
                    var supplierIdentification;
                    var saleZoneIdentification;
                    var supplierZoneIdentification;

                    if (payload != undefined && payload.switchCDRProcessConfiguration != undefined) {
                        generalIdentification = payload.switchCDRProcessConfiguration.GeneralIdentification;
                        customerIdentification = payload.switchCDRProcessConfiguration.CustomerIdentification;
                        supplierIdentification = payload.switchCDRProcessConfiguration.SupplierIdentification;
                        saleZoneIdentification = payload.switchCDRProcessConfiguration.SaleZoneIdentification;
                        supplierZoneIdentification = payload.switchCDRProcessConfiguration.SupplierZoneIdentification;
                    }

                    var promises = [];

                    //Loading CDPN Selector
                    var cdpnSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    var cdpnSelectorPayload = {
                        selectedIds: generalIdentification ? generalIdentification.CDPNIdentification : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(cdpnSelectorAPI, cdpnSelectorPayload, cdpnSelectorLoadDeferred);
                    promises.push(cdpnSelectorLoadDeferred.promise);

                    //Loading CustomerCDPN Selector
                    var customerCDPNSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    var customerCDPNSelectorPayload = {
                        selectedIds: customerIdentification ? customerIdentification.CDPNIdentification : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(customerCDPNSelectorAPI, customerCDPNSelectorPayload, customerCDPNSelectorLoadDeferred);
                    promises.push(customerCDPNSelectorLoadDeferred.promise);

                    //Loading SupplierCDPN Selector
                    var supplierCDPNSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    var supplierCDPNSelectorPayload = {
                        selectedIds: supplierIdentification ? supplierIdentification.CDPNIdentification : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(supplierCDPNSelectorAPI, supplierCDPNSelectorPayload, supplierCDPNSelectorLoadDeferred);
                    promises.push(supplierCDPNSelectorLoadDeferred.promise);

                    //Loading SaleZoneCDPN Selector
                    var saleZoneCDPNSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    var saleZoneCDPNSelectorPayload = {
                        selectedIds: saleZoneIdentification ? saleZoneIdentification.CDPNIdentification : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(saleZoneCDPNSelectorAPI, saleZoneCDPNSelectorPayload, saleZoneCDPNSelectorLoadDeferred);
                    promises.push(saleZoneCDPNSelectorLoadDeferred.promise);

                    //Loading SupplierZoneCDPN Selector
                    var supplierZoneCDPNSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    var supplierZoneCDPNSelectorPayload = {
                        selectedIds: supplierZoneIdentification ? supplierZoneIdentification.CDPNIdentification : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(supplierZoneCDPNSelectorAPI, supplierZoneCDPNSelectorPayload, supplierZoneCDPNSelectorLoadDeferred);
                    promises.push(supplierZoneCDPNSelectorLoadDeferred.promise);


                    return UtilsService.waitMultiplePromises(promises);
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.Entities.SwitchCDRProcessConfiguration, TOne.WhS.BusinessEntity.Entities",
                        GeneralIdentification: {
                            $type: "TOne.WhS.BusinessEntity.Entities.GeneralIdentification, TOne.WhS.BusinessEntity.Entities",
                            CDPNIdentification: cdpnSelectorAPI.getSelectedIds()
                        },
                        CustomerIdentification: {
                            $type: "TOne.WhS.BusinessEntity.Entities.CustomerIdentification, TOne.WhS.BusinessEntity.Entities",
                            CDPNIdentification: customerCDPNSelectorAPI.getSelectedIds()
                        },
                        SupplierIdentification: {
                            $type: "TOne.WhS.BusinessEntity.Entities.SupplierIdentification, TOne.WhS.BusinessEntity.Entities",
                            CDPNIdentification: supplierCDPNSelectorAPI.getSelectedIds()
                        },
                        SaleZoneIdentification: {
                            $type: "TOne.WhS.BusinessEntity.Entities.SaleZoneIdentification, TOne.WhS.BusinessEntity.Entities",
                            CDPNIdentification: saleZoneCDPNSelectorAPI.getSelectedIds()
                        },
                        SupplierZoneIdentification: {
                            $type: "TOne.WhS.BusinessEntity.Entities.SupplierZoneIdentification, TOne.WhS.BusinessEntity.Entities",
                            CDPNIdentification: supplierZoneCDPNSelectorAPI.getSelectedIds()
                        }
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);