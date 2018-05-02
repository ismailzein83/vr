'use strict';

app.directive('vrWhsBeGetcdpnsforzonematchstep', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericRuleTypeConfigAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_GenericRuleTypeConfigAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new GetCDPNsForZoneMatchStepCtor(ctrl, $scope);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/MappingSteps/GetCDPNsForZoneMatch/Templates/GetCDPNsForZoneMatchStepTemplate.html';
            }
        };

        function GetCDPNsForZoneMatchStepCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var stepPayload;

            var ruleDefinitionDirectiveAPI;
            var ruleDefinitionDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var effectiveTimeDirectiveAPI;
            var effectiveTimeDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var cdpnDirectiveReadyAPI;
            var cdpnDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var cdpnInDirectiveReadyAPI;
            var cdpnInDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var cdpnOutDirectiveReadyAPI;
            var cdpnOutDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var switchIdDirectiveReadyAPI;
            var switchIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var customerIdDirectiveReadyAPI;
            var customerIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var supplierIdDirectiveReadyAPI;
            var supplierIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var saleZoneCDPNDirectiveReadyAPI;
            var saleZoneCDPNDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var supplierZoneCDPNDirectiveReadyAPI;
            var supplierZoneCDPNDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onRuleDefinitionReady = function (api) {
                    ruleDefinitionDirectiveAPI = api;
                    ruleDefinitionDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onEffectiveTimeDirectiveReady = function (api) {
                    effectiveTimeDirectiveAPI = api;
                    effectiveTimeDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onCDPNReady = function (api) {
                    cdpnDirectiveReadyAPI = api;
                    cdpnDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onCDPNInReady = function (api) {
                    cdpnInDirectiveReadyAPI = api;
                    cdpnInDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onCDPNOutReady = function (api) {
                    cdpnOutDirectiveReadyAPI = api;
                    cdpnOutDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onSwitchIdReady = function (api) {
                    switchIdDirectiveReadyAPI = api;
                    switchIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onCustomerIdReady = function (api) {
                    customerIdDirectiveReadyAPI = api;
                    customerIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onSupplierIdReady = function (api) {
                    supplierIdDirectiveReadyAPI = api;
                    supplierIdDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onSaleZoneCDPNReady = function (api) {
                    saleZoneCDPNDirectiveReadyAPI = api;
                    saleZoneCDPNDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onSupplierZoneCDPNReady = function (api) {
                    supplierZoneCDPNDirectiveReadyAPI = api;
                    supplierZoneCDPNDirectiveReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    stepPayload = payload;

                    //Loading Normalization Rule Definition Selector
                    var loadRuleDefinitionSelectorPromiseDeferred = getLoadRuleDefinitionSelectorPromiseDeferred();
                    promises.push(loadRuleDefinitionSelectorPromiseDeferred.promise);

                    //Loading EffectiveTime Directive
                    var loadEffectiveTimeDirectivePromiseDeferred = getLoadEffectiveTimeDirectivePromiseDeferred();
                    promises.push(loadEffectiveTimeDirectivePromiseDeferred.promise);

                    //Loading CDPN Directive
                    var loadCDPNDirectivePromiseDeferred = getLoadCDPNDirectivePromiseDeferred();
                    promises.push(loadCDPNDirectivePromiseDeferred.promise);

                    //Loading CDPNIn Directive
                    var loadCDPNInDirectivePromiseDeferred = getLoadCDPNInDirectivePromiseDeferred();
                    promises.push(loadCDPNInDirectivePromiseDeferred.promise);

                    //Loading CDPNOut Directive
                    var loadCDPNOutDirectivePromiseDeferred = getLoadCDPNOutDirectivePromiseDeferred();
                    promises.push(loadCDPNOutDirectivePromiseDeferred.promise);

                    //Loading SwitchId Directive
                    var loadSwitchIdDirectivePromiseDeferred = getLoadSwitchIdDirectivePromiseDeferred();
                    promises.push(loadSwitchIdDirectivePromiseDeferred.promise);

                    //Loading CustomerId Directive
                    var loadCustomerIdDirectivePromiseDeferred = getLoadCustomerIdDirectivePromiseDeferred();
                    promises.push(loadCustomerIdDirectivePromiseDeferred.promise);

                    //Loading Supplier Directive
                    var loadSupplierIdDirectivePromiseDeferred = getLoadSupplierIdDirectivePromiseDeferred();
                    promises.push(loadSupplierIdDirectivePromiseDeferred.promise);

                    //Loading SaleZoneCDPN Directive
                    var loadSaleZoneCDPNDirectivePromiseDeferred = getLoadSaleZoneCDPNDirectivePromiseDeferred();
                    promises.push(loadSaleZoneCDPNDirectivePromiseDeferred.promise);

                    //Loading SupplierZoneCDPN Directive
                    var loadSupplierZoneCDPNDirectivePromiseDeferred = getLoadSupplierZoneCDPNDirectivePromiseDeferred();
                    promises.push(loadSupplierZoneCDPNDirectivePromiseDeferred.promise);


                    function getLoadRuleDefinitionSelectorPromiseDeferred() {
                        var loadRuleDefinitionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                        var ruleTypeName = "VR_NormalizationRule";

                        VR_GenericData_GenericRuleTypeConfigAPIService.GetGenericRuleTypeByName(ruleTypeName).then(function (response) {
                            var ruleTypeEntity = response;

                            ruleDefinitionDirectiveReadyPromiseDeferred.promise.then(function () {

                                var payloadRuleDefinition = {
                                    filter: { RuleTypeId: ruleTypeEntity.ExtensionConfigurationId },
                                    specificTypeName: ruleTypeName,
                                    showaddbutton: true
                                };
                                if (payload.stepDetails != undefined) {
                                    payloadRuleDefinition.selectedIds = payload.stepDetails.NormalizationRuleDefinitionId;
                                }
                                VRUIUtilsService.callDirectiveLoad(ruleDefinitionDirectiveAPI, payloadRuleDefinition, loadRuleDefinitionDirectivePromiseDeferred);
                            });
                        });

                        return loadRuleDefinitionDirectivePromiseDeferred;
                    }
                    function getLoadEffectiveTimeDirectivePromiseDeferred() {
                        var loadEffectiveTimeDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                        effectiveTimeDirectiveReadyPromiseDeferred.promise.then(function () {

                            var effectiveTimePayload = {};
                            if (payload.context != undefined)
                                effectiveTimePayload.context = payload.context;
                            if (payload.stepDetails != undefined)
                                effectiveTimePayload.selectedRecords = payload.stepDetails.EffectiveTime;
                            VRUIUtilsService.callDirectiveLoad(effectiveTimeDirectiveAPI, effectiveTimePayload, loadEffectiveTimeDirectivePromiseDeferred);
                        });

                        return loadEffectiveTimeDirectivePromiseDeferred;
                    }
                    function getLoadCDPNDirectivePromiseDeferred() {
                        var loadCDPNDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                        cdpnDirectiveReadyPromiseDeferred.promise.then(function () {

                            var cdpnPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                cdpnPayload.selectedRecords = payload.stepDetails.CDPN;

                            VRUIUtilsService.callDirectiveLoad(cdpnDirectiveReadyAPI, cdpnPayload, loadCDPNDirectivePromiseDeferred);
                        });

                        return loadCDPNDirectivePromiseDeferred;
                    }
                    function getLoadCDPNInDirectivePromiseDeferred() {
                        var loadCDPNInDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                        cdpnInDirectiveReadyPromiseDeferred.promise.then(function () {

                            var cdpnInPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                cdpnInPayload.selectedRecords = payload.stepDetails.CDPNIn;

                            VRUIUtilsService.callDirectiveLoad(cdpnInDirectiveReadyAPI, cdpnInPayload, loadCDPNInDirectivePromiseDeferred);
                        });

                        return loadCDPNInDirectivePromiseDeferred;
                    }
                    function getLoadCDPNOutDirectivePromiseDeferred() {
                        var loadCDPNOutDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                        cdpnOutDirectiveReadyPromiseDeferred.promise.then(function () {

                            var cdpnOutPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                cdpnOutPayload.selectedRecords = payload.stepDetails.CDPNOut;

                            VRUIUtilsService.callDirectiveLoad(cdpnOutDirectiveReadyAPI, cdpnOutPayload, loadCDPNOutDirectivePromiseDeferred);
                        });

                        return loadCDPNOutDirectivePromiseDeferred;
                    }
                    function getLoadSwitchIdDirectivePromiseDeferred() {
                        var loadSwitchIdDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                        switchIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var switchIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                switchIdPayload.selectedRecords = payload.stepDetails.SwitchId;

                            VRUIUtilsService.callDirectiveLoad(switchIdDirectiveReadyAPI, switchIdPayload, loadSwitchIdDirectivePromiseDeferred);
                        });

                        return loadSwitchIdDirectivePromiseDeferred;
                    }
                    function getLoadCustomerIdDirectivePromiseDeferred() {
                        var loadCustomerIdDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                        customerIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var customerIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                customerIdPayload.selectedRecords = payload.stepDetails.CustomerId;

                            VRUIUtilsService.callDirectiveLoad(customerIdDirectiveReadyAPI, customerIdPayload, loadCustomerIdDirectivePromiseDeferred);
                        });

                        return loadCustomerIdDirectivePromiseDeferred;
                    }
                    function getLoadSupplierIdDirectivePromiseDeferred() {
                        var loadSupplierIdDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                        supplierIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var supplierIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                supplierIdPayload.selectedRecords = payload.stepDetails.SupplierId;

                            VRUIUtilsService.callDirectiveLoad(supplierIdDirectiveReadyAPI, supplierIdPayload, loadSupplierIdDirectivePromiseDeferred);
                        });

                        return loadSupplierIdDirectivePromiseDeferred;
                    }


                    function getLoadSaleZoneCDPNDirectivePromiseDeferred() {
                        var loadSaleZoneCDPNDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        saleZoneCDPNDirectiveReadyPromiseDeferred.promise.then(function () {

                            var saleZoneCDPNPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                saleZoneCDPNPayload.selectedRecords = payload.stepDetails.SaleZoneCDPN;

                            VRUIUtilsService.callDirectiveLoad(saleZoneCDPNDirectiveReadyAPI, saleZoneCDPNPayload, loadSaleZoneCDPNDirectivePromiseDeferred);
                        });

                        return loadSaleZoneCDPNDirectivePromiseDeferred;
                    }
                    function getLoadSupplierZoneCDPNDirectivePromiseDeferred() {
                        var loadSupplierZoneCDPNDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        supplierZoneCDPNDirectiveReadyPromiseDeferred.promise.then(function () {

                            var supplierZoneCDPNPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                supplierZoneCDPNPayload.selectedRecords = payload.stepDetails.SupplierZoneCDPN;

                            VRUIUtilsService.callDirectiveLoad(supplierZoneCDPNDirectiveReadyAPI, supplierZoneCDPNPayload, loadSupplierZoneCDPNDirectivePromiseDeferred);
                        });

                        return loadSupplierZoneCDPNDirectivePromiseDeferred;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.MappingSteps.GetCDPNsForZoneMatchStep, TOne.WhS.BusinessEntity.MainExtensions",
                        NormalizationRuleDefinitionId: ruleDefinitionDirectiveAPI.getSelectedIds(),
                        EffectiveTime: effectiveTimeDirectiveAPI != undefined ? effectiveTimeDirectiveAPI.getData() : undefined,
                        CDPN: cdpnDirectiveReadyAPI.getData(),
                        CDPNIn: cdpnInDirectiveReadyAPI.getData(),
                        CDPNOut: cdpnOutDirectiveReadyAPI.getData(),
                        SwitchId: switchIdDirectiveReadyAPI.getData(),
                        CustomerId: customerIdDirectiveReadyAPI.getData(),
                        SupplierId: supplierIdDirectiveReadyAPI.getData(),

                        SaleZoneCDPN: saleZoneCDPNDirectiveReadyAPI.getData(),
                        SupplierZoneCDPN: supplierZoneCDPNDirectiveReadyAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);