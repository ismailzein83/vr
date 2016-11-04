'use strict';

app.directive('vrWhsBeCdrimportSettingsEditor', ['UtilsService', 'VRUIUtilsService', 
    function (UtilsService, VRUIUtilsService, WhS_BE_PrimarySaleEntityEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
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
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/CDRImportSettings/Templates/CDRImportSettingsTemplate.html"
        };

        function cdrImportEditorCtor(ctrl, $scope, $attrs) {

            var customerCDPNSelectorAPI;
            var customerCDPNReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onCustomerCDPNSelectorReady = function (api) {
                    customerCDPNSelectorAPI = api;
                    customerCDPNReadyDeferred.resolve();
                }

                UtilsService.waitMultiplePromises([customerCDPNReadyDeferred.promise]).then(function () {
                    defineAPI();
                })
                
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    console.log(payload);

                    var customerCDPNId;
                    var supplierCDPNId;
                    var cdpnId;
                    var saleZoneCDPNId;
                    var supplierZoneCDPNId;

                    if (payload != undefined && payload.data != undefined) {
                        customerCDPNId = payload.data.CustomerCDPNId;
                        supplierCDPNId = payload.data.SupplierCDPNId;
                        cdpnId = payload.data.CDPNId;
                        saleZoneCDPNId = payload.data.SaleZoneCDPNId;
                        supplierZoneCDPNId = payload.data.SupplierZoneCDPNId;
                    }

                    var promises = [];

                    //Loading CustomerCDPNSelector
                    var customerCDPNSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    var customerCDPNSelectorPayload = {
                        selectedIds: customerCDPNId
                    };
                    VRUIUtilsService.callDirectiveLoad(customerCDPNSelectorAPI, customerCDPNSelectorPayload, customerCDPNSelectorLoadDeferred);

                    promises.push(customerCDPNSelectorLoadDeferred.promise);


                    return UtilsService.waitMultiplePromises(promises);
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.Entities.CDRImportSettings, TOne.WhS.BusinessEntity.Entities",
                        CustomerCDPNId: customerCDPNSelectorAPI.getSelectedIds(),
                        SupplierCDPNId: null,
                        CDPNId: null,
                        SaleZoneCDPNId: null,
                        SupplierZoneCDPNId: null
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);